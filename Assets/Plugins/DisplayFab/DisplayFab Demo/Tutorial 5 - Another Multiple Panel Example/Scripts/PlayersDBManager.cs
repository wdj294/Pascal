using UnityEngine;
using System;
using System.Collections.Generic;




[Serializable]
public class PlayerInfo
{
    public int playerUID; //Player's Unique ID
    public string playerName;
    public int playerStrength;
    public int playerTeamUID; //Team UID the player belongs to
}

public class PlayersDBManager : MonoBehaviour
{

    public List<PlayerInfo> playerData;

    /// <summary>
    /// This is used to filter players by team UID and display it in the instantiated list using DisplayFab
    /// </summary>
    public List<PlayerInfo> filteredPlayerData;

    public void FetchFilteredPlayerList(int teamUID)
    {
        filteredPlayerData = new List<PlayerInfo>();

        filteredPlayerData = playerData.FindAll(x => x.playerTeamUID == teamUID);

    }


}