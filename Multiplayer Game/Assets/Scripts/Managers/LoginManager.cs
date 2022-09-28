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
    private string password_confirmation;
    private string username;

    private bool loadedSucsess;
    private bool loggedInSucess;
    private bool majorError;
    private bool minorError;

    private bool hold;

    [SerializeField] GameObject loginScreen;
    [SerializeField] TMP_InputField emailImputField;
    [SerializeField] TMP_InputField passwordImputField;

    [SerializeField] GameObject homePage;
    [SerializeField] TMP_Text homePageText;

    [SerializeField] GameObject panel3;

    [SerializeField] GameObject signUpPanel;
    [SerializeField] TMP_InputField emailImputField2;
    [SerializeField] TMP_InputField passwordInputField2;
    [SerializeField] TMP_InputField comfirmPasswordInputField;
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] TMP_Text logBox;

    // Start is called before the first frame update
    void Start()
    {
        if (gameToken == "" || gameID == "")
        {
            loadedSucsess = false;
            majorError = true;
            //EditorUtility.DisplayDialog("Add ID and Token", "Please add your token and ID, if you do not have one, you can create a free account from cloudlogin.dev", "OK");
            throw new Exception("Token and ID Invalid");
        }
        else
        {
            Debug.Log("CloudLogin start");
            CloudLogin.SetUpGame(gameID, gameToken, ApplicationSetUp, true);
        }

        //print(PlayerPrefs.GetString("email") + " : " + PlayerPrefs.HasKey("password"));

        if (PlayerPrefs.HasKey("email") && PlayerPrefs.HasKey("password"))
        {
            hold = true;
            StartCoroutine(TryToSignInWithPlayerPrefs());
        }
    }

    void Update()
    {

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
            majorError = true;
            StopCoroutine(TryToSignInWithPlayerPrefs());

            print("error setting aplication");
            print(message);
        }
        else
        {
            loadedSucsess = true;

            print("<color=yellow>GAME CONNECTED!!" + CloudLogin.GetGameId() + "</color>");
            print("Store Items: noinoasnosdnoadonasdnoasnoda");
            print("*****************************************");


            //CloudLogin.SignUp(userEmail, "password", "password", username, SignedUp); this will be handled by the login function
        }

    }


    //=================================== skip log in ===================================



    public void SkipAll()
    {
        SignIn("guest@guest.com", "Password2020");
        StartCoroutine(loadIntoGame());
    }




    //=================================== sign up ===================================

    public void SignUpForGame()
    {
        //if (CloudLogin.GetGameName() == "Multiplayer Game")
        //    print("yes");

        if (string.IsNullOrEmpty(emailImputField2.text) || string.IsNullOrEmpty(passwordInputField2.text) || string.IsNullOrEmpty(comfirmPasswordInputField.text) || string.IsNullOrEmpty(usernameInputField.text))
        {
            logBox.text = "A field or two are empty, make sure you fill out all the boxes";
            return;
        }



        email = emailImputField2.text;
        password = passwordInputField2.text;
        password_confirmation = comfirmPasswordInputField.text;
        username = usernameInputField.text;

        print($"{email} / {password} / {password_confirmation} / {username}");

        SignUp(email, password, password_confirmation, username);

        //PlayerPrefs.SetString("email", email);
        //PlayerPrefs.SetString("password", password);

        //StartCoroutine(loadIntoGame());

        logBox.text = "<color=\"red\">?";
    }

    void SignUp(string email, string password, string password_confirmation, string username)
    {
        CloudLogin.SignUp(email, password, password_confirmation, username, SignedUp);
    }

    void SignedUp(string message, bool hasError)
    {
        if (hasError)
        {
            Debug.LogError("Error signign up: " + message);
            logBox.text = "Error signign up: " + message;
        }
        else
        {
            Debug.Log("Signed Up: " + CloudLoginUser.CurrentUser.GetUsername() + " go log in!");
            logBox.text = "Signed Up: " + CloudLoginUser.CurrentUser.GetUsername() + " go log in!";
            //insert auto login
        }
    }



    //=================================== auto sign in ===================================

    IEnumerator TryToSignInWithPlayerPrefs()
    {
        while (loadedSucsess == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        loginScreen.SetActive(false);
        panel3.SetActive(true);

        while (hold == true)
        {
            yield return new WaitForSeconds(0.1f);
            if (majorError == true)
            {
                StopCoroutine(TryToSignInWithPlayerPrefs());
            }
        }

        SignIn(PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password"));
        StartCoroutine(loadIntoGame());
    }

    public void StopAutoLogIn() // button thing to stop auto sign in
    {
        StopCoroutine(TryToSignInWithPlayerPrefs());
        loginScreen.SetActive(true);
        panel3.SetActive(false);
    }

    public void ContinueAutoLogin()
    {
        hold = false;
        panel3.SetActive(false);
    }


    //=================================== sign up or sign in toggle ===================================

    public void SignUpOrLogIn()
    {
        signUpPanel.SetActive(!signUpPanel.activeSelf);
        loginScreen.SetActive(!loginScreen.activeSelf);
    }




    //=================================== sign in ===================================

    public void Login()
    {

        if (string.IsNullOrEmpty(emailImputField.text) || string.IsNullOrEmpty(passwordImputField.text))
            return;



        email = emailImputField.text;
        password = passwordImputField.text;

        SignIn(email, password);

        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("password", password);

        StartCoroutine(loadIntoGame());
    }

    IEnumerator loadIntoGame()
    {
        while (!loggedInSucess)
        {
            yield return new WaitForSeconds(0.1f);
            print("wait loadIntoGame");
            if (minorError)
            {
                break; // break out of while as it holds the IEnum hostage
            }
        }

        if (minorError)
        {
            minorError = false;
            loginScreen.SetActive(true);
            StopCoroutine(loadIntoGame());
        }
        else
        {
            loginScreen.SetActive(false);
            panel3.SetActive(false);

            homePage.SetActive(true);

            yield return new WaitForSeconds(2f);

            SceneManager.LoadScene(1);
        }

        //print("currently attribute admin " + CloudLoginUser.CurrentUser.GetAttributeValue("admin"));
        //
        //if (CloudLoginUser.CurrentUser.GetAttributeValue("admin") == null || CloudLoginUser.CurrentUser.GetAttributeValue("admin") == "false")
        //    CloudLoginUser.CurrentUser.SetAttribute("admin", "true", SetAttributeCallback);
        //print("currently attribute admin " + CloudLoginUser.CurrentUser.GetAttributeValue("admin"));
    }

    void SignIn(string _email, string _password)
    {
        CloudLogin.SignIn(_email, _password, SignedIn);
        print("attempting sign in");
    }

    void SignedIn(string message, bool hasError)
    {
        if (hasError)
        {
            loggedInSucess = false;
            minorError = true;
            StopCoroutine(loadIntoGame());

            Debug.LogError("Error loggin in: " + message);
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
