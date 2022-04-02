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
        DontDestroyOnLoad(this.gameObject);
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
        bool success = PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
        if (success) {
            PhotonNetwork.LoadLevel("GameplayScene");
            return "";
        } else {
            return "Room \"" + roomName + "\" could not be created";
        }
    }

    public string JoinRoom(string roomName, string playerName) {
        PhotonNetwork.NickName = playerName;
        bool success = PhotonNetwork.JoinRoom(roomName);
        if (success) {
            return "";
        } else {
            return "Room \"" + roomName + "\" not found";
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
            go.transform.parent = parent.transform;
        }
        return go;
    }

    public bool IsConnected() {
        return PhotonNetwork.IsConnected;
    }

    public string GetPlayerName() {
        return PhotonNetwork.NickName;
    }

    public bool IsHost() {
        return PhotonNetwork.IsMasterClient;
    }

    public Player[] GetPlayerList() {
        return PhotonNetwork.PlayerList;
    }

    public int GetPlayerCount() {
        return PhotonNetwork.PlayerList.Length;
    }
}
