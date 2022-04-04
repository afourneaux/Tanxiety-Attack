using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballCollider : MonoBehaviour
{
    public bool isZombie = false;

    void OnCollisionEnter(Collision collision) {
        OnTriggerEnter(collision.collider);
    }

    void OnTriggerEnter(Collider collider) {
        // Ignore zombies
        if (collider.gameObject.layer == 9) {
            return;
        }
        // Ignore if I'm shooting an object that belongs to me
        if (isZombie == false && NetworkController.instance.CheckIsMine(gameObject) == false) {
            return;
        }

        // Hit a tank
        Tank tankHit = collider.GetComponentInParent<Tank>();
        if (tankHit != null) {
            // Hit ourselves - ignore
            if (isZombie == false && tankHit == PlayerManager.instance.tank) {
                return;
            }
            // Hit another player
            if (isZombie == false) {
                MainController.instance.score++;
            }
            tankHit.OnHit();
        }

        // Hit destructible terrain
        DestructibleTerrain terrainHit = collider.GetComponentInParent<DestructibleTerrain>();
        if (terrainHit != null) {
            NetworkController.instance.DestroyTerrain(terrainHit, transform.position);
        }

        // Regardless of what was hit, destroy this cannonball
        AudioController.instance.PlaySound(3);
        NetworkController.instance.DestroyNetworkedObject(gameObject.transform.parent.gameObject);
    }
}
