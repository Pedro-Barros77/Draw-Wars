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
    public enum Pages
    {
        MainMenu,
        RoomSelector,
        Lobby
    }
    Dictionary<Pages, GameObject> MenuList = new Dictionary<Pages, GameObject>();

    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private MenuEnterRoom _menuEnterRoom;
    [SerializeField] private MenuLobby _menuLobby;

    /// <summary>
    /// Chamado ao iniciar o jogo, antes do primeiro frame
    /// </summary>
    private void Start()
    {
        MenuList.AddRange(new KeyValuePair<Pages, GameObject>[] {
            new KeyValuePair<Pages, GameObject>(Pages.MainMenu,_mainMenu.gameObject),
            new KeyValuePair<Pages, GameObject>(Pages.RoomSelector, _menuEnterRoom.gameObject),
            new KeyValuePair<Pages, GameObject>(Pages.Lobby, _menuLobby.gameObject)
        });

        SetMenu(_mainMenu.gameObject);
    }

    /// <summary>
    /// Atualiza o menu, desativando todos os outros e ativando o alvo
    /// </summary>
    /// <param name="menu">GameObject do Menu alvo a ser ativado</param>
    public void SetMenu(GameObject menu)
    {
        if (!MenuList.ContainsValue(menu))
            return;

        ChangeMenu(menu);
    }
    /// <summary>
    /// Atualiza o menu, desativando todos os outros e ativando o alvo
    /// </summary>
    /// <param name="menu">Enum do Menu alvo a ser ativado</param>
    public void SetMenu(Pages page)
    {
        if (!MenuList.ContainsKey(page))
            return;

        ChangeMenu(MenuList[page]);
    }

    /// <summary>
    /// Desativa todos os Menus e ativa Menu o alvo
    /// </summary>
    /// <param name="menu">GameObject do Menu alvo a ser ativado</param>
    void ChangeMenu(GameObject Menu)
    {
        foreach (GameObject obj in MenuList.Values)
        {
            obj.SetActive(false);
        }
        Menu.SetActive(true);
    }

    /// <summary>
    /// Deixa a sala atual
    /// </summary>
    public void LeaveLobby()
    {
        NetworkManager.Instance.LeaveRoom();
        SetMenu(_menuEnterRoom.gameObject);
    }

    /// <summary>
    /// Inicia o jogo
    /// </summary>
    /// <param name="sceneName">Nome da cena no Unity a ser carregada</param>
    public void StartGame(string sceneName)
    {
        NetworkManager.Instance.photonView.RPC("StartGame", RpcTarget.All, sceneName);
    }

    /// <summary>
    /// Chamado assim que a conexão for estabelecida
    /// </summary>
    public override void OnConnectedToMaster()
    {
        _mainMenu.SetConnected(true);
    }

    /// <summary>
    /// Chamado sempre que um jogador entrar nesta sala (incluindo o criador)
    /// </summary>
    public override void OnJoinedRoom()
    {
        SetMenu(_menuLobby.gameObject);
        _menuLobby.photonView.RPC("UpdateNamesList", RpcTarget.All);
    }

    /// <summary>
    /// Chamado sempre que um jogador sair desta sala
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _menuLobby.UpdateNamesList();
    }
}
