using System;
using QRCodeUtility = ThoughtWorks.QRCode.Codec.Util.QRCodeUtility;

namespace ThoughtWorks.QRCode.Geom
{	
	public class Line
	{
        internal int x1, y1, x2, y2;

		virtual public bool Horizontal
		{
			get
			{
				if (y1 == y2)
					return true;
				else
					return false;
			}
			
		}
		virtual public bool Vertical
		{
			get
			{
				if (x1 == x2)
					return true;
				else
					return false;
			}
			
		}
		virtual public Point Center
		{
			get
			{
				int x = (x1 + x2) / 2;
				int y = (y1 + y2) / 2;
				return new Point(x, y);
			}
			
		}
		virtual public int Length
		{
			get
			{
				int x = System.Math.Abs(x2 - x1);
				int y = System.Math.Abs(y2 - y1);
				int r = QRCodeUtility.sqrt(x * x + y * y);
				return r;
			}
			
		}		
		
		public Line()
		{
			x1 = y1 = x2 = y2 = 0;
		}

		public Line(int x1, int y1, int x2, int y2)
		{
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;
		}
		public Line(Point p1, Point p2)
		{
			x1 = p1.X;
			y1 = p1.Y;
			x2 = p2.X;
			y2 = p2.Y;
		}
		public virtual Point getP1()
		{
			return new Point(x1, y1);
		}
		
		public virtual Point getP2()
		{
			return new Point(x2, y2);
		}
		
		public virtual void  setLine(int x1, int y1, int x2, int y2)
		{
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;
		}
		public virtual void  setP1(Point p1)
		{
			x1 = p1.X;
			y1 = p1.Y;
		}
		public virtual void  setP1(int x1, int y1)
		{
			this.x1 = x1;
			this.y1 = y1;
		}
		public virtual void  setP2(Point p2)
		{
			x2 = p2.X;
			y2 = p2.Y;
		}
		public virtual void  setP2(int x2, int y2)
		{
			this.x2 = x2;
			this.y2 = y2;
		}
		
		public virtual void  translate(int dx, int dy)
		{
			this.x1 += dx;
			this.y1 += dy;
			this.x2 += dx;
			this.y2 += dy;
		}
		
		//check if two lines are neighboring. allow only 1 dot difference 
		public static bool isNeighbor(Line line1, Line line2)
		{
			if ((System.Math.Abs(line1.getP1().X - line2.getP1().X) < 2 && System.Math.Abs(line1.getP1().Y - line2.getP1().Y) < 2) && (System.Math.Abs(line1.getP2().X - line2.getP2().X) < 2 && System.Math.Abs(line1.getP2().Y - line2.getP2().Y) < 2))
				return true;
			else
				return false;
		}
		
		public static bool isCross(Line line1, Line line2)
		{
			if (line1.Horizontal && line2.Vertical)
			{
				if (line1.getP1().Y > line2.getP1().Y && line1.getP1().Y < line2.getP2().Y && line2.getP1().X > line1.getP1().X && line2.getP1().X < line1.getP2().X)
					return true;
			}
			else if (line1.Vertical && line2.Horizontal)
			{
				if (line1.getP1().X > line2.getP1().X && line1.getP1().X < line2.getP2().X && line2.getP1().Y > line1.getP1().Y && line2.getP1().Y < line1.getP2().Y)
					return true;
			}
			
			return false;
		}
		public static Line getLongest(Line[] lines)
		{
			Line longestLine = new Line();
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].Length > longestLine.Length)
				{
					longestLine = lines[i];
				}
			}
			return longestLine;
		}
		public override String ToString()
		{
			return "(" + System.Convert.ToString(x1) + "," + System.Convert.ToString(y1) + ")-(" + System.Convert.ToString(x2) + "," + System.Convert.ToString(y2) + ")";
		}
	}
}