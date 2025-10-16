using UnityEngine;
using Fusion;

public class PlayerColorController : NetworkBehaviour
{
    [SerializeField] private Renderer playerRenderer;

    [Networked, OnChangedRender(nameof(UpdateVisuals))]
    public Color PlayerColor { get; set; }

    [Networked, OnChangedRender(nameof(UpdateVisuals))]
    public NetworkString<_16> PlayerName { get; set; }

    public override void Spawned()
    {
        UpdateVisuals();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetColorAndName(Color color, NetworkString<_16> name)
    {
        PlayerColor = color;
        PlayerName = name;
    }

    private void UpdateVisuals()
    {
        if (playerRenderer != null)
            playerRenderer.material.color = PlayerColor;

        var billboard = GetComponentInChildren<BillboardUI>();
        if (billboard != null && !string.IsNullOrEmpty(PlayerName.Value))
            billboard.SetPlayerName(PlayerName.Value);
    }
}