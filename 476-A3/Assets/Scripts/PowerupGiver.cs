using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupGiver : MonoBehaviour
{
    public POWERUP type;

    void OnCollisionEnter(Collision collision) {
        OnTriggerEnter(collision.collider);
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer != 6) {   // Ignore all but player tanks
            return;
        }

        // When a tank bumps a powerup, assign the powerup to that tank
        Tank tank = collider.GetComponentInParent<Tank>();
        tank.Powerup(type);
        
        gameObject.SetActive(false);
    }
}
