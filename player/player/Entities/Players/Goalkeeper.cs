using RoboCup.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using RoboCup.Infrastructure;


//namespace player.Entities.Players
namespace RoboCup
{
    /// <summary>
    /// Also called goalie, or keeper
    /// The goalkeeper is simply known as the guy with gloves who keeps the opponents from scoring. He has a special position because only him can play the ball with his hands (provided that he is inside his own penalty area and the ball was not deliberately passed to him by a team mate).
    /// Aside from being the last line of defense, the goalkeeper is the first person in attack. That is why keepers who can make good goal kicks and strategic ball throws to team mates are valuable.
    /// The goalie has four main roles: saving, clearing, directing the defense, and distributing the ball. Saving is the act of preventing the ball from entering the net while clearing means keeping the ball far from the goal area.
    /// The goalkeeper has the role of directing the defense since he is the farthest player at the back and he can see where the defenders should position themselves.
    /// Distributing the ball happens when a goalkeeper decides whether to kick the ball or throw it after making a save. Where the keeper throws or kicks the ball is the first instance of attack
    /// </summary>
    public class Goalkeeper : Player
    {
        private const int WAIT_FOR_MSG_TIME = 10;

        public Goalkeeper(Team team, ICoach coach)
            : base(team, coach)
        {
            m_startPosition = new PointF(m_sideFactor * 10, 0);
        }

        public override void play()
        {

            // first ,ove to start position
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            SeenObject obj;
            SeenObject objSelf;

            while (!m_timeOver)
            {
                var bodyInfo = GetBodyInfo();
                obj = m_memory.GetSeenObject("ball");
                objSelf = m_memory.GetSeenObject("player France 3");
                if (m_side == 'l') obj = m_memory.GetSeenObject("goal l");
                else obj = m_memory.GetSeenObject("goal r");

                if (obj == null)
                {
                    m_robot.Turn(40);
                    m_memory.waitForNewInfo();
                }
                else
                {
                    if (obj.Distance.Value < 1.5) return;
                    if (obj.Direction.Value > 20) m_robot.Turn(obj.Direction.Value);
                    else m_robot.Dash(100);
                    m_memory.waitForNewInfo();
                }

                //for (int i=0; i<100; i++)
                //{
                //    if (m_side == 'l') obj = m_memory.GetSeenObject("goal l");
                //    else obj = m_memory.GetSeenObject("goal r");

                //    if (obj == null)
                //    {
                //        m_robot.Turn(90);
                //        m_memory.waitForNewInfo();
                //    }
                //    else
                //    {
                //        m_robot.Turn(obj.Direction.Value);
                //        m_robot.Dash(100);
                //    }
                //}

                //obj = m_memory.GetSeenObject("ball");
                //if (obj == null)
                //{
                //    // If you don't know where is ball then find it
                //    m_robot.Turn(40);
                //    m_memory.waitForNewInfo();
                //}
                //else if (obj.Distance.Value > 1.5)
                //{
                //    // If ball is too far then
                //    // turn to ball or 
                //    // if we have correct direction then go to ball
                //    if (obj.Direction.Value != 0)
                //        m_robot.Turn(obj.Direction.Value);
                //    else
                //        m_robot.Dash(10 * obj.Distance.Value);
                //}
                //else
                //{
                //    // We know where is ball and we can kick it
                //    // so look for goal
                //    if (m_side == 'l')
                //        obj = m_memory.GetSeenObject("goal r");
                //    else
                //        obj = m_memory.GetSeenObject("goal l");

                //    if (obj == null)
                //    {
                //        m_robot.Turn(40);
                //        m_memory.waitForNewInfo();
                //    }
                //    else
                //        m_robot.Kick(200, obj.Direction.Value);
                //}

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

        private SenseBodyInfo GetBodyInfo()
        {
            m_robot.SenseBody();
            SenseBodyInfo bodyInfo = null;
            while (bodyInfo == null)
            {
                Thread.Sleep(WAIT_FOR_MSG_TIME);
                bodyInfo = m_memory.getBodyInfo();
            }

            return bodyInfo;
        }

    }
}
