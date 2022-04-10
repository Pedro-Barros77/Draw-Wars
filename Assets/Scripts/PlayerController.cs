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

    [PunRPC]
    public void Init(Player player)
    {
        this._photonPlayer = player;
        this._id = player.ActorNumber;
        GameManager.Instance.Players.Add(this);
        this._isPlayerOwner = this.photonView.IsMine;
    }

    private void Start() => this.drawingController = this.GetComponent<DrawingController>();

    private void Update()
    {
        if (PhotonNetwork.IsConnected && !this._isPlayerOwner)
            return;
        this.transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f) * this.MovementSpeed * Time.deltaTime);
    }

    public void ClearDrawings() => this.photonView.RPC("ClearEnemyDrawings", NetworkManager.Instance.GetEnemyPlayer());

    [PunRPC]
    public void ClearEnemyDrawings() => this.drawingController.ClearEnemyDrawings();

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
            catch (IndexOutOfRangeException ex)
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
