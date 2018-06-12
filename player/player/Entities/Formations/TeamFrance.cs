using RoboCup.Entities;
using System.Collections.Generic;

namespace RoboCup
{
    public class TeamFrance : IFormation
    {
        public TeamFrance()
        {
        }

        public List<Player> InitTeam(Team team, ICoach coach)
        {
            var players = new List<Player>();
            for (int i = 1; i < 2 ;i++)
                players.Add(new RegularPlayer(team, coach, i));
           // players.Add(new AttackerExample(team, coach));

            return players;
        }
    }
}
