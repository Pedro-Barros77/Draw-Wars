using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Menu : MonoBehaviourPunCallbacks
{
    [SerializeField] private MenuEnterRoom _menuEnterRoom;
    [SerializeField] private MenuLobby _menuLobby;

    private void Start()
    {
        _menuEnterRoom.gameObject.SetActive(false);
        _menuLobby.gameObject.SetActive(false);
    }

    public void SetMenu(GameObject menu)
    {
        _menuEnterRoom.gameObject.SetActive(false);
        _menuLobby.gameObject.SetActive(false);

        menu.gameObject.SetActive(true);
    }

    public void LeaveLobby()
    {
        NetworkManager.Instance.LeaveRoom();
        SetMenu(_menuEnterRoom.gameObject);
    }

    public void StartGame(string sceneName)
    {
        NetworkManager.Instance.photonView.RPC("StartGame", RpcTarget.All, sceneName);
    }

    public override void OnConnectedToMaster()
    {
        _menuEnterRoom.gameObject.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        SetMenu(_menuLobby.gameObject);
        _menuLobby.photonView.RPC("UpdateNamesList", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _menuLobby.UpdateNamesList();
    }
}
