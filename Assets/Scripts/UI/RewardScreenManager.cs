using UnityEngine;
using TMPro;

public class RewardScreenManager : MonoBehaviour
{
    //Should probalbly refactor into seperate manager script but this works for now since it's not that many objects
    public GameObject rewardUI;
    public TextMeshProUGUI buttonText;
    void Start()
    {
        rewardUI.SetActive(false);
    }

    // Update is called once per frame
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


}
