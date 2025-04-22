using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public sealed class EnemyDatabase : MonoBehaviour
{
    public static EnemyDatabase I { get; private set; }

    public IReadOnlyList<Enemy>               AllEnemies => _enemies;
    public IReadOnlyDictionary<string, Enemy> ById       => _byId;

    public IReadOnlyList<Level> Levels => _levels;

    public Enemy Get(string id) =>
        _byId.TryGetValue(id, out var e) ? e : null;

    List<Enemy>               _enemies;
    Dictionary<string, Enemy> _byId;
    List<Level>               _levels;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    void Load()
    {
        // Enemies
        var enemyAsset = Resources.Load<TextAsset>("enemies");
        if (enemyAsset == null)
        {
            Debug.LogError("[EnemyDatabase] Resources/enemies.json not found."); 
            return;
        }

        _enemies = JsonConvert.DeserializeObject<List<Enemy>>(enemyAsset.text);
        _byId    = _enemies.ToDictionary(e => e.name, e => e);
        Debug.Log($"[EnemyDatabase] Loaded {_enemies.Count} enemy definitions.");

        // Levels
        var levelAsset = Resources.Load<TextAsset>("levels");
        if (levelAsset == null)
        {
            Debug.LogError("[EnemyDatabase] Resources/levels.json not found.");
            return;
        }

        _levels = JsonConvert.DeserializeObject<List<Level>>(levelAsset.text);
        Debug.Log($"[EnemyDatabase] Loaded {_levels.Count} level definitions.");
    }
}
