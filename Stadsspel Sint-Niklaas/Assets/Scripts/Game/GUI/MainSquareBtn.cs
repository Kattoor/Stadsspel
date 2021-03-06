﻿using UnityEngine;

public class MainSquareBtn : MonoBehaviour
{
	private MainSquareArrow m_Arrow;

	// Use this for initialization

	/// <summary>
	/// Event for the show home button. Toggles the visibility state of the home arrow.
	/// </summary>
	public void ToggleMainSquareArrow(bool turnOn)
	{
		if(!m_Arrow) {
			m_Arrow = GameManager.s_Singleton.Player.gameObject.GetComponentInChildren<MainSquareArrow>();

		}

		m_Arrow.gameObject.SetActive(turnOn);
	}
}
