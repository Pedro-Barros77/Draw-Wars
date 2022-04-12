using Assets.Scripts.Extensions;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviourPunCallbacks
{
    Dictionary<string, GameObject> MenuList = new Dictionary<string, GameObject>();

    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private MenuEnterRoom _menuEnterRoom;
    [SerializeField] private MenuLobby _menuLobby;

    private void Start()
    {
        MenuList.AddRange(new KeyValuePair<string, GameObject>[] {
            new KeyValuePair<string, GameObject>("MainMenu",_mainMenu.gameObject),
            new KeyValuePair<string, GameObject>("EnterRoom", _menuEnterRoom.gameObject),
            new KeyValuePair<string, GameObject>("Lobby", _menuLobby.gameObject)
        });

        SetMenu(_mainMenu.gameObject);
    }

    public void SetMenu(GameObject menu)
    {
        if (!MenuList.ContainsValue(menu))
            return;

        ChangeMenu(menu);
    }
    public void SetMenu(string name)
    {
        if (!MenuList.ContainsKey(name))
            return;

        ChangeMenu(MenuList[name]);
    }

    void ChangeMenu(GameObject Menu)
    {
        foreach (GameObject obj in MenuList.Values)
        {
            obj.SetActive(false);
        }
        Menu.SetActive(true);
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
        _mainMenu.SetConnected(true);
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
