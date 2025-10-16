using Fusion;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NetworkObject))]
public class NetworkDoor : NetworkBehaviour
{
    [Networked] public bool IsOpen { get; set; }

    [Header("Door Settings")]
    [SerializeField] private float smooth = 1.0f;
    [SerializeField] private float doorOpenAngle = -90.0f;
    [SerializeField] private float doorCloseAngle = 0.0f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openDoor;
    [SerializeField] private AudioClip closeDoor;

    private Quaternion targetRotation;
    private bool _lastIsOpen;

    public override void Spawned()
    {
        targetRotation = transform.localRotation;
        _lastIsOpen = IsOpen;

        SetDoorRotationInstant();
    }

    public override void FixedUpdateNetwork()
    {
        Quaternion target = IsOpen
            ? Quaternion.Euler(0, doorOpenAngle, 0)
            : Quaternion.Euler(0, doorCloseAngle, 0);

        targetRotation = Quaternion.Slerp(
            targetRotation,
            target,
            Runner.DeltaTime * 5f * smooth
        );

        transform.localRotation = targetRotation;
    }

    public override void Render()
    {
        if (IsOpen != _lastIsOpen)
        {
            UpdateDoorVisual();
            _lastIsOpen = IsOpen;
        }
    }

    private void UpdateDoorVisual()
    {
        if (audioSource != null)
        {
            audioSource.clip = IsOpen ? openDoor : closeDoor;
            audioSource.Play();
        }

        Debug.Log($"[NetworkDoor] {(IsOpen ? "Opened" : "Closed")}");
    }

    private void SetDoorRotationInstant()
    {
        transform.localRotation = IsOpen
            ? Quaternion.Euler(0, doorOpenAngle, 0)
            : Quaternion.Euler(0, doorCloseAngle, 0);
    }
}