using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class GameResultManager : MonoBehaviour
{
    public Text musicTitleUI;
    public Text scoreUI;
    public Text maxComboUI;
    public Image RankUI;

    public Text rank1UI;
    public Text rank2UI;
    public Text rank3UI;

    // Start is called before the first frame update
    void Start()
    {
        musicTitleUI.text = PlayerInformation.musicTitle;
        scoreUI.text = "점수: " + (int) PlayerInformation.score;
        maxComboUI.text = "최대 콤보: " + PlayerInformation.maxCombo;
        TextAsset textAsset = Resources.Load<TextAsset>("Beats/" + PlayerInformation.selectedMusic);
        StringReader stringReader = new StringReader(textAsset.text);
        stringReader.ReadLine();
        stringReader.ReadLine();
        string beatInformation = stringReader.ReadLine();
        int scoreS = Convert.ToInt32(beatInformation.Split(' ')[3]);
        int scoreA = Convert.ToInt32(beatInformation.Split(' ')[4]);
        int scoreB = Convert.ToInt32(beatInformation.Split(' ')[5]);
        if (PlayerInformation.score >= scoreS)
        {
            RankUI.sprite = Resources.Load<Sprite>("Sprites/Rank S");
        }
        else if(PlayerInformation.score >= scoreA)
        {
            RankUI.sprite = Resources.Load<Sprite>("Sprites/Rank A");
        }
        else if(PlayerInformation.score >= scoreB)
        {
            RankUI.sprite = Resources.Load<Sprite>("Sprites/Rank B");
        }
        else
        {
            RankUI.sprite = Resources.Load<Sprite>("Sprites/Rank C");
        }
        rank1UI.text = "데이터를 불러오는 중입니다.";
        rank2UI.text = "데이터를 불러오는 중입니다.";
        rank3UI.text = "데이터를 불러오는 중입니다.";
        DatabaseReference reference;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unity-rhythm-game-practice.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.GetReference("ranks").Child(PlayerInformation.selectedMusic);
        reference.OrderByChild("score").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                List<String> rankList = new List<String>();
                List<String> emailList = new List<String>();
                DataSnapshot snapshot = task.Result;
                foreach(DataSnapshot data in snapshot.Children)
                {
                    IDictionary rank = (IDictionary)data.Value;
                    emailList.Add(rank["email"].ToString());
                    rankList.Add(rank["score"].ToString());
                }
                emailList.Reverse();
                rankList.Reverse();
                rank1UI.text = "플레이 한 사용자가 없습니다.";
                rank2UI.text = "플레이 한 사용자가 없습니다.";
                rank3UI.text = "플레이 한 사용자가 없습니다.";
                List<Text> textlist = new List<Text>();
                textlist.Add(rank1UI);
                textlist.Add(rank2UI);
                textlist.Add(rank3UI);
                int count = 1;
                for(int i=0; i < rankList.Count && i < 3; i++)
                {
                    textlist[i].text = count + "위" + emailList[i] + " (" + rankList[i] + " 점)";
                    count++;
                }
            }
        });
    }

    public void Replay()
    {
        SceneManager.LoadScene("SongSelectScene");
    }
}