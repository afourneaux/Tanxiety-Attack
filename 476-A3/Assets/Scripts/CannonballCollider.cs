using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballCollider : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {
        OnTriggerEnter(collision.collider);
    }

    void OnTriggerEnter(Collider collider) {
        Debug.Log(collider.name);

        if (NetworkController.instance.CheckIsMine(gameObject) == false) {
            return;
        }

        Tank tankHit = collider.GetComponentInParent<Tank>();
        if (tankHit != null) {
            if (tankHit == PlayerManager.instance.tank) {
                // Hit ourselves - ignore
                return;
            }
            // Hit another player
            tankHit.OnHit();
        }
        DestructibleTerrain terrainHit = collider.GetComponentInParent<DestructibleTerrain>();
        if (terrainHit != null) {
            NetworkController.instance.DestroyTerrain(terrainHit, transform.position);
        }

        // Regardless of what was hit, destroy this cannonball
        NetworkController.instance.DestroyNetworkedObject(gameObject.transform.parent.gameObject);
    }
}
