using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using RoboCup.Entities;
using RoboCup.Infrastructure;
using RoboCup.Logic;
using RoboCup;

namespace RoboCup
{
	public class RegularAttacker : RegularPlayer
	{

		public RegularAttacker(Team team, ICoach coach, PointF startPoint, PolygonBorders borders, int playerNumber)
			: base(team, coach, startPoint, borders, playerNumber) { }

		public override void play()
		{
			float sideFactor = 1;
			if (m_side == 'r')
			{
				m_borders.chageSide((float)-1);        //fix borders acording to side
				m_startPosition.X = m_startPosition.X * -1;
				sideFactor = -1;

			}

			// First go to start position
			GoToPosition(m_startPosition.X, m_startPosition.Y);

			while (!m_timeOver)
			{
				try
				{
					//var memory = m_memory.getBodyInfo();
					//if (memory != null)
					//	Console.WriteLine("PLayer: " + m_number + ", " + memory.StaminaValue);

					// Get ball position from coach
					var coachBall = m_coach.GetBall();
					if (coachBall == null)
					{
						// No ball position
					}
					else
					{
						if (!m_borders.IsInBorders(coachBall.Pos.Value))
						{
							// Ball is not in borders
							// Go To start position TODO: something else

							//GoToPosition(m_startPosition.X, m_startPosition.Y);

							if(m_side=='l' && coachBall.Pos.Value.X  > 10) GoToPosition(m_startPosition.X + 15, m_startPosition.Y);
							else if (m_side == 'r' && coachBall.Pos.Value.X < -10) GoToPosition(m_startPosition.X - 15, m_startPosition.Y);
							else GoToPosition(m_startPosition.X, m_startPosition.Y);


						}
						else
						{
							var bodyInfo = GetBodyInfo();

							var myBall = m_memory.GetSeenObject("ball");
							if (myBall == null)
							{
								// If you don't know where is ball then find it
								var angel = GetAngleToPoint(coachBall.Pos.Value);
								m_robot.Turn(angel);
							}
							else if (myBall.Distance.Value > 1.3)
							{
								// If ball is too far then
								// turn to ball or 
								// if we have correct direction then go to ball
								//if (obj.Direction.Value != 0)
								if (myBall.Direction.Value > 20)
									m_robot.Turn(myBall.Direction.Value);
								else
									m_robot.Dash(10 * myBall.Distance.Value);
							}
							else
							{

								float Gx;
								float Gy = 0;
								if (m_side == 'l')
								{
									Gx = FieldLocations.LeftLine;
								}
								else
								{
									Gx = FieldLocations.RightLine;
								}
								var myPos = m_coach.GetMyPos(m_number);
								var dis = LogicCalc.GetDistance(Gx, Gy, myPos.Pos.Value.X, myPos.Pos.Value.Y);
								// We know where is ball and we can kick it
								// so look for goal
								if (m_side == 'l')
								{
									if (dis > 25)
									{
										if (NeedToPass())
										{
											var p = GetNextPlayerToPass(out double distance);
											float powerPass = 100;
											if (distance >= 20) powerPass = 100;
											else powerPass = ((float)distance / 20) * 100;
											Kick(p.X, p.Y, powerPass);

										}
										else
										{
											Kick(FieldLocations.RightLine, 0, 20); // TODO: kick forward
										}
										
									}
									else
									{
											double angel = ShotToGoal();
											m_robot.Kick(100, angel);
											//Kick(FieldLocations.LeftLine, 0, 100); // TODO: kick to angel
									}
								}
								else
								{
									
									if (dis > 25)
									{
										if (NeedToPass())
										{
											var p = GetNextPlayerToPass(out double distance);

											float powerPass = 100;
											if (distance >= 20) powerPass = 100;
											else powerPass = ((float)distance / 20) * 100;
											Kick(p.X, p.Y, powerPass);

										}
										else
										{
											Kick(FieldLocations.LeftLine, 0, 20); // TODO: kick forward
										}
									}
									else
									{
						
											double angel = ShotToGoal();
											m_robot.Kick(100, angel);
											//Kick(FieldLocations.LeftLine, 0, 100); // TODO: kick to angel
										
										
									}
								}

							}
						}
					}
				}
				catch (Exception e)
				{

				}

				// sleep one step to ensure that we will not send
				// two commands in one cycle.
				try
				{
					Thread.Sleep(2 * SoccerParams.simulator_step);
				}
				catch (Exception e)
				{

				}

			} // play()
		}


		public bool NeedToPass()
		{
			var myPlayer = m_coach.GetMyPos(m_number);
			var opponentPlayers = m_coach.GetOpponentPlayers();
			int num = 0;

			int attcker2;
			if (m_number == 1) attcker2 = 2;
			else attcker2 = 1;

			var attcker2Pos = m_coach.GetMyPos(attcker2);
			if (attcker2Pos != null)
			{
				var dist = LogicCalc.GetDistance(myPlayer.Pos.Value, attcker2Pos.Pos.Value);
				if (dist < 10) return false;
				if(attcker2Pos.Pos.Value.X < myPlayer.Pos.Value.X) return false;	//Pass only Fwd

			}

			foreach (var opponentPlayer in opponentPlayers)
			{
				var dis = LogicCalc.GetDistance(myPlayer.Pos.Value, opponentPlayer.Pos.Value);
				if (dis < 10)
				{
					num += 1;
					if (num >= 2)
					{
						return true;
					}
				}
			}
			return (num >= 2);
		}

		public override PointF GetNextPlayerToPass(out double distance)
		{
			//

			int attcker2;
			if (m_number == 1) attcker2 = 2;
			else attcker2 = 1;

			var myPlayer = m_coach.GetMyPos(m_number);
			var attcker2Pos = m_coach.GetMyPos(attcker2);
			if (attcker2Pos != null)
			{
				distance = LogicCalc.GetDistance(myPlayer.Pos.Value, attcker2Pos.Pos.Value);
				return attcker2Pos.Pos.Value;
			}
			else
			{
				var nib = GetNymberOfNerestPlayers(20);
				var dis = GetNymberOfNerestPlayers();

				var name1 = dis.FirstOrDefault(x => x.Value == dis.Values.Min()).Key;
				var dis1 = dis[name1];
				dis.Remove(name1);
				var name2 = dis.FirstOrDefault(x => x.Value == dis.Values.Min()).Key;
				var dis2 = dis[name2];
				var pos = m_coach.GetPlayer(nib[name1] < nib[name2] ? name1 : name2);

				distance = nib[name1] < nib[name2] ? dis1 : dis2;
				return pos.Pos.Value;
			}
			
		}

	} // class
} // namespace
