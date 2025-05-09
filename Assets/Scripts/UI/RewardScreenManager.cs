using UnityEngine;
using TMPro;

public class RewardScreenManager : MonoBehaviour
{
    public static RewardScreenManager Instance;

    public GameObject rewardUI;
    public TextMeshProUGUI buttonText;

    public PlayerController player;
    public SpellUIContainer spellUIContainer;

    private Spell rewardSpell;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rewardUI.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND || GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            rewardUI.SetActive(true);
            if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
            {
                buttonText.text = "Return to Menu";
            }
        }
        else
        {
            rewardUI.SetActive(false);
        }
    }

    public void ShowReward()
    {
        rewardUI.SetActive(true);
        Debug.Log("Reward screen activated");

        // Generate a new random spell as a reward
        rewardSpell = new SpellBuilder().Build(player.spellcaster);

        // Log what the player is being offered
        Debug.Log($"New spell reward: {rewardSpell.GetName()} (Mana: {rewardSpell.GetManaCost()}, Damage: {rewardSpell.GetDamage()})");
    }

    public void AcceptReward()
    {
        if (rewardSpell == null) return;

        // Replace spell in slot 0
        player.spellcaster.spell = rewardSpell;
        spellUIContainer.spellUIs[0].GetComponent<SpellUI>().SetSpell(rewardSpell);

        Debug.Log($"Equipped new reward spell: {rewardSpell.GetName()}");

        rewardUI.SetActive(false);
        GameManager.Instance.NextWave();
    }
}