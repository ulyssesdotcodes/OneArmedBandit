using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class ThirdPersonController : MonoBehaviour {
	
	public float animSpeed = 1;
	public float moveSpeed = 5f;
	public GameObject bulletPrefab;
	public Material bulletMat;
	public float grenadeSpeed = 10.0f;
	
	private Animator anim;
	private AnimatorStateInfo currentState;
	private Plane playingPlane;
	private bool fired = false;
	private Vector3 mousePositionOnPlayingPlane;
	
	// Use this for initialization
	void Start () {
		playingPlane = new Plane(Vector3.up, Vector3.zero);
		anim = this.GetComponent<Animator>();
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
			
			ShootGrenade();
			
			fired = true;
		} else if(fired && Input.GetButtonUp("Fire1")){
			fired = false;
		}
	}
	
	void ShootGrenade(){
		//Create the grenade
		GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		bullet.renderer.material = bulletMat;
		bullet.AddComponent<Rigidbody>();
		bullet.AddComponent<GrenadeLogic>();
		bullet.transform.position = this.gameObject.transform.position + Vector3.up * 1f + this.transform.forward * 0.5f;
		bullet.transform.rigidbody.useGravity = true;
		
		
		Vector3 flattenedPosition = this.gameObject.transform.position;
		flattenedPosition.y = 0;
		
		Vector3 playerToMouseClick = mousePositionOnPlayingPlane - flattenedPosition;
		playerToMouseClick.Normalize();
		playerToMouseClick *= grenadeSpeed;
		playerToMouseClick.y += 0.5f;
	
		bullet.rigidbody.velocity = playerToMouseClick;
		
	}
}
