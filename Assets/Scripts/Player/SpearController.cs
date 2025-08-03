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
        if (canPickup || other.gameObject == parentObj)
        {
            return;
        }

        Vector3 direction = other.transform.position - transform.position;
        direction.y = 0;

        bool isMainCharacter = parentObj.GetComponent<PlayerController>().IsMainCharacter();

        if (other.CompareTag("Gladiator"))
        {
            if (other.TryGetComponent<GladiatorDeath>(out GladiatorDeath deathScript))
            {
                deathScript.TriggerDisable(false, -direction);

                if (isMainCharacter)
                {
                    SoundManager.Instance.PlaySoundEffect("PlayerScoresTakedown");
                    GameManager.Instance.TakedownScore(false);
                }
                else
                {
                    GameManager.Instance.GloryPenalty();
                }
            }
        }

        if (other.CompareTag("First Objective"))
        {
            if (other.TryGetComponent<GladiatorDeath>(out GladiatorDeath deathScript))
            {
                deathScript.TriggerDeath(-direction);

                if (isMainCharacter)
                {
                    SoundManager.Instance.PlaySoundEffect("PlayerScoresTakedown");
                    GameManager.Instance.TakedownScore(false);
                }
            }
        }

        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<GladiatorDeath>(out GladiatorDeath deathScript))
            {
                deathScript.TriggerDisable(true, -direction);
            }
        }

        if (spearMesh != null) spearMesh.localRotation = Quaternion.Euler(130, 0, 0);
        if (pickUpSparkles != null) pickUpSparkles.SetActive(true);
        rb.linearVelocity = Vector3.zero;
        canPickup = true;
    }
}
