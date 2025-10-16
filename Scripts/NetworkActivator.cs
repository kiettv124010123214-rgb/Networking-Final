using UnityEngine;
using Fusion;

public class NetworkActivator : NetworkBehaviour
{
    [SerializeField] private GameObject[] _ownerObjects;

    public override void Spawned()
    {
        bool isLocalPlayer = Object.HasInputAuthority;
        foreach (var obj in _ownerObjects)
        {
            if (obj != null)
                obj.SetActive(isLocalPlayer);
        }
    }
}