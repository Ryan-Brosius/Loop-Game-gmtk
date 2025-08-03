using UnityEngine;

public class GladiatorDeath : MonoBehaviour
{
    [SerializeField] GameObject deathPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerDisable(bool isPlayer, Vector3 direction)
    {
        if (!isPlayer)
        {
            Instantiate(deathPrefab, transform.position, Quaternion.LookRotation(direction));

            GameManager.Instance.RemoveGladiator(this.gameObject);
            this.gameObject.SetActive(false);

            SoundManager.Instance.PlaySoundEffect("OtherGladiatorDeath");
        }

        if (isPlayer)
        {
            this.gameObject.SetActive(false);
            GameManager.Instance.RoundLost();

            SoundManager.Instance.PlaySoundEffect("PlayerDeath");
        }
    }

    public void TriggerDeath(Vector3 direction)
    {
        Instantiate(deathPrefab, transform.position, Quaternion.LookRotation(direction));

        GameManager.Instance.RemoveGladiator(this.gameObject);

        SoundManager.Instance.PlaySoundEffect("OtherGladiatorDeath");

        Destroy(gameObject);
    }
}
