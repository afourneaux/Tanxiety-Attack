using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Camera mainCamera;
    const float CAMERA_MAX_DISTANCE = 0.2f;
    const float CAMERA_HEIGHT = 0.05f;
    const float CAMERA_MIN_DISTANCE = 0.1f;
    const float CAMERA_SPEED = 0.5f;
    const float CAMERA_ANGLE = 20f;
    const float CAMERA_GRANULARITY = 0.05f;
    float cameraDistance = CAMERA_MAX_DISTANCE;
    float desiredDistance = CAMERA_MAX_DISTANCE;

    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update() {
        if (cameraDistance > desiredDistance) {
            cameraDistance = Mathf.Max(desiredDistance, cameraDistance - CAMERA_SPEED * Time.fixedTime);
        } else {
            cameraDistance = Mathf.Min(desiredDistance, cameraDistance + CAMERA_SPEED * Time.fixedTime);
        }

        mainCamera.transform.position = transform.position - (mainCamera.transform.forward * cameraDistance) + (mainCamera.transform.up * CAMERA_HEIGHT);
    }

    void FixedUpdate()
    {
        mainCamera.transform.rotation = transform.rotation * Quaternion.AngleAxis(CAMERA_ANGLE, new Vector3(1f, 0f, 0f));

        GetDesiredCameraDistance();
    }

    void GetDesiredCameraDistance() {
        float lastDistance;
        Vector3 positionCandidate =  transform.position - mainCamera.transform.forward * cameraDistance;
        Vector3 cameraToObject = positionCandidate - transform.position;
        float distance = lastDistance = cameraDistance;

        if (Physics.Raycast(mainCamera.transform.position, cameraToObject, cameraToObject.magnitude) == true) {
            if (distance <= CAMERA_MIN_DISTANCE) {
                return;
            }
            do {
                lastDistance = distance;
                positionCandidate += mainCamera.transform.forward * CAMERA_GRANULARITY;
                distance -= CAMERA_GRANULARITY;
                cameraToObject = positionCandidate - transform.position;
            } while (distance > CAMERA_MIN_DISTANCE && Physics.Raycast(positionCandidate, cameraToObject, cameraToObject.magnitude) == true);
            if (distance <= CAMERA_MIN_DISTANCE) {
                lastDistance = CAMERA_MIN_DISTANCE;
            }
        } else {
            if (distance >= CAMERA_MAX_DISTANCE) {
                return;
            }
            do {
                lastDistance = distance;
                positionCandidate -= mainCamera.transform.forward * CAMERA_GRANULARITY;
                distance += CAMERA_GRANULARITY;
                cameraToObject = positionCandidate - transform.position;
            } while (distance < CAMERA_MAX_DISTANCE && Physics.Raycast(positionCandidate, cameraToObject, cameraToObject.magnitude) == false);
            if (distance >= CAMERA_MAX_DISTANCE) {
                lastDistance = CAMERA_MAX_DISTANCE;
            }
        }

        desiredDistance = lastDistance;
    }
}
