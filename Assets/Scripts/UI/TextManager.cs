using UnityEngine;
using TMPro;

public class PlayerStatTextManager : MonoBehaviour
{
    public static PlayerStatTextManager Instance; 

    private TextMeshProUGUI _text;
    private string _rewardMsg = string.Empty;      

    private void Awake()
    {
        Instance = this;                         
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start() => _text.text = string.Empty;

    public void SetRewardMessage(string msg) => _rewardMsg = msg;

    private void Update()
    {
        switch (GameManager.Instance.state)
        {
            case GameManager.GameState.WAVEEND:
            case GameManager.GameState.GAMEOVER:
                _text.text =
                    $"TOTAL TIME: {GameManager.Instance.timeSpent}\n" +
                    $"DAMAGE DEALT: {GameManager.Instance.damageDealt}\n" +
                    $"DAMAGE RECEIVED: {GameManager.Instance.damageReceived}";

                if (!string.IsNullOrEmpty(_rewardMsg))
                    _text.text += $"\n{_rewardMsg}";
                break;

            default:
                // Hide all text between waves
                if (!string.IsNullOrEmpty(_text.text)) _text.text = string.Empty;
                _rewardMsg = string.Empty;                          // reset so next wave starts clean
                break;
        }
    }
}
