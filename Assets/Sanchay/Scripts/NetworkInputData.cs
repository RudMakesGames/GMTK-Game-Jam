using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 moveInput;
    public NetworkBool isJumping;
    public NetworkBool canAttack;
    public Vector2 mouseInput;
    public float camRotY;
    public NetworkBool parachuteRequested;
    public NetworkBool isFireButtonPressed;
}