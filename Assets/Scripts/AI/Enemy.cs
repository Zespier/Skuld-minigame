using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IHealth {

    [Header("States")]
    public StateMachine stateMachine;
    public PatrolState patrolState;
    public AimToTarget aimtToTargetState;
    public ChargeAttackState chargeAttackState;
    public IdleState idleState;
    public AttackState attackState;

    [Header("Enemy properties")]
    public EnemyType type;
    public EnemyStrength strength;
    public LayerMask layersToDetect;
    public SpriteRenderer enemySprite;
    public Rigidbody2D rB;
    public bool canAttack;

    public enum EnemyType { Static, StaticBig, Moveable, Flight }
    public enum EnemyStrength { Weak, Strong }
    [HideInInspector] public Vector2 initialPos;


    [Header("Health properties")]
    public int _currentHP;

    public int _maxHP;

    public int CurrentHP { get => _currentHP; set => _currentHP = value; }
    public int MaxHP { get => _maxHP; set => _maxHP = value; }

    [Header("Animations")]
    private Animator _animator;
    private string _lastAnimationName;

    [Header("Death Animations")]
    public Animator _deatAnimator;

    private Vector3 _initialPositionInModule;

    private void OnValidate() {
        initialPos = transform.position;
    }

    private void Awake() {
        enemySprite = GetComponentInChildren<SpriteRenderer>();
        rB = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();

        _initialPositionInModule = transform.localPosition;
    }

    private void Start() {

        stateMachine = new StateMachine();

        patrolState.enemy = this;
        aimtToTargetState.enemy = this;
        chargeAttackState.enemy = this;
        idleState.enemy = this;
        attackState.enemy = this;

        switch (type) {
            case EnemyType.Static:
            case EnemyType.StaticBig:
            case EnemyType.Moveable:
                stateMachine.Initialize(idleState);
                break;
            case EnemyType.Flight:
                stateMachine.Initialize(patrolState);
                break;
            default:
                break;
        }
    }
    private void Update() {
        stateMachine.currentState.Update();

        if (type != EnemyType.Static) {

            enemySprite.flipX = rB.velocity.x > 0 ? true : false;

            if (rB.rotation < -90 || rB.rotation > 90) enemySprite.flipY = true; else enemySprite.flipY = false;
        }
    }
    private void FixedUpdate() {
        stateMachine.currentState.PhysicsUpdate();
    }
    private void OnDrawGizmos() {
        if (PlayerController.instance != null && type == EnemyType.Flight) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, patrolState.detectionRadius);

            Gizmos.color = Color.red;

            for (int i = 0; i < patrolState.waypoints.Length; i++) {

                Vector2 path = initialPos + patrolState.waypoints[i];

                Gizmos.DrawWireSphere(path, 0.05f);
            }

            // Tiempo de predicci�n basado en la distancia entre el enemigo y el jugador, y el impulso
            float timeToTarget = Vector3.Distance(transform.position, PlayerController.instance.transform.position) / chargeAttackState.impulse * aimtToTargetState.predictionFactor;

            // Calcula la direcci�n predicha del jugador
            Vector3 predictedPosition = PlayerController.instance.transform.position + (PlayerController.instance.transform.right * timeToTarget + (Vector3)rB.velocity);

            // Calcula la direcci�n hacia la posici�n predicha
            Vector3 directionToPredictedPosition = predictedPosition - transform.position;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, directionToPredictedPosition);
        } else if (type == EnemyType.Static) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackState.attackingRadius);
        }

    }

    public void ReduceHp(int value) {
        _currentHP -= value;

        if (_currentHP <= 0) {
            Death();
        }
    }

    public void PlayAnimation(string animationName) {
        if (_lastAnimationName != animationName) {
            _animator.Play(animationName);
        }

        _lastAnimationName = animationName;
    }

    public void ResetEnemy() {
        enemySprite.enabled = true;
        _currentHP = _maxHP;

        transform.localPosition = _initialPositionInModule;
        switch (type) {
            case EnemyType.Static:
            case EnemyType.StaticBig:
            case EnemyType.Moveable:
                stateMachine.Initialize(idleState);
                break;
            case EnemyType.Flight:
                stateMachine.Initialize(patrolState);
                break;
            default:
                break;
        }
    }

    public void Death() {
        if (enemySprite.enabled) {
            _deatAnimator.Play("Ploof");
        }

        enemySprite.enabled = false;
        if (strength == EnemyStrength.Weak) {
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
        } else {
            Economy.instance.AddSoulCollectable(); // Me hace much�sima gracia verlo 20 veces seguidas, as� que se queda xd
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
            Economy.instance.AddSoulCollectable();
        }
    }
}
