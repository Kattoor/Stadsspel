﻿using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class Element : NetworkBehaviour
{
	private float mActionRadius;

	public Element()
	{
		throw new System.NotImplementedException();
	}

	public Vector2 Position {
		get {
			throw new System.NotImplementedException();
		}

		set {
		}
	}

	/// <summary>
	/// Checks if the given position is within range based on the action radius
	/// </summary>
	public bool IsInRadius(Vector2 pos)
	{
		throw new System.NotImplementedException();
	}
}