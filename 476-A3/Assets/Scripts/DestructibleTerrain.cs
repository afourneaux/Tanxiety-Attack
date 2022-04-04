using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleTerrain : MonoBehaviour
{
    bool isHit;
    const float GRAVITY = 5.0f;
    const float MAX_FALL_ANGLE = 75f;
    float rotateSpeed = 0.0f;
    float angle = 0f;
    float fallDirection = 1f;

    // Disable collisions and start the process of the terrain falling over and eventually being destroyed
    public void OnHit(bool fallBackwards) {
        isHit = true;

        fallDirection = fallBackwards ? -1f : 1f;
        GetComponent<BoxCollider>().enabled = false;
    }

    void Update() {
        if (isHit == true) {
            rotateSpeed += GRAVITY * Time.deltaTime;
            angle += rotateSpeed;
            transform.Rotate(Vector3.right * rotateSpeed * fallDirection);

            if (angle >= MAX_FALL_ANGLE) {
                Destroy(gameObject);
            }
        }
    }
}
