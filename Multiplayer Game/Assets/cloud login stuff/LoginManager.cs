using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudLoginUnity;
using System.Linq;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    private string gameToken = "b5a2ef4d-9e06-4ecb-82b6-4af2ffd7432f";
    private string gameID = "173";

    private string email;
    private string password;

    bool loadedSucsess;
    bool loggedInSucess;

    [SerializeField] GameObject loginScreen;
    [SerializeField] TMP_InputField emailImputField;
    [SerializeField] TMP_InputField passwordImputField;

    [SerializeField] GameObject homePage;
    [SerializeField] TMP_Text homePageText;

    // Start is called before the first frame update
    void Start()
    {
        if (gameToken == "" || gameID == "")
        {
            loadedSucsess = false;
            EditorUtility.DisplayDialog("Add ID and Token", "Please add your token and ID, if you do not have one, you can create a free account from cloudlogin.dev", "OK");
            throw new Exception("Token and ID Invalid");
        }
        else
        {
            Debug.Log("CloudLogin start");
            CloudLogin.SetVerboseLogging(true);
            CloudLogin.SetUpGame(gameID, gameToken, ApplicationSetUp, true);
        }

    }

    void Update()
    {
        if (loginScreen.activeSelf)
        {
            if (PlayerPrefs.HasKey("email") && PlayerPrefs.HasKey("password"))
            {
                SignIn(PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password"));
            }
        }

        if (homePage.activeSelf)
        {
            homePageText.text = "yay! sucessfully logged in as:<br>" + CloudLoginUser.CurrentUser.GetUsername();
        }

    }

    void ApplicationSetUp(string message, bool hasError)
    {
        if (hasError)
        {
            loadedSucsess = false;

            print("error setting aplication");
            print(message);
        }
        else
        {
            loadedSucsess = true;

            print("<color=yellow>GAME CONNECTED!!" + CloudLogin.GetGameId() + "</color>");
            print("Store Items:");
            print("*****************************************");


            //CloudLogin.SignUp(userEmail, "password", "password", username, SignedUp); this will be handled by the login function
        }

    }

    public void Login()
    {
        if (CloudLogin.GetGameName() == "Multiplayer Game")
            print("yes");

        if (string.IsNullOrEmpty(emailImputField.text) || string.IsNullOrEmpty(passwordImputField.text))
            return;



        email = emailImputField.text;
        password = passwordImputField.text;

        SignIn(email, password);

        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("passwords", password);

        StartCoroutine(DoShit());
    }

    IEnumerator DoShit()
    {
        while (!loggedInSucess)
        {
            yield return new WaitForSeconds(0.1f);
            print("wait");
        }

        //print("currently attribute admin " + CloudLoginUser.CurrentUser.GetAttributeValue("admin"));
        //
        //if (CloudLoginUser.CurrentUser.GetAttributeValue("admin") == null || CloudLoginUser.CurrentUser.GetAttributeValue("admin") == "false")
        //    CloudLoginUser.CurrentUser.SetAttribute("admin", "true", SetAttributeCallback);
        //print("currently attribute admin " + CloudLoginUser.CurrentUser.GetAttributeValue("admin"));

        loginScreen.SetActive(false);
        homePage.SetActive(true);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(0);

    }

    void SignIn(string _email, string _password)
    {
        CloudLogin.SignIn(_email, _password, SignedIn);
        print("attempting sign in");
        print(CloudLoginUser.CurrentUser.GetUsername());
    }

    void SignedIn(string message, bool hasError)
    {
        if (hasError)
        {
            loggedInSucess = false;
            StopCoroutine(DoShit());

            Debug.LogError("Error signign up: " + message);
        }
        else
        {
            loggedInSucess = true;


            Debug.Log("Logged In: " + CloudLoginUser.CurrentUser.GetUsername());
            //Debug.Log("Current Credits: " + CloudLoginUser.CurrentUser.GetCredits());
        }
    }

    void SetAttributeCallback(string message, bool hasError)
    {
        if (hasError)
        {
            print("Error adding attribute: " + message);
        }
        else
        {
            print("currently attribute admin is null?" + (CloudLoginUser.CurrentUser.GetAttributeValue("admin") == null).ToString());
            print("currently attribute admin " + CloudLoginUser.CurrentUser.GetAttributeValue("admin"));

        }
    }
}
