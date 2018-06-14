using RoboCup.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup
{
    public class Formation_4_4_2 : IFormation
    {
        public List<Player> InitTeam(Team team, ICoach coach)
        {
            var players = new List<Player>();
            //players.Add(new DefenderExample(team, coach));
            //players.Add(new AttackerExample(team, coach));
            //players.Add(new Goalkeeper(team, coach));
            //players.Add(new RegularPlayer(team, coach,1));
            players.Add(new RegularGoalkeeper(team, coach, 1));
            //players.Add(new RegularPlayer(team, coach, 1));
            players.Add(new AttackerExample(team, coach));
            players.Add(new AttackerExample(team, coach));

            //players.Add(new Goalkeeper(team, coach));
            return players;
        }
    }
}
