using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void ChangeNickname(string nickName)
    {
        PhotonNetwork.NickName = nickName;
    }

    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    public void EnterRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void StartGame(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    public string[] GetPlayersNicknames()
    {
        return PhotonNetwork.PlayerList.Select(p => p.NickName).ToArray();
    }

    public bool IsRoomHost()
    {
        return PhotonNetwork.IsMasterClient;
    }

    public bool IsConnected()
    {
        return PhotonNetwork.IsConnected;
    }

    public int GetPlayersCount()
    {
        return PhotonNetwork.PlayerList.Length;
    }

    public Player GetEnemyPlayer()
    {
        return PhotonNetwork.PlayerListOthers.FirstOrDefault();
    }
}

public static class JsonArray
{
    public static T[] FromJsonArray<T>(string json, string key)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json.Replace(key, "Items"));
        return wrapper.Items;
    }

    public static string ToJsonArray<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
