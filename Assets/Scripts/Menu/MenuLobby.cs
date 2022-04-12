using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MenuLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text _playersNamesList;
    [SerializeField] private Button _StartGame;

    /// <summary>
    /// Chamado pelo servidor, atualiza a lista de jogadores no lobby
    /// </summary>
    [PunRPC]
    public void UpdateNamesList()
    {
        _StartGame.interactable = NetworkManager.Instance.IsRoomHost();
        _playersNamesList.text = string.Join("\n", NetworkManager.Instance.GetPlayersNicknames());
    }
}

