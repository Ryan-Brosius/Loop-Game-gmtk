using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] List<GameObject> gladiatorsList;
    private int currentLoop;
    public string playerName;
    private int losses = 5;

    [Header("Fame & Glory")]
    [SerializeField] bool decayActive = false;
    [SerializeField] int fameAmount = 0;
    [SerializeField] int famePerKill = 10;
    [SerializeField] float currentGlory = 0f;
    [SerializeField] float maxGlory = 3f;

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI fameText;
    [SerializeField] GloryBar gloryBar;
    [SerializeField] GameObject startScreen;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] GameObject victoryPanel;
    [SerializeField] GameObject lossPanel;
    [SerializeField] TextMeshProUGUI loopText;
    [SerializeField] TextMeshProUGUI spawnText;
    [SerializeField] TextMeshProUGUI killerTitle;
    [SerializeField] GameObject gloryLoss;
    [SerializeField] GameObject deathLoss;
    [SerializeField] GameObject leaderboard;

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

        Time.timeScale = 0;
        startScreen.SetActive(true);
    }

    public void EnterArena()
    {
        if (!string.IsNullOrEmpty(nameInput.text))
        {
            startScreen.SetActive(false);
            Time.timeScale = 1;
            playerName = nameInput.text;
        }
    }

    private void Update()
    {
        if (decayActive) DecayGlory();

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (lossPanel.activeSelf || victoryPanel.activeSelf)
            {
                InputRecorderManager.Instance.SpawnNewPlayer();
                HidePanels();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (startScreen.activeSelf)
            {
                EnterArena();
            }
        }
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
        decayActive = false;
        SoundManager.Instance.PlaySoundEffect("PlayerWinsRound");
        currentLoop++;
        InputRecorderManager.Instance.KillCurrentPlayer();
        yield return new WaitForSeconds(1f);
        ShowVictoryPanel();
    }

    public void RoundLost()
    {
        StartCoroutine(RoundLostRoutine());
    }

    public void RoundLost(string numeral)
    {
        losses++;

        decayActive = false;
        SoundManager.Instance.PlaySoundEffect("PlayerDeath");
        if (gladiatorsList.Count > 0)
        {
            foreach (GameObject actor in gladiatorsList)
            {
                actor.SetActive(false);
            }
        }

        if (losses < 5)
        {
            ShowLossPanel(numeral);
        }
        else
        {
            ShowLeaderboardCanvas();
        }
    }

    IEnumerator RoundLostRoutine()
    {
        losses++;

        decayActive = false;
        SoundManager.Instance.PlaySoundEffect("PlayerDeath");
        if (gladiatorsList.Count > 0)
        {
            foreach (GameObject actor in gladiatorsList)
            {
                actor.SetActive(false);
            }
        }

        yield return new WaitForSeconds(1f);

        if (losses < 5)
        {
            ShowLossPanel();
        }
        else
        {
            ShowLeaderboardCanvas();
        }
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

    public void ShowVictoryPanel()
    {
        if (victoryPanel)
        {
            loopText.text = "Loop " + currentLoop.ToString();
            victoryPanel.SetActive(true);

            string nextGate = InputRecorderManager.Instance.GetSpawnCardinal();

            spawnText.text = "Now You will enter from " + nextGate; 
        }
    }

    public void ShowLossPanel()
    {
        if (lossPanel)
        {
            gloryLoss.SetActive(true);
            deathLoss.SetActive(false);
            lossPanel.SetActive(true);
        }
    }

    public void ShowLossPanel(string numeral)
    {
        if (lossPanel)
        {
            gloryLoss.SetActive(false);
            killerTitle.text = "Gladiator " + numeral;
            deathLoss.SetActive(true);
            lossPanel.SetActive(true);
        }
    }

    public void HidePanels()
    {
        victoryPanel.SetActive(false);
        lossPanel.SetActive(false);
    }

    public void ShowLeaderboardCanvas()
    {
        // Assuming this doesnt break when this is opened the user should add their score??
        // Could break but idk

        Leaderboard.Instance.SetLeaderboardEntry(playerName, fameAmount, $"{currentLoop}");

        HidePanels();
        leaderboard.SetActive(true);
    }
}
