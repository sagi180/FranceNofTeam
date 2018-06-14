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
    public class RegularPlayer : Player
    {
        public int PlayerNumber { get; set; }
		private PolygonBorders m_borders;
		private uint stuckCounter;
        public RegularPlayer(Team team, ICoach coach , int playerNumber) : base(team, coach)
        {
            PlayerNumber = playerNumber;
            m_startPosition = new PointF(playerNumber * 5, playerNumber*5);
			stuckCounter = 0;

		}

		public RegularPlayer(Team team, ICoach coach, PointF startPoint, PolygonBorders borders, int playerNumber)
			: base(team, coach)
		{
			m_startPosition = startPoint;
			m_borders = borders;
			PlayerNumber = playerNumber;
			stuckCounter = 0;
		}

		//public override void play()
		//      {
		//	//////////// Goal Keeper //////////////
		//	// GoToPosition( (float)-52.5, (float) 0.0);
		//	///////////////////////////////////////

		//	//GoToPosition(m_startPosition.X, m_startPosition.Y);
		//	SeenObject obj;

		//	while (!m_timeOver)
		//          {
		//		var currentPosition = m_coach.GetMyPos(PlayerNumber);
		//		if (currentPosition != null)
		//		{
		//			// check if we are in borders
		//			if (!m_borders.IsInBorders(currentPosition.Pos.Value))
		//			{
		//				GoToPosition(m_startPosition.X, m_startPosition.Y);
		//			}
		//		}
		//		//else
		//		//{
		//		//	continue;
		//		//}

		//		//var ballPosition = m_coach.GetBall();
		//		//if (ballPosition != null)
		//		//{
		//		//	// check if ball is in borders
		//		//	if (m_borders.IsInBorders(ballPosition.Pos.Value))
		//		//	{
		//		//		var bodyInfo = GetBodyInfo();

		//		//		obj = m_memory.GetSeenObject("ball");
		//		//		if (obj == null)
		//		//		{
		//		//			// If you don't know where is ball then find it
		//		//			m_robot.Turn(40);
		//		//			m_memory.waitForNewInfo();
		//		//		}
		//		//		//else if (obj.Distance.Value > 1.5)
		//		//		else if (obj.Distance.Value > 1.0)
		//		//		{
		//		//			// If ball is too far then
		//		//			// turn to ball or 
		//		//			// if we have correct direction then go to ball
		//		//			if (obj.Direction.Value != 0)
		//		//				m_robot.Turn(obj.Direction.Value);
		//		//			else
		//		//				m_robot.Dash(10 * obj.Distance.Value);
		//		//		}
		//		//		else
		//		//		{
		//		//			// We know where is ball and we can kick it
		//		//			// so look for goal
		//		//			if (m_side == 'l')
		//		//				obj = m_memory.GetSeenObject("goal r");
		//		//			else
		//		//				obj = m_memory.GetSeenObject("goal l");

		//		//			if (obj == null)
		//		//			{
		//		//				m_robot.Turn(40);
		//		//				m_memory.waitForNewInfo();
		//		//			}
		//		//			else
		//		//				m_robot.Kick(200, obj.Direction.Value);
		//		//		}

		//		//		// sleep one step to ensure that we will not send
		//		//		// two commands in one cycle.
		//		//		try
		//		//		{
		//		//			Thread.Sleep(2 * SoccerParams.simulator_step);
		//		//		}
		//		//		catch (Exception e)
		//		//		{

		//		//		}

		//		//		//if (LogicCalc.GetDistance(currentPosition.Pos.Value.X, currentPosition.Pos.Value.Y, ballPosition.Pos.Value.X, ballPosition.Pos.Value.Y) < 1.5)
		//		//		//{
		//		//		//	var obj = m_memory.GetSeenObject("goal r");
		//		//		//	if (obj == null)
		//		//		//	{
		//		//		//		m_robot.Turn(40);
		//		//		//		m_memory.waitForNewInfo();
		//		//		//	}
		//		//		//	else
		//		//		//		m_robot.Kick(200, obj.Direction.Value);
		//		//		//}
		//		//		//else
		//		//		//{
		//		//		//	GoToPosition(ballPosition.Pos.Value.X, ballPosition.Pos.Value.Y);
		//		//		//}
		//		//	}
		//		//}

		//		//// m_robot.Dash(PlayerNumber);
		//		//m_robot.Turn(30);
		//  //        }
		//  //    }


		public override void play()
		{
			SeenObject obj;

			// First go to start position
			GoToPosition(m_startPosition.X, m_startPosition.Y);

			while (!m_timeOver)
			{
					// Get ball position from coach
				var ballPosition = m_coach.GetBall();
				if (ballPosition == null) // No ball position
				{
					continue;
				}
				else 
				{
					if (!m_borders.IsInBorders(ballPosition.Pos.Value))
					{
						continue; // Ball is not in borders
					}

				}

				var bodyInfo = GetBodyInfo();

				obj = m_memory.GetSeenObject("ball");
				if (obj == null)
				{
					// If you don't know where is ball then find it
					if (stuckCounter > 8)
					{
						stuckCounter = 0;
						if (obj != null)
							m_robot.Turn(180 - obj.Direction.Value);
						m_robot.Dash(200);
					
					}
					else
					{
						stuckCounter++;
						m_robot.Turn(45);
						m_memory.waitForNewInfo();
					}
				}
				//else if (obj.Distance.Value > 1.5)
				else if (obj.Distance.Value > 1.0)
				{
					// If ball is too far then
					// turn to ball or 
					// if we have correct direction then go to ball
					if (obj.Direction.Value != 0)
						m_robot.Turn(obj.Direction.Value);
					else
						m_robot.Dash(10 * obj.Distance.Value);
				}
				else
				{
					// We know where is ball and we can kick it
					// so look for goal
					if (m_side == 'l')
						obj = m_memory.GetSeenObject("goal r");
					else
						obj = m_memory.GetSeenObject("goal l");

					if (obj == null)
					{
						m_robot.Turn(41);
						m_memory.waitForNewInfo();
					}
					else
						m_robot.Kick(200, obj.Direction.Value);
				}

				var currentPosition = m_coach.GetMyPos(PlayerNumber);
				if (currentPosition != null)
				{
					// check if we are in borders
					if (!m_borders.IsInBorders(currentPosition.Pos.Value))
					{
						GoToPosition(m_startPosition.X, m_startPosition.Y);
					}
				}
				else
				{
					continue;
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

		private void GoToPositionOneStep(float x, float y)
        {
    
            var seenCoachObject = m_coach.GetMyPos(PlayerNumber);
            while(seenCoachObject == null)
            {
                m_memory.waitForNewInfo();
                seenCoachObject = m_coach.GetMyPos(PlayerNumber);
            }
            var pos = seenCoachObject.Pos;
            var angel = LogicCalc.GetAngleBetweenTwoPoints(pos.Value.X, pos.Value.Y, x, y);
            m_robot.Turn( (double)(angel + seenCoachObject.BodyAngle) % 360);
            bool isInPos = false;
            double? last_dis = null;
    //        while(!isInPos && !m_timeOver)
    //        {
				/////
				Console.WriteLine(pos.Value.X.ToString() + ", " + pos.Value.Y.ToString());
				///
				seenCoachObject = m_coach.GetMyPos(PlayerNumber);
                pos = seenCoachObject.Pos;

                var dis = LogicCalc.GetDistance(x, y, pos.Value.X, pos.Value.Y);
                System.Console.WriteLine($"{dis}");
                if(last_dis != null)
                {
                    angel = LogicCalc.GetAngleBetweenTwoPoints(pos.Value.X, pos.Value.Y, x, y);
                    if (Math.Abs((double)(angel - seenCoachObject.BodyAngle)) > 10)
                    {
                        m_robot.Turn((double)(angel - seenCoachObject.BodyAngle));
                    }
                    else if (dis > last_dis.Value)
                    {
                        m_robot.Turn((double)(angel + seenCoachObject.BodyAngle) % 360);
                    }
                }
                last_dis = dis;
                if (dis  < 1.5)
                {
                    return;
                }
                m_robot.Dash(100);
                m_memory.waitForNewInfo();
            //}

        }
		private void GoToPosition(float x, float y)
		{

			var seenCoachObject = m_coach.GetMyPos(PlayerNumber);
			while (seenCoachObject == null)
			{
				m_memory.waitForNewInfo();
				seenCoachObject = m_coach.GetMyPos(PlayerNumber);
			}
			var pos = seenCoachObject.Pos;
			var angel = LogicCalc.GetAngleBetweenTwoPoints(pos.Value.X, pos.Value.Y, x, y);
			m_robot.Turn((double)(angel + seenCoachObject.BodyAngle) % 360);
			bool isInPos = false;
			double? last_dis = null;
			while (!isInPos && !m_timeOver)
			{
				///
				Console.WriteLine(pos.Value.X.ToString() + ", " + pos.Value.Y.ToString());
				///
				seenCoachObject = m_coach.GetMyPos(PlayerNumber);
				pos = seenCoachObject.Pos;

				var dis = LogicCalc.GetDistance(x, y, pos.Value.X, pos.Value.Y);
				System.Console.WriteLine($"{dis}");
				if (last_dis != null)
				{
					angel = LogicCalc.GetAngleBetweenTwoPoints(pos.Value.X, pos.Value.Y, x, y);
					if (Math.Abs((double)(angel - seenCoachObject.BodyAngle)) > 10)
					{
						m_robot.Turn((double)(angel - seenCoachObject.BodyAngle));
					}
					else if (dis > last_dis.Value)
					{
						m_robot.Turn((double)(angel + seenCoachObject.BodyAngle) % 360);
					}
				}
				last_dis = dis;
				if (dis < 1.5)
				{
					return;
				}
				m_robot.Dash(100);
				m_memory.waitForNewInfo();
			}

		}

		private SenseBodyInfo GetBodyInfo()
		{
			m_robot.SenseBody();
			SenseBodyInfo bodyInfo = null;
			while (bodyInfo == null)
			{
				//Thread.Sleep(WAIT_FOR_MSG_TIME);
				bodyInfo = m_memory.getBodyInfo();
			}

			return bodyInfo;
		}

		//private void TurnToGate()
		//{
		//	//var seenCoachObject = m_coach.GetMyPos(PlayerNumber);
		//	//var angel = GetAngelFromGate();
		//	//m_robot.Turn((double)(angel - seenCoachObject.BodyAngle));
		//	if (obj == null)
		//	{
		//		m_robot.Turn(40);
		//		m_memory.waitForNewInfo();
		//	}
		//}
		//private double GetAngelFromGate()
		//{
		//	//SeenObject obj;
		//	//obj = m_memory.GetSeenObject("goal r");
		//	////var seenCoachObject = m_coach.GetMyPos(PlayerNumber);
		//	////var angel = LogicCalc.GetAngleBetweenTwoPoints(seenCoachObject.Pos.Value.X, seenCoachObject.Pos.Value.Y, 52.5f, 0);
		//	//return angel;
		//}
	}
}
