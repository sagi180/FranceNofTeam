using RoboCup.Entities;
using System.Drawing;
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
            PolygonBorders leftUpBorders = new PolygonBorders(new PointF[] { new PointF(-52.5f, -34), new PointF(-1, -34), new PointF(-1, 1), new PointF(-40, 1), new PointF(-40, -10), new PointF(-52.5f, -10) });
            PolygonBorders rightUpBorders = new PolygonBorders(new PointF[] { new PointF(-8, -34), new PointF(52.5f, -34), new PointF(52.5f, 1), new PointF(-8, 1) });
            PolygonBorders leftDownBorders = new PolygonBorders(new PointF[] { new PointF(-52.5f, 34), new PointF(-1, 34), new PointF(-1, -1), new PointF(-40, -1), new PointF(-40, 10), new PointF(-52.5f, 10) });
            PolygonBorders rightDownBorders = new PolygonBorders(new PointF[] { new PointF(-8, -1), new PointF(52.5f, -1), new PointF(52.5f, 34), new PointF(-8, 34) });
            PolygonBorders goalKeeperBorders = new PolygonBorders(new PointF[] { new PointF(-52.5f, 10), new PointF(-40f, 10), new PointF(-40f, -10), new PointF(-52.5f, -10) });

            var players = new List<Player>();

            players.Add(new RegularAttacker(team, coach, new PointF(0, -10), rightUpBorders, 1));
            players.Add(new RegularAttacker(team, coach, new PointF(0, 10), rightDownBorders, 2));
            //players.Add(new RegularPlayer(team, coach, new PointF(-35, 13), leftDownBorders, 3));
            //players.Add(new RegularPlayer(team, coach, new PointF(-35, -13), leftUpBorders, 4));
            players.Add(new RegularDefender(team, coach, new PointF(-35, 10), leftDownBorders, 3));
            players.Add(new RegularDefender(team, coach, new PointF(-35, -10), leftUpBorders, 4));
            players.Add(new RegularGoalkeeper(team, coach, new PointF(-52.5f, 0), goalKeeperBorders, 5));


            //var players = new List<Player>();
            //         for (int i = 1; i < 2 ;i++)
            //             players.Add(new RegularPlayer(team, coach, i));

            return players;
        }
    }
}
