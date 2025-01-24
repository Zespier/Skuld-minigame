using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float maxSpeed = 4f;
    public float acceleration = 1.5f;
    public float jumpSpeed = 5f;
    public Rigidbody2D rb;

    private float _currentSpeed;

    private void Update() {
        Accelerate();
        Movement();

        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }
    }

    private void Accelerate() {
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, maxSpeed, acceleration * Time.deltaTime);
    }

    private void Movement() {
        Vector3 velocity = rb.velocity;
        velocity.x = _currentSpeed;
        rb.velocity = velocity;
    }

    private void Jump() {
        Vector3 velocity = rb.velocity;
        velocity.y = jumpSpeed;
        rb.velocity = velocity;
    }
}
