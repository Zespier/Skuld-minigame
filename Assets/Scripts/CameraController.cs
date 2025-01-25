using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform player;
    public Vector3 offset = new Vector3(2, 0, 0);
    public bool followPlayerHeight;

    private Vector3 _defaultPosition;

    private void Awake() {
        _defaultPosition = player.transform.position;
    }

    private void FixedUpdate() {
        Movement();
    }

    private void Movement() {
        if (followPlayerHeight) {
            transform.position = Vector3.Lerp(transform.position, player.position + offset, Time.deltaTime * 10f);

        } else {
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, _defaultPosition.y, 0) + offset, Time.deltaTime * 10f);
        }
    }
}
