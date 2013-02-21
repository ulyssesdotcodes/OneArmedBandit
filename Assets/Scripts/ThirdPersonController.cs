using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class ThirdPersonController : MonoBehaviour {
	
	public float animSpeed = 1;
	public static float moveSpeed = 5f;
	public float healthMax = 10f;
	public GameObject grenadePrefab;
	public GameObject normalBulletPrefab;
	public int enemyCount = 0;
	public Vector3 movingDirection;
	public UISlider healthSlider;
	
	private Animator anim;
	private AnimatorStateInfo currentState;
	private Plane playingPlane;
	private bool fired = false;
	private float firedTime;
	private Vector3 mousePositionOnPlayingPlane;
	private GameObject selectedBullet;
	private string selectedBulletScript;
	private float health;
	public bool dead = false;
	
	// Use this for initialization
	void Start () {
		playingPlane = new Plane(Vector3.up, Vector3.zero);
		anim = this.GetComponent<Animator>();
		
		selectedBullet = normalBulletPrefab;
		
		firedTime = 0;
		
		health = healthMax;
		UpdateHealth();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(dead) return;
		
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		
		
		
		//Look where the mouse is
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float dist;
		playingPlane.Raycast(ray, out dist);
		mousePositionOnPlayingPlane = ray.GetPoint(dist);
		
		/*transform.LookAt(mousePositionOnPlayingPlane);
		Vector3 normalizedMousePosition = mousePositionOnPlayingPlane.normalized;
		Vector3 inputVector = new Vector3(h, 0, v);
		transform.LookAt(mousePositionOnPlayingPlane);
		transform.position = transform.position + inputVector * moveSpeed * Time.deltaTime;
		*/
		if(!fired){
			movingDirection = new Vector3(h, 0, v);
			transform.LookAt(this.transform.position + movingDirection);
			anim.SetFloat("Speed", movingDirection.magnitude);
		}
		
		if(transform.position.x > 28) transform.position = new Vector3(28f, transform.position.y, transform.position.z);
		else if(transform.position.x < -28) transform.position = new Vector3(-28f, transform.position.y, transform.position.z);
		if(transform.position.z > 28) transform.position = new Vector3(transform.position.x, transform.position.y, 28f);
		else if(transform.position.z < -28) transform.position = new Vector3(transform.position.x, transform.position.y, -28f);
		/*
		float mousePositionInputDot = Vector3.Dot(inputVector, normalizedMousePosition);
		float forward = (inputVector != Vector3.zero) ? Mathf.Sign(mousePositionInputDot) : 0;
			
		anim.SetFloat("Forward", forward);
		*/
		
		currentState = anim.GetCurrentAnimatorStateInfo(0);
		
		
				
		if(!fired && Input.GetButton("Fire1")){
			
			anim.SetFloat("Speed", 0);
			transform.LookAt(mousePositionOnPlayingPlane);
			
			ShootBullet(selectedBullet);
			firedTime = Time.time;
			fired = true;
		} else if(fired && Time.time - firedTime > 0.5f){
			fired = false;
		}
		
		//BulletSlector
		if(Input.GetButton("SelectBullet1")){
			if(selectedBullet != normalBulletPrefab){
				selectedBullet = normalBulletPrefab;
			}
		} else if(Input.GetButton("SelectBullet2")){
			if(selectedBullet != grenadePrefab){
				selectedBullet = grenadePrefab;
			}
		}
	}
	
	void ShotBulletCallback(){
		
	}
	
	void ShootBullet(GameObject bulletPrefab){
		GameObject bullet = (GameObject) Instantiate(bulletPrefab, 
			this.gameObject.transform.position + Vector3.up * 1f + this.transform.forward * 0.5f,
			Quaternion.identity);
		
		Vector3 flattenedPosition = this.gameObject.transform.position;
		flattenedPosition.y = 0;
		
		Vector3 playerToMouseClick = mousePositionOnPlayingPlane - flattenedPosition;
		playerToMouseClick.Normalize();
		
		bullet.SendMessage("PlayerMouseVectorLogic", playerToMouseClick, 
			SendMessageOptions.DontRequireReceiver);
	}
	
	public void LoseLife(float life){
		Debug.Log("Player lost " + life + " life.");
		health -= life;
		UpdateHealth();
		if(health <= 0){
			Debug.Log("You dead mate.");
			this.transform.LookAt(transform.up);
			dead = true;
		}
	}
	
	public void UpdateHealth(){
		healthSlider.sliderValue = health / healthMax;
	}
			
			
}
