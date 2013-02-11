using UnityEngine;
using System.Collections;

public class GrenadeLogic : MonoBehaviour {
	
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
			Detonator detonator = this.gameObject.AddComponent<Detonator>();
			detonator.autoCreateFireball = true;
			detonator.Explode();
			detonator.destroyTime = 7;
			Destroy(this.renderer);
		}
	}
}
