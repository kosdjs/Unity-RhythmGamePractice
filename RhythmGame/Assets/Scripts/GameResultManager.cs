using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class GameResultManager : MonoBehaviour
{
    public Text musicTitleUI;
    public Text scoreUI;
    public Text maxComboUI;
    public Image RankUI;

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
    }

    public void Replay()
    {
        SceneManager.LoadScene("SongSelectScene");
    }
}