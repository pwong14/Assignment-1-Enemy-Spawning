using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;    

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Level currentLevel;                  // The selected level (from levels.json)
    private int currentWave = 1;                 // Current wave number (starts at 1)

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        // Wait until player is initialized
        while (GameManager.Instance.player == null)
        {
            yield return null;  // wait one frame
        }

        StartLevel("Easy"); 
        //foreach (var level in EnemyDatabase.I.Levels)   
        //{
        //    GameObject selector = Instantiate(button, level_selector.transform);
        //    selector.transform.localPosition = new Vector3(0, 130);
        //    selector.GetComponent<MenuSelectorController>().spawner = this;
        //    selector.GetComponent<MenuSelectorController>().SetLevel("Start");
        //}
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel(string levelname)
    {
        level_selector.gameObject.SetActive(false);
        currentLevel = EnemyDatabase.I.Levels.FirstOrDefault(level => level.name == levelname);
        // Check if level exists
        if (currentLevel == null)
        {
            Debug.LogError($"[EnemySpawner] Level '{levelname}' not found in levels.json!");
            return;
        }

        // Reset the wave number
        currentWave = 1;

        Debug.Log($"[EnemySpawner] Starting level: {currentLevel.name} with {currentLevel.waves} waves.");
        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        StartCoroutine(SpawnWave());
    }

    public void NextWave()
    {
        currentWave++;
        StartCoroutine(SpawnWave());
    }


    IEnumerator SpawnWave()
    {
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        Debug.Log($"[EnemySpawner] Spawning wave {currentWave} for level {currentLevel.name}.");
        foreach (var spawnConfig in currentLevel.spawns)
        {
            Enemy enemyType = EnemyDatabase.I.Get(spawnConfig.enemy);
            if (enemyType == null)
            {
                Debug.LogError($"[EnemySpawner] Enemy '{spawnConfig.enemy}' not found in enemies.json!");
                continue;
            }

            // Evaluate count using RPN
            float count = RPNEvaluator.Evaluate(spawnConfig.count, new Dictionary<string, float>
            {
                { "wave", currentWave },
                { "base", enemyType.hp }
            });

            yield return StartCoroutine(SpawnEnemyBatch(spawnConfig, enemyType, Mathf.RoundToInt(count)));
        }

        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
    }

    IEnumerator SpawnEnemyBatch(Spawn spawnConfig, Enemy enemyType, int totalToSpawn)
    {
        Debug.Log($"[EnemySpawner] Preparing to spawn {totalToSpawn} '{enemyType.name}'.");
        int[] sequence = spawnConfig.sequence.ToArray();
        if (sequence.Length == 0)
            sequence = new int[] { 1 }; // default sequence

        float delay = float.Parse(spawnConfig.delay);

        int spawnIndex = 0;
        int spawned = 0;

        while (spawned < totalToSpawn)
        {
            int groupSize = sequence[spawnIndex % sequence.Length];
            int remaining = totalToSpawn - spawned;
            int thisGroup = Mathf.Min(groupSize, remaining);

            for (int i = 0; i < thisGroup; i++)
            {
                SpawnPoint spawnPoint = ChooseSpawnPoint(spawnConfig.location);
                Vector2 offset = Random.insideUnitCircle * 1.8f;
                Vector3 spawnPosition = spawnPoint.transform.position + new Vector3(offset.x, offset.y, 0);

                GameObject newEnemy = Instantiate(enemy, spawnPosition, Quaternion.identity);
                newEnemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(enemyType.sprite);

                // Calculate stats using RPN
                int hp = Mathf.RoundToInt(RPNEvaluator.Evaluate(spawnConfig.hp, new Dictionary<string, float> { { "wave", currentWave }, { "base", enemyType.hp } }));
                float speed = RPNEvaluator.Evaluate(spawnConfig.speed, new Dictionary<string, float> { { "wave", currentWave }, { "base", enemyType.speed } });
                int damage = Mathf.RoundToInt(RPNEvaluator.Evaluate(spawnConfig.damage, new Dictionary<string, float> { { "wave", currentWave }, { "base", enemyType.damage } }));

                EnemyController en = newEnemy.GetComponent<EnemyController>();
                en.hp = new Hittable(hp, Hittable.Team.MONSTERS, newEnemy);
                en.speed = Mathf.RoundToInt(speed);
                en.damage = damage;  // Make sure your EnemyController has a 'damage' field!

                GameManager.Instance.AddEnemy(newEnemy);
            }

            spawned += thisGroup;
            spawnIndex++;
            if (spawned < totalToSpawn)
                yield return new WaitForSeconds(delay);
        }
    }

    SpawnPoint ChooseSpawnPoint(string location)
    {
        if (location == "random" || string.IsNullOrEmpty(location))
            return SpawnPoints[Random.Range(0, SpawnPoints.Length)];

        string type = location.Replace("random ", "").ToUpper();

        var filtered = SpawnPoints.Where(p => p.kind.ToString() == type).ToArray();
        if (filtered.Length == 0)
        {
            Debug.LogWarning($"No spawn points of type '{type}' found! Defaulting to random.");
            return SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        }

        return filtered[Random.Range(0, filtered.Length)];
    }
}
