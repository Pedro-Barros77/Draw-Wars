using UnityEngine;
using UnityEngine.UI;

public class MenuEnterRoom : MonoBehaviour
{
    [SerializeField]
    private Text _playerName, _roomName;

    public void CreateRoom()
    {
        NetworkManager.Instance.ChangeNickname(_playerName.text);
        NetworkManager.Instance.CreateRoom(_roomName.text);
    }

    public void EnterRoom()
    {
        NetworkManager.Instance.ChangeNickname(_playerName.text);
        NetworkManager.Instance.EnterRoom(_roomName.text);
    }
}
