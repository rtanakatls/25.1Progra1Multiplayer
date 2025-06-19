using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Player : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> playerName=new NetworkVariable<FixedString32Bytes>();
    private Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] private TextMesh playerNameText;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        playerNameText.text = playerName.Value.ToString();
        playerName.OnValueChanged += (oldName, newName) =>
        {
            playerNameText.text = newName.Value.ToString();
        };
    }

    public void SetName(string name)
    {
        if(IsOwner)
        {
            SendNameToServerRpc(name);
        }
    }

    [Rpc(SendTo.Server)]
    private void SendNameToServerRpc(string name)
    {
        playerName.Value = name;
        SendNameToClientsRpc(name);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SendNameToClientsRpc(string name)
    {
        playerNameText.text = name;
    }



    void Update()
    {
        if (IsOwner)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector2 direction = new Vector2(h, v);
            direction.Normalize();
            rb.linearVelocity = new Vector3(direction.x, 0, direction.y) * speed + Vector3.up * rb.linearVelocity.y;
        }
    }
}
