using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using RoboCup.Entities;
using RoboCup.Infrastructure;
using RoboCup.Logic;

namespace RoboCup
{
    public class RegularPlayer : Player
    {
        public int PlayerNumber { get; set; }

        public RegularPlayer(Team team, ICoach coach , int playerNumber) : base(team, coach)
        {
            PlayerNumber = playerNumber;
            m_startPosition = new PointF(playerNumber * 5, playerNumber*5);
        }

        public override void play()
        {
            // first ,ove to start position
            //m_robot.Move(20,20+ PlayerNumber);
            //GoToPosition( 0,0);
            GoToPosition( (float)-52.5, (float) 0.0);
            //m_robot.Turn(90);
            //for (var i =0; i < 100; i++)
            //{
            //    m_robot.Dash(10);
            //    Thread.Sleep(50);
            //}
            while (!m_timeOver)
            {
               // m_robot.Dash(PlayerNumber);
                m_robot.Turn(30);
            }

        }

        private void GoToPosition(float x, float y)
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
            while(!isInPos && !m_timeOver)
            {
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
            }

        }
    }
}
