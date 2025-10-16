using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;

    public void ShowMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
            messageText.gameObject.SetActive(true);
        }
    }

    public void HideMessage()
    {
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }
}