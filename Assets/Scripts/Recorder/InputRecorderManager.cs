using System.Collections.Generic;
using UnityEngine;

public class InputRecorderManager : MonoBehaviour
{
    private Dictionary<int, InputRecord> recordings = new Dictionary<int, InputRecord>();
    private Dictionary<int, ActorObject> actors = new Dictionary<int, ActorObject>();

    public static InputRecorderManager Instance;

    [SerializeField] private GameObject currentPlayer;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] Transform[] spawnPoints;
    private int lastSpawnIndex;

    [Header("Enable debug settings here")]
    [SerializeField] private bool DEBUG_MODE = false;

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

    private void Update()
    {
        if (!DEBUG_MODE)
            return;


        // KILL PLAYER
        // SAVE THEIR RECORDING LOL!!
        if (Input.GetKeyDown(KeyCode.K))
        {
            KillCurrentPlayer();
        }


        // RESTART THE TEST
        // SPAWN ALL OF THE RECORDINGS
        // SPAWN NEW PLAYER
        if ((Input.GetKeyDown(KeyCode.R)))
        {
            SpawnNewPlayer();
        }
    }

    public void AddRecording(InputRecord recording, ActorObject actor)
    {
        int key = recordings.Count;
        recordings[key] = recording;
        actors[key] = actor;
    }

    public void PlayAllActors()
    {
        for (int i = 0; i < recordings.Count; i++)
        {
            if (recordings.ContainsKey(i) && actors.ContainsKey(i))
            {
                actors[i].SetRecord(recordings[i]);
                actors[i].gameObject.transform.position = recordings[i].startPosition;
                actors[i].gameObject.SetActive(true);

                if(actors[i].TryGetComponent<PlayerController>(out PlayerController controller))
                {
                    controller.canAttack = true;
                }

                if (actors[i].TryGetComponent<PlayerIdentifier>(out PlayerIdentifier identifier))
                {
                    identifier.MakeBlue();
                    identifier.IdentifierNumber(i+1);
                }
            }
        }

        GameManager.Instance.LevelReset();
    }

    public void ReverseAllRecordings()
    {
        foreach (var actors in actors.Values)
        {
            actors.SetState(ActorStates.Playback);
        }
    }

    public void ResetAllRecordings()
    {
        for (int i = 0; i < recordings.Count; ++i)
        {
            var actor = actors[i];
            var recording = recordings[i];

            actor.gameObject.transform.position = recording.startPosition;
            actor.currentStep = 0;
        }
    }

    public void KillCurrentPlayer()
    {
        currentPlayer.tag = "Gladiator";
        PlayerController controller = currentPlayer.GetComponent<PlayerController>();

        Destroy(controller.gameObject.GetComponent<UnityEngine.InputSystem.PlayerInput>());
        var inputRecord = controller.KillMyselfStopRecording();
        var actor = controller.gameObject.AddComponent<ActorObject>();
        actor.SetRecord(inputRecord);
        

        AddRecording(inputRecord, actor);
        controller.gameObject.SetActive(false);
    }

    public void SpawnNewPlayer()
    {
        if (currentPlayer.activeSelf) currentPlayer.SetActive(false);

        GameObject player = Instantiate(playerPrefab, GetSpawnPoint(), Quaternion.identity);
        player.tag = "Player";
        currentPlayer = player;
        PlayAllActors();
    }

    private Vector3 GetSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return new Vector3(0, 0, 0);
        }
        else
        {
            int index;
            do
            {
                index = Random.Range(0, spawnPoints.Length);
            } while (index == lastSpawnIndex);

            lastSpawnIndex = index;
            return spawnPoints[index].position;
        }
    }
}
