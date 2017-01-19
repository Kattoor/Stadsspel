﻿using UnityEngine;
using UnityEngine.Networking;

public class Friend : Person
{
	private int mDetectionRadius = 120;

	private new void Start()
	{
		tag = "Team";
		base.Start();
		Destroy(transform.GetChild(1).gameObject);
		Destroy(transform.GetChild(2).gameObject);
		Destroy(transform.GetChild(3).gameObject);
		Destroy(GetComponent<MoveAvatar>());
		GetComponent<NetworkIdentity>().localPlayerAuthority = false;

		NetworkProximityChecker proximityChecker = gameObject.AddComponent<NetworkProximityChecker>();
		proximityChecker.checkMethod = NetworkProximityChecker.CheckMethod.Physics2D;
		proximityChecker.visRange = mDetectionRadius;
	}
}
