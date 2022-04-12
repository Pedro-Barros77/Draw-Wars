using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public static NetworkManager Instance { get; private set; }
    private static List<RoomInfo> OpenRooms { get; set; }

    /// <summary>
    /// Chamado antes de iniciar o jogo
    /// </summary>
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

    /// <summary>
    /// Chamado ao iniciar o jogo, antes do primeiro frame
    /// </summary>
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Altera o nome do jogador atual
    /// </summary>
    /// <param name="nickName">Novo nome</param>
    public void ChangeNickname(string nickName)
    {
        PhotonNetwork.NickName = nickName;
    }

    /// <summary>
    /// Cria uma nova sala com o nome informado
    /// </summary>
    /// <param name="roomName">Nome da sala</param>
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    /// <summary>
    /// Entra na sala com o nome informado
    /// </summary>
    /// <param name="roomName">Nome da sala para entrar</param>
    public void EnterRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        UpdateOpenRooms(PhotonNetwork.CurrentRoom);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        string p1 = "", p2 = "";
        switch (PhotonNetwork.CurrentRoom.PlayerCount)
        {
            case 1:
                p1 = PhotonNetwork.PlayerList[0].NickName;
                p2 = "";
                break;
            case 2:
                p1 = PhotonNetwork.PlayerList[0].NickName;
                p2 = PhotonNetwork.PlayerList[1].NickName;
                break;
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
        {
            { "Player1", p1 },
            { "Player2", p2 }
        });
    }

    /// <summary>
    /// Deixa a sala
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    void UpdateOpenRooms(RoomInfo newRoom)
    {
        photonView.RPC("GetUpdatedOpenRooms", RpcTarget.All, newRoom.Name, newRoom.CustomProperties["Player1"], newRoom.CustomProperties["Player2"]);
    }

    [PunRPC]
    public void GetUpdatedOpenRooms(string roomName, string p1, string p2)
    {
        Room newRoom = new Room(roomName, new RoomOptions());
        newRoom.SetCustomProperties(new Hashtable()
        {
            { "Player1", p1 },
            { "Player2", p2 }
        });
        RoomInfo roomInfo = newRoom;

        OpenRooms.Add(roomInfo);
    }

    /// <summary>
    /// Retorna uma lista contendo as salas existentes
    /// </summary>
    /// <returns>Lista de RoomInfo das salas abertas</returns>
    public List<RoomInfo> GetRooms()
    {
        return OpenRooms;
    }

    /// <summary>
    /// Chamado pelo servidor para todos os jogadores desta sala, para que iniciem o jogo ao mesmo tempo
    /// </summary>
    /// <param name="sceneName">Nome da cena a ser carregada no unity (Game)</param>
    [PunRPC]
    public void StartGame(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    /// <summary>
    /// Retorna uma lista contento os nomes dos jogadores desta sala
    /// </summary>
    /// <returns>Lista de nomes</returns>
    public string[] GetPlayersNicknames()
    {
        return PhotonNetwork.PlayerList.Select(p => p.NickName).ToArray();
    }

    /// <summary>
    /// Verifica se o jogador atual é o Host da sala (criador/primeiro a entrar)
    /// </summary>
    /// <returns>True se o jogador for o host, Falso se não</returns>
    public bool IsRoomHost()
    {
        return PhotonNetwork.IsMasterClient;
    }

    /// <summary>
    /// Verifica se há conexão com o servidor
    /// </summary>
    /// <returns>True se está conectado, False se não</returns>
    public bool IsConnected()
    {
        return PhotonNetwork.IsConnected;
    }

    /// <summary>
    /// Retorna a quantidade de jogadores nesta sala
    /// </summary>
    /// <returns>Int para a quantidade de jogadores</returns>
    public int GetPlayersCount()
    {
        return PhotonNetwork.PlayerList.Length;
    }

    /// <summary>
    /// Retorna o jogador inimigo nesta sala
    /// </summary>
    /// <returns>Player inimigo</returns>
    public Player GetEnemyPlayer()
    {
        return PhotonNetwork.PlayerListOthers.FirstOrDefault();
    }
}

/// <summary>
/// Classe auxiliar para converter entre Array dentro de um JSON e o objeto
/// </summary>
public static class JsonArray
{
    /// <summary>
    /// Converte de Json Array para o Array do tipo T[] informado
    /// </summary>
    /// <typeparam name="T">O tipo do array destino</typeparam>
    /// <param name="json">A string contendo o JSON a ser convertido</param>
    /// <param name="key">O nome do array dentro do JSON a ser capturado</param>
    /// <returns>Um array de tipo informado T[]</returns>
    public static T[] FromJsonArray<T>(string json, string key)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json.Replace(key, "Items"));
        return wrapper.Items;
    }

    /// <summary>
    /// Converte um array em JSON
    /// </summary>
    /// <typeparam name="T">O tipo do array fonte</typeparam>
    /// <param name="array">O array fonte</param>
    /// <param name="prettyPrint">Define se a string será formatada para melhor leitura</param>
    /// <returns></returns>
    public static string ToJsonArray<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    /// <summary>
    /// Classe utilizada para conter o array, para que JsonUtility consiga interpretar
    /// </summary>
    /// <typeparam name="T">O tipo do array a ser envolvido</typeparam>
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
