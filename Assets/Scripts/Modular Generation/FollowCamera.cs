using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    public float movementPercentage = 20f;

    private Vector3 _lastCameraPosition;

    private void Start() {
        _lastCameraPosition = CameraController.instance.transform.position;
    }

    private void Update() {
        Vector3 distanceTraveled = 0.01f * (100 - movementPercentage) * (CameraController.instance.transform.position - _lastCameraPosition);

        transform.position += distanceTraveled;

        _lastCameraPosition = CameraController.instance.transform.position;
    }

    public void ResetVariables() {
        _lastCameraPosition = CameraController.instance.transform.position;
    }
}
