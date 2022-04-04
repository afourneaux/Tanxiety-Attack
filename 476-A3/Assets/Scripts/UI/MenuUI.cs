using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    TMPro.TMP_InputField playerName;
    TMPro.TMP_InputField roomName;
    TMPro.TMP_Text errorOut;
    GameObject errorBG;
    string musicID;

    void Start() {
        playerName = transform.Find("NameInput/Text Input").GetComponent<TMPro.TMP_InputField>();
        roomName = transform.Find("RoomInput/Text Input").GetComponent<TMPro.TMP_InputField>();
        errorOut = transform.Find("ErrorOut").GetComponent<TMPro.TMP_Text>();

        musicID = AudioController.instance.PlayMusic(7);    // Title music
    }

    // Open a room using the given room name, or join if it already exists
    public void HostGame() {
        AudioController.instance.PlaySound(0);  // Click sound effect
        string room = roomName.text;
        if (string.IsNullOrEmpty(room)) {
            ShowError("Room name is required!");
            return;
        }

        string player = playerName.text;
        if (string.IsNullOrEmpty(player)) {
            ShowError("Player name is required!");
            return;
        }

        string error = NetworkController.instance.HostRoom(room, player);
        if (error != "") {
            ShowError(error);
        }
    }

    // Join a game with the given room name
    public void JoinGame() {
        AudioController.instance.PlaySound(0);  // Click sound effect
        string room = roomName.text;
        if (string.IsNullOrEmpty(room)) {
            ShowError("Room name is required!");
            return;
        }

        string player = playerName.text;
        if (string.IsNullOrEmpty(player)) {
            ShowError("Player name is required!");
            return;
        }

        string error = NetworkController.instance.JoinRoom(room, player);
        if (error != "") {
            ShowError(error);
        }
    }

    // Display all errors aside the main menu
    void ShowError(string text) {
        errorOut.text = text;
    }

    void OnDestroy() {
        // End the title screen music
        AudioController.instance.StopByID(musicID);
    }
}
