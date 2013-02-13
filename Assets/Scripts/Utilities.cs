using System;
using UnityEngine;

public class Utilities
{
	public static Vector3 RemoveY(Vector3 input){
		return new Vector3(input.x, 0, input.z);
	}
	
	public static Vector3 MatchY(Vector3 newVector, Vector3 yVal){
		return new Vector3(newVector.x, yVal.y, newVector.z);
	}
}

