using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> scores;

    private string key = "16e353d37c872b7b64e8dfde81ad5ccc809c4ce2aa71ccd2deb7831c7faeb106";

    public static Leaderboard Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        GetLeaderBoard();   
    }

    public void GetLeaderBoard()
    {
        LeaderboardCreator.GetLeaderboard(key, ((msg) =>
        {
            int count = Mathf.Min(msg.Length, scores.Count);

            for (int i = 0; i < count; i++)
            {
                // 1.) [username] [loop] [X]
                scores[i].text = $"{i+1}.) {msg[i].Username}    {msg[i].Extra}    {msg[i].Score}";
            }
        }));
    }

    public void SetLeaderboardEntry(string username, int score, string loopNumber)
    {
        LeaderboardCreator.UploadNewEntry(key, username, score, loopNumber, (msg) =>
        {
            GetLeaderBoard();
        });
    }
}
