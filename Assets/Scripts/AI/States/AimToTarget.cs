using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AimToTarget : State {
    public float turnSpeed;
    public Transform player;

    public float cooldownToAttack;

    public AimToTarget(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine) {
    }

    public override void Enter() {
        Debug.Log("Aim");

        player = Transform.FindAnyObjectByType<PlayerController>().transform;
    }

    public override void Exit() {

    }

    public override void PhysicsUpdate() {

    }

    public override void Update() {

        // Calcula la direcci�n hacia el jugador
        Vector3 directionToPlayer = player.position - enemy.transform.position;

        directionToPlayer.z = 0;

        // Calcula el �ngulo de rotaci�n necesario para apuntar hacia el jugador
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // Creamos un quaternion de rotaci�n en el plano 2D alrededor del eje Z
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Realiza una suave rotacion entre la rotaci�n actual y la rotaci�n objetivo
        enemy.transform.rotation = Quaternion.Slerp(
            enemy.transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }
}
