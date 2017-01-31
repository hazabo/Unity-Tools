using UnityEngine;
using System.Collections;

/// <summary>
/// State - This class represents a State that the game may currently be in
/// </summary>
public class State
{
    /// <summary>
    /// m_StateName - The name of this state, 
    /// The StateName property is used to retrieve this data
    /// </summary>
    protected string m_StateName;
    public string StateName
    {
        get { return m_StateName; }
    }
    /// <summary>
    /// m_fDuration - the length of time this state is active for
    /// Duration is the property for this data
    /// </summary>
    protected float m_fDuration;
    public float Duration
    {
        get { return m_fDuration; }
    }
    /// <summary>
    /// _isBlocking boolean value to represent whether this state allows states 
    /// that are below it on the state stack to process or if they are blocked.
    /// </summary>
    private bool _isBlocking = false;
    public bool IsBlocking
    {
        get { return _isBlocking; }
        set { _isBlocking = value; }
    }
    /// <summary>
    /// Constructor to create a state with a given name
    /// </summary>
    /// <param name="a_stateName"></param>
    public State(string a_stateName)
    {
        m_StateName = a_stateName;
    }
    /// <summary>
    /// Virtual Functions that are to be used for processing the current 
    /// process of the state
    /// </summary>
    protected virtual void Initialise(float a_fTimeStep) { }
    protected virtual void Update(float a_fTimeStep) { }
    protected virtual void Leave(float a_fTimeStep) { }
    /// <summary>
    /// StateProcess is a function descriptor that allows us to have a function pointer that can point to 
    /// a function which has an identical argument list
    /// </summary>
    /// <param name="a_fDeltaTime"></param>
    public delegate void StateProcess(float a_fDeltaTime);

    /// <summary>
    /// m_StateProcess is the function pointer variable that the property Process will both set and retrieve
    /// </summary>
    private StateProcess m_stateProcess;
    public StateProcess Process
    {
        get { return m_stateProcess; }
        set { m_stateProcess = value; }
    }
}