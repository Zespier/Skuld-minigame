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

    private void OnEnable() {
        initialPos = transform.position;
    }
    private void Update() {
        if (transform.position.x >= initialPos.x - offset) {
            rB.velocity -= (Vector2)transform.right * speed * Time.deltaTime;
        } else {
            rB.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        if (DistanceSquared(PlayerController.instance.playerCenter.position, attackPoint.position) < attackRadius * attackRadius) {
            PlayerController.instance.GetHit();
        }
    }

    public static float DistanceSquared(Vector3 a, Vector3 b) {
        return (a.x - b.x) * (a.x - b.x) +
               (a.y - b.y) * (a.y - b.y) +
               (a.z - b.z) * (a.z - b.z);
    }
}
