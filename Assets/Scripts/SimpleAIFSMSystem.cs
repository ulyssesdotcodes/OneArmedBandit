
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
/**
A Finite State Machine System based on Chapter 3.1 of Game Programming Gems 1 by Eric Dybsand
 
Written by Roberto Cezar Bianchini, July 2010
 
 
How to use:
	1. Place the labels for the transitions and the states of the Finite State System
	    in the corresponding enums.
 
	2. Write new class(es) inheriting from FSMState and fill each one with pairs (transition-state).
	    These pairs represent the state S2 the FSMSystem should be if while being on state S1, a
	    transition T is fired and state S1 has a transition from it to S2. Remember this is a Deterministic FSM. 
	    You can't have one transition leading to two different states.
 
	   Method Reason is used to determine which transition should be fired.
	   You can write the code to fire transitions in another place, and leave this method empty if you
	   feel it's more appropriate to your project.
 
	   Method Act has the code to perform the actions the NPC is supposed do if it's on this state.
	   You can write the code for the actions in another place, and leave this method empty if you
	   feel it's more appropriate to your project.
 
	3. Create an instance of FSMSystem class and add the states to it.
 
	4. Call Reason and Act (or whichever methods you have for firing transitions and making the NPCs
	     behave in your game) from your Update or FixedUpdate methods.
 
	Asynchronous transitions from Unity Engine, like OnTriggerEnter, SendMessage, can also be used, 
	just call the Method PerformTransition from your FSMSystem instance with the correct Transition 
	when the event occurs.
 
 
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE 
AND NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
 
/// <summary>
/// Place the labels for the Transitions in this enum.
/// Don't change the first label, NullTransition as FSMSystem class uses it.
/// </summary>
public enum SimpleAITransition
{
    NullTransition = 0, // Use this transition to represent a non-existing transition in your system
	FoundPlayer = 1,
	LostPlayer = 2
}
 
/// <summary>
/// Place the labels for the States in this enum.
/// Don't change the first label, NullTransition as FSMSystem class uses it.
/// </summary>
public enum SimpleAIStateID
{
    NullStateID = 0, // Use this ID to represent a non-existing State in your system	
	Idle = 1,
	ChasePlayer = 2
} 

public abstract class SimpleAIFSMState
{
    protected Dictionary<SimpleAITransition, SimpleAIStateID> map = new Dictionary<SimpleAITransition, SimpleAIStateID>();
    protected SimpleAIStateID stateID;
    public SimpleAIStateID ID { get { return stateID; } }
 
    public void AddTransition(SimpleAITransition trans, SimpleAIStateID id)
    {
        // Check if anyone of the args is invalid
        if (trans == SimpleAITransition.NullTransition)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed for a real transition");
            return;
        }
 
        if (id == SimpleAIStateID.NullStateID)
        {
            Debug.LogError("FSMState ERROR: NullStateID is not allowed for a real ID");
            return;
        }
 
        // Since this is a Deterministic FSM,
        //   check if the current transition was already inside the map
        if (map.ContainsKey(trans))
        {
            Debug.LogError("FSMState ERROR: State " + stateID.ToString() + " already has transition " + trans.ToString() + 
                           "Impossible to assign to another state");
            return;
        }
 
        map.Add(trans, id);
    }
 
    /// <summary>
    /// This method deletes a pair transition-state from this state's map.
    /// If the transition was not inside the state's map, an ERROR message is printed.
    /// </summary>
    public void DeleteTransition(SimpleAITransition trans)
    {
        // Check for NullTransition
        if (trans == SimpleAITransition.NullTransition)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return;
        }
 
        // Check if the pair is inside the map before deleting
        if (map.ContainsKey(trans))
        {
            map.Remove(trans);
            return;
        }
        Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + stateID.ToString() + 
                       " was not on the state's transition list");
    }
 
    /// <summary>
    /// This method returns the new state the FSM should be if
    ///    this state receives a transition and 
    /// </summary>
    public SimpleAIStateID GetOutputState(SimpleAITransition trans)
    {
        // Check if the map has this transition
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }
        return SimpleAIStateID.NullStateID;
    }
 
    /// <summary>
    /// This method is used to set up the State condition before entering it.
    /// It is called automatically by the FSMSystem class before assigning it
    /// to the current state.
    /// </summary>
    public virtual void DoBeforeEntering() { }
 
    /// <summary>
    /// This method is used to make anything necessary, as reseting variables
    /// before the FSMSystem changes to another one. It is called automatically
    /// by the FSMSystem before changing to a new state.
    /// </summary>
    public virtual void DoBeforeLeaving() { } 
 
    /// <summary>
    /// This method decides if the state should transition to another on its list
    /// NPC is a reference to the object that is controlled by this class
    /// </summary>
    public abstract void Reason();
 
    /// <summary>
    /// This method controls the behavior of the NPC in the game World.
    /// Every action, movement or communication the NPC does should be placed here
    /// NPC is a reference to the object that is controlled by this class
    /// </summary>
    public abstract void Act();
 
} // class FSMState


 
/// <summary>
/// FSMSystem class represents the Finite State Machine class.
///  It has a List with the States the NPC has and methods to add,
///  delete a state, and to change the current state the Machine is on.
/// </summary>
public class SimpleAIFSMSystem
{
    private List<SimpleAIFSMState> states;
 
    // The only way one can change the state of the FSM is by performing a transition
    // Don't change the CurrentState directly
    private SimpleAIStateID currentStateID;
    public SimpleAIStateID CurrentStateID { get { return currentStateID; } }
    private SimpleAIFSMState currentState;
    public SimpleAIFSMState CurrentState { get { return currentState; } }
 
    public SimpleAIFSMSystem()
    {
        states = new List<SimpleAIFSMState>();
    }
 
    /// <summary>
    /// This method places new states inside the FSM,
    /// or prints an ERROR message if the state was already inside the List.
    /// First state added is also the initial state.
    /// </summary>
    public void AddState(SimpleAIFSMState s)
    {
        // Check for Null reference before deleting
        if (s == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }
 
        // First State inserted is also the Initial state,
        //   the state the machine is in when the simulation begins
        if (states.Count == 0)
        {
            states.Add(s);
            currentState = s;
            currentStateID = s.ID;
            return;
        }
 
        // Add the state to the List if it's not inside it
        foreach (SimpleAIFSMState state in states)
        {
            if (state.ID == s.ID)
            {
                Debug.LogError("FSM ERROR: Impossible to add state " + s.ID.ToString() + 
                               " because state has already been added");
                return;
            }
        }
        states.Add(s);
    }
 
    /// <summary>
    /// This method delete a state from the FSM List if it exists, 
    ///   or prints an ERROR message if the state was not on the List.
    /// </summary>
    public void DeleteState(SimpleAIStateID id)
    {
        // Check for NullState before deleting
        if (id == SimpleAIStateID.NullStateID)
        {
            Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
            return;
        }
 
        // Search the List and delete the state if it's inside it
        foreach (SimpleAIFSMState state in states)
        {
            if (state.ID == id)
            {
                states.Remove(state);
                return;
            }
        }
        Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString() + 
                       ". It was not on the list of states");
    }
 
    /// <summary>
    /// This method tries to change the state the FSM is in based on
    /// the current state and the transition passed. If current state
    ///  doesn't have a target state for the transition passed, 
    /// an ERROR message is printed.
    /// </summary>
    public void PerformTransition(SimpleAITransition trans)
    {
        // Check for NullTransition before changing the current state
        if (trans == SimpleAITransition.NullTransition)
        {
            Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
            return;
        }
 
        // Check if the currentState has the transition passed as argument
        SimpleAIStateID id = currentState.GetOutputState(trans);
        if (id == SimpleAIStateID.NullStateID)
        {
            Debug.LogError("FSM ERROR: State " + currentStateID.ToString() +  " does not have a target state " + 
                           " for transition " + trans.ToString());
            return;
        }
 
        // Update the currentStateID and currentState		
        currentStateID = id;
        foreach (SimpleAIFSMState state in states)
        {
            if (state.ID == currentStateID)
            {
                // Do the post processing of the state before setting the new one
                currentState.DoBeforeLeaving();
 
                currentState = state;
 
                // Reset the state to its desired condition before it can reason or act
                currentState.DoBeforeEntering();
                break;
            }
        }
 
    } // PerformTransition()
 
} //class FSMSystem

public class Idle : SimpleAIFSMState
{
	int moveSpeed;
	GameObject npc, pc;
	Animator anim;
	SimpleAIFSMSystem fsm;
	
	public Idle(SimpleAIFSMSystem fsm, GameObject npc, GameObject pc)
	{
		stateID = SimpleAIStateID.Idle;
		this.npc = npc;
		this.pc = pc;
		this.fsm = fsm;
		
		anim = npc.GetComponent<Animator>();
	}
	
	public override void DoBeforeEntering ()
	{
		if(anim != null)
			anim.SetFloat("Speed", 0f);
	}
	
	public override void Reason ()
	{
		if(AIActions.CanSee(npc, pc))
		{
			fsm.PerformTransition(SimpleAITransition.FoundPlayer);
		}
	}
	
	public override void Act ()
	{
		AIActions.Wait();
	}
}

public class ChasePlayer : SimpleAIFSMState
{
	GameObject npc, pc;
	long startTime;
	Animator anim;
	SimpleAIFSMSystem fsm;
	
	public ChasePlayer(SimpleAIFSMSystem fsm, GameObject npc, GameObject pc)
	{
		stateID = SimpleAIStateID.ChasePlayer;
		this.npc = npc;
		this.pc = pc;
		anim = npc.GetComponent<Animator>();
		this.fsm = fsm;
	}
	
	public override void DoBeforeEntering ()
	{
		if(anim != null)
			anim.SetFloat("Speed", 1f);
	}
	
	public override void Reason ()
	{
		if(!AIActions.CanSee(npc, pc))
			fsm.PerformTransition(SimpleAITransition.LostPlayer);
	}
	
	public override void Act ()
	{
		npc.transform.LookAt(Utilities.MatchY(pc.transform.position, npc.transform.position));	
	}
}