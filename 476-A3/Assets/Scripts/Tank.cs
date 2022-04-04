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
    const float POWERUP_DURATION = 20.0f;
    const float SOUND_DEADZONE = 0.2f;
    const float SOUND_REDUCTION = 0.2f;

    float turretSwivel = 0f;
    float turretRise = TURRET_MAX_RISE;
    public float cannonCharge = 0f;
    public float cooldown = 0f;

    public float reloadPowerupCountdown = 0f;
    public float invincibilityPowerupCountdown = 0f;
    public float speedPowerupCountdown = 0f;

    string engineAudio;
    string swivelAudio;

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

    public PlayerManager playerManager;

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
        if (speedPowerupCountdown >= 0) {
            speedPowerupCountdown -= Time.deltaTime;
        }
        if (reloadPowerupCountdown >= 0) {
            reloadPowerupCountdown -= Time.deltaTime;
        }
        if (invincibilityPowerupCountdown >= 0) {
            invincibilityPowerupCountdown -= Time.deltaTime;
        }

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

                if (reloadPowerupCountdown <= 0f) {
                    cooldown = WEAPON_COOLDOWN;
                }
            }
        } else {
            if (reloadPowerupCountdown > 0f) {
                cooldown = 0f;
            } else {
                cooldown -= Time.deltaTime;
            }

            if (cooldown <= 0f) {
                AudioController.instance.PlaySound(5);
            }
        }
    }

    void FixedUpdate() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float rise = Input.GetAxis("TurretRise");
        float swivel = Input.GetAxis("TurretSwivel");

        float soundModifier = photonView.IsMine ? 1f : SOUND_REDUCTION;

        if (Mathf.Abs(vertical) >= SOUND_DEADZONE) {
            if (engineAudio == null) {
                engineAudio = AudioController.instance.PlaySound(1, true, soundModifier);
            }
        } else {
            if (engineAudio != null) {
                AudioController.instance.StopByID(engineAudio);
                engineAudio = null;
            }
        }

        if (Mathf.Abs(swivel) >= SOUND_DEADZONE) {
            if (swivelAudio == null) {
                swivelAudio = AudioController.instance.PlaySound(6, true, soundModifier);
            }
        } else {
            if (swivelAudio != null) {
                AudioController.instance.StopByID(swivelAudio);
                swivelAudio = null;
            }
        }

        if (photonView.IsMine == false) {
            return;
        }

        turretSwivel += swivel * TURRET_SWIVEL_SPEED * Time.fixedDeltaTime;
        turretRise += rise * TURRET_RISE_SPEED * Time.fixedDeltaTime;
        float forward = vertical * MOVE_SPEED * Time.fixedDeltaTime;
        if (speedPowerupCountdown > 0f) {
            forward *= 2;
        }
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
        if (invincibilityPowerupCountdown > 0f) {
            return;
        }
        NetworkController.instance.RespawnTank(photonView.Owner);
        
        NetworkController.instance.DestroyOtherPlayersObject(photonView.ViewID);
    }

    public void OnDestroy() {
        AudioController.instance.StopByID(engineAudio);
        AudioController.instance.StopByID(swivelAudio);
    }

    public void Powerup(POWERUP type) {
        switch(type) {
            case POWERUP.RELOAD:
            reloadPowerupCountdown += POWERUP_DURATION;
            break;
            case POWERUP.SHIELD:
            invincibilityPowerupCountdown += POWERUP_DURATION;
            break;
            case POWERUP.SPEED:
            speedPowerupCountdown += POWERUP_DURATION;
            break;
            case POWERUP.XRAY:
            if (photonView.IsMine) {
                MainController.instance.score++;
            }
            break;
        }
    } 
}
