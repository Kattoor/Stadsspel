﻿using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
	static public GameManager s_Singleton;

	[SerializeField]
	private GameObject mteamPrefab;

	private float mGameLength;
	private bool mGameIsRunning = false;
	private float nextMoneyUpdateTime;
	private float moneyUpdateTimeInterval = 5;

	private Team[] mTeams;

	private Player mPlayer;

	public Player Player {
		get {
			return mPlayer;
		}

		set {
			mPlayer = value;
		}
	}

	public Team[] Teams {
		get {
			return mTeams;
		}

		set {
			mTeams = value;
		}
	}

	//public GameObject lobbyServerEntry;

	// Use this for initialization
	private void Start()
	{
		if (s_Singleton != null) {
			Destroy(this);
			return;
		}
		s_Singleton = this;
		nextMoneyUpdateTime = moneyUpdateTimeInterval;
		DontDestroyOnLoad(gameObject);
    mPlayer = GameObject.FindWithTag("Player").GetComponent<Player>();
	}

	// Update is called once per frame
	private void Update()
	{
		if (mGameIsRunning) {
			if (Time.timeSinceLevelLoad > mGameLength) {

				//Debug.Log("The game has ended");
			}
			if (Time.timeSinceLevelLoad > nextMoneyUpdateTime) {

				// Call GainMoneyOverTime() from each financial object
				nextMoneyUpdateTime = Time.timeSinceLevelLoad + moneyUpdateTimeInterval;
			}
		}
	}

	public void StartGame(int amountOfTeams)
	{
		CmdCreateTeams(amountOfTeams);
		mGameIsRunning = true;
	}

	[Command]
	public void CmdCreateTeams(int amountOfTeams)
	{
		for (int i = 0; i < amountOfTeams; i++) {
			GameObject temp = Instantiate(mteamPrefab);
			NetworkServer.Spawn(temp);
			temp.GetComponent<Team>().TeamID = (TeamID)(i + 1);
		}
	}

	public void UpdateGameDuration(float duration)
	{
		Debug.Log(duration);
		mGameLength = duration;
	}
}