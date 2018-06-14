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
    public class RegularPlayer : Player
    {
        public int PlayerNumber { get; set; }
        protected PolygonBorders m_borders;

        public RegularPlayer(Team team, ICoach coach, int playerNumber) : base(team, coach)
        {
            PlayerNumber = playerNumber;
            m_startPosition = new PointF(playerNumber * 5, playerNumber * 5);
        }

        public RegularPlayer(Team team, ICoach coach, PointF startPoint, PolygonBorders borders, int playerNumber)
            : base(team, coach)
        {
            m_startPosition = startPoint;
            m_borders = borders;
            PlayerNumber = playerNumber;
        }

		public override void HalfTimeEvent(object sender, HearEventArgs e)
		{
			m_robot.Move(m_startPosition.X, m_startPosition.Y);
		}
		public override void kick_off_l_event(object sender, HearEventArgs e)
		{
			m_robot.Move(m_startPosition.X, m_startPosition.Y);
		}
		public override void kick_off_r_event(object sender, HearEventArgs e)
		{
			m_robot.Move(m_startPosition.X, m_startPosition.Y);
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
							if (m_side == 'l')
							{
								var goalR = m_memory.GetSeenObject("goal r");
								if (goalR != null && goalR.Distance.Value > 20)
								{
									Kick(FieldLocations.RightLine, 0, 30); // TODO: kick forward
								}
								else Kick(FieldLocations.RightLine, 0, 100); // TODO: kick to angel
							}

							else
							{
								var goalL = m_memory.GetSeenObject("goal l");
								if (goalL != null && goalL.Distance.Value > 20)
								{
									Kick(FieldLocations.LeftLine, 0, 30); // TODO: kick forward
								}
								else Kick(FieldLocations.LeftLine, 0, 100); // TODO: kick to angel
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
			}

		}

        protected void GoToPosition(float x, float y)
        {
            var seenCoachObject = m_coach.GetMyPos(PlayerNumber);
			if (seenCoachObject == null)
			{
				return;
			}

            var pos = seenCoachObject.Pos;

            var angel = GetAngleToPoint(new PointF(x, y), seenCoachObject);
			if (Math.Abs(angel) > 10)
			{
				m_robot.Turn(angel);
			}
			else
			{
				var dis = LogicCalc.GetDistance(x, y, pos.Value.X, pos.Value.Y);
				float power = (float)10 * (float)dis;
				if (power > 70) power = 70;
				m_robot.Dash(power);
			}
        }

		

		const double Rad2Deg = 180.0 / Math.PI;
        const double Deg2Rad = Math.PI / 180.0;

        public double GetAngleToPoint(PointF targetPoint)
        {
            var myPosByCoach = GetMyPlayerDetailsByCoach();
            var angleToTarget = Calc2PointsAngleByXAxis(myPosByCoach.Pos.Value, targetPoint);
            var myAbsAngle = myPosByCoach.BodyAngle;

            var turnAngle = -1 * (Convert.ToDouble(myAbsAngle) + angleToTarget);

            var fixedAngle = NormalizeTo180(turnAngle);

            return turnAngle;
        }

		

		public SeenCoachObject GetMyPlayerDetailsByCoach()
        {
            var res = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {m_number}");
            while (res == null)
            {
                m_memory.waitForNewInfo();
                res = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {m_number}");
            }
            return res;
        }

		public double GetAngleToPoint(PointF targetPoint, SeenCoachObject myPosByCoach)
		{
			var angleToTarget = Calc2PointsAngleByXAxis(myPosByCoach.Pos.Value, targetPoint);
			var myAbsAngle = myPosByCoach.BodyAngle;

			var turnAngle = -1 * (Convert.ToDouble(myAbsAngle) + angleToTarget);

			var fixedAngle = NormalizeTo180(turnAngle);

			return turnAngle;
		}

		private static double Calc2PointsAngleByXAxis(PointF start, PointF end)
        {
            return Math.Atan2(start.Y - end.Y, end.X - start.X) * Rad2Deg;
        }

        private static double NormalizeTo180(double angle)
        {
            while (Math.Abs(angle) > 180)
            {
                if (angle > 0)
                {
                    angle = angle - 360;
                }
                else
                {
                    angle = angle + 360;
                }
            }

            return angle;
        }

        public void Kick(float x, float y, float power)
        {
            //Console.WriteLine($"Kick ({x},{y}) , {power}");
            var ball_info = m_memory.GetSeenObject("ball");
            while (ball_info == null)
            {
                m_memory.waitForNewInfo();
                ball_info = m_memory.GetSeenObject("ball");
            }

            if (ball_info.Distance.Value < 1.3)
            {
                m_robot.Turn(ball_info.Direction.Value);
                Thread.Sleep(2 * SoccerParams.simulator_step);
                var angel = GetAngleToPoint(new PointF(x, y));
				var myAngl = m_coach.GetMyPos(m_number);
                if (myAngl != null && Math.Abs((double)myAngl.BodyAngle - angel) > 170)
                {
					m_robot.Kick(15, (angel<0)?(angel + 90): (angel - 90));
					
				}
                else
                {
                    m_robot.Kick(power, angel);
                }
                //Thread.Sleep(2 * SoccerParams.simulator_step);
            }


        }
        protected SenseBodyInfo GetBodyInfo()
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


        public virtual PointF GetNextPlayerToPass(out double distance)
        {
            var nib =  GetNymberOfNerestPlayers(20);
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

        public Dictionary<string, double> GetNymberOfNerestPlayers(float r)
        {
            var myPlayers = m_coach.GetMyPlayers(m_number);
            var opponentPlayers = m_coach.GetOpponentPlayers();
            var dic = new Dictionary<string, double>();
            foreach (var myPlayer in myPlayers)
            {
                dic.Add(myPlayer.Name, 0);
                foreach (var opponentPlayer in opponentPlayers)
                {
                    var dis = LogicCalc.GetDistance(myPlayer.Pos.Value, opponentPlayer.Pos.Value);
                    if (dis < r)
                    {
                        dic[myPlayer.Name] += 1;
                    }
                }
            }
            return dic;
        }

        public Dictionary<string, double> GetNymberOfNerestPlayers()
        {
            var myPlayers = m_coach.GetMyPlayers(m_number);
            var ball = m_coach.GetBall();
            var dic = new Dictionary<string, double>();
            foreach (var myPlayer in myPlayers)
            {
                var dis = LogicCalc.GetDistance(myPlayer.Pos.Value, ball.Pos.Value);
                dic.Add(myPlayer.Name, dis);
            }
            return dic;
        }

		public PointF GetNerestPlayerInMyBoarder()
		{
			var myPlayers = m_coach.GetOpponentPlayers();
			var me = m_coach.GetMyPos(m_number);
			PointF minPos = m_startPosition;
			double dis = double.MinValue;
			foreach (var player in myPlayers)
			{
				if (m_borders.IsInBorders(player.Pos.Value))
				{
					var disTemp = LogicCalc.GetDistance(player.Pos.Value, me.Pos.Value);
					if (disTemp < dis)
					{
						dis = disTemp;
						minPos = player.Pos.Value;
					}
				}
			}
			return minPos;
		}

		public double ShotToGoal()
		{
			var my_pos = m_coach.GetMyPos(m_number);
			var allPos = m_coach.GetAlllayers(m_number);
			List<PointF> playetPos = new List<PointF>();
			foreach (var p in allPos)
			{
				playetPos.Add(p.Pos.Value);
			}
			var forbiddenAngle = LogicCalc.GetAllForbiddenAngle(my_pos, playetPos, m_side == 'l' ? GoalDirection.right : GoalDirection.Left);

			double angle = 0;
			double diff = float.MinValue;
			for (int i = 0; i < forbiddenAngle.Count-1; i++)
			{
				var diff_temp = forbiddenAngle[i + 1] - forbiddenAngle[i];
				if (diff_temp > diff)
				{
					angle = (forbiddenAngle[i + 1] - forbiddenAngle[i]) * 0.5;
				}
			}
			return angle + forbiddenAngle[0];
		}


	}
}
