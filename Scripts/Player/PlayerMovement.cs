using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour, INetworkRunnerCallbacks
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float rotationSmoothTime = 0.12f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float groundedCheckOffset = 0.5f;
    
    private WinTrigger winTrigger;
    private CharacterController characterController;
    private Animator animator;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction sprintAction;
    private float currentSpeed;
    private float verticalVelocity;
    private float rotationVelocity;
    private Transform cameraTransform;

    public struct NetworkInputData : INetworkInput
    {
        public Vector2 movementInput;
        public bool isSprinting;
    }

    public override void Spawned()
    {
        verticalVelocity = 0f;
        currentSpeed = 0f;
        if (Object.HasInputAuthority)
        {
            cameraTransform = Camera.main?.transform;
            Runner.AddCallbacks(this);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Object.HasInputAuthority)
            runner.RemoveCallbacks(this);
    }

    private void Awake()
    {
        winTrigger = FindFirstObjectByType<WinTrigger>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.Enable();
        if (sprintAction != null) sprintAction.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (sprintAction != null) sprintAction.Disable();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!Object.HasInputAuthority) return;
        var data = new NetworkInputData
        {
            movementInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero,
            isSprinting = sprintAction != null && sprintAction.IsPressed()
        };
        input.Set(data);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority || cameraTransform == null) return;

        if (!GetInput(out NetworkInputData data)) return;

        Vector2 moveInput = Vector2.ClampMagnitude(data.movementInput, 1f);
        float targetSpeed = (moveInput.magnitude > 0.1f) ? (data.isSprinting ? sprintSpeed : walkSpeed) : 0f;

        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        Vector3 inputDirection = (camRight * moveInput.x + camForward * moveInput.y).normalized;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Runner.DeltaTime * acceleration);

        if (inputDirection.magnitude > 0.1f)
        {
            float targetYaw = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            float smoothYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYaw, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothYaw, 0f);
        }

        bool grounded = characterController.isGrounded || Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, groundedCheckOffset);
        if (grounded && verticalVelocity < 0f) verticalVelocity = -2f;
        else verticalVelocity += gravity * Runner.DeltaTime;

        Vector3 move = inputDirection * currentSpeed + Vector3.up * verticalVelocity;
        characterController.Move(move * Runner.DeltaTime);

        if (animator != null)
        {
            float normalizedSpeed = Mathf.InverseLerp(0f, sprintSpeed, currentSpeed);
            animator.SetFloat("speed", normalizedSpeed);
        }
    }

    public void SetCamera(Transform cam)
    {
        cameraTransform = cam;
    }

    //private void OnTriggerEnter(Collider other)
    //{
       // if (other.CompareTag("Win") && Object.HasInputAuthority)
        //{
       //     winTrigger.RpcReportWin(Object.InputAuthority); 
       // }
    //}



    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }
}