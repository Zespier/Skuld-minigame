using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour {
    public float speed = 10f;
    public Rigidbody2D rB;
    public float offset = 20;

    private Vector3 initialPos;

    private void Start() {
        rB = GetComponent<Rigidbody2D>();
    }

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
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector2(initialPos.x - offset, initialPos.y));
    }
}
