using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChargeAttackState : State {

    public float impulse;

    private Vector3 direction;

    public ChargeAttackState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine) {
    }

    public override void Enter() {
        Debug.Log("Ataque!");
        direction = enemy.player.position - enemy.transform.position;
    }

    public override void Exit() {
        enemy.DestroyObject();
    }

    public override void PhysicsUpdate() {
        enemy.rB.velocity = direction * impulse;

        if (Vector2.Distance(enemy.rB.position, enemy.player.position) <= 0.1f) {
            Exit();
        }
    }

    public override void Update() {
    }
}
