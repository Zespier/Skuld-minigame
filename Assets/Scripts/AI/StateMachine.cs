using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine {

    public State currentState { get; private set; }

    public void Initialize(State startingState) {
        currentState = startingState;
        startingState.Enter();
    }
    public void ChangeState(State newState) {
        currentState.Exit();

        currentState = newState;
        newState.Enter();
    }
}
