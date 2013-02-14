using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GrenadeLogic : MonoBehaviour {
	
	public float grenadeSpeed = 5;
	private float startTime, currentTime; 
	
	// Use this for initialization
	void Start () {
		startTime = Time.time;
		currentTime = startTime;
	}
	
	// Update is called once per frame
	void Update () {
		currentTime = Time.time;
		if(currentTime - startTime >= 1){
			Detonator detonator = gameObject.AddComponent<Detonator>();
			detonator.autoCreateForce = false;
			detonator.Reset();
			detonator.Explode();
			detonator.destroyTime = 7;
			Destroy(this.renderer);
		}
	}
	
	public void PlayerMouseVectorLogic(Vector3 playerToMouse){
		Vector3 grenadeVel = playerToMouse;
		grenadeVel *= grenadeSpeed;
		grenadeVel.y = 0.5f;
		
		this.rigidbody.velocity = grenadeVel;
	}
}
