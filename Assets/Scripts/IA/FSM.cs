using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate bool Condition();
public delegate void Action();

public class FSMTransition {
    private Condition Condition;
    private FSMState Target;

    public List<Action> Actions;

    public FSMTransition(Condition condition, FSMState target) {
        Condition = condition;
        Target = target;
        Actions = new List<Action>();
    }

    public void Fire() {
        foreach (Action a in Actions) {
            a();
        }
    }

    public bool CheckCondition() {
        return Condition();
    }

    public FSMState GetTarget() {
        return Target;
    }
}

public class FSMState {

    private List<Action> EnterActions;
    private List<Action> StayActions;
    private List<Action> ExitActions;

    private List<FSMTransition> Transitions;

    public FSMState() {
        EnterActions = new List<Action>();
        StayActions = new List<Action>();
        ExitActions = new List<Action>();
        Transitions = new List<FSMTransition>();
    }

    public void Enter() { foreach (Action a in EnterActions) a(); }
    public void Stay() { foreach (Action a in StayActions) a(); }
    public void Exit() { foreach (Action a in ExitActions) a(); }

    public void AddTransition(FSMTransition transition) {
        Transitions.Add(transition);
    }

    public void AddEnterAction(Action action) {
        EnterActions.Add(action);
    }

    public void AddStayAction(Action action) {
        StayActions.Add(action);
    }
    public void AddExitAction(Action action) {
        ExitActions.Add(action);
    }

    public List<FSMTransition> GetTransitions() {
        return Transitions;
    }
}

public class FSM {

    public FSMState Current;

    public FSM(FSMState current) {
        Current = current;
        Current.Enter();
    }

    public void Update() {
        foreach (FSMTransition t in Current.GetTransitions()) {
            if (t.CheckCondition()) {
                Current.Exit();
                t.Fire();
                Current = t.GetTarget();
                Current.Enter();
            }
        }
        Current.Stay();
    }
}
