using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AimToTarget : State {
    public float turnSpeed;

    public float cooldownToAttack;
    private float timer;

    public AimToTarget(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine) {
    }

    public override void Enter() {
        Debug.Log("Aim");

        timer = cooldownToAttack;
    }

    public override void Exit() {

    }

    public override void PhysicsUpdate() {

    }

    public override void Update() {

        // Calcula la dirección hacia el jugador
        Vector3 directionToPlayer = enemy.player.position - enemy.transform.position;

        directionToPlayer.z = 0;

        // Calcula el ángulo de rotación necesario para apuntar hacia el jugador
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // Creamos un quaternion de rotación en el plano 2D alrededor del eje Z
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Realiza una suave rotacion entre la rotación actual y la rotación objetivo
        enemy.transform.rotation = Quaternion.Slerp(
            enemy.transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        if (timer <= 0) {
            enemy.stateMachine.ChangeState(enemy.chargeAttackState);
        } else {
            timer -= Time.deltaTime;
        }
    }
}
