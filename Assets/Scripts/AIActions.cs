using System;
using UnityEngine;

	
public class AIActions
{
	public static double visibleRadius = 10;
	
	public static bool CanSee(GameObject npc, GameObject pc){
		return Vector3.Dot(Utilities.MatchY(npc.transform.position - pc.transform.position, npc.transform.position).normalized, 
			Utilities.MatchY(npc.transform.forward.normalized, npc.transform.position)) < 0
			&& (npc.transform.position - pc.transform.position).magnitude < visibleRadius;
	}
	
	public static void Wait(){}
}

