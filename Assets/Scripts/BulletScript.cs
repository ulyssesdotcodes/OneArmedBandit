using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {
	public float bulletSpeed = 50.0f;
	Vector3 flattenedPlayerToCamera;
	public GameObject bulletPrefab;
	public Material bulletMat;
	
	private Plane playingPlane;
	
	// Use this for initialization
	void Start () {
		flattenedPlayerToCamera = this.gameObject.transform.position - Camera.main.transform.position;
		flattenedPlayerToCamera.y = 0;
		playingPlane = new Plane(Vector3.up, Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () {
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		float dist;
		playingPlane.Raycast(ray, out dist);
		
		Vector3 mousePositionOnPlayingPlane = ray.GetPoint(dist);
		
		this.gameObject.transform.LookAt(mousePositionOnPlayingPlane);
		
		if(Input.GetButton("Fire1")){
			Debug.Log("Shooting");
			//Create the bullet
			GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			bullet.renderer.material = bulletMat;
			bullet.AddComponent<Rigidbody>();
			bullet.transform.position = this.gameObject.transform.position;
			bullet.collider.isTrigger = true;
			bullet.rigidbody.useGravity = false;
			
			Vector3 flattenedPosition = this.gameObject.transform.position;
			flattenedPosition.y = 0;
			
			
			Vector3 playerToMouseClick = mousePositionOnPlayingPlane - flattenedPosition;
			playerToMouseClick.Normalize();
			playerToMouseClick *= bulletSpeed;
			
			Debug.Log("player to mouse " + (playerToMouseClick * bulletSpeed));
		
			bullet.rigidbody.velocity = playerToMouseClick;
		}
	}
}
