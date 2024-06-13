using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_Text userInfo;

    string tempHolderForName;

    private void Start()
    {

        // insert some magic to set username
        // REWORK AND TESTING REQUIRED

        //print(CloudLoginUnity.CloudLoginUser.CurrentUser.IsSignedIn());
        try
        {
            if (CloudLoginUnity.CloudLoginUser.CurrentUser.IsSignedIn())
            {
                if (CloudLoginUnity.CloudLoginUser.CurrentUser.GetUsername() == "domibron")
                {
                    usernameInput.text = $"<color=yellow>{CloudLoginUnity.CloudLoginUser.CurrentUser.GetUsername()}</color>";
                    PhotonNetwork.NickName = $"<color=yellow>{CloudLoginUnity.CloudLoginUser.CurrentUser.GetUsername()}</color>";
                    PlayerPrefs.SetString("username", usernameInput.text);
                    // PlayerPrefs.Save();
                    print("hello domibron");
                    OnUsernameValueChanged();
                }
                else
                {
                    usernameInput.text = CloudLoginUnity.CloudLoginUser.CurrentUser.GetUsername();
                    PhotonNetwork.NickName = CloudLoginUnity.CloudLoginUser.CurrentUser.GetUsername();
                    PlayerPrefs.SetString("username", usernameInput.text);
                    // PlayerPrefs.Save();
                    OnUsernameValueChanged();
                }
            }
            else if (PlayerPrefs.HasKey("username"))
            {
                tempHolderForName = "Guest " + Random.Range(0, 100).ToString("0000");

                PhotonNetwork.NickName = tempHolderForName;
                PlayerPrefs.SetString("username", tempHolderForName);
                // PlayerPrefs.Save();
                usernameInput.text = tempHolderForName;

                OnUsernameValueChanged();
            }
            else
            {
                usernameInput.text = "Guest " + Random.Range(0, 100).ToString("0000");
                OnUsernameValueChanged();
            }



            userInfo.text = $"Logged In as:<br>Username: {CloudLoginUnity.CloudLoginUser.CurrentUser.GetUsername()}<br>Confirm: {CloudLoginUnity.CloudLoginUser.CurrentUser.IsSignedIn()}<br><color=\"red\">Connection: --<br>Data Leaks: --";
        }
        catch
        {
            usernameInput.text = "<color=red>ERROR</color>";
            PhotonNetwork.NickName = "<color=red>ERROR</color>";
        }
    }

    public void OnUsernameValueChanged()
    {
        try
        {
            if (CloudLoginUnity.CloudLoginUser.CurrentUser.IsSignedIn())
            {
                if (CloudLoginUnity.CloudLoginUser.CurrentUser.GetUsername() == "domibron")
                {
                    usernameInput.text = $"<color=yellow>{CloudLoginUnity.CloudLoginUser.CurrentUser.GetUsername()}</color>";
                    PhotonNetwork.NickName = $"<color=yellow>{CloudLoginUnity.CloudLoginUser.CurrentUser.GetUsername()}</color>";
                    PlayerPrefs.SetString("username", usernameInput.text);
                    // PlayerPrefs.Save();
                }
                else
                {
                    usernameInput.text = CloudLoginUnity.CloudLoginUser.CurrentUser.GetUsername();
                    PhotonNetwork.NickName = CloudLoginUnity.CloudLoginUser.CurrentUser.GetUsername();
                    PlayerPrefs.SetString("username", usernameInput.text);
                }
            }
            else
            {
                usernameInput.text = tempHolderForName;
                PhotonNetwork.NickName = tempHolderForName;
                //PlayerPrefs.SetString("username", usernameInput.text);
            }
        }
        catch
        {
            usernameInput.text = "<color=red>ERROR</color>";
            PhotonNetwork.NickName = "<color=red>ERROR</color>";
        }
    }
}
