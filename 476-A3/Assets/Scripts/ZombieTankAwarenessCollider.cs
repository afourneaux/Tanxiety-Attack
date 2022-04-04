using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTankAwarenessCollider : MonoBehaviour
{
    ZombieTank tank;

    void OnEnable() {
        tank = GetComponentInParent<ZombieTank>();
    }

    void OnCollisionStay(Collision collision) {
        OnTriggerStay(collision.collider);
    }

    void OnTriggerStay(Collider collider) {
        if (collider.gameObject.layer == 6) {
            if (collider.GetComponentInParent<Tank>() != null) {
                tank.OnSpot(collider.transform.position);
            }
        }
    }
}
