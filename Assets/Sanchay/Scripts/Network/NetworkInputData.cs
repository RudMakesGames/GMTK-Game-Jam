using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 moveInput;
    public NetworkBool isJumping;
    public Vector2 mouseInput;
    public float camRotY;
    public NetworkBool parachuteRequested;
    public NetworkBool isFireButtonPressed;
    public NetworkBool isAiming;
}