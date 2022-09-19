using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canvasGroup.alpha == 0)
        {
            canvasGroup.alpha = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && canvasGroup.alpha == 1)
        {
            canvasGroup.alpha = 0;
        }
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
