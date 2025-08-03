using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] List<GameObject> gladiatorsList;

    [Header("Fame & Glory")]
    [SerializeField] bool decayActive = false;
    [SerializeField] int fameAmount = 0;
    [SerializeField] int famePerKill = 10;
    [SerializeField] float currentGlory = 0f;
    [SerializeField] float maxGlory = 3f;

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI fameText;
    [SerializeField] GloryBar gloryBar;

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
            decayActive = false;
        }

        if (gloryBar) gloryBar.SetMaxGlory(maxGlory);

        SoundManager.Instance.PlayMusic("MainTheme");
    }

    private void Update()
    {
        if (decayActive) DecayGlory();
    }

    public void LevelReset()
    {
        ClearArena();
        gladiatorsList.Clear();
        CountGladiators();
        ResetGlory();
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

        SoundManager.Instance.PlaySoundEffect("PlayerWinsRound");
    }

    public void RoundLost()
    {
        StartCoroutine(RoundLostRoutine());
    }

    IEnumerator RoundLostRoutine()
    {
        SoundManager.Instance.PlaySoundEffect("PlayerDeath");
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

    public void TakedownScore(bool isStopped)
    {
        if (isStopped)
        {
            AddFame(famePerKill / 2);
            AddGlory();
        }
        else
        {
            AddFame(famePerKill);
            AddGlory();
        }
    }

    public void AddGlory()
    {
        currentGlory += 5;
        if (gloryBar) gloryBar.SetGlory(currentGlory);
    }

    public void GloryPenalty()
    {
        if (currentGlory >= maxGlory / 3)
        {
            currentGlory -= 1;
            if (gloryBar) gloryBar.SetGlory(currentGlory);
        }
    }

    private void ResetGlory()
    {
        currentGlory = maxGlory;
        decayActive = true;

        if (gloryBar) gloryBar.SetGlory(currentGlory);
    }

    private void AddFame(int baseAmount)
    {
        fameAmount += baseAmount + (int)currentGlory;

        if (fameText)
        {
            fameText.text = fameAmount.ToString();
            fameText.rectTransform.DOPunchAnchorPos(Vector2.up * 25, 0.25f, 20, 1);
        }
    }

    private void DecayGlory()
    {
        currentGlory -= Time.deltaTime;
        currentGlory = Mathf.Clamp(currentGlory, 0f, maxGlory);

        if (gloryBar) gloryBar.SetGlory(currentGlory);

        if (currentGlory <= 0f)
        {
            decayActive = false;
            RoundLost();
        }
    }
}
