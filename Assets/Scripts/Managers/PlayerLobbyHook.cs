using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerLobbyHook : UnityStandardAssets.Network.LobbyHook 
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        if (lobbyPlayer == null)
            return;

        UnityStandardAssets.Network.LobbyPlayer lp = lobbyPlayer.GetComponent<UnityStandardAssets.Network.LobbyPlayer>();

        if(lp != null)
            GameManager.AddPlayer(gamePlayer, lp.slot, lp.playerColor, lp.nameInput.text, lp.playerControllerId);
    }
}
