﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class Person : Element
{
	private List<int> illegalItems = new List<int>();

	//legalItems[(int)Items.diploma] = 10; Bijvoorbeeld
	private List<int> legalItems = new List<int>();

	[SyncVar]
	[SerializeField]
	private int mAmountOfMoney = 0;

	protected new void Start()
	{
		ActionRadius = 40;

		//instantiate list with 3 numbers for each list.
		for (int i = 0; i < 3; i++) {
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
		for (int i = 0; i < legalItems.Count; i++) {
			legalItems[i] = 0;
		}
	}

	public void ResetIllegalItems()
	{
		for (int i = 0; i < illegalItems.Count; i++) {
			illegalItems[i] = 0;
		}
	}

	public void GetRobbed()
	{
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
		for (int i = 0; i < items.Count; i++) {
			legalItems[i] += items[i];
		}
	}

	public void AddIllegalItems(List<int> items)
	{
		for (int i = 0; i < items.Count; i++) {
			illegalItems[i] += items[i];
		}
	}

	public void MoneyTransaction(int money)
	{
		mAmountOfMoney += money;
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

	public override void OnNameChange(string newName)
	{
		transform.GetChild(0).GetComponent<TextMesh>().text = newName;
	}
}
