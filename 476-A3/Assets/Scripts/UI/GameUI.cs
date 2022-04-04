using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public TMPro.TMP_Text scoreText;
    public TMPro.TMP_Text reloadText;
    public TMPro.TMP_Text chargeText;
    public TMPro.TMP_Text speedText;
    public TMPro.TMP_Text shieldText;
    public TMPro.TMP_Text freeloadText;

    void Start() {
        scoreText = transform.Find("ScoreText").GetComponent<TMPro.TMP_Text>();
        reloadText = transform.Find("ReloadText").GetComponent<TMPro.TMP_Text>();
        chargeText = transform.Find("ChargeText").GetComponent<TMPro.TMP_Text>();
        speedText = transform.Find("SpeedText").GetComponent<TMPro.TMP_Text>();
        shieldText = transform.Find("ShieldText").GetComponent<TMPro.TMP_Text>();
        freeloadText = transform.Find("FreeloadText").GetComponent<TMPro.TMP_Text>();
    }

    void Update() {
        // Update all text fields with the current player status
        scoreText.text = "Score: " + MainController.instance.score;
        if (PlayerManager.instance != null) {
            reloadText.text = string.Format("Reload: {0:0.0}", PlayerManager.instance.tank.cooldown);
            chargeText.text = string.Format("Charge: {0:0.0}", PlayerManager.instance.tank.cannonCharge);
            if (PlayerManager.instance.tank != null) {
                // Update all powerup statuses or clear the field if empty
                if (PlayerManager.instance.tank.speedPowerupCountdown >= 0) {
                    speedText.gameObject.SetActive(true);
                    speedText.text = string.Format("2x Speed: {0:0.0}", PlayerManager.instance.tank.speedPowerupCountdown);
                } else {
                    speedText.gameObject.SetActive(false);
                }
                if (PlayerManager.instance.tank.invincibilityPowerupCountdown >= 0) {
                    freeloadText.gameObject.SetActive(true);
                    shieldText.text = string.Format("Invincible: {0:0.0}", PlayerManager.instance.tank.invincibilityPowerupCountdown);
                } else {
                    shieldText.gameObject.SetActive(false);
                }
                if (PlayerManager.instance.tank.reloadPowerupCountdown >= 0) {
                    freeloadText.gameObject.SetActive(true);
                    freeloadText.text = string.Format("Free Reloads: {0:0.0}", PlayerManager.instance.tank.reloadPowerupCountdown);
                } else {
                    freeloadText.gameObject.SetActive(false);
                }
            }
        }
    }
}
