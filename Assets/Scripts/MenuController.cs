using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private string gameplaySceneName;

    public void OnJoinNowClicked()
    {
        string playerName = nameInputField.text;
        if (string.IsNullOrEmpty(playerName)) return;
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameplaySceneName);
    }
    public void OnQuitClicked()
    {
        Application.Quit();
    }
}