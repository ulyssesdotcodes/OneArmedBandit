using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(Collider))]
public class SimpleAIBehavior : MonoBehaviour {

	private SimpleAIFSMSystem fsm;
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
		
		if(health <= 0)
			Destroy(gameObject);
		
		anim.SetBool("Exploded", false);
	}
	
	private void MakeFSM()
	{
		fsm = new SimpleAIFSMSystem();
		
		Idle idle =  new Idle(fsm, gameObject, pc);
		idle.AddTransition(SimpleAITransition.FoundPlayer, SimpleAIStateID.ChasePlayer);
		
		ChasePlayer chasePlayer = new ChasePlayer(fsm, gameObject, pc);
		chasePlayer.AddTransition(SimpleAITransition.LostPlayer, SimpleAIStateID.Idle);
		
		fsm.AddState(idle);
		fsm.AddState(chasePlayer);
	}
	
	public void OnDetonatorForceHit(){
		Debug.Log("Hit!");
		
		health -= 1;
		
		anim.SetBool("Exploded", true);
	}
}
