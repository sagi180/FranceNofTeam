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
	public class PolygonBorders
	{
		PointF[] m_points;
		public PolygonBorders(PointF[] points)
		{
			m_points = points;
		}

		public bool IsInBorders(PointF point)
		{
			var result = false;
			int j = m_points.Count() - 1;
			for (int i = 0; i < m_points.Count(); i++)
			{
				if (m_points[i].Y < point.Y && m_points[j].Y >= point.Y || m_points[j].Y < point.Y && m_points[i].Y >= point.Y)
				{
					if (m_points[i].X + (point.Y - m_points[i].Y) / (m_points[j].Y - m_points[i].Y) * (m_points[j].X - m_points[i].X) < point.X)
					{
						result = !result;
					}
				}
				j = i;
			}
			return result;
		}
	}
}
