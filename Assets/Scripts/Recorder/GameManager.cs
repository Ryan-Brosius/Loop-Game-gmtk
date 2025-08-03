using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] List<GameObject> gladiatorsList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (gladiatorsList.Count <= 0)
        {
            gladiatorsList.Add(GameObject.FindGameObjectWithTag("First Objective"));
        }
    }

    public void LevelReset()
    {
        ClearArena();
        gladiatorsList.Clear();
        CountGladiators();
    }

    public void CountGladiators()
    {
        foreach (GameObject actor in GameObject.FindGameObjectsWithTag("Gladiator"))
        {
            if (actor.activeSelf) gladiatorsList.Add(actor);
        }
    }

    public void RemoveGladiator(GameObject actorToRemove)
    {
        gladiatorsList.Remove(actorToRemove);
        if (gladiatorsList.Count <= 0) StartCoroutine(RoundWonRoutine());
    }

    public void RoundWon()
    {
        InputRecorderManager.Instance.KillCurrentPlayer();
        InputRecorderManager.Instance.SpawnNewPlayer();
    }

    IEnumerator RoundWonRoutine()
    {
        InputRecorderManager.Instance.KillCurrentPlayer();
        yield return new WaitForSeconds(1f);
        InputRecorderManager.Instance.SpawnNewPlayer();
    }

    public void RoundLost()
    {
        StartCoroutine(RoundLostRoutine());
    }

    IEnumerator RoundLostRoutine()
    {
        if (gladiatorsList.Count > 0)
        {
            foreach (GameObject actor in gladiatorsList)
            {
                actor.SetActive(false);
            }
        }

        yield return new WaitForSeconds(1f);
        InputRecorderManager.Instance.SpawnNewPlayer();
    }

    public void ClearArena()
    {
        GameObject[] spears = GameObject.FindGameObjectsWithTag("Spear");
        foreach (GameObject spear in spears) Destroy(spear);

        GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");
        foreach (GameObject corpse in corpses) Destroy(corpse);
    }
}
