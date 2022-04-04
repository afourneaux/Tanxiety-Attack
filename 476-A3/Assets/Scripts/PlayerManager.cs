using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static PlayerManager instance;
    public static List<PlayerManager> AllPlayers;
    public static bool isDirty = false;
    public bool needsRespawn = false;
    float respawnCounter = 0f;

    const float RESPAWN_TIME = 5f;

    GameObject PlayerManagerListGO;
    public Player player {
        get; protected set;
    }

    private bool _isReady;
    public bool isReady {
        get {
            return _isReady;
        }
        set {
            _isReady = value;
            isDirty = true;
        }
    }
    public bool isTankColourSet = false;
    public Tank tank;
    private Color _colour = Color.HSVToRGB(1f,1f,1f);
    public Color colour {
        get {
            return _colour;
        }
        set {
            _colour = value;
            isDirty = true;
        }
    }

    public override void OnEnable() {
        if (photonView.AmOwner) {
            instance = this;
        }
        if (AllPlayers == null) {
            AllPlayers = new List<PlayerManager>();
        }
        isDirty = true;
        AllPlayers.Add(this);
        
        PlayerManagerListGO = transform.Find("/Players").gameObject;
        PhotonView pv = GetComponent<PhotonView>();
        NetworkController.instance.RpcSetParent(pv.ViewID, PlayerManagerListGO);
        player = pv.Owner;

        base.OnEnable();
    }

    public override void OnDisable()
    {
        AllPlayers.Remove(this);
        isDirty = true;
        base.OnDisable();
    }

    void Start()
    {
    }

    void Update()
    {
        if (tank != null && isTankColourSet == false) {
            tank.SetColour(colour);
            isTankColourSet = true;
        }

        if (photonView.IsMine && needsRespawn) {
            respawnCounter += Time.deltaTime;
            if (respawnCounter >= RESPAWN_TIME) {
                MainController.instance.SpawnTank();
                respawnCounter = 0f;
                needsRespawn = false;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(isReady);
            stream.SendNext(colour.r);
            stream.SendNext(colour.g);
            stream.SendNext(colour.b);
        } else {
            isReady = (bool)stream.ReceiveNext();
            float r = (float)stream.ReceiveNext();
            float g = (float)stream.ReceiveNext();
            float b = (float)stream.ReceiveNext();
            colour = new Color(r, g, b);
        }
    }
}
