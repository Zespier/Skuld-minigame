using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class PatrolState : State {

    public float speed;
    public float detectionRadius;
    public Vector2[] waypoints;
    public Vector2 lastPos;

    private Vector3 currentPos;
    private bool closeToWayPoint;
    public int currentWaypointIndex;

    public PatrolState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine) {
    }

    public override void Enter() {
        Debug.Log("Patrol");

    }

    public override void Exit() {
        lastPos = currentPos;
        enemy.rB.velocity = Vector2.zero;
    }

    public override void PhysicsUpdate() {

    }

    public override void Update() {
        PatrolWaypoints(waypoints, speed, detectionRadius);

        if (Physics2D.OverlapCircle(enemy.transform.position, detectionRadius, enemy.layersToDetect)) {

            enemy.stateMachine.ChangeState(enemy.aimtToTargetState);
        }
    }

    public void PatrolWaypoints(Vector2[] waypoints, float speed, float detectionRadius) {
        // Obtén la posición actual
        currentPos = enemy.rB.position;

        // Si no hay waypoints, no hacemos nada
        if (waypoints.Length == 0) return;

        // Obtén la posición del waypoint actual
        Vector2 targetWaypoint = waypoints[currentWaypointIndex] + enemy.initialPos;

        // Calcula la distancia al waypoint actual
        float distanceToWaypoint = Vector2.Distance(currentPos, targetWaypoint);
        Debug.Log(distanceToWaypoint);

        // Verifica si el enemigo está cerca del waypoint
        if (distanceToWaypoint <= 0.1f) {
            // Detén el movimiento y pasa al siguiente waypoint
            enemy.rB.velocity = Vector2.zero;

            // Avanza al siguiente waypoint en el array
            currentWaypointIndex++;

            // Si llegamos al final del array, vuelve al inicio (loop)
            if (currentWaypointIndex >= waypoints.Length) {
                currentWaypointIndex = 0;
            }

        } else {
            // Calcula la dirección hacia el waypoint
            Vector2 direction = (targetWaypoint - (Vector2)currentPos).normalized;

            // Ajusta la velocidad para moverse hacia el waypoint
            enemy.rB.velocity = direction * speed;
        }
    }
}
