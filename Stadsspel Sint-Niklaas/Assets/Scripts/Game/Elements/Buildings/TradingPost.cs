using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Stadsspel.Elements
{
	public class TradingPost : Building
	{
		private SyncListInt visitedTeams = new SyncListInt();

		//[Command]
		public void CmdAddTeamToList()
		{
			visitedTeams.Add((int)GameObject.FindWithTag("Player").GetComponent<Person>().Team);
		}

		private new void Start()
		{
			mBuildingType = BuildingType.Tradingpost;
			base.Start();
		}

		public List<int> VisitedTeams {
			get {
				List<int> teams = new List<int>();
				foreach(int team in visitedTeams) {
					teams.Add(team);
				}
				return teams;
			}
		}
	}
}