﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class BankAccountManager : NetworkBehaviour
{
	private static List<BankAccount> bankAccounts = new List<BankAccount>();

	void Start()
	{
		CmdAddToList();
	}

	public static List<BankAccount> BankAccounts {
		get { return bankAccounts; }
	}

	public BankAccount CreateBankAccount(TeamID id)
	{
		return new BankAccount(id);
	}

	[Command]
	void CmdAddToList()
	{
		// this code is only executed on the server
		RpcAddToList(); // invoke Rpc on all clients
	}

	[ClientRpc]
	void RpcAddToList()
	{
		// this code is executed on all clients
		for (byte i = 0; i < GameManager.s_Singleton.Teams.Length; i++) {
			bankAccounts.Add(CreateBankAccount((TeamID)i));
		}
	}

	[Command]
	public void CmdTransaction(TeamID teamID, int amount)
	{
		RpcTransaction(teamID, amount);
	}

	[ClientRpc]
	void RpcTransaction(TeamID teamID, int amount)
	{
		bankAccounts[(int)teamID].Transaction(amount);
	}

}