using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class ThirdPersonController : MonoBehaviour {
	
	public float animSpeed = 1;
	public float moveSpeed = 5f;
	public GameObject grenadePrefab;
	public GameObject normalBulletPrefab;
	
	private Animator anim;
	private AnimatorStateInfo currentState;
	private Plane playingPlane;
	private bool fired = false;
	private Vector3 mousePositionOnPlayingPlane;
	private GameObject selectedBullet;
	private string selectedBulletScript;
	
	// Use this for initialization
	void Start () {
		playingPlane = new Plane(Vector3.up, Vector3.zero);
		anim = this.GetComponent<Animator>();
		
		selectedBullet = normalBulletPrefab;
		selectedBulletScript = "NormalBulletLogic";
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		
		
		
		//Look where the mouse is
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float dist;
		playingPlane.Raycast(ray, out dist);
		mousePositionOnPlayingPlane = ray.GetPoint(dist);
		
		//transform.LookAt(mousePositionOnPlayingPlane);
		Vector3 normalizedMousePosition = mousePositionOnPlayingPlane.normalized;
		Vector3 inputVector = new Vector3(h, 0, v);
		transform.LookAt(mousePositionOnPlayingPlane);
		transform.position = transform.position + inputVector * moveSpeed * Time.deltaTime;
		float mousePositionInputDot = Vector3.Dot(inputVector, normalizedMousePosition);
		float forward = (inputVector != Vector3.zero) ? Mathf.Sign(mousePositionInputDot) : 0;
			
		anim.SetFloat("Forward", forward);
		
		currentState = anim.GetCurrentAnimatorStateInfo(0);
		
		
				
		if(!fired && Input.GetButton("Fire1")){
			Debug.Log("Shooting");
			
			ShootBullet(selectedBullet, selectedBulletScript);
			
			fired = true;
		} else if(fired && Input.GetButtonUp("Fire1")){
			fired = false;
		}
		
		//BulletSlector
		if(Input.GetButton("SelectBullet1")){
			if(selectedBullet != normalBulletPrefab){
				selectedBullet = normalBulletPrefab;
				selectedBulletScript = "NormalBulletLogic";
			}
		} else if(Input.GetButton("SelectBullet2")){
			if(selectedBullet != grenadePrefab){
				selectedBullet = grenadePrefab;
				selectedBulletScript = "GrenadeLogic";
			}
		}
	}
	
	void ShootBullet(GameObject bulletPrefab, string bulletType){
		GameObject bullet = (GameObject) Instantiate(bulletPrefab, 
			this.gameObject.transform.position + Vector3.up * 1f + this.transform.forward * 0.5f,
			Quaternion.identity);
		bullet.AddComponent(bulletType);
		
		Vector3 flattenedPosition = this.gameObject.transform.position;
		flattenedPosition.y = 0;
		
		Vector3 playerToMouseClick = mousePositionOnPlayingPlane - flattenedPosition;
		playerToMouseClick.Normalize();
		
		bullet.SendMessage("PlayerMouseVectorLogic", playerToMouseClick, 
			SendMessageOptions.DontRequireReceiver);
	}
}
