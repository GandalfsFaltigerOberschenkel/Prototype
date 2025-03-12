using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

using System.Linq;
using TMPro;
[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public TimeSpan time;

    public LeaderboardEntry(string name, TimeSpan time)
    {
        this.playerName = name;
        this.time = time;
    }
}
public class LeaderBoardManager : MonoBehaviour
{
    [Header("Azure Storage Settings")]
    [SerializeField] private string storageUrl;
    [SerializeField] private string sasToken;
    [SerializeField] private string leaderboardFileName = "leaderboard.csv";

    public TMP_Text time;
    public TMP_InputField nameInput;
    public GameObject leaderBoardEntry;
    public GameObject continueButton;
    public LeaderboardEntry currentEntry;
    private List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    public GameObject leaderboardParent;
    public void ShowLeaderboard()
    {
        entries.OrderBy(e => e.time);
        foreach (Transform child in leaderboardParent.transform)
        {
            Destroy(child.gameObject);
        }
        int rank = 1;
        foreach (var entry in entries)
        {
            GameObject go = Instantiate(leaderBoardEntry, leaderboardParent.transform);
            string playerName = entry.playerName;
            if(playerName == "Dnold") { playerName = "Dnold (Developer)"; go.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(255, 215, 0); }
            go.transform.GetChild(0).GetComponent<TMP_Text>().text = playerName;
            go.transform.GetChild(1).GetComponent<TMP_Text>().text = entry.time.ToString(@"hh\:mm\:ss");
            go.transform.GetChild(2).GetComponent<TMP_Text>().text = rank.ToString();
        }
        if(continueButton != null)
            continueButton.SetActive(true);
    }
    private void Start()
    {
        StartCoroutine(DownloadLeaderboard());
        //if (entries.Count > 100)
        //    entries.RemoveRange(100, entries.Count - 100);
       
    }
    public void ChangeName()
    {
        currentEntry.playerName = nameInput.text;

    }
    public void Skip()
    {
        SceneManager.LoadScene(0);
    }
    public void UploadToLeaderboard()
    {
        if (currentEntry.playerName == "Player" || currentEntry.playerName == "")
        {
            Debug.LogError("Name is empty");
            return;
        }
        //if (currentEntry.time <= TimeSpan.FromSeconds(292))
        //{
        //    Debug.LogError("Time is too short");
        //    return;
        //}
        StartCoroutine(UploadLeaderboard(new LeaderboardEntry(nameInput.text, TimeSpan.Parse(time.text))));
        StartCoroutine(DownloadLeaderboard());
        SceneManager.LoadScene(0);
    }
    public IEnumerator DownloadLeaderboard()
    {
        string url = $"{storageUrl}{leaderboardFileName}{sasToken}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Download failed: {request.error}");
                yield break;
            }

            ParseCSV(request.downloadHandler.text);
            if (TimerThing.instance != null)
            {

                currentEntry = new LeaderboardEntry("Player", TimerThing.instance.timer.Elapsed);
                time.text = currentEntry.time.ToString("hh\\:mm\\:ss");

            }
            ShowLeaderboard();
        }
    }

    public IEnumerator UploadLeaderboard(LeaderboardEntry newEntry)
    {
        // Add new entry and sort
        entries.Add(newEntry);
        entries = entries.OrderBy(e => e.time).Take(10).ToList(); // Keep top 10

        // Create CSV
        string csv = "PlayerName,Time\n";
        foreach (var entry in entries)
        {
            csv += $"{EscapeCSV(entry.playerName)},{entry.time}\n";
        }

        // Upload to Azure
        string url = $"{storageUrl}{leaderboardFileName}{sasToken}";
        byte[] csvBytes = System.Text.Encoding.UTF8.GetBytes(csv);

        UnityWebRequest request = new UnityWebRequest(url, "PUT")
        {
            uploadHandler = new UploadHandlerRaw(csvBytes),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "text/csv");
        request.SetRequestHeader("x-ms-blob-type", "BlockBlob");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Upload failed: {request.error}");
        }
    }

    private void ParseCSV(string csvData)
    {
        entries.Clear();

        using (StringReader reader = new StringReader(csvData))
        {
            reader.ReadLine(); // Skip header
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 2 &&
                    TimeSpan.TryParse(parts[1], out TimeSpan time))
                {
                    entries.Add(new LeaderboardEntry(
                        UnescapeCSV(parts[0]),
                        time
                    ));
                }
            }
        }
    }

    private string EscapeCSV(string input)
    {
        return $"\"{input.Replace("\"", "\"\"")}\"";
    }

    private string UnescapeCSV(string input)
    {
        return input.Trim('"').Replace("\"\"", "\"");
    }
}