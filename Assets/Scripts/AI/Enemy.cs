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
    public EnemyType type;
    public LayerMask layersToDetect;
    public SpriteRenderer enemySprite;
    public PlayerController player;
    public Rigidbody2D rB;

    public enum EnemyType { Static, Moveable, Flight }
    [HideInInspector] public Vector2 initialPos;
    private void OnValidate() {
        initialPos = transform.position;
    }
    private void OnEnable() {
        player = Transform.FindAnyObjectByType<PlayerController>();
        enemySprite = GetComponentInChildren<SpriteRenderer>();
        rB = GetComponent<Rigidbody2D>();
    }
    void Start() {

        stateMachine = new StateMachine();

        patrolState.enemy = this;
        aimtToTargetState.enemy = this;
        chargeAttackState.enemy = this;

        stateMachine.Initialize(patrolState);
    }
    void Update() {
        stateMachine.currentState.Update();

        enemySprite.flipX = rB.velocity.x > 0 ? true : false;

        if (rB.rotation < -90 || rB.rotation > 90) enemySprite.flipY = true; else enemySprite.flipY = false;
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

            Gizmos.DrawWireSphere(path, 0.05f);
        }
        if (player == null) return;
        // Tiempo de predicción basado en la distancia entre el enemigo y el jugador, y el impulso
        float timeToTarget = Vector3.Distance(transform.position, player.transform.position) / chargeAttackState.impulse * aimtToTargetState.predictionFactor;

        // Calcula la dirección predicha del jugador
        Vector3 predictedPosition = player.transform.position + (player.transform.right * timeToTarget + (Vector3)rB.velocity);

        // Calcula la dirección hacia la posición predicha
        Vector3 directionToPredictedPosition = predictedPosition - transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, directionToPredictedPosition);

    }

    public void DestroyObject() => Destroy(gameObject, 5f);
}
