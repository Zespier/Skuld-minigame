using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class State {
    [HideInInspector]public Enemy enemy;
    protected StateMachine stateMachine;
    protected State(Enemy enemy, StateMachine stateMachine) {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void PhysicsUpdate();
    public abstract void Exit();
}
