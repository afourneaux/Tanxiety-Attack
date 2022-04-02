using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    GameObject PlayerListGO;
    public GameObject PlayerLinePrefab;
    bool isSetup = false;
    bool isReady = false;
    int playerCount = 0;

    void Update() {
        if (!isSetup) {
            if (NetworkController.instance.IsConnected()) {
                
                PlayerListGO = transform.Find("PlayerList").gameObject;

                isSetup = true;
            }
        }

        if (isSetup) {
            int newPlayerCount = NetworkController.instance.GetPlayerCount();
            if (newPlayerCount != playerCount) {
                playerCount = newPlayerCount;
                foreach (Transform child in PlayerListGO.transform) {
                    Destroy(child.gameObject);
                }

                foreach (Player player in NetworkController.instance.GetPlayerList()) {
                    GameObject lobbyLineGO = Instantiate(PlayerLinePrefab, Vector3.zero, Quaternion.identity, PlayerListGO.transform);
                    lobbyLineGO.transform.Find("PlayerName").GetComponent<TMPro.TMP_Text>().text = player.NickName;
                    lobbyLineGO.transform.Find("ReadyStatus").GetComponent<TMPro.TMP_Text>().text = "Not Ready";
                }
            }
        }
    }

    public void Ready() {
        
    }
}
