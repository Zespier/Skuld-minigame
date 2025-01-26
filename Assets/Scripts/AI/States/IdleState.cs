using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IdleState : State {
    public IdleState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine) {
    }

    public override void Enter() {
        Debug.Log("Quietou");
    }

    public override void Exit() {

    }

    public override void PhysicsUpdate() {
        if (Physics2D.OverlapCircle(enemy.transform.position, enemy.attackState.attackingRadius) && enemy.canAttack) {
            enemy.stateMachine.ChangeState(enemy.attackState);
        }
    }

    public override void Update() {

    }
}
