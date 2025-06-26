using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    private Vector3 direction;
    private Rigidbody rb;
    [SerializeField] private float speed;
    private ulong ownerId;
    
    public void Init(Vector3 direction, ulong ownerId)
    {
        this.direction = direction;
        this.ownerId = ownerId;
    }

    public void Start()
    {
        if (IsServer)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.gameObject.CompareTag("Player") && other.GetComponent<Player>().OwnerClientId != ownerId)
        {
            NetworkObject.Despawn();
        }
    }

}
