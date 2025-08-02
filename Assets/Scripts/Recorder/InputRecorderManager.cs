using System.Collections.Generic;
using UnityEngine;

public class InputRecorderManager : MonoBehaviour
{
    private Dictionary<int, InputRecord> recordings = new Dictionary<int, InputRecord>();
    private Dictionary<int, ActorObject> actors = new Dictionary<int, ActorObject>();

    public static InputRecorderManager Instance;

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
            }
        }
    }

    public void ReverseAllRecordings()
    {
        foreach (var actors in actors.Values)
        {
            actors.SetState(ActorStates.Playback);
        }
    }
}
