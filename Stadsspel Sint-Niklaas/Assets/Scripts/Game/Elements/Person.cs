using System.Collections.Generic;
using UnityEngine;

namespace Stadsspel.Elements
{
	public class Person : Element
	{
		private List<int> illegalItems = new List<int>();

		//legalItems[(int)Items.diploma] = 10; Bijvoorbeeld
		private List<int> legalItems = new List<int>();

		//[SyncVar]
		[SerializeField]
		private int mAmountOfMoney = 0;

		protected new void Start()
		{
			m_Team = Stadsspel.Networking.TeamExtensions.GetTeam(photonView.owner);
			if(PhotonNetwork.player == photonView.owner) {
				gameObject.AddComponent<Player>();
			} else if(m_Team == Stadsspel.Networking.TeamExtensions.GetTeam(PhotonNetwork.player)) {
				gameObject.AddComponent<Friend>();
			} else {
				gameObject.AddComponent<Enemy>();
			}

			ActionRadius = 40;
	
			//instantiate list with 3 numbers for each list.
			for(int i = 0; i < 3; i++) {
				legalItems.Add(0);
				illegalItems.Add(0);
			}
		}

		//public void Rob()
		//{
		//	foreach (GameObject enemy in enemiesInRadius) {
		//		AddGoods(enemy.GetComponent<Person>().AmountOfMoney, enemy.GetComponent<Person>().LookUpLegalItems, enemy.GetComponent<Person>().LookUpIllegalItems);
		//		enemy.GetComponent<Person>().GetRobbed();
		//	}
		//}

		public void ResetLegalItems()
		{
			for(int i = 0; i < legalItems.Count; i++) {
				legalItems[i] = 0;
			}
		}

		public void ResetIllegalItems()
		{
			for(int i = 0; i < illegalItems.Count; i++) {
				illegalItems[i] = 0;
			}
		}

		public void GetRobbed()
		{
			GameManager.s_Singleton.Teams[(int)m_Team].AddOrRemoveMoney(-mAmountOfMoney);
			mAmountOfMoney = 0;
			ResetLegalItems();
			ResetIllegalItems();
		}

		public List<int> LookUpLegalItems {
			get { return legalItems; }
		}

		public List<int> LookUpIllegalItems {
			get { return illegalItems; }
		}

		public void AddLegalItems(List<int> items)
		{
			for(int i = 0; i < items.Count; i++) {
				legalItems[i] += items[i];
			}
		}

		public void AddIllegalItems(List<int> items)
		{
			for(int i = 0; i < items.Count; i++) {
				illegalItems[i] += items[i];
			}
		}

		public void MoneyTransaction(int money)
		{
			mAmountOfMoney += money;
			GameManager.s_Singleton.Teams[(int)m_Team].AddOrRemoveMoney(money);
		}

		public void AddGoods(int money, List<int> legalItems, List<int> illegalItems)
		{
			MoneyTransaction(money);
			AddLegalItems(legalItems);
			AddIllegalItems(illegalItems);
		}

		public int AmountOfMoney {
			get { return mAmountOfMoney; }
		}

		public void AddGoods(int money)
		{ 
			mAmountOfMoney += money; 
		}


		public void Update()
		{
			/*int amountOfTeams; //= LobbyPlayerList._instance.LobbyPlayerMatrix.GetLength(0);
		if(!mIsReady && GameManager.s_Singleton.transform.childCount == amountOfTeams && Name != "Not set" && Team != TeamID.NotSet) {
			mIsReady = true;
			Debug.Log("Starting game");

			transform.GetChild(0).GetComponent<TextMesh>().text = Name;
			GetComponent<Renderer>().material.color = TeamData.GetColor(mTeam);
			transform.SetParent(GameManager.s_Singleton.transform.GetChild(((int)Team) - 1));

			NetworkIdentity networkIdentity = GetComponent<NetworkIdentity>();
			if(networkIdentity.isLocalPlayer) {
				Player player = gameObject.AddComponent<Player>();
				Debug.Log("GameManager Player has been set");
				GameManager.s_Singleton.Player = player;
				GameManager.s_Singleton.DistrictManager = GameObject.FindWithTag("Districts").GetComponent<DistrictManager>();
				GameManager.s_Singleton.DistrictManager.mPlayerTrans = transform;
				name = "Player ID:" + networkIdentity.netId + " (" + Name + ")";
			} else if(LobbyPlayer.mLocalPlayerTeam == mTeam) {
				Friend friend = gameObject.AddComponent<Friend>();
				name = "Friend ID:" + networkIdentity.netId + " (" + Name + ")";
			} else {
				Enemy enemy = gameObject.AddComponent<Enemy>();
				name = "Enemy ID:" + networkIdentity.netId + " (" + Name + ")";
			}
		}*/
		}
	}
}