using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private string _playerPrefabPath;


    private List<PlayerController> _Players;
    public List<PlayerController> Players { get => _Players; private set => _Players = value; }
    private int _connectedPlayers = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Players = new List<PlayerController>();
        _Players = new List<PlayerController>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
            photonView.RPC("AddPlayer", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void AddPlayer()
    {
        _connectedPlayers++;
        if (_connectedPlayers == PhotonNetwork.PlayerList.Length)
        {
            CreatePlayer();
        }
    }

    public void CreatePlayer()
    {
        var playerObj = PhotonNetwork.Instantiate(_playerPrefabPath, Vector3.zero, Quaternion.identity);
        var playerController = playerObj.GetComponent<PlayerController>();
        playerController.photonView.RPC("Init", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }
}
