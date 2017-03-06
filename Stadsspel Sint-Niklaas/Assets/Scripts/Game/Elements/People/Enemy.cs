﻿using UnityEngine;

public class Enemy : MonoBehaviour
{
	private int mDetectionRadius = 150;
	private float mDetectionTime = .5f;
	private float mTimer;
	private Person mPerson;

	private MeshRenderer mCircleMesh;
	private MeshRenderer mTextMesh;

	private void Start()
	{
		mPerson = GetComponent<Person>();
		tag = "Enemy";
		Destroy(transform.GetChild(1).gameObject);
		Destroy(transform.GetChild(2).gameObject);
		Destroy(transform.GetChild(3).gameObject);

		mTimer = mDetectionTime;

		mCircleMesh = GetComponent<MeshRenderer>();
		mTextMesh = transform.GetChild(0).GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		mTimer -= Time.deltaTime;
		if (mTimer < 0) {
			mTimer = mDetectionTime;
			if (Vector2.Distance(transform.position, GameManager.s_Singleton.Player.transform.position) > mDetectionRadius) {
				mCircleMesh.enabled = false;
				mTextMesh.enabled = false;
			}
			else {
				mCircleMesh.enabled = true;
				mTextMesh.enabled = true;
			}
		}
	}
}