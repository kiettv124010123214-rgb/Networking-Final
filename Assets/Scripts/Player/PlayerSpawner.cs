using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Linq;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [Header("Spawn Area Settings")]
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Transform spawnAreaCenter;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(5f, 1f, 5f);

    [Header("Color Settings")]
    [SerializeField] private Color[] availableColors = { Color.red, Color.blue, Color.green, Color.yellow };

    [Header("Doors")]
    [SerializeField] private NetworkDoor[] lobbyDoors;

    [Header("UI Manager")]
    [SerializeField] private LobbyManager lobbyUI;

    private List<Color> usedColors = new List<Color>();
    private int requiredPlayers = 2;
    private bool isGameStarted;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            var spawnedPlayer = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

            if (spawnedPlayer != null)
            {
                Runner.SetPlayerObject(player, spawnedPlayer);

                string playerName = PlayerPrefs.GetString("PlayerName", $"Player{Random.Range(1000, 9999)}");
                var colorController = spawnedPlayer.GetComponent<PlayerColorController>();

                if (colorController != null)
                {
                    Color selectedColor = GetUniqueRandomColor();
                    colorController.RPC_SetColorAndName(selectedColor, playerName);

                    if (Runner.IsServer || Runner.IsSharedModeMasterClient)
                        usedColors.Add(selectedColor);
                }
            }
        }

        if (Runner.IsServer || Runner.IsSharedModeMasterClient)
            UpdateLobbyState();
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Runner.TryGetPlayerObject(player, out var playerObj))
        {
            var colorController = playerObj.GetComponent<PlayerColorController>();
            if (colorController != null && (Runner.IsServer || Runner.IsSharedModeMasterClient))
                usedColors.Remove(colorController.PlayerColor);

            Runner.Despawn(playerObj);
        }

        if (Runner.IsServer || Runner.IsSharedModeMasterClient)
            UpdateLobbyState();
    }

    private void UpdateLobbyState()
    {
        int currentPlayers = Runner.ActivePlayers.Count();

        // tất cả client đều thấy khi chưa đủ người
        if (currentPlayers < requiredPlayers)
        {
            isGameStarted = false;
            lobbyUI?.ShowMessage($"Require {requiredPlayers} players to start");
            CloseDoors();
        }
        else if (!isGameStarted)
        {
            // chỉ host hiện "Press F"
            if (Runner.IsServer || Runner.IsSharedModeMasterClient)
                lobbyUI?.ShowMessage("Press F to Start Now");
            else
                lobbyUI?.HideMessage();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!(Runner.IsServer || Runner.IsSharedModeMasterClient)) return;

        if (!isGameStarted && Runner.ActivePlayers.Count() >= requiredPlayers)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isGameStarted = true;
                lobbyUI?.HideMessage();
                OpenDoors();
            }
        }
    }

    private void OpenDoors()
    {
        foreach (var door in lobbyDoors)
            if (door != null) door.IsOpen = true;
    }

    private void CloseDoors()
    {
        foreach (var door in lobbyDoors)
            if (door != null) door.IsOpen = false;
    }

    private Color GetUniqueRandomColor()
    {
        List<Color> available = availableColors.Where(c => !usedColors.Contains(c)).ToList();
        return available.Count > 0 ? available[Random.Range(0, available.Count)] : availableColors[Random.Range(0, availableColors.Length)];
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnAreaCenter == null) return Vector3.zero;
        float randomX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float randomZ = Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);
        return spawnAreaCenter.position + new Vector3(randomX, 0f, randomZ);
    }
}
