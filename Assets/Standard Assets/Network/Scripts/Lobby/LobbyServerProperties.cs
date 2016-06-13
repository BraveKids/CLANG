using System.Collections;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class LobbyServerProperties {
    public string serverInfoText;
    public string slotInfo;
    public NetworkID networkID;
    public Color color;

    public LobbyServerProperties (MatchDesc match, Color c) {
        serverInfoText = match.name;
        slotInfo = match.currentSize.ToString() + "/" + match.maxSize.ToString(); ;
        networkID = match.networkId;
        color = c;
    }
}
