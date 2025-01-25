using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AimToTarget : State {
    public float turnSpeed;
    public float predictionFactor;

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
        // Tiempo de predicci�n basado en la distancia entre el enemigo y el jugador, y el impulso
        float timeToTarget = Vector3.Distance(enemy.transform.position, enemy.player.transform.position) / enemy.chargeAttackState.impulse * predictionFactor;

        // Calcula la direcci�n predicha del jugador
        Vector3 predictedPosition = enemy.player.transform.position + (enemy.player.transform.right * timeToTarget + (Vector3)enemy.rB.velocity);

        // Calcula la direcci�n hacia la posici�n predicha
        Vector3 directionToPredictedPosition = predictedPosition - enemy.transform.position;

        // Calcula la direcci�n hacia el jugador
        //Vector3 directionToPlayer = enemy.player.position + (Vector3)enemy.rB.velocity * predictionFactor - enemy.transform.position;

        //directionToPredictedPosition.x += enemy.rB.velocity.x - (enemy.chargeAttackState.impulse);
        directionToPredictedPosition.z = 0;

        // Calcula el �ngulo de rotaci�n necesario para apuntar hacia el jugador
        float angle = Mathf.Atan2(directionToPredictedPosition.y, directionToPredictedPosition.x) * Mathf.Rad2Deg;

        // Creamos un quaternion de rotaci�n en el plano 2D alrededor del eje Z
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Realiza una suave rotacion entre la rotaci�n actual y la rotaci�n objetivo
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
