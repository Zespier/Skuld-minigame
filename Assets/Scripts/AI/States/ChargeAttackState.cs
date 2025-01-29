using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChargeAttackState : State {

    public float impulse;
    public float distanceToChangeToAttackState = 3f;

    private Vector3 _initialPosition;

    public ChargeAttackState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine) {
    }

    public override void Enter() {
        enemy.PlayAnimation("Charge");

        _initialPosition = enemy.transform.position;
    }

    public override void Exit() {

    }

    public override void PhysicsUpdate() {

        enemy.rB.velocity = -enemy.transform.right * impulse;

        Vector3 attackPoint = enemy.attackState.attackPoint != null ? enemy.attackState.attackPoint.position : enemy.transform.position;

        if (enemy.canAttack && Vector2.Distance(attackPoint, PlayerController.instance.playerCenter.position) <= 0.3f) {
            enemy.stateMachine.ChangeState(enemy.attackState);
            PlayerController.instance.GetHit(enemy.gameObject);

        } else if (enemy.canAttack && DistanceSquared(_initialPosition, enemy.transform.position) > distanceToChangeToAttackState * distanceToChangeToAttackState) {
            enemy.stateMachine.ChangeState(enemy.attackState);
        }
    }

    public override void Update() {
    }

    public static float DistanceSquared(Vector3 a, Vector3 b) {
        return (a.x - b.x) * (a.x - b.x) +
               (a.y - b.y) * (a.y - b.y) +
               (a.z - b.z) * (a.z - b.z);
    }
}
