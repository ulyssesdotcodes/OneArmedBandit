using System;
using UnityEngine;

	
public class BotActions
{
	public static int moveSpeed = 5;
	public static double nearbyRadius = 0.1;
	
	public static bool CanSee(GameObject npc, GameObject pc){
		return Vector3.Dot(npc.transform.forward, pc.transform.position) > 0
			&& (npc.transform.position - pc.transform.position).magnitude < 10;
	}
	
	public static void Wait(){}
}

