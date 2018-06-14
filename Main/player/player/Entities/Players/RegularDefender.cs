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
using System.Collections.Generic;

namespace RoboCup
{
    public class RegularDefender : RegularPlayer
    {

        public RegularDefender(Team team, ICoach coach, int playerNumber) 
            : base(team, coach, playerNumber)
        {
        }

        public RegularDefender(Team team, ICoach coach, PointF startPoint, PolygonBorders borders, int playerNumber)
            : base(team, coach, startPoint, borders, playerNumber)
        {
        }


		public override void play()
		{
			if (m_side == 'r')
			{
				m_borders.chageSide((float)-1);        //fix borders acording to side
				m_startPosition.X = m_startPosition.X * -1;
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
							GoToPosition(m_startPosition.X, m_startPosition.Y);
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
								// We know where is ball and we can kick it
								// so look for goal
								var p = GetNextPlayerToPass(out double distance);
								float powerPass = 100;
								if (distance >= 20) powerPass = 100;
								else powerPass = ((float)distance / 20) * 100;
								Kick(p.X, p.Y, powerPass);

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
			}

		}

	}
}
