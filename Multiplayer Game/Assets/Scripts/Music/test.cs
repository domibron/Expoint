using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.Events;

public class test : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<AudioClip> music = new List<AudioClip>();

    private AudioSource audioSource;

    private PhotonView PV;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PV = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            // AudioClip tempAC = music[Random.Range(0, music.Count - 1)];
            // audioSource.clip = tempAC;
            // audioSource.Play();

            PV.RPC(nameof(RPC_SetMusicToPlayer), RpcTarget.All, Random.Range(0, music.Count - 1));

            //PV.RPC(nameof(RPC_SetMusicToPlayer), RpcTarget.All, tempAC);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying && PhotonNetwork.IsMasterClient)
        {
            // AudioClip tempAC = music[Random.Range(0, music.Count - 1)];
            // audioSource.clip = tempAC;
            // audioSource.Play();

            PV.RPC(nameof(RPC_SetMusicToPlayer), RpcTarget.All, Random.Range(0, music.Count - 1));

            //PV.RPC(nameof(RPC_SetMusicToPlayer), RpcTarget.Others, tempAC);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            audioSource.mute = !audioSource.mute;
        }
    }

    void reloadmusiclol()
    {
        PV.RPC(nameof(RPC_SetMusicToPlayer), RpcTarget.All, Random.Range(0, music.Count - 1));
    }

    [PunRPC]
    void RPC_SetMusicToPlayer(int acint)
    {
        SetMusicPlay(acint);
    }

    private void SetMusicPlay(int acint)
    {
        audioSource.clip = music[acint];
        audioSource.Play();
    }
}
