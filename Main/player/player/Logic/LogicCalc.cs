using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using RoboCup.Infrastructure;

namespace RoboCup.Logic
{
    public static class LogicCalc
    {
        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public static double GetAngleBetweenTwoPoints(float x1, float y1 , float x2, float y2)
        {
            return RadianToDegree(Math.Atan2(y2 - y1, x2 - x1));
        }

 
        public static double area(float x1, float y1, float x2,
                           float y2, float x3, float y3)
        {
            return Math.Abs((x1 * (y2 - y3) +
                             x2 * (y3 - y1) +
                             x3 * (y1 - y2)) / 2.0);
        }

        public static bool isPointInTriangle(float x1, float y1, float x2, float y2, float x3, float y3, float x, float y)
        {
            /* Calculate area of triangle ABC */
            double A = area(x1, y1, x2, y2, x3, y3);

            /* Calculate area of triangle PBC */
            double A1 = area(x, y, x2, y2, x3, y3);

            /* Calculate area of triangle PAC */
            double A2 = area(x1, y1, x, y, x3, y3);

            /* Calculate area of triangle PAB */
            double A3 = area(x1, y1, x2, y2, x, y);

            /* Check if sum of A1, A2 and A3 is same as A */
            return (A == A1 + A2 + A3);
        }

        public static List<double> GetAllForbiddenAngle(SeenCoachObject myPos, List<System.Drawing.PointF> playetPos , GoalDirection goalDirection)
        {
            List<double> angels = new List<double>();

            float GBx, GTx;
            float GTy = FieldLocations.TopGoal;
            float GBy = FieldLocations.ButtomGoal;
            if (goalDirection == GoalDirection.Left)
            {
                GTx = FieldLocations.LeftLine;
                GBx = FieldLocations.LeftLine;
            }
            else
            {
                GTx = FieldLocations.RightLine;
                GBx = FieldLocations.RightLine;
            }

            var angle = GetAngleToPoint(myPos, GBx, GBy);
            angels.Add(angle);
            angle = GetAngleToPoint(myPos, GTx, GTy);
            angels.Add(angle);

            foreach (var point in playetPos)
            {
                if(isPointInTriangle(myPos.Pos.Value.X, myPos.Pos.Value.Y, GTx, GTy, GBx, GBy , point.X , point.Y))
                {
                    angle = GetAngleToPoint(myPos, point.X, point.Y);
                    angels.Add(angle);
                }
            }

            angels.Sort();
            return angels;
        }

		public static double GetAngleToPoint(SeenCoachObject myPosByCoach,float x1, float y1)
		{
			return GetAngleToPoint(new PointF(x1,y1), myPosByCoach);
		}

		public static double GetAngleToPoint(PointF targetPoint, SeenCoachObject myPosByCoach)
		{
			var angleToTarget = Calc2PointsAngleByXAxis(myPosByCoach.Pos.Value, targetPoint);
			var myAbsAngle = myPosByCoach.BodyAngle;

			var turnAngle = -1 * (Convert.ToDouble(myAbsAngle) + angleToTarget);

			var fixedAngle = NormalizeTo180(turnAngle);

			return turnAngle;
		}
		const double Rad2Deg = 180.0 / Math.PI;
		const double Deg2Rad = Math.PI / 180.0;
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

		public static double GetDistance(float x1, float y1, float x2, float y2)
        {
            //pythagorean theorem c^2 = a^2 + b^2
            //thus c = square root(a^2 + b^2)
            double a = (double)(x2 - x1);
            double b = (double)(y2- y1);

            return Math.Sqrt(a * a + b * b);
        }


        public static double GetDistance(PointF p1, PointF p2)
        {
            return GetDistance(p1.X, p1.Y, p2.X, p2.Y);

        }
    }

    public enum GoalDirection
    {
        Left, right
    }
}
