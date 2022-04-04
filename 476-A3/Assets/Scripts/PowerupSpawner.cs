using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    const float SPAWN_DELAY = 30f;
    const float SPEED = 5f;
    const float ORBIT_RADIUS = 0.05f;
    float positionInOrbit = 0f;
    bool isSpawning = false;
    Vector3 baseLocation;
    float spawnTimer = 0f;

    void Start() {
        baseLocation = transform.position;
    }

    // Spawner sits as a parent to a powerup and manages its oscillation as well as re-enabling it
    // after it has been collected
    void Update() {
        positionInOrbit += Time.deltaTime * SPEED;
        transform.position = baseLocation + Vector3.up * Mathf.Sin(positionInOrbit) * ORBIT_RADIUS;

        if (!isSpawning && transform.Find("Capsule").gameObject.activeInHierarchy == false) {
            isSpawning = true;
        }

        if (isSpawning) {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= SPAWN_DELAY) {
                transform.Find("Capsule").gameObject.SetActive(true);
                isSpawning = false;
                spawnTimer = 0f;
            }
        }
    }
}
