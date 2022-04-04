using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    GameObject GameUI;
    GameObject PlayerListGO;
    GameObject PlayerManagerGO;
    GameObject ColourPreviewGO;
    public GameObject PlayerLinePrefab;
    int playerCount = 0;
    Color colour = Color.HSVToRGB(1f, 1f, 1f);

    void Start() {
        PlayerManagerGO = transform.Find("/Players").gameObject;
        ColourPreviewGO = transform.Find("RightSide/ColourDisplay").gameObject;
        PlayerListGO = transform.Find("LeftSide/PlayerList").gameObject;
        GameUI = transform.parent.Find("GameUI").gameObject;
        
        ColourPreviewGO.GetComponent<Image>().color = colour;
    }

    void Update() {

        if (NetworkController.instance.IsConnected()) {
            if (PlayerManager.isDirty) {
                Debug.Log("UpdateUI");
                foreach (Transform child in PlayerListGO.transform) {
                    Destroy(child.gameObject);
                }

                bool startGame = true;

                foreach (PlayerManager pm in PlayerManager.AllPlayers) {
                    GameObject lobbyLineGO = Instantiate(PlayerLinePrefab, Vector3.zero, Quaternion.identity, PlayerListGO.transform);
                    lobbyLineGO.transform.Find("PlayerName").GetComponent<TMPro.TMP_Text>().text = pm.player.NickName;
                    lobbyLineGO.transform.Find("ReadyStatus").GetComponent<TMPro.TMP_Text>().text = pm.isReady ? "Ready!" : "Not Ready";
                    lobbyLineGO.transform.Find("ColourDisplay").GetComponent<Image>().color = pm.colour;
                    if (pm.isReady == false) {
                        startGame = false;
                    }
                }
                PlayerManager.isDirty = false;

                if (PlayerManager.AllPlayers.Count > 0 && startGame) {
                    MainController.instance.SpawnTank();
                    GameUI.SetActive(true);
                    Destroy(gameObject);
                }
            }
        }
    }

    public void UpdateColour(Color newColour) {
        colour = newColour;
        PlayerManager.instance.colour = newColour;
        ColourPreviewGO.GetComponent<Image>().color = newColour;
    }

    public void Ready() {
        AudioController.instance.PlaySound(0);
        PlayerManager.instance.isReady = true;
    }
}
