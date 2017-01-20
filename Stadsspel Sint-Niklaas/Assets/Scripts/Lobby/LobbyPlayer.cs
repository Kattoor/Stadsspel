﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Prototype.NetworkLobby
{
	//Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
	//Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
	public class LobbyPlayer : NetworkLobbyPlayer
	{
		[SerializeField]
		private Button colorButton;

		[SerializeField]
		private InputField nameInput;

		[SerializeField]
		private Button readyButton;

		[SerializeField]
		private Button removePlayerButton;

		[SerializeField]
		private Text mIcon;

		//OnMyName function will be invoked on clients when server change the value of playerName
		[SyncVar(hook = "OnMyName")]
		public string mPlayerName = "";

		[SyncVar(hook = "OnMyTeam")]
		public TeamID mPlayerTeam = TeamID.NotSet;

		[SyncVar(hook = "OnReady")]
		public bool mIsReady = false;

		public static TeamID mLocalPlayerTeam = TeamID.NotSet;
		public static uint mLocalPlayerNetID = 0;

		private Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
		private Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);
		private Color LocalPlayer = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

		private Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
		private Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
		private Color TransparentColor = new Color(0, 0, 0, 0);

		private bool mIsHost = false;

		private static bool mHostInstance = false;
		private static bool mIsFirst = true;

		private string mHostIcon = "";
		private string mLocalPlayerIcon = "";
		private string mOtherPlayerIcon = "";

		public override void OnStartClient()
		{
			base.OnStartClient();
			LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);
			SetReadyButton(false);
			if (mIsFirst) {
				mIsHost = true;
				mIcon.text = mHostIcon;
			}
			StartCoroutine(Init(mIsFirst));
			mIsFirst = false;
		}

		public IEnumerator Init(bool isFirst)
		{
			// need to wait a frame so that isLocalPlayer is set
			yield return new WaitForEndOfFrame();

			if (isLocalPlayer) {
				if (isFirst) {
					mHostInstance = true;
				}
				SetupLocalPlayer();
			}
			else {
				SetupOtherPlayer();
			}
		}

		private void SetupLocalPlayer()
		{
			Debug.Log("You entered the lobby");
			GetComponent<Image>().color = LocalPlayer;

			CmdNameChanged("Speler " + (LobbyPlayerList._instance.playerListContentTransform.childCount));
			OnColorClicked();

			if (mIsHost) {
				OnReadyClicked();
				readyButton.interactable = false;
			}
			else {
				mIcon.text = mLocalPlayerIcon;
			}

			if (isServer) {
				tag = "host";
			}

			mLocalPlayerNetID = netId.Value;

			nameInput.onEndEdit.RemoveAllListeners();
			nameInput.onEndEdit.AddListener(OnNameChanged);

			colorButton.onClick.RemoveAllListeners();
			colorButton.onClick.AddListener(OnColorClicked);

			readyButton.onClick.RemoveAllListeners();
			readyButton.onClick.AddListener(OnReadyClicked);

			//we switch from simple name display to name input
			colorButton.interactable = true;
			nameInput.interactable = true;
		}

		private void SetupOtherPlayer()
		{
			Debug.Log("New Player joined");
			nameInput.interactable = false;
			colorButton.interactable = false;
			readyButton.interactable = false;

			OnMyName(mPlayerName);
			OnMyTeam(mPlayerTeam);

			if (mIsHost) {
				SetReadyButton(true);
				tag = "host";
			}
			else {
				mIcon.text = mOtherPlayerIcon;
				mIcon.fontSize = 59;
				if (mIsReady) {
					SetReadyButton(true);
				}
			}

			if (mHostInstance) {
				removePlayerButton.gameObject.SetActive(true);
				removePlayerButton.onClick.RemoveAllListeners();
				removePlayerButton.onClick.AddListener(OnRemovePlayerClicked);
				Debug.Log("test");
				LobbyManager.s_Singleton.SetStartButton(false);
			}
		}

		public override void OnClientReady(bool readyState)
		{
			if (readyState) {
				SetReadyButton(true);

				if (!mIsHost && isLocalPlayer) {
					colorButton.interactable = false;
					nameInput.interactable = false;
				}
			}
			else {
				SetReadyButton(false);

				if (isLocalPlayer) {
					colorButton.interactable = true;
					nameInput.interactable = true;
				}
			}
		}

		private void SetReadyButton(bool isReady)
		{
			Color c, textColor;
			string text;
			if (isReady) {
				c = TransparentColor;
				textColor = ReadyColor;
				text = "GEREED";
			}
			else {
				c = NotReadyColor;
				textColor = Color.white;
				text = "...";
			}

			ColorBlock b = readyButton.colors;
			b.normalColor = c;
			b.pressedColor = c;
			b.highlightedColor = c;
			b.disabledColor = c;

			readyButton.colors = b;

			Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
			textComponent.text = text;

			textComponent.color = textColor;
		}

		public void OnPlayerListChanged(int idx)
		{
			if (!isLocalPlayer) {
				GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
			}
		}

		///===== callback from sync var

		public void OnMyName(string newName)
		{
			mPlayerName = newName;
			nameInput.text = mPlayerName;
		}

		public void OnMyTeam(TeamID newTeam)
		{
			if (newTeam != TeamID.NotSet) {
				LobbyPlayerList._instance.RemovePlayer(this);
				mPlayerTeam = newTeam;
				LobbyPlayerList._instance.AddPlayer(this);

				Debug.Log("Player with netID " + netId.Value + " is now in team: " + mPlayerTeam.ToString());
				colorButton.GetComponent<Image>().color = TeamData.GetColor(mPlayerTeam);
			}
		}

		public void OnReady(bool isReady)
		{
			SetReadyButton(isReady);
		}

		//===== UI Handler

		//Note that those handler use Command function, as we need to change the value on the server not locally
		//so that all client get the new value through syncvar
		public void OnColorClicked()
		{
			TeamID newTeam = mPlayerTeam;

			int count = 0;
			do {
				newTeam = TeamData.GetNextTeam(newTeam);
				count++;
				if (count > LobbyPlayerList._instance.AmountOfTeams) {
					newTeam = TeamID.NotSet;
					Debug.Log("No free team found!!!");
					break;
				}
			} while (LobbyPlayerList._instance.IsTeamFull(newTeam));

			mLocalPlayerTeam = newTeam;

			CmdTeamChange(newTeam);
		}

		private void OnReadyClicked()
		{
			mIsReady = !mIsReady;
			OnClientReady(mIsReady);
			if (mIsReady) {
				SendReadyToBeginMessage();
			}
			else {
				SendNotReadyToBeginMessage();
				CmdDeactivateStartButton();
			}
			CmdReadyChanged(mIsReady);
		}

		[Command]
		private void CmdDeactivateStartButton()
		{
			LobbyManager.s_Singleton.startButton.SetActive(false);
		}

		public void OnNameChanged(string str)
		{
			CmdNameChanged(str);
		}

		public void OnRemovePlayerClicked()
		{
			if (isLocalPlayer) {
				RemovePlayer();
			}
			else if (isServer) {
				LobbyManager.s_Singleton.KickPlayer(connectionToClient);
			}
		}

		[ClientRpc]
		public void RpcUpdateCountdown(int countdown)
		{
			LobbyManager.s_Singleton.countdownPanel.UIText.text = "Match start in " + countdown;
			LobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
			//if (countdown == 0) {
			//	LobbyManager.s_Singleton.gameObject.transform.FindChild("EventSystem").gameObject.SetActive(false);
			//}
		}

		//====== Server Command

		[Command]
		public void CmdTeamChange(TeamID newTeam)
		{
			mPlayerTeam = newTeam;
		}

		[Command]
		public void CmdNameChanged(string newName)
		{
			mPlayerName = newName;
		}

		[Command]
		public void CmdReadyChanged(bool newReadyState)
		{
			mIsReady = newReadyState;
		}

		//Cleanup thing when get destroy (which happen when client kick or disconnect)
		private void OnDestroy()
		{
			LobbyPlayerList._instance.RemovePlayer(this);
		}
	}
}
