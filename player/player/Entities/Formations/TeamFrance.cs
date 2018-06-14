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
			PolygonBorders leftUpBorders = new PolygonBorders(new PointF[] { new PointF(-52.5f, -34), new PointF(1, -34), new PointF(1, 7), new PointF(-52.5f, 7) });
			PolygonBorders rightUpBorders = new PolygonBorders(new PointF[] { new PointF(-1, -34), new PointF(52.5f, -34), new PointF(52.5f, 7), new PointF(-1, 7) });
			PolygonBorders leftDownBorders = new PolygonBorders(new PointF[] { new PointF(-52.5f, -7), new PointF(1, -7), new PointF(1, 34), new PointF(-52.5f, 34) });
			PolygonBorders rightDownBorders = new PolygonBorders(new PointF[] { new PointF(-1, -7), new PointF(52.5f, -7), new PointF(52.5f, 34), new PointF(-1, 34) });
			//PolygonBorders goalKeeperBorders = new PolygonBorders(new PointF[] { new PointF(-52.5f, 7), new PointF(-47.5f, 7), new PointF(-47.5f, -7), new PointF(-52.5f, -7) });

			var players = new List<Player>();
			players.Add(new RegularPlayer(team, coach, new PointF(-26, -17), leftUpBorders, 1));
			players.Add(new RegularPlayer(team, coach, new PointF(26, -17), rightUpBorders, 2));
			players.Add(new RegularPlayer(team, coach, new PointF(-26, 17), leftDownBorders, 3));
			players.Add(new RegularPlayer(team, coach, new PointF(26, 17), rightDownBorders, 4));
			//players.Add(new RegularPlayer(team, coach, new PointF(-52.5f, 0), goalKeeperBorders, 5));

			//var players = new List<Player>();
			//         for (int i = 1; i < 2 ;i++)
			//             players.Add(new RegularPlayer(team, coach, i));

			return players;
        }
    }
}
