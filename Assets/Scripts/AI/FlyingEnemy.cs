using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour {
    public float speed = 10f;
    public Rigidbody2D rB;
    public float offset = 20;
    public float attackRadius = 0.3f;
    public Transform attackPoint;

    private Vector3 initialPos;

    public float LeftCameraLimit => CameraController.instance.transform.position.x - ModuleContainer.instance.mainCamera.orthographicSize * ModuleContainer.instance.mainCamera.aspect;

    private void OnEnable() {
        initialPos = transform.position;
    }
    private void Update() {
        rB.velocity = -(Vector2)transform.right * speed * Time.deltaTime;
        if (transform.position.x < LeftCameraLimit - 2f) {
            rB.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        if (DistanceSquared(PlayerController.instance.playerCenter.position, attackPoint.position) < attackRadius * attackRadius) {
            PlayerController.instance.GetHit(gameObject);
        }
    }

    public static float DistanceSquared(Vector3 a, Vector3 b) {
        return (a.x - b.x) * (a.x - b.x) +
               (a.y - b.y) * (a.y - b.y) +
               (a.z - b.z) * (a.z - b.z);
    }
}
