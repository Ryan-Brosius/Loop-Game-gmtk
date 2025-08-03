using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> scores;

    private string key = "81b11985a88802c21f42f3743d31f74e30bd9691c70e2d82a476300f3d72cfa3";

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
                scores[i].text = $"{msg[i].Username} {msg[i].Score} {msg[i].Extra}";
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
