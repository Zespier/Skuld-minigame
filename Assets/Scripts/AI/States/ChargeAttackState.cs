using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChargeAttackState : State {

    public float impulse;

    public ChargeAttackState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine) {
    }

    public override void Enter() {
        Debug.Log("Ataque!");
    }

    public override void Exit() {
        enemy.DestroyObject();
    }

    public override void PhysicsUpdate() {

        enemy.rB.velocity = enemy.transform.right * impulse;

        if (Vector2.Distance(enemy.rB.position, enemy.player.transform.position) <= 0.1f) {
            Exit();
        }
    }

    public override void Update() {
    }
}
