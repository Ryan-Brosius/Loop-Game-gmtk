using UnityEngine;

public class SpearController : MonoBehaviour
{
    [SerializeField] float flySpeed;
    [SerializeField] float pickupRadius = 1f;
    [SerializeField] Transform spearMesh;
    [SerializeField] GameObject pickUpSparkles;
    public bool canPickup = false;
    public GameObject parentObj;
    public LayerMask gladiatorLayer;
    private Rigidbody rb;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (canPickup)
        {
            if (Physics.CheckSphere(transform.position, pickupRadius, gladiatorLayer))
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius, gladiatorLayer);
                foreach(var hit in hits)
                {
                    if (hit.TryGetComponent<PlayerController>(out PlayerController gladiator))
                    {
                        if (!gladiator.canAttack)
                        {
                            gladiator.canAttack = true;
                            canPickup = false;
                            Destroy(this.gameObject);
                            return;
                        }
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

    public void SpawnSpear(GameObject parent)
    {
        parentObj = parent;
        if (rb != null) rb.linearVelocity = transform.forward * flySpeed;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (canPickup)
        {
            return;
        }
        if (other.gameObject == parentObj)
        {
            return;
        }

        if (other.CompareTag("Gladiator") && other.gameObject != parentObj && !canPickup)
        {
            other.gameObject.SetActive(false);
        }

        if (spearMesh != null) spearMesh.localRotation = Quaternion.Euler(130, 0, 0);
        if (pickUpSparkles != null) pickUpSparkles.SetActive(true);
        rb.linearVelocity = Vector3.zero;
        canPickup = true;
    }
}
