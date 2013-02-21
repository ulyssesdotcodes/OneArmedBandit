using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(Collider))]
public class WolfAIBehavior : MonoBehaviour {

	private WolfAIFSMSystem fsm;
	public GameObject pc;
	Animator anim;
	
	private long startExplosionTime;
	
	public int health = 10;
	
	// Use this for initialization
	void Start () 
	{
		MakeFSM();
		anim = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		fsm.CurrentState.Reason();
		fsm.CurrentState.Act();
		AIActions.MoveBackInBounds(gameObject);
	}
	
	private void MakeFSM()
	{
		fsm = new WolfAIFSMSystem();
		
		if(pc == null)
			pc = GameObject.FindGameObjectWithTag("Player");
		
		pc.GetComponent<ThirdPersonController>().enemyCount ++;
		
		WolfIdle idle =  new WolfIdle(fsm, gameObject, pc);
		idle.AddTransition(WolfAITransition.FoundPlayer, WolfAIStateID.ChasePlayer);
		
		WolfChasePlayer chasePlayer = new WolfChasePlayer(fsm, gameObject, pc);
		chasePlayer.AddTransition(WolfAITransition.LostPlayer, WolfAIStateID.Idle);
		chasePlayer.AddTransition(WolfAITransition.ReachedPlayer, WolfAIStateID.AttackPlayer);
		
		
		WolfAttackPlayer attackPlayer = new WolfAttackPlayer(fsm, gameObject, pc);
		attackPlayer.AddTransition(WolfAITransition.PlayerRan, WolfAIStateID.ChasePlayer);
		attackPlayer.AddTransition(WolfAITransition.PlayerDead, WolfAIStateID.Idle);
		
		fsm.AddState(idle);
		fsm.AddState(chasePlayer);
		fsm.AddState(attackPlayer);
	}
	
	public void LoseLife(int life){
		Debug.Log("Wolf lost " + life + " life.");			
		
		health -= life;
		
		
		if(health <= 0){
			Debug.Log("Dying now. Peace.");
			pc.GetComponent<ThirdPersonController>().enemyCount --;
			Destroy(gameObject);
		}
	}
}
