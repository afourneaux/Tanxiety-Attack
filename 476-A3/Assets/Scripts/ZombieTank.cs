using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTank : MonoBehaviour
{
    public float orbitRadius;
    float positionInOrbit = 0f;
    float direction = 1f;
    const float SLOWDOWN_RANGE = 0.5f;
    const float SPEED = 1f;
    const float FIRE_STRENGTH = 2.0f;
    const float WEAPON_COOLDOWN = 10f;
    const float WHEEL_ROTATE_SPEED = 720f;
    float cooldown = 0f;
    const float SLOWDOWN_STRENGTH = 0.8f;

    GameObject turret;
    GameObject bulletSpawnPoint;
    GameObject bulletTargetPoint;
    GameObject wheelFR;
    GameObject wheelFL;
    GameObject wheelBR;
    GameObject wheelBL;

    void Awake()
    {
        turret = transform.Find("Turret").gameObject;
        bulletSpawnPoint = transform.Find("Turret/RotatePoint/Turret/BulletSpawn").gameObject;
        bulletTargetPoint = transform.Find("Turret/RotatePoint/Turret/BulletSpawn/BulletTarget").gameObject;
        wheelFR = transform.Find("WheelFR").gameObject;
        wheelFL = transform.Find("WheelFL").gameObject;
        wheelBR = transform.Find("WheelBR").gameObject;
        wheelBL = transform.Find("WheelBL").gameObject;
    }

    void Update() {
        cooldown -= Time.deltaTime;
        float slowdown = 1f;

        float distanceToEnd = orbitRadius - direction * positionInOrbit;
        if (distanceToEnd <= SLOWDOWN_RANGE) {
            slowdown = (distanceToEnd / SLOWDOWN_RANGE * SLOWDOWN_STRENGTH) + (1f - SLOWDOWN_STRENGTH);
        }

        if (direction < 0 && positionInOrbit <= -orbitRadius) {
            direction = 1f;
        }
        else if (direction > 0 && positionInOrbit >= orbitRadius) {
            direction = -1f;
        }

        float movement = direction * slowdown * SPEED * Time.deltaTime;

        positionInOrbit += movement;

        transform.Translate(movement * Vector3.forward);

        wheelFR.transform.Rotate(Vector3.right * movement * WHEEL_ROTATE_SPEED);
        wheelFL.transform.Rotate(Vector3.right * movement * WHEEL_ROTATE_SPEED);
        wheelBR.transform.Rotate(Vector3.right * movement * WHEEL_ROTATE_SPEED);
        wheelBL.transform.Rotate(Vector3.right * movement * WHEEL_ROTATE_SPEED);
    }

    public void OnSpot(Vector3 position) {
        turret.transform.LookAt(position, Vector3.up);
        if (NetworkController.instance.IsMaster() && cooldown <= 0) {
            GameObject cannonball = NetworkController.instance.SpawnNetworkedObject("Cannonball", bulletSpawnPoint.transform.position, Quaternion.identity);
            cannonball.transform.LookAt(bulletTargetPoint.transform);
            cannonball.GetComponent<Cannonball>().Fire(FIRE_STRENGTH);
            cannonball.GetComponentInChildren<CannonballCollider>().isZombie = true;
            cooldown = WEAPON_COOLDOWN;
        }
    }
}
