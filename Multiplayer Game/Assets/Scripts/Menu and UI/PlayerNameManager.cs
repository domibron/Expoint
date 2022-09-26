using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;

    private void Start()
    {
        // insert some magic to set username
        // REWORK AND TESTING REQUIRED

        if ((usernameInput.text.Contains("#") || PhotonNetwork.NickName.Contains("#")))// && CloudLoginUnity.CloudLoginUser.CurrentUser.GetAttributeValue("admin") != "true")
        {
            usernameInput.text.Replace("#", "O");
            PhotonNetwork.NickName.Replace("#", "O");
        }

        if (PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        }
        else
        {
            usernameInput.text = "Player " + Random.Range(0, 100).ToString("0000");
            OnUsernameValueChanged();
        }
    }

    public void OnUsernameValueChanged()
    {
        if ((usernameInput.text.Contains("#") || PhotonNetwork.NickName.Contains("#")))// && CloudLoginUnity.CloudLoginUser.CurrentUser.GetAttributeValue("admin") != "true")
        {
            usernameInput.text.Replace("#", "O");
            PhotonNetwork.NickName.Replace("#", "O");

            PhotonNetwork.NickName = usernameInput.text;
            PlayerPrefs.SetString("username", usernameInput.text);
        }
        else
        {
            PhotonNetwork.NickName = usernameInput.text;
            PlayerPrefs.SetString("username", usernameInput.text);
        }
    }
}
