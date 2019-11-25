﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class NoteController : MonoBehaviour
{
    class Note
    {
        public int noteType { get; set; }
        public int order { get; set; }
        public Note(int noteType, int order)
        {
            this.noteType = noteType;
            this.order = order;
        }
    }

    public GameObject[] Notes;

    private ObjectPooler noteObjectPooler;
    private List<Note> notes = new List<Note>();
    private float x, z, startY = 8.0f;

    void MakeNote(Note note)
    {
        GameObject obj = noteObjectPooler.getObject(note.noteType);
        x = obj.transform.position.x;
        z = obj.transform.position.z;
        obj.transform.position = new Vector3(x, startY, z);
        obj.GetComponent<NoteBehaviour>().Initialize();
        obj.SetActive(true);
    }

    private string musicTitle;
    private string musicArtist;
    private int bpm;
    private int divider;
    private float startingPoint;
    private float beatCount;
    private float beatInterval;

    IEnumerator AwaitMakeNote (Note note)
    {
        int noteType = note.noteType;
        int order = note.order;
        yield return new WaitForSeconds(startingPoint + order * beatInterval);
        MakeNote(note);
    }

    void Start()
    {
        noteObjectPooler = gameObject.GetComponent<ObjectPooler>();
        TextAsset textAsset = Resources.Load<TextAsset>("Beats/" + PlayerInformation.selectedMusic);
        StringReader reader = new StringReader(textAsset.text);
        musicTitle = reader.ReadLine();
        musicArtist = reader.ReadLine();
        string beatInformation = reader.ReadLine();
        bpm = Convert.ToInt32(beatInformation.Split(' ')[0]);
        divider = Convert.ToInt32(beatInformation.Split(' ')[1]);
        startingPoint = (float)Convert.ToDouble(beatInformation.Split(' ')[2]);
        beatCount = (float)bpm / divider;
        beatInterval = 1 / beatCount;
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            Note note = new Note(
                Convert.ToInt32(line.Split(' ')[0]) + 1,
                Convert.ToInt32(line.Split(' ')[1])
            );
            notes.Add(note);
        }
        for (int i=0; i<notes.Count; i++)
        {
            StartCoroutine(AwaitMakeNote(notes[i]));
        }
        StartCoroutine(AwaitGameResult(notes[notes.Count - 1].order));
    }
    
    IEnumerator AwaitGameResult(int order)
    {
        yield return new WaitForSeconds(startingPoint + order * beatInterval + 8.0f);
        GameResult();
    }

    void GameResult()
    {
        PlayerInformation.maxCombo = GameManager.instance.maxCombo;
        PlayerInformation.score = GameManager.instance.score;
        PlayerInformation.musicTitle = musicTitle;
        PlayerInformation.musicArtist = musicArtist;
        AddRank();
        SceneManager.LoadScene("GameResultScene");
    }

    class Rank
    {
        public string email;
        public int score;
        public double timestamp;

        public Rank(string email, int score, double timestamp)
        {
            this.email = email;
            this.score = score;
            this.timestamp = timestamp;
        }
    }

    void AddRank()
    {
        DatabaseReference reference;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unity-rhythm-game-practice.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        DateTime now = DateTime.Now.ToLocalTime();
        TimeSpan span = (now - new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime());
        int timestamp = (int)span.TotalSeconds;
        Rank rank = new Rank(PlayerInformation.auth.CurrentUser.Email, (int)PlayerInformation.score, timestamp);
        string json = JsonUtility.ToJson(rank);
        reference.Child("ranks").Child(PlayerInformation.selectedMusic).Child(PlayerInformation.auth.CurrentUser.UserId).SetRawJsonValueAsync(json);
    }

    void Update()
    {
        
    }
}
