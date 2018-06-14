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
    public class RegularGoalkeeper : RegularPlayer
    {
        public RegularGoalkeeper(Team team, ICoach coach, int playerNumber) :
            base(team, coach, playerNumber)
        {

        }

        public RegularGoalkeeper(Team team, ICoach coach, PointF startPoint, PolygonBorders borders, int playerNumber)
            : base(team, coach, startPoint, borders, playerNumber) { }



		public override void play()
		{
			SeenCoachObject last_ball_pos = null;
			float goalLine;

			if (m_side == 'l')
			{
				goalLine = FieldLocations.LeftLine;

			}
			else
			{
				goalLine = FieldLocations.RightLine;
			}

			SeenObject obj;


			while (!m_timeOver)
			{
				try
				{
					// Get ball position from coach
					var coachBall = m_coach.GetBall();



					if (coachBall == null)
					{
						// No ball position
						GoToPosition(goalLine, (float)0);
					}
					else
					{
						last_ball_pos = coachBall;

						if (!m_borders.IsInBorders(coachBall.Pos.Value))
						{
							// Ball is not in borders

							var disAB = LogicCalc.GetDistance(last_ball_pos.Pos.Value.X, last_ball_pos.Pos.Value.Y, goalLine, (float)7);
							var disAC = LogicCalc.GetDistance(last_ball_pos.Pos.Value.X, last_ball_pos.Pos.Value.Y, goalLine, (float)-7);
							var yGK_Pos = 7 - ((14 * (disAB / disAC)) / (1 + (disAB / disAC)));
							GoToPosition(goalLine, (float)yGK_Pos);
						}
						else
						{
							obj = m_memory.GetSeenObject("ball");
							if (obj == null)
							{
								// If you don't know where is ball then find it
								var ball = m_coach.GetBall();
								if (ball != null)
								{
									var angel = GetAngleToPoint(ball.Pos.Value);
									m_robot.Turn(angel);
								}
								else
								{
									m_robot.Turn(40);
								}
								m_memory.waitForNewInfo();
							}

							else if (obj.Distance.Value > 1.3)
							{
								// If ball is too far then
								// turn to ball or 
								// if we have correct direction then go to ball
								//if (obj.Direction.Value != 0)
								if (obj.Direction.Value > 20)
									m_robot.Turn(obj.Direction.Value);
								else
									m_robot.Dash(20 * obj.Distance.Value);
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

								//if (m_side == 'l')
								//{
								//	obj = m_memory.GetSeenObject("goal r");
								//	var pos = GetNextPlayerToPass();
								//	Kick(pos.X, pos.Y, 100);
								//}

								//else
								//{
								//	obj = m_memory.GetSeenObject("goal l");
								//	var pos = GetNextPlayerToPass();
								//	Kick(pos.X, pos.Y, 100);
								//}

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





				//		//////////////////////////////////////////////////////////
				//		var ball_info = m_coach.GetBall();
				//if (ball_info != null)
				//{
				//	last_ball_pos = ball_info;
				//	m_memory.waitForNewInfo();
				//}

				////triangle ABC: A is the ball, BC is goal 
				//if (!m_borders.IsInBorders(last_ball_pos.Pos.Value))
				//{
				//	var disAB = LogicCalc.GetDistance(last_ball_pos.Pos.Value.X, last_ball_pos.Pos.Value.Y, goalLine, (float)7);
				//	var disAC = LogicCalc.GetDistance(last_ball_pos.Pos.Value.X, last_ball_pos.Pos.Value.Y, goalLine, (float)-7);
				//	var yGK_Pos = 7 - ((14 * (disAB / disAC)) / (1 + (disAB / disAC)));
				//	GoToPosition(FieldLocations.LeftLine, (float)yGK_Pos);
				//}

				//if (ball_info != null)
				//{
				//	//var dis = LogicCalc.GetDistance(ball_info.Pos.Value.X, ball_info.Pos.Value.Y, FieldLocations.LeftLine, (float)yGK_Pos);
				//	//if (dis < 1.5)
				//	//{
				//	//	//Kick(0,0,100);
				//	//	m_robot.Kick(100, 45);
				//	//}

				//	obj = m_memory.GetSeenObject("ball");
				//	if (obj == null)
				//	{
				//		// If you don't know where is ball then find it
				//		var ball = m_coach.GetBall();
				//		if (ball != null)
				//		{
				//			var angel = GetAngleToPoint(ball.Pos.Value);
				//			m_robot.Turn(angel);
				//		}
				//		else
				//		{
				//			m_robot.Turn(40);
				//		}
				//		m_memory.waitForNewInfo();
				//	}

				//	else if (obj.Distance.Value > 1.3)
				//	{
				//		// If ball is too far then
				//		// turn to ball or 
				//		// if we have correct direction then go to ball
				//		//if (obj.Direction.Value != 0)
				//		if (obj.Direction.Value > 20)
				//			m_robot.Turn(obj.Direction.Value);
				//		else
				//			m_robot.Dash(20 * obj.Distance.Value);
				//	}
				//	else
				//	{
				//		// We know where is ball and we can kick it
				//		// so look for goal
				//		var p = GetNextPlayerToPass(out double distance);
				//		float powerPass = 100;
				//		if (distance >= 20) powerPass = 100;
				//		else powerPass = ((float)distance / 20) * 100;
				//		Kick(p.X, p.Y, powerPass);

				//		//if (m_side == 'l')
				//		//{
				//		//	obj = m_memory.GetSeenObject("goal r");
				//		//	var pos = GetNextPlayerToPass();
				//		//	Kick(pos.X, pos.Y, 100);
				//		//}

				//		//else
				//		//{
				//		//	obj = m_memory.GetSeenObject("goal l");
				//		//	var pos = GetNextPlayerToPass();
				//		//	Kick(pos.X, pos.Y, 100);
				//		//}

				//	}

				//}




			
		}
	}
}



