﻿using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook {

	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        SetupLocalPlayer localPlayer = gamePlayer.GetComponent<SetupLocalPlayer>();

        localPlayer.pName = lobby.nameInput.text;
        localPlayer.playerColor = lobby.playerColor;
        localPlayer.teamID = lobby.teamColor;
    }
}
