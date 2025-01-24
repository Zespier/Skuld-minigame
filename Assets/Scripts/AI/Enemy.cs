using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Header("States")]
    public StateMachine stateMachine;
    public PatrolState patrolState;
    public AimToTarget aimtToTargetState;

    [Header("Enemy variables")]
    public LayerMask layersToDetect;
    public SpriteRenderer sprite;
    public Rigidbody2D rB;


    [HideInInspector] public Vector2 initialPos;
    private void OnValidate() {
        initialPos = transform.position;
    }
    void Start() {
        stateMachine = new StateMachine();

        patrolState.enemy = this;
        aimtToTargetState.enemy = this;

        stateMachine.Initialize(patrolState);
    }
    void Update() {
        stateMachine.currentState.Update();

        sprite.flipX = rB.velocity.x > 0 ? true : false;

        if (rB.rotation < -90 || rB.rotation > 90) sprite.flipY = true; else sprite.flipY = false;
    }
    private void FixedUpdate() {
        stateMachine.currentState.PhysicsUpdate();
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolState.detectionRadius);

        Gizmos.color = Color.red;

        for (int i = 0; i < patrolState.waypoints.Length; i++) {

            Vector2 path = initialPos + patrolState.waypoints[i];

            Gizmos.DrawWireSphere(path, 0.2f);
        }
    }
}
