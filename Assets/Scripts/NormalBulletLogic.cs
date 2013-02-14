using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class NormalBulletLogic : MonoBehaviour {
	
	public float bulletSpeed = 100;
	private float startTime, currentTime;
	private Vector3 playerToMouse;
	private float destroyTime = 0.5f;
	private Vector3 startPosition;
	private float hitDistance = -1f;
	
	
	// Use this for initialization
	void Start () {
		startTime = Time.time;
		currentTime = startTime;
		startPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		currentTime = Time.time;
		this.transform.position = this.transform.position + this.playerToMouse.normalized * bulletSpeed * Time.fixedDeltaTime;
		if(currentTime - startTime >= destroyTime || 
			(hitDistance != -1f && (this.transform.position - startPosition).magnitude > hitDistance)){
			Destroy(this.gameObject);
		}
	}
	
	public void PlayerMouseVectorLogic(Vector3 playerToMouse){
		this.playerToMouse = playerToMouse;
		Ray ray = new Ray(transform.position, playerToMouse);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 20)){
			hit.collider.SendMessage("LoseLife", 2, SendMessageOptions.DontRequireReceiver);
			hitDistance = hit.distance;
		}
	}
}
