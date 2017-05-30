﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Domain;
using UnityEngine;

/// <summary>
/// A singleton that holds all information regarding the current (or last) game known.
/// </summary>
public class CurrentGame : Singleton<CurrentGame>
{
	public WebsocketImpl Ws { get; private set; }
	public string ClientToken { get; set; }
	public bool isHost { get; set; }
	public string GameId { get; set; }
	public string PasswordUsed { get; set; }
	public Game gameDetail { get; set; }
	public LocalPlayer LocalPlayer { get; set; }
	public ServerTeam PlayerTeam { get; set; }
	public bool IsGameRunning { get; set; }
	public bool IsInLobby { get; set; }

	[Serializable]
	public class Game
	{
		private string id;
		private string roomName;
		private List<AreaLocation> districts;
		private List<AreaLocation> markets;
		private List<PointLocation> tradePosts;
		private List<PointLocation> banks;
		private List<ServerTeam> teams;
		private int maxPlayersPerTeam;
		private int maxTeams;

		public Game(int mAmountOfTeams)
		{
			teams = new List<ServerTeam>(0);
			for (int i = 0; i < mAmountOfTeams; i++)
			{
				teams.Add(new ServerTeam {
					BankAccount = 0,
					TeamName = "TEAM" + (i + 1),
					Treasury = 0,
					Players = new Dictionary<string, ServerPlayer>(),
					CustomColor = "#FF" + (i + 1) + "000"
				}
				);
			}
		}

		/// <summary>
		/// Searches for the team a specific client belongs to.
		/// Returns null if no team contains the given id.
		/// </summary>
		/// <param name="clientId"></param>
		/// <returns></returns>
		public ServerTeam findTeamByPlayer(string clientId)
		{
			foreach (ServerTeam serverTeam in teams)
			{
				if (serverTeam.ContainsPlayer(clientId))
				{
					return serverTeam;
				}
			}
			return null;
		}

		/// <summary>
		/// Searches for the index of a specific team.
		/// Returns -1 if the requested team is not found.
		/// </summary>
		/// <param name="team"></param>
		/// <returns></returns>
		public int IndexOfTeam(ServerTeam team)
		{
			for (int i = 0; i < teams.Count; i++)
			{
				if (team.TeamName.Equals(teams[i].TeamName))
				{
					return i;
				}
			}
			return -1;
		}

		public ServerTeam GetTeamByIndex(int i)
		{
			return teams[i];
		}
	}

	private CurrentGame()
	{
		LocalPlayer = new LocalPlayer();
		LocalPlayer.name = "Player";
		IsGameRunning = false;
		IsInLobby = false;
	}

	public void Awake()
	{
		Ws = (WebsocketImpl)WebsocketImpl.Instance;
		LocalPlayer.name = "Player";
		LocalPlayer.clientID = SystemInfo.deviceUniqueIdentifier;
	}

	public void Connect()
	{
		StartCoroutine(Ws.Connect("ws://localhost:8090/user", GameId, LocalPlayer.clientID));
	}

	public void Clear()
	{
		ClientToken = null;
		GameId = null;
		PasswordUsed = null;
	}

	private IEnumerator SendPlayerLocation()
	{
		Debug.Log("START GAME LOLOL");
		while (IsGameRunning)
		{
			if (LocalPlayer!=null && LocalPlayer.location!=null)
			{
				Debug.Log("START GAME LOL");
				Ws.SendLocation(LocalPlayer.location);
			}
			yield return new WaitForSeconds(1); //todo tweak
			Debug.Log("START GAME LOL2");
		}
	}

	private IEnumerator SendHearthbeats()
	{
		while ((!IsGameRunning) && IsInLobby)
		{
			Ws.SendHearthbeat();
			yield return new WaitForSeconds(45);
		}
	}

	public void StartGame()
	{
		IsGameRunning = true;
		StartCoroutine(SendPlayerLocation());
	}

	public void StopGame()
	{
		IsGameRunning = false;
	}

	public void EnterLobby()
	{
		IsInLobby = true;
		StartCoroutine(SendHearthbeats());
	}

	public void LeaveLobby()
	{
		IsInLobby = false;
	}
}
