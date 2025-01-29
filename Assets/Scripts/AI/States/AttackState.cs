using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class AttackState : State {

    public float attackingRadius;
    public float attackDelay = 1 / 6f;
    public Transform attackPoint;

    public AttackState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine) {
    }

    public override void Enter() {
        enemy.rB.velocity = Vector2.zero;

        enemy.PlayAnimation("Attack");

        enemy.StartCoroutine(C_AttackDelay());
    }

    public override void Exit() {

    }

    public override void PhysicsUpdate() {

    }

    public override void Update() {
    }

    private IEnumerator C_AttackDelay() {

        float timer = attackDelay;
        while (timer >= 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        Vector3 attackPoint = this.attackPoint != null ? this.attackPoint.position : enemy.transform.position;

        if (DistanceSquared(PlayerController.instance.playerCenter.position, attackPoint) < attackingRadius * attackingRadius) {
            PlayerController.instance.GetHit(enemy.enemySprite.gameObject);
        }
    }

    public static float DistanceSquared(Vector3 a, Vector3 b) {
        return (a.x - b.x) * (a.x - b.x) +
               (a.y - b.y) * (a.y - b.y) +
               (a.z - b.z) * (a.z - b.z);
    }
}
