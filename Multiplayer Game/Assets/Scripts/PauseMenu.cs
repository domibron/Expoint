using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject pauseMenu;

    bool isPauseMenuActive;

    private void Awake()
    {
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPauseMenuActive = !isPauseMenuActive; 
        }

        pauseMenu.SetActive(isPauseMenuActive);

        Cursor.lockState = (isPauseMenuActive ? CursorLockMode.Confined : CursorLockMode.Locked) ;
        Cursor.visible = isPauseMenuActive;
    }

    public void leaveRoom()
    {
        Destroy(RoomManager.Instance.gameObject);

        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        //PhotonNetwork.Disconnect();
        PhotonNetwork.LeaveRoom();
        //while(PhotonNetwork.InRoom)
        while (PhotonNetwork.InRoom)
            yield return null;
        SceneManager.LoadScene(0);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("left room");
    }
}
