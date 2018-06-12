using System;
using System.Collections.Generic;
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

        public static List<double> GetAllForbiddenAngle(int x1, int y1 , List<System.Drawing.PointF> playetPos , GoalDirection goalDirection)
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

            var angle = GetAngleBetweenTwoPoints(x1, y1, GBx, GBy);
            angels.Add(angle);
            angle = GetAngleBetweenTwoPoints(x1, y1, GTx, GTy);
            angels.Add(angle);

            foreach (var point in playetPos)
            {
                if(isPointInTriangle(x1,y1, GTx, GTy, GBx, GBy , point.X , point.Y))
                {
                    angle = GetAngleBetweenTwoPoints(x1, y1, point.X, point.Y);
                    angels.Add(angle);
                }
            }

            angels.Sort();
            return angels;
        }

        public static double GetDistance(float x1, float y1, float x2, float y2)
        {
            //pythagorean theorem c^2 = a^2 + b^2
            //thus c = square root(a^2 + b^2)
            double a = (double)(x2 - x1);
            double b = (double)(y2- y1);

            return Math.Sqrt(a * a + b * b);
        }

    

    }

    public enum GoalDirection
    {
        Left, right
    }
}
