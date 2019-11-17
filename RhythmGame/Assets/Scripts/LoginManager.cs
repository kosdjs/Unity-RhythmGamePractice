using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    private FirebaseAuth auth;
    public InputField emailInputField;
    public InputField passwordInputField;

    public Text messageUI;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        messageUI.text = "";
    }

    public void Login()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(
            task =>
            {
                if(task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    PlayerInformation.auth = auth;
                    SceneManager.LoadScene("SongSelectScene");
                }
                else
                {
                    messageUI.text = "계정을 다시 확인해주세요.";
                }
            }
        );
    }

    public void GoToJoin()
    {
        SceneManager.LoadScene("JoinScene");
    }
}
