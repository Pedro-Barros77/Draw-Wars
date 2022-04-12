// Decompiled with JetBrains decompiler
// Type: PlayerController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C068AE61-1BFB-4193-98D8-E27C534A19E2
// Assembly location: F:\Users\Pedro Henrique\source\repos\Draw Wars Erro\Build\Draw Wars_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool _isPlayerOwner = false;
    private Player _photonPlayer;
    private int _id;
    private DrawingController drawingController;

    public float MovementSpeed { get; private set; } = 10f;

    /// <summary>
    /// Chamado pelo servidor para cada jogador ao iniciar o jogo
    /// </summary>
    /// <param name="player">O jogador</param>
    [PunRPC]
    public void Init(Player player)
    {
        this._photonPlayer = player;
        this._id = player.ActorNumber;
        GameManager.Instance.Players.Add(this);
        this._isPlayerOwner = this.photonView.IsMine;
    }

    /// <summary>
    /// Chamado ao iniciar o jogo, antes do primeiro frame
    /// </summary>
    private void Start()
    {
        this.drawingController = this.GetComponent<DrawingController>();
    }

    /// <summary>
    /// Chamado uma vez por frame. Varia de acordo com a capacidade da máquina
    /// </summary>
    private void Update()
    {
        if (PhotonNetwork.IsConnected && !this._isPlayerOwner)
            return;
        this.transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f) * this.MovementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Envia para o servidor a informação de que o jogador limpou seus desenhos, para que seja removido da tela deo inimigo
    /// </summary>
    public void ClearDrawings()
    {
        this.photonView.RPC("ClearEnemyDrawings", NetworkManager.Instance.GetEnemyPlayer());
    }

    /// <summary>
    /// Chamado pelo servidor quando o inimigo limpa seus desenhos
    /// </summary>
    [PunRPC]
    public void ClearEnemyDrawings()
    {
        this.drawingController.ClearEnemyDrawings();
    }

    /// <summary>
    /// Similar ao Update, envia(se isWriting = true) ou recebe(se isWriting = false) informações para o servidor diversas vezes por segundo
    /// </summary>
    /// <param name="stream">O componente PhotonStream deste GameObject</param>
    /// <param name="info">Informações sobre o RPC</param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            string json = JsonUtility.ToJson((object)this.drawingController.MyDrawings, true);
            if (!json.Contains("Coords"))
                return;
            stream.SendNext((object)json);
        }
        else
        {
            string next;
            try
            {
                next = (string)stream.ReceiveNext();
            }
            catch (IndexOutOfRangeException)
            {
                Debug.Log((object)"Out Of Range");
                return;
            }
            if (!next.Contains("Coords"))
                return;
            this.drawingController.EnemyDrawings = new Drawings()
            {
                Shapes = JsonArray.FromJsonArray<Shape>(next, "Shapes")
            };
        }
    }
}
