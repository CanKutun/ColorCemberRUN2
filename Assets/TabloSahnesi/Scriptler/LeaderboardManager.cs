using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class LeaderboardManager : MonoBehaviour
{
    public FlagDatabase flagDatabase;

    [System.Serializable]
    public class ScoreEntry
    {
        public string name;
        public int score;
        public string countryCode;

        public ScoreEntry(string n, int s, string c)
        {
            name = n;
            score = s;
            countryCode = c;
        }
    }

    public Transform contentParent;
    public GameObject rowPrefab;

    public Sprite goldBg;
    public Sprite silverBg;
    public Sprite bronzeBg;
    public Sprite normalBg;
    public Sprite testFlag;

    public TMP_Text pageNumberText;
    public int rowsPerPage = 10;

    private int currentPage = 0;
    private List<ScoreEntry> data = new List<ScoreEntry>();
    private HashSet<string> reportedMissingFlags = new HashSet<string>();

    void Start()
    {
        LoadRealLeaderboard();
    }

    public void NextPage()
    {
        int maxPage = Mathf.CeilToInt(data.Count / (float)rowsPerPage) - 1;
        if (currentPage < maxPage)
            ShowPage(currentPage + 1);
    }

    public void PrevPage()
    {
        if (currentPage > 0)
            ShowPage(currentPage - 1);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void ShowPage(int page)
    {
        currentPage = page;

        int start = currentPage * rowsPerPage;
        int end = Mathf.Min(start + rowsPerPage, data.Count);

        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        for (int i = start; i < end; i++)
        {
            GameObject row = Instantiate(rowPrefab, contentParent);
            row.SetActive(true);

            TMP_Text rankText = row.transform.Find("ContentRow/Sira").GetComponent<TMP_Text>();
            TMP_Text nameText = row.transform.Find("ContentRow/OyuncuAdi").GetComponent<TMP_Text>();
            TMP_Text scoreText = row.transform.Find("ContentRow/Skor").GetComponent<TMP_Text>();
            Image flagImage = row.transform.Find("ContentRow/Bayrak").GetComponent<Image>();
            Image bgImage = row.transform.Find("Background").GetComponent<Image>();

            rankText.text = (i + 1).ToString();
            nameText.text = data[i].name;
            scoreText.text = data[i].score.ToString();

            string myName = PlayerPrefs.GetString("PlayerName", "");
            if (!string.IsNullOrEmpty(myName) && data[i].name == myName)
            {
                nameText.color = Color.red;
                scoreText.color = Color.white;
                rankText.color = Color.white;
            }
            else
            {
                nameText.color = Color.black;
                scoreText.color = Color.green;
                rankText.color = Color.green;
            }

            // BAYRAK
            Sprite flag = null;
            if (flagDatabase != null && !string.IsNullOrEmpty(data[i].countryCode))
                flag = flagDatabase.GetFlag(data[i].countryCode);

            if (flag != null)
            {
                flagImage.sprite = flag;
            }
            else
            {
                flagImage.sprite = testFlag;
                ReportMissingFlag(data[i].countryCode);
            }

            if (i == 0 && goldBg != null) bgImage.sprite = goldBg;
            else if (i == 1 && silverBg != null) bgImage.sprite = silverBg;
            else if (i == 2 && bronzeBg != null) bgImage.sprite = bronzeBg;
            else if (normalBg != null) bgImage.sprite = normalBg;
        }

        int totalPages = Mathf.CeilToInt(data.Count / (float)rowsPerPage);
        pageNumberText.text = $"PAGE {currentPage + 1} / {totalPages}";
    }

    public void LoadRealLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScore",
            StartPosition = 0,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(request,
            result =>
            {
                data.Clear();
                StartCoroutine(ProcessLeaderboard(result.Leaderboard));
            },
            error =>
            {
                Debug.LogError("Leaderboard error: " + error.GenerateErrorReport());
            }
        );
    }

    IEnumerator ProcessLeaderboard(List<PlayerLeaderboardEntry> leaderboard)
    {
        int pending = leaderboard.Count;

        foreach (var entry in leaderboard)
        {
            string playerName = string.IsNullOrEmpty(entry.DisplayName) ? "Guest" : entry.DisplayName;
            int score = entry.StatValue;
            string playFabId = entry.PlayFabId;

            StartCoroutine(FetchCountryFast(playFabId, playerName, score, () =>
            {
                pending--;
            }));
        }

        while (pending > 0)
            yield return null;

        data.Sort((a, b) => b.score.CompareTo(a.score));
        ShowPage(0);
    }

    IEnumerator FetchCountryFast(string playFabId, string name, int score, System.Action onDone)
    {
        PlayFabClientAPI.GetUserData(
            new GetUserDataRequest { PlayFabId = playFabId },
            result =>
            {
                string country = "xx";

                if (result.Data != null && result.Data.ContainsKey("country"))
                    country = result.Data["country"].Value;

                data.Add(new ScoreEntry(name, score, country));
                onDone?.Invoke();
            },
            error =>
            {
                data.Add(new ScoreEntry(name, score, "xx"));
                onDone?.Invoke();
            }
        );

        yield return null;
    }

    void ReportMissingFlag(string countryCode)
    {
        if (string.IsNullOrEmpty(countryCode))
            return;

        if (reportedMissingFlags.Contains(countryCode))
            return;

        reportedMissingFlags.Add(countryCode);

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        var evt = new WriteClientPlayerEventRequest
        {
            EventName = "MissingFlagDetected",
            Body = new Dictionary<string, object>
            {
                { "country", countryCode }
            }
        };

        PlayFabClientAPI.WritePlayerEvent(evt,
            r => Debug.Log("Missing flag reported: " + countryCode),
            e => Debug.LogError("Missing flag report failed: " + e.GenerateErrorReport())
        );
#endif
    }
}