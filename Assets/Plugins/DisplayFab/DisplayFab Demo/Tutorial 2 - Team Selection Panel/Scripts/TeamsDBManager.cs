using UnityEngine;
using System;
using System.Collections.Generic;


 

[Serializable]
public class TeamInfo  {

    public string teamName;
    public int teamUID; //added in DisplayFab v1.2.0 demo
    public int teamStrength;
    public int teamSkill;
    public int teamRanking;
    public Sprite teamLogo;
 
}

public class TeamsDBManager : MonoBehaviour
{
  
    public List<TeamInfo> teamData;

    public Sprite GetTeamLogoByUID(int teamUID)
    {
        return teamData.Find(x => x.teamUID == teamUID).teamLogo;
    }

}