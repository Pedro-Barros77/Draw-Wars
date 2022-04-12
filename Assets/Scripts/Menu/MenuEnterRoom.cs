using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuEnterRoom : MonoBehaviour
{
    public MenuController controller;

    [SerializeField]
    private Text txtPlayerName, txtRoomName;

    [SerializeField]
    private Button btnNewRoom, btnQuickMatch;

    [SerializeField]
    private GameObject createContainer, rowPrefab, roomsGrid;

    void UpdateOpenRooms(List<RoomInfo> rooms)
    {
        Debug.Log("Called");
        if (rooms == null || !rooms.Any()) return;

        Debug.Log("Rooms: " + rooms.Count);
        foreach (RoomInfo room in rooms)
        {
            GameObject row = Instantiate(rowPrefab, roomsGrid.transform);
            row.transform.Find("RoomName").GetComponent<Text>().text = room.Name;
            var p1Name = (string)room.CustomProperties["Player1"];
            var p2Name = (string)room.CustomProperties["Player2"];
            var txtP1 = row.transform.Find("P1").GetComponent<Text>();
            var txtP2 = row.transform.Find("P2").GetComponent<Text>();
            txtP1.text = p1Name;
            var btnEnter = row.transform.Find("btnEnter").gameObject; ;
            if (string.IsNullOrEmpty(p1Name))
            {
                txtP2.gameObject.SetActive(false);
                btnEnter.SetActive(true);
            }
            else
            {
                txtP2.text = p2Name;
                btnEnter.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Chamado com o click do botão para criar uma nova sala com o nome informado
    /// </summary>
    public void CreateRoom()
    {
        NetworkManager.Instance.ChangeNickname(txtPlayerName.text);
        NetworkManager.Instance.CreateRoom(txtRoomName.text);
        CancelCreate();
    }

    /// <summary>
    /// Chamado com o click do botão para entrar na sala com o nome informado
    /// </summary>
    public void EnterRoom()
    {
        NetworkManager.Instance.ChangeNickname(txtPlayerName.text);
        NetworkManager.Instance.EnterRoom(txtRoomName.text);
        CancelCreate();
    }

    /// <summary>
    /// Chamado com o click do botão para voltar ao menu principal
    /// </summary>
    public void BackButton()
    {
        txtPlayerName.text = "";
        CancelCreate();
        controller.SetMenu(MenuController.Pages.MainMenu);
    }

    /// <summary>
    /// Chamado com o click do botão para ativar o formulário de nova sala
    /// </summary>
    public void NewRoom()
    {
        createContainer.SetActive(true);
        btnNewRoom.interactable = false;
        btnQuickMatch.gameObject.SetActive(false);
    }

    /// <summary>
    /// Chamado com o click do botão para desativar o formulário de nova sala
    /// </summary>
    public void CancelCreate()
    {
        txtRoomName.text = "";
        createContainer.SetActive(false);
        btnNewRoom.interactable = true;
        btnQuickMatch.gameObject.SetActive(true);
    }

    /// <summary>
    /// Chamado sempre que o GameObject deste script é ativado (SetActive(true))
    /// </summary>
    private void OnEnable()
    {
        UpdateOpenRooms(NetworkManager.Instance.GetRooms());
    }
}
