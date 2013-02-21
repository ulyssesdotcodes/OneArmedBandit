using System;
using UnityEngine;

	
public class AIActions
{
	public static bool CanSee(GameObject npc, GameObject pc, float visibleRadius){
		return Vector3.Dot(Utilities.MatchY(npc.transform.position - pc.transform.position, npc.transform.position).normalized, 
			Utilities.MatchY(npc.transform.forward.normalized, npc.transform.position)) < 0
			&& (npc.transform.position - pc.transform.position).magnitude < visibleRadius;
	}
	
	public static bool CanSeeAllDirections(GameObject npc, GameObject pc, float visibleRadius){
		return (npc.transform.position - pc.transform.position).magnitude < visibleRadius;
	}
	
	public static void MoveForward(GameObject npc, float moveSpeed){
		npc.transform.position = npc.transform.position + npc.transform.forward * Time.fixedDeltaTime * moveSpeed;
	}
	
	public static bool CanAttack(GameObject npc, GameObject pc, float attackRadius){
		return (npc.transform.position - pc.transform.position).magnitude < attackRadius;
	}
	
	public static void Wait(){}
	
	public static void MoveBackInBounds(GameObject npc){
		if(npc.transform.position.x > 28) npc.transform.position = new Vector3(28f, npc.transform.position.y, npc.transform.position.z);
		else if(npc.transform.position.x < -28) npc.transform.position = new Vector3(-28f, npc.transform.position.y, npc.transform.position.z);
		if(npc.transform.position.z > 28) npc.transform.position = new Vector3(npc.transform.position.x, npc.transform.position.y, 28f);
		else if(npc.transform.position.z < -28) npc.transform.position = new Vector3(npc.transform.position.x, npc.transform.position.y, -28f);
	}
	
	public static void AILookAtPosition(GameObject npc, Vector3 position, float damping){
		Quaternion rotation = Quaternion.LookRotation(position - npc.transform.position);
		npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, rotation, Time.deltaTime * damping);
	}
}

