using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerLobbyHook : UnityStandardAssets.Network.LobbyHook 
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        if (lobbyPlayer == null)
            return;

        UnityStandardAssets.Network.AutoLobbyPlayer lp = lobbyPlayer.GetComponent<UnityStandardAssets.Network.AutoLobbyPlayer>();

        if(lp != null)
            GameManager.AddPlayer(gamePlayer, lp.slot, lp.playerColor, lp.playerName, lp.playerControllerId);
    }
}
