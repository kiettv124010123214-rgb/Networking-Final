using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Fusion;

public class WinPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private GameObject winPanel; 
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private GameObject escPanel;
    
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
        if (escPanel != null)
            escPanel.SetActive(false);
    }

    private void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        HideCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escPanel != null)
            {
                bool isActive = escPanel.activeSelf;
                escPanel.SetActive(!isActive);
                if (isActive)
                {
                    HideCursor();
                    if (playerMovement != null)
                        playerMovement.enabled = true;
                }
                else
                {
                    ShowCursor();
                    if (playerMovement != null)
                        playerMovement.enabled = false;
                }
                
            }
        }
    }

    private void HideCursor()
    {
        Cursor.lockState  = CursorLockMode.Locked;
        Cursor.visible = false; }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetWinnerName(string name)
    {
        Debug.Log($"[WinPanelController] SetWinnerName called, name={name}");

        if (winnerText != null)
            winnerText.text = $"Winner: {name}";

        if (winPanel != null)
        {
            Debug.Log("[WinPanelController] Activating winPanel");
            winPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("WinPanel reference is missing!");
        }
    }

    public void OnBackToMenuButtonClick()
    {
        if (runner != null)
            runner.Shutdown();
        SceneManager.LoadScene("Menu");
    }
}