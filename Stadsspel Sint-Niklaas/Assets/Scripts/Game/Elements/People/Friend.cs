﻿using UnityEngine;

public class Friend : MonoBehaviour
{
	private Person mPerson;

	private void Start()
	{
		mPerson = GetComponent<Person>();
		tag = "Team";
		Destroy(transform.GetChild(1).gameObject);
		Destroy(transform.GetChild(2).gameObject);
		Destroy(transform.GetChild(3).gameObject);
	}
}