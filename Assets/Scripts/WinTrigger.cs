using Fusion;
using UnityEngine;

public class WinTrigger : NetworkBehaviour
{
    [SerializeField] private WinPanelController winPanel;

    private PlayerRef winner;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger detected: {other.tag}");
        if (other.CompareTag("Player"))
        {
            NetworkObject playerObject = other.GetComponent<NetworkObject>();
            PlayerColorController playerColor = other.GetComponent<PlayerColorController>();
            Debug.Log("Player entered the win zone");
            if (playerObject != null && playerColor != null)
            {
                if (Runner.IsSharedModeMasterClient)
                {
                    string playerName = playerColor.PlayerName.ToString();
                    RpcShowWinPanel(playerName);
                    Debug.Log(playerName);
                }
                else
                {
                    // Report win to server
                    RpcReportWin(playerObject.InputAuthority);
                }
            }
        }
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcReportWin(PlayerRef player)
    {
        if (Runner.IsServer && winner == PlayerRef.None)
        {
            winner = player;
            var playerObject = Runner.GetPlayerObject(winner);
            var colorController = playerObject.GetComponent<PlayerColorController>();
            string winnerName = colorController != null ? colorController.PlayerName.ToString() : "Unknown";
            RpcShowWinPanel(winnerName);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcShowWinPanel(string winnerName)
    {
        Debug.Log($"[WinTrigger] RpcShowWinPanel called, winner={winnerName}");
        if (winPanel != null)
        {
            winPanel.SetWinnerName(winnerName);
        }
    }
}
