using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackState : State {
    public float attackingRadius;
    public float attackCooldown;

    private float timer;
    public AttackState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine) {
    }

    public override void Enter() {

    }

    public override void Exit() {

    }

    public override void PhysicsUpdate() {

    }

    public override void Update() {
        if (timer <= 0) {
            Debug.Log("Ataco");
            timer = attackCooldown;

        } else timer -= Time.deltaTime;
    }
}
