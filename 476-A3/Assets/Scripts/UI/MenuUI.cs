using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    TMPro.TMP_Text playerName;
    TMPro.TMP_Text roomName;
    TMPro.TMP_Text errorOut;
    GameObject errorBG;

    void Start() {
        playerName = transform.Find("NameInput/Text Input/Text Area/Text").GetComponent<TMPro.TMP_Text>();
        roomName = transform.Find("RoomInput/Text Input/Text Area/Text").GetComponent<TMPro.TMP_Text>();
        errorOut = transform.Find("ErrorOut").GetComponent<TMPro.TMP_Text>();
        errorBG = transform.Find("ErrorOut/Background").gameObject;
    }

    public void HostGame() {
        string room = roomName.text;
        if (room == null || room == string.Empty) {
            ShowError("Room name is required!");
            return;
        }

        string player = playerName.text;
        if (room == null || room == string.Empty) {
            ShowError("Player name is required!");
            return;
        }

        string error = NetworkController.instance.HostRoom(room, player);
        if (error != "") {
            ShowError(error);
        }
    }

    public void JoinGame() {
        string room = roomName.text;
        if (room == null || room == string.Empty) {
            ShowError("Room name is required!");
            return;
        }

        string player = playerName.text;
        if (room == null || room == string.Empty) {
            ShowError("Player name is required!");
            return;
        }

        string error = NetworkController.instance.JoinRoom(room, player);
        if (error != "") {
            ShowError(error);
        }
    }

    void ShowError(string text) {
        errorOut.text = text;
        errorBG.SetActive(true);
    }
}
