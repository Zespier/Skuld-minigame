using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Header("States")]
    public StateMachine stateMachine;
    public PatrolState patrolState;
    public AimToTarget aimtToTargetState;
    public ChargeAttackState chargeAttackState;

    [Header("Enemy properties")]
    public LayerMask layersToDetect;
    public Transform player;
    public SpriteRenderer playerSprite;
    public Rigidbody2D rB;
    public EnemyType type;

    public enum EnemyType { Static, Moveable, Flight }
    [HideInInspector] public Vector2 initialPos;
    private void OnValidate() {
        initialPos = transform.position;
    }
    void Start() {
        player = Transform.FindAnyObjectByType<PlayerController>().transform;

        stateMachine = new StateMachine();

        patrolState.enemy = this;
        aimtToTargetState.enemy = this;
        chargeAttackState.enemy = this;

        stateMachine.Initialize(patrolState);
    }
    void Update() {
        stateMachine.currentState.Update();

        playerSprite.flipX = rB.velocity.x > 0 ? true : false;

        if (rB.rotation < -90 || rB.rotation > 90) playerSprite.flipY = true; else playerSprite.flipY = false;
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

    public void DestroyObject() => Destroy(gameObject, 5f);
}
