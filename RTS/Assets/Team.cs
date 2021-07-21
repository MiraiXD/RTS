using UnityEngine;
public class Team 
{
    public Team(Color teamColor)
    {
        ID = teamCount;
        teamCount++;

        this.teamColor = teamColor;
    }
    private static int teamCount = 0;
    private readonly int ID;

    public readonly Color teamColor;

    public bool CompareTeams(Team other)
    {
        return true;// this == other;
    }
}