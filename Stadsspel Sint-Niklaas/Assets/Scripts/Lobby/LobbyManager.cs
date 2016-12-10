using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;


namespace Prototype.NetworkLobby
{
	public class LobbyManager : NetworkLobbyManager
	{
		static public LobbyManager s_Singleton;

		static short MsgKicked = MsgType.Highest + 1;

		[Header("Unity UI Lobby")]
		[Tooltip("Time in second between all players ready & match start")]
		public float prematchCountdown = 5.0f;

		[Space]
		[Header("UI Reference")]
		public LobbyTopPanel topPanel;
		public RectTransform mCreateLobbyPanel;
		public RectTransform lobbyPanel;
		public LobbyInfoPanel infoPanel;
		public LobbyCountdownPanel countdownPanel;
		public LobbyInputPanel inputPanel;
		public GameObject LobbyPlayer;
		public GameObject startButton;
		public Button backButton;
		public Text lobbyNamePanel;

		private string lobbyNameToJoin;
		private RectTransform currentPanel;

		//Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
		//of players, so that even client know how many player there is.
		[HideInInspector]
		public int _playerNumber = 0;

		protected bool _disconnectServer = false;

		protected ulong _currentMatchID;

		protected LobbyHook _lobbyHooks;

		[HideInInspector]
		public static GameManager mGameManager;
		private int mNumberOfTeams = 3;

		void Start()
		{
			s_Singleton = this;
			_lobbyHooks = GetComponent<LobbyHook>();
			currentPanel = mCreateLobbyPanel;

			mGameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
			mGameManager.GenerateTeams(mNumberOfTeams);

			backButton.gameObject.SetActive(false);
		}

		public void UpdateNumberOfPlayers(Slider slider)
		{
			maxPlayers = (int)slider.value;
			slider.GetComponentInChildren<Text>().text = "AANTAL SPELERS: " + maxPlayers.ToString();
		}

		public void OnLobbyJoinUpdateName(string lobbyName)
		{
			lobbyNamePanel.text = lobbyName.ToUpper();
			lobbyNamePanel.gameObject.SetActive(true);
		}
		public void SetLobbyNameToJoin(string lobbyName)
		{
			lobbyNameToJoin = lobbyName;
		}

		public override void OnLobbyClientSceneChanged(NetworkConnection conn)
		{
			if (SceneManager.GetSceneAt(0).name == lobbyScene) {
				if (topPanel.isInGame) {
					ChangeTo(lobbyPanel);
					if (conn.playerControllers[0].unetView.isServer) {
						backDelegate = StopHostClbk;
					}
					else {
						backDelegate = StopClientClbk;
					}

				}
				else {
					ChangeTo(mCreateLobbyPanel);
				}

				topPanel.ToggleVisibility(true);
				topPanel.isInGame = false;
			}
			else {
				ChangeTo(null);

				Destroy(GameObject.Find("MainMenuUI(Clone)"));

				//backDelegate = StopGameClbk;
				topPanel.isInGame = true;
				topPanel.ToggleVisibility(false);
			}
		}

		public void ChangeTo(RectTransform newPanel)
		{
			if (currentPanel != null) {
				currentPanel.gameObject.SetActive(false);
			}

			if (newPanel != null) {
				newPanel.gameObject.SetActive(true);
			}

			currentPanel = newPanel;

			if (currentPanel != mCreateLobbyPanel) {
				backButton.gameObject.SetActive(true);
			}
			else {
				backButton.gameObject.SetActive(false);
			}
		}

		public void DisplayIsConnecting()
		{
			var _this = this;
			infoPanel.Display("Verbinden...", "Annuleren", () => {
				_this.backDelegate();
			});

		}

		public void DisplayPasswordRequest(LobbyServerEntry lobbyServerEntry)
		{
			var _this = this;
			inputPanel.Display("Paswoord:", "Ok", "Terug", () => {
				_this.backDelegate();
			}, lobbyServerEntry);
		}


		public delegate void BackButtonDelegate();
		public BackButtonDelegate backDelegate;
		public void GoBackButton()
		{
			backDelegate();
			topPanel.isInGame = false;
			lobbyNamePanel.gameObject.SetActive(false);
		}

		// ----------------- Server management

		public void AddLocalPlayer()
		{
			TryToAddPlayer();
		}

		public void RemovePlayer(LobbyPlayer player)
		{
			player.RemovePlayer();
		}

		public void SimpleBackClbk()
		{
			ChangeTo(mCreateLobbyPanel);
		}

		public void StopHostClbk()
		{
			matchMaker.DestroyMatch((NetworkID)_currentMatchID, 0, OnDestroyMatch);
			_disconnectServer = true;



			ChangeTo(mCreateLobbyPanel);
		}

		public void StopClientClbk()
		{
			StopClient();

			StopMatchMaker();


			ChangeTo(mCreateLobbyPanel);
		}

		public void StopServerClbk()
		{
			StopServer();
			ChangeTo(mCreateLobbyPanel);
		}

		class KickMsg : MessageBase
		{
		}
		public void KickPlayer(NetworkConnection conn)
		{
			conn.Send(MsgKicked, new KickMsg());
		}

		public void KickedMessageHandler(NetworkMessage netMsg)
		{
			infoPanel.Display("Kicked door host", "Sluiten", null);
			netMsg.conn.Disconnect();
		}

		//===================

		public override void OnStartHost()
		{
			base.OnStartHost();

			ChangeTo(lobbyPanel);
			backDelegate = StopHostClbk;
		}

		public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
		{
			base.OnMatchCreate(success, extendedInfo, matchInfo);
			_currentMatchID = (System.UInt64)matchInfo.networkId;
		}

		public override void OnDestroyMatch(bool success, string extendedInfo)
		{
			base.OnDestroyMatch(success, extendedInfo);
			if (_disconnectServer) {
				StopMatchMaker();
				StopHost();
			}
		}

		//allow to handle the (+) button to add/remove player
		public void OnPlayersNumberModified(int count)
		{
			_playerNumber += count;

			int localPlayerCount = 0;
			foreach (PlayerController p in ClientScene.localPlayers)
				localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;
		}

		// ----------------- Server callbacks ------------------

		//we want to disable the button JOIN if we don't have enough player
		//But OnLobbyClientConnect isn't called on hosting player. So we override the lobbyPlayer creation
		public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
		{
			return Instantiate(lobbyPlayerPrefab.gameObject);
		}

		public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
		{
			//This hook allows you to apply state data from the lobby-player to the game-player
			//just subclass "LobbyHook" and add it to the lobby object.

			if (_lobbyHooks)
				_lobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

			return true;
		}

		// --- Countdown management

		public override void OnLobbyServerPlayersReady()
		{
			bool allready = true;
			for (int i = 0; i < lobbySlots.Length; ++i) {
				if (lobbySlots[i] != null)
					allready &= lobbySlots[i].readyToBegin;
			}

			if (allready)
				startButton.SetActive(true);
		}
		public void OnStartButtonClicked()
		{
			StartCoroutine(ServerCountdownCoroutine());
		}

		public IEnumerator ServerCountdownCoroutine()
		{
			float remainingTime = prematchCountdown;
			int floorTime = Mathf.FloorToInt(remainingTime);

			while (remainingTime > 0) {
				yield return null;

				remainingTime -= Time.deltaTime;
				int newFloorTime = Mathf.FloorToInt(remainingTime);

				if (newFloorTime != floorTime) {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
					floorTime = newFloorTime;

					for (int i = 0; i < lobbySlots.Length; ++i) {
						if (lobbySlots[i] != null) {//there is maxPlayer slots, so some could be == null, need to test it before accessing!
							(lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
						}
					}
				}
			}

			for (int i = 0; i < lobbySlots.Length; ++i) {
				if (lobbySlots[i] != null) {
					(lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
				}
			}
			mGameManager.StartGame();
			ServerChangeScene(playScene);
		}

		// ----------------- Client callbacks ------------------

		public override void OnClientConnect(NetworkConnection conn)
		{
			base.OnClientConnect(conn);

			infoPanel.gameObject.SetActive(false);
			OnLobbyJoinUpdateName(lobbyNameToJoin);

			conn.RegisterHandler(MsgKicked, KickedMessageHandler);

			if (!NetworkServer.active) {//only to do on pure client (not self hosting client)
				ChangeTo(lobbyPanel);
				backDelegate = StopClientClbk;
			}
		}


		public override void OnClientDisconnect(NetworkConnection conn)
		{
			base.OnClientDisconnect(conn);
			ChangeTo(mCreateLobbyPanel);
		}

		public override void OnClientError(NetworkConnection conn, int errorCode)
		{
			ChangeTo(mCreateLobbyPanel);
			infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
		}
	}
}