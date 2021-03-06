using Photon;
using Stadsspel.Elements;
using UnityEngine;

public class Team : PunBehaviour
{
	[SerializeField]
	private TeamID m_TeamID;

	[SerializeField]
	private int m_TotalMoney = 0;

	private int m_AmountOfDistricts = 0;

	private BankAccount m_BankAccount;

	public TeamID TeamID {
		get {
			return m_TeamID;
		}
	}

	public int AmountOfDistricts {
		get {
			return m_AmountOfDistricts;
		}
	}

	public int TotalMoney {
		get {
			return m_TotalMoney;
		}
	}

	public BankAccount BankAccount {
		get {
			return m_BankAccount;
		}

		set {
			m_BankAccount = value;
		}
	}

	/// <summary>
	/// Initialises the class before Start.
	/// </summary>
	private void Awake()
	{
		transform.SetParent(GameManager.s_Singleton.transform);
		m_TeamID = (TeamID)(transform.GetSiblingIndex() + 1);
		name = TeamID.ToString();
	}

	/// <summary>
	/// [PunRPC] Adds or removes the passed amount of money to the team's money.
	/// </summary>
	[PunRPC]
	public void AddOrRemoveMoney(int amount)
	{
		m_TotalMoney += amount;
	}

	/// <summary>
	/// Adds or removes the amount of districts passed.
	/// </summary>
	public void AddOrRemoveDistrict(int amount)
	{
		m_AmountOfDistricts += amount;
	}

	/// <summary>
	/// Initialises the class.
	/// </summary>
	private void Start()
	{
		m_BankAccount = GetComponent<BankAccount>();
		m_BankAccount.Team = m_TeamID;
	}

	/// <summary>
	/// Starts a money transaction over RPC.
	/// </summary>
	public void PlayerTransaction(int amount)
	{
		m_BankAccount.PlayerTransaction(amount);
		m_BankAccount.GetComponent<PhotonView>().RPC("Transaction", PhotonTargets.All, amount);
	}
}
