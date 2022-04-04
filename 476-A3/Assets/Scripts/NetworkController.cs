using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public static NetworkController instance;

    public override void OnEnable() {
        instance = this;
        DontDestroyOnLoad(gameObject);
        base.OnEnable();
    }

    void Start() {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public string HostRoom(string roomName, string playerName) {
        RoomOptions options = new RoomOptions();
        options.IsVisible = true;
        options.MaxPlayers = 4;
        PhotonNetwork.NickName = playerName;
        bool success = PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
        if (success) {
            PhotonNetwork.LoadLevel("GameplayScene");
            return "";
        } else {
            return "Error creating room";
        }
    }

    public string JoinRoom(string roomName, string playerName) {
        PhotonNetwork.NickName = playerName;
        bool success = PhotonNetwork.JoinRoom(roomName);
        if (success) {
            return "";
        } else {
            return "Error joining room";
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster()");
        base.OnConnectedToMaster();
    }

    public override void OnConnected()
    {
        Debug.Log("OnConnected()");
        base.OnConnected();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom()");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected() - " + cause.ToString());
        base.OnDisconnected(cause);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom()");
        base.OnJoinedRoom();
    }

    public GameObject SpawnNetworkedObject(string prefabName, Vector3 position, Quaternion rotation, GameObject parent = null) {
        GameObject go = PhotonNetwork.Instantiate(prefabName, position, rotation);
        if (parent != null) {
            RpcSetParent(go.GetComponent<PhotonView>().ViewID, parent);
        }
        return go;
    }

    public void DestroyNetworkedObject(GameObject go) {
        PhotonNetwork.Destroy(go);
    }

    public bool IsConnected() {
        return PhotonNetwork.InRoom;
    }

    public string GetPlayerName() {
        return PhotonNetwork.NickName;
    }

    public bool IsHost() {
        return PhotonNetwork.IsMasterClient;
    }

    public Dictionary<int, Player> GetPlayerList() {
        return PhotonNetwork.CurrentRoom.Players;
    }

    public bool CheckIsMine(GameObject go) {
        PhotonView pv = go.GetComponentInParent<PhotonView>();
        if (pv == null) {
            return false;
        }
        return pv.IsMine;
    }

    public string GetOwner(GameObject go) {
        PhotonView pv = go.GetComponentInParent<PhotonView>();
        if (pv == null) {
            return null;
        }
        return pv.Owner.UserId;
    }

    public int GetPlayerCount() {
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public bool IsMaster() {
        return PhotonNetwork.IsMasterClient;
    }

    public void AssignTank(GameObject tank, PlayerManager player) {
        int tankID = tank.GetComponent<PhotonView>().ViewID;
        int playerID = player.GetComponent<PhotonView>().ViewID;
        photonView.RPC("RpcAssignTank", RpcTarget.All, tankID, playerID);
    }

    public void DestroyTerrain(DestructibleTerrain terrain, Vector3 source) {
        bool fallBackwards = Vector3.Dot(terrain.transform.position - source, terrain.transform.forward) < 0;
        int terrainID = terrain.gameObject.GetComponent<PhotonView>().ViewID;
        photonView.RPC("RpcDestroyTerrain", RpcTarget.All, terrainID, fallBackwards);
    }
    
    public void DestroyOtherPlayersObject(int viewID) {
        photonView.RPC("RpcDestroyOtherPlayersObject", PhotonView.Find(viewID).Owner, viewID);
    }

    public void RespawnTank(Player owner) {
        photonView.RPC("RpcRespawnTank", owner);
    }

    [PunRPC]
    public void RpcSetParent(int viewID, GameObject parent) {
        PhotonView child = PhotonNetwork.GetPhotonView(viewID);
        child.transform.parent = parent.transform;
    }

    [PunRPC]
    public void RpcAssignTank(params object[] parameters) {
        int tankID = (int)parameters[0];
        int playerID = (int)parameters[1];
        Tank tank = PhotonNetwork.GetPhotonView(tankID).GetComponent<Tank>();
        PlayerManager player = PhotonNetwork.GetPhotonView(playerID).GetComponent<PlayerManager>();
        player.tank = tank;
        player.isTankColourSet = false;
        tank.playerManager = player;
        Debug.Log("Tank " + tankID.ToString() + " set for player " + playerID.ToString());
    }

    [PunRPC]
    public void RpcDestroyTerrain(params object[] parameters) {
        int terrainID = (int)parameters[0];
        bool fallBackwards = (bool)parameters[1];
        DestructibleTerrain terrain = PhotonNetwork.GetPhotonView(terrainID).GetComponent<DestructibleTerrain>();
        terrain.OnHit(fallBackwards);
    }

    [PunRPC]
    public void RpcDestroyOtherPlayersObject(int viewID) {
        PhotonView pv = PhotonView.Find(viewID);
        if (pv.IsMine) {
            DestroyNetworkedObject(pv.gameObject);
        }
    }

    [PunRPC]
    public void RpcRespawnTank() {
        PlayerManager.instance.needsRespawn = true;
    }
}
