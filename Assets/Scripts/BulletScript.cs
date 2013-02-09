using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {
	public float bulletSpeed = 0.5f;
	Vector3 flattenedPlayerToCamera;
	public GameObject bulletPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
	
	
	// Use this for initialization
	void Start () {
		flattenedPlayerToCamera = this.gameObject.transform.position - Camera.main.transform.position;
		flattenedPlayerToCamera.y = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1")){
			Debug.Log("Shooting");
			//Create the bullet
			GameObject bullet = (GameObject) Instantiate(bulletPrefab);
			bullet.AddComponent<Rigidbody>();
			
			//Find the vector from the camera to the mouse click position in the x-y plane
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 flattenedRay = ray.direction;
			flattenedRay.y = 0;
			
			//Find the vector from the this player transform to the place where the mouse clicked
			Vector3 playerToMouseClick = flattenedRay + flattenedPlayerToCamera;
			
			//Add that vector to the bullet velocity
			bullet.rigidbody.velocity = playerToMouseClick.normalized * bulletSpeed;
		}
	}
}
