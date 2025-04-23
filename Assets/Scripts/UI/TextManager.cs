using UnityEngine;
using TMPro;

public class PlayerStatTextManager : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake() => _text = GetComponent<TextMeshProUGUI>();

    private void Start() => _text.text = string.Empty;

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
                break;
            default:
                if (!string.IsNullOrEmpty(_text.text)) _text.text = string.Empty;
                break;
        }
    }
}
