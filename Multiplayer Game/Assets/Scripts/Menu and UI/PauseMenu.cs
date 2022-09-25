using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerController playerController;

    [SerializeField] GameObject pauseMenu;

    [SerializeField] Slider sensitivitySlider;
    [SerializeField] TMP_Text sensitivitySliderText;

    float pauseMenuSensitivity;

    bool isPauseMenuActive;

    private void Awake()
    {
        pauseMenu.SetActive(false);

    }

    void Start()
    {
        pauseMenuSensitivity = playerController.mouseSensitivity;
        sensitivitySlider.value = playerController.mouseSensitivity * 10;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPauseMenuActive = !isPauseMenuActive;
        }

        pauseMenu.SetActive(isPauseMenuActive);

        Cursor.lockState = (isPauseMenuActive ? CursorLockMode.Confined : CursorLockMode.Locked);
        Cursor.visible = isPauseMenuActive;

        pauseMenuSensitivity = sensitivitySlider.value / 10;

        sensitivitySliderText.text = pauseMenuSensitivity.ToString();
        playerController.mouseSensitivity = pauseMenuSensitivity;

    }

    public void leaveRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = false;

        Destroy(RoomManager.Instance.gameObject);

        //Destroy(DiscordHanderler.Instance.gameObject);

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
