using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Tank : MonoBehaviourPunCallbacks, IPunObservable
{
    public Material CamoMaterial;
    public Color colour;

    const float TURRET_SWIVEL_SPEED = 45f;
    const float TURRET_RISE_SPEED = 45f;
    const float TURRET_MAX_RISE = 80f;
    const float TURRET_MIN_RISE = 45f;
    const float MOVE_SPEED = 100f;
    const float ROTATE_SPEED = 0.5f;
    const float WHEEL_SPIN_SPEED = 360f;
    const float WEAPON_COOLDOWN = 5f;
    const float CHARGE_SPEED = 1f;
    const float MAX_CHARGE = 2f;

    float turretSwivel = 0f;
    float turretRise = TURRET_MAX_RISE;
    float cannonCharge = 0f;
    float cooldown = 0f;

    GameObject body;
    GameObject turret;
    GameObject rotatePoint;
    GameObject bulletSpawnPoint;
    GameObject bulletTargetPoint;
    GameObject wheelFR;
    GameObject wheelFL;
    GameObject wheelBR;
    GameObject wheelBL;
    Rigidbody rb;

    void Awake()
    {
        body = transform.Find("Body").gameObject;
        turret = transform.Find("Turret").gameObject;
        rotatePoint = transform.Find("Turret/RotatePoint").gameObject;
        bulletSpawnPoint = transform.Find("Turret/RotatePoint/Turret/BulletSpawn").gameObject;
        bulletTargetPoint = transform.Find("Turret/RotatePoint/Turret/BulletSpawn/BulletTarget").gameObject;
        wheelFR = transform.Find("WheelFR").gameObject;
        wheelFL = transform.Find("WheelFL").gameObject;
        wheelBR = transform.Find("WheelBR").gameObject;
        wheelBL = transform.Find("WheelBL").gameObject;
        rb = GetComponent<Rigidbody>();
    }

    void Start() {
        if (photonView.IsMine) {
            turret.AddComponent<CameraFollow>();
        }
    }

    void Update()
    {
        if (photonView.IsMine == false) {
            return;
        }

        if (cooldown <= 0f) {
            if (Input.GetKey(KeyCode.Space)) {
                cannonCharge = Mathf.Min(cannonCharge + CHARGE_SPEED * Time.deltaTime, MAX_CHARGE);
            }

            if (Input.GetKeyUp(KeyCode.Space)) {
                GameObject cannonball = NetworkController.instance.SpawnNetworkedObject("Cannonball", bulletSpawnPoint.transform.position, Quaternion.identity);
                cannonball.transform.LookAt(bulletTargetPoint.transform);
                cannonball.GetComponent<Cannonball>().Fire(cannonCharge);
                cannonCharge = 0f;
                cooldown = WEAPON_COOLDOWN;
            }
        } else {
            cooldown -= Time.deltaTime;
        }
    }

    void FixedUpdate() {
        if (photonView.IsMine == false) {
            return;
        }
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float rise = Input.GetAxis("TurretRise");
        float swivel = Input.GetAxis("TurretSwivel");

        turretSwivel += swivel * TURRET_SWIVEL_SPEED * Time.fixedDeltaTime;
        turretRise += rise * TURRET_RISE_SPEED * Time.fixedDeltaTime;
        float forward = vertical * MOVE_SPEED * Time.fixedDeltaTime;
        float rotate = horizontal * ROTATE_SPEED * Time.fixedDeltaTime;
        float wheelRotate = vertical * WHEEL_SPIN_SPEED * Time.fixedDeltaTime;
        float wheelRotateRotateModifier = horizontal * WHEEL_SPIN_SPEED / 2 * Time.fixedDeltaTime;

        turretSwivel %= 360;
        rotate %= 360;
        wheelRotate %= 360;
        turretRise = Mathf.Clamp(turretRise, TURRET_MIN_RISE, TURRET_MAX_RISE);

        turret.transform.rotation = Quaternion.AngleAxis(turretSwivel, Vector3.up);
        rotatePoint.transform.rotation = Quaternion.AngleAxis(turretRise, turret.transform.right);
        wheelFL.transform.rotation *= Quaternion.AngleAxis(wheelRotate + wheelRotateRotateModifier, Vector3.right);
        wheelFR.transform.rotation *= Quaternion.AngleAxis(wheelRotate - wheelRotateRotateModifier, Vector3.right);
        wheelBL.transform.rotation *= Quaternion.AngleAxis(wheelRotate + wheelRotateRotateModifier, Vector3.right);
        wheelBR.transform.rotation *= Quaternion.AngleAxis(wheelRotate - wheelRotateRotateModifier, Vector3.right);
        rb.AddForce(forward * transform.forward);
        rb.AddTorque(rotate * Vector3.up);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }

    public void SetColour(Color colour) {
        Material mat = Instantiate(CamoMaterial) as Material;
        mat.color = colour;
        body.GetComponent<Renderer>().material = mat;
        turret.GetComponent<Renderer>().material = mat;
    }

    public void OnHit() {
        NetworkController.instance.DestroyOtherPlayersObject(photonView.ViewID);
    }
}
