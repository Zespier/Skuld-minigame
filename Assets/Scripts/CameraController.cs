using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform player;
    public Vector3 offset = new Vector3(2, 0, 0);
    public float gameplayHeightOffset = 2.82f;
    public float idleHeightOffset = -2.56f;
    public bool followPlayerHeight;

    private Vector3 _defaultPosition;

    public bool IsInIdleSide => player.transform.position.y < -1.5f;
    public float HeightOffset => IsInIdleSide ? idleHeightOffset : gameplayHeightOffset;

    public static CameraController instance;
    private void Awake() {
        if (!instance) { instance = this; }
        _defaultPosition = player.transform.position;
    }

    private void FixedUpdate() {
        Movement();
    }

    private void Movement() {
        if (followPlayerHeight) {
            transform.position = Vector3.Lerp(transform.position, player.position , Time.deltaTime * 10f);

        } else {
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, HeightOffset, 0) + offset, Time.deltaTime * 10f);
        }
    }
}
