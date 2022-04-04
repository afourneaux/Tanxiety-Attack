using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    Vector3 direction;
    float force;
    float downwardForce = 0f;
    const float GRAVITY = 0.1f;
    const float SOUND_REDUCTION = 0.2f;

    public void Fire(float impulse) {
        force = impulse;
    }

    void Start() {
        float soundModifier = NetworkController.instance.CheckIsMine(gameObject) ? 1f : SOUND_REDUCTION;
        AudioController.instance.PlaySound(2, false, soundModifier);
    }

    void Update() {
        if (NetworkController.instance.CheckIsMine(gameObject) == false) {
            return;
        }
        
        downwardForce += GRAVITY * Time.deltaTime;
        transform.Translate((Vector3.forward * force * Time.deltaTime) + (Vector3.down * downwardForce * Time.deltaTime));
    }
}
