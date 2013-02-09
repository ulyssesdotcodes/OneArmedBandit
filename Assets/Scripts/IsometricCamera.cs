using UnityEngine;
using System.Collections;

public class IsometricCamera : MonoBehaviour {
	
	Vector3 playerLastPosition;
	
	// Use this for initialization
	void Start () {
		playerLastPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 deltaPosition = this.transform.position - playerLastPosition;
		deltaPosition.y = 0;
		
		Camera.main.transform.Translate(deltaPosition, Space.World);
		playerLastPosition = this.transform.position;
	}
}
