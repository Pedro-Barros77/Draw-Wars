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

    private void Start()
    {
        List<RoomInfo> rooms = NetworkManager.Instance.GetRooms();
        if (rooms != null && rooms.Any())
        {
            foreach (RoomInfo room in rooms)
            {
                GameObject row = Instantiate(rowPrefab, roomsGrid.transform);
                var playersList = ((List<string>)room.CustomProperties["PlayersList"]);
                row.transform.Find("RoomName").GetComponent<Text>().text = room.Name;
                var txtP1 = row.transform.Find("P1").GetComponent<Text>();
                var txtP2 = row.transform.Find("P2").GetComponent<Text>();
                txtP1.text = playersList.FirstOrDefault();
                var btnEnter = row.transform.Find("btnEnter").gameObject; ;
                if (playersList.Count > 1)
                {
                    txtP2.text = playersList[1];
                    btnEnter.SetActive(false);
                }
                else
                {
                    txtP2.gameObject.SetActive(false);
                    btnEnter.SetActive(true);
                }
            }
        }
    }

    public void CreateRoom()
    {
        NetworkManager.Instance.ChangeNickname(txtPlayerName.text);
        NetworkManager.Instance.CreateRoom(txtRoomName.text);
    }

    public void EnterRoom()
    {
        NetworkManager.Instance.ChangeNickname(txtPlayerName.text);
        NetworkManager.Instance.EnterRoom(txtRoomName.text, txtPlayerName.text);
    }

    public void BackButton()
    {
        createContainer.SetActive(false);
        btnNewRoom.interactable = true;
        btnQuickMatch.gameObject.SetActive(true);
        controller.SetMenu("MainMenu");
    }

    public void NewRoom()
    {
        createContainer.SetActive(true);
        btnNewRoom.interactable = false;
        btnQuickMatch.gameObject.SetActive(false);
    }

    public void CancelCreate()
    {
        createContainer.SetActive(false);
        btnNewRoom.interactable = true;
        btnQuickMatch.gameObject.SetActive(true);
    }
}
