using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IdleState : State {

    public IdleState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine) {
    }

    public override void Enter() {
        enemy.PlayAnimation("Idle");
    }

    public override void Exit() {

    }

    public override void PhysicsUpdate() {
        if (Physics2D.OverlapCircle(enemy.transform.position, enemy.attackState.attackingRadius) && enemy.canAttack) {
            enemy.stateMachine.ChangeState(enemy.attackState);
        } else if (Mathf.Abs((PlayerController.instance.transform.position - enemy.transform.position).x) < 4) {
            enemy.stateMachine.ChangeState(enemy.chargeAttackState);
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
