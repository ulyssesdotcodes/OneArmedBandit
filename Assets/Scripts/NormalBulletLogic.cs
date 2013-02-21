using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class NormalBulletLogic : MonoBehaviour {
	
	public float bulletSpeed = 500;
	public Material muzzleFlashMat;
	private float startTime, currentTime;
	private Vector3 playerToMouse;
	private Vector3 startPosition;
	public float maxDistance = 20f;
	private LineRenderer line;
	
	
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
		if(currentTime - startTime > 1.5f)
			Destroy(gameObject);
		line.material.color = Color.Lerp(Color.clear, Color.white, 1f - (currentTime-startTime));
	}
	
	public void PlayerMouseVectorLogic(Vector3 playerToMouse){
		line = gameObject.GetComponent<LineRenderer>();
		this.playerToMouse = playerToMouse;
		Ray ray = new Ray(transform.position, playerToMouse);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, maxDistance)){
			hit.collider.SendMessage("LoseLife", 2, SendMessageOptions.DontRequireReceiver);
			maxDistance = hit.distance;
		}
		line.SetPosition(0, this.transform.position);
		line.SetPosition(1, ray.GetPoint(maxDistance));
		line.SetWidth(0.1f, 0.1f);
	}
}
