using UnityEngine;
using System;
using System.Collections.Generic;

public class GameStateManager : Manager<GameStateManager>
{
    private State _currentState = null;
    public State CurrentState
    {
        get { return _currentState; }
    }

    private Stack<State> m_pActiveStates;
    private Dictionary<string, Type> registeredStates;

    // Private Constructor so that only the Manager Base Class can create an instance of this object
    private GameStateManager()
    {
        m_pActiveStates = new Stack<State>();
    }

    protected override void Terminate()
    {
    }

    // Update is called once per frame
    public void Update()
    {
        float deltaTime = Time.deltaTime;
        foreach (State state in m_pActiveStates)
        {
            state.Process.Invoke(deltaTime);
            if (state.IsBlocking)
            {
                break;
            }
        }
    }

    /// <summary>
    /// StateExists - Test to see if a state is already present 
    /// </summary>
    /// <returns>The state if it exists, null otherwise</returns>
    /// <param name="a_stateName">The name of the state to locate.</param>
    public State StateExists(string a_stateName)
    {
        foreach (State state in m_pActiveStates)
        {
            string pName = state.StateName;
            if (pName != null && pName == a_stateName)
            {
                return state;
            }
        }
        return null;
    }

    public bool EnterState(string a_stateName)
    {
        State pState = StateExists(a_stateName);
        if (pState != null)
        {
            //Our state is already in the list of active States
            State tempState = CurrentState;
            PopToState(pState);
            tempState.Process.Invoke(0.0f);//when a state is left invoke it so process changes to leave
            tempState.Process.Invoke(0.0f);//then call leave so everything is deleted
            return true;
        }
        else
        {
            if (registeredStates.ContainsKey(a_stateName))
            {
                State nextState = Activator.CreateInstance(registeredStates[a_stateName], a_stateName) as State;
                PushState(nextState);
                return true;
            }
        }
        return false;
    }

    private void PopToState(State a_state)
    {
        while (m_pActiveStates.Count != 0 && m_pActiveStates.Peek() != a_state)
        {
            m_pActiveStates.Pop();
        }
        _currentState = m_pActiveStates.Peek();

    }

    private void PushState(State a_state)
    {
        m_pActiveStates.Push(a_state);
        _currentState = m_pActiveStates.Peek();
    }

    private void PopState()
    {
        m_pActiveStates.Pop();
        _currentState = m_pActiveStates.Peek();
    }

    public void RegisterState<T>(string a_stateName)
    where T : State
    {
        if (registeredStates == null)
        {
            registeredStates = new Dictionary<string, Type>();
        }
        registeredStates.Add(a_stateName, typeof(T));
    }
}
