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
	
	public void LoseLife(int life){
		Debug.Log("Losing " + life + " life.");			
		
		health -= life;
		
		
		if(health <= 0){
			Debug.Log("Dying now. Peace.");
			Destroy(gameObject);
		}
	}
}
