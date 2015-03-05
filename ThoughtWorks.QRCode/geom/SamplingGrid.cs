using System;
namespace ThoughtWorks.QRCode.Geom
{
	
	/// <summary> This class is used for sampling grid
	/// It allows one area to have a different size from another area
	/// </summary>
	public class SamplingGrid
	{
		virtual public int TotalWidth
		{
			get
			{
				int total = 0;
				for (int i = 0; i < grid.Length; i++)
				{
					total += grid[i][0].Width;
					if (i > 0)
						total -= 1;
				}
				return total;
			}
			
		}
		virtual public int TotalHeight
		{
			get
			{
				int total = 0;
				for (int i = 0; i < grid[0].Length; i++)
				{
					total += grid[0][i].Height;
					if (i > 0)
						total -= 1;
				}
				return total;
			}
			
		}
	
        /// <summary> A grid for a single area</summary>
		private class AreaGrid
		{
            private SamplingGrid enclosingInstance;
            private Line[] xLine;
            private Line[] yLine;

			private void  InitBlock(SamplingGrid enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			
			virtual public int Width
			{
				get
				{
					return (xLine.Length);
				}
				
			}
			virtual public int Height
			{
				get
				{
					return (yLine.Length);
				}
				
			}
			virtual public Line[] XLines
			{
				get
				{
					return xLine;
				}
				
			}
			virtual public Line[] YLines
			{
				get
				{
					return yLine;
				}
				
			}
			public SamplingGrid Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
		
			
			public AreaGrid(SamplingGrid enclosingInstance, int width, int height)
			{
				InitBlock(enclosingInstance);
				xLine = new Line[width];
				yLine = new Line[height];
			}
			
			public virtual Line getXLine(int x)
			{
				return xLine[x];
			}
			
			public virtual Line getYLine(int y)
			{
				return yLine[y];
			}
			
			public virtual void  setXLine(int x, Line line)
			{
				xLine[x] = line;
			}
			
			public virtual void  setYLine(int y, Line line)
			{
				yLine[y] = line;
			}
		}
		
		
		private AreaGrid[][] grid;
		
		public SamplingGrid(int sqrtNumArea)
		{
			grid = new AreaGrid[sqrtNumArea][];
			for (int i = 0; i < sqrtNumArea; i++)
			{
				grid[i] = new AreaGrid[sqrtNumArea];
			}
		}
		
		public virtual void  initGrid(int ax, int ay, int width, int height)
		{
			grid[ax][ay] = new AreaGrid(this, width, height);
		}
		
		public virtual void  setXLine(int ax, int ay, int x, Line line)
		{
			grid[ax][ay].setXLine(x, line);
		}
		
		public virtual void  setYLine(int ax, int ay, int y, Line line)
		{
			grid[ax][ay].setYLine(y, line);
		}
		
		public virtual Line getXLine(int ax, int ay, int x)
		{
			return (grid[ax][ay].getXLine(x));
		}
		
		public virtual Line getYLine(int ax, int ay, int y)
		{
			return (grid[ax][ay].getYLine(y));
		}
		
		public virtual Line[] getXLines(int ax, int ay)
		{
			return (grid[ax][ay].XLines);
		}
		
		public virtual Line[] getYLines(int ax, int ay)
		{
			return (grid[ax][ay].YLines);
		}
		
		public virtual int getWidth()
		{
			return (grid[0].Length);
		}
		
		public virtual int getHeight()
		{
			return (grid.Length);
		}
		
		public virtual int getWidth(int ax, int ay)
		{
			return (grid[ax][ay].Width);
		}
		
		public virtual int getHeight(int ax, int ay)
		{
			return (grid[ax][ay].Height);
		}
		
		
		public virtual int getX(int ax, int x)
		{
			int total = x;
			for (int i = 0; i < ax; i++)
			{
				total += grid[i][0].Width - 1;
			}
			return total;
		}
		
		public virtual int getY(int ay, int y)
		{
			int total = y;
			for (int i = 0; i < ay; i++)
			{
				total += grid[0][i].Height - 1;
			}
			return total;
		}
		
		public virtual void  adjust(Point adjust)
		{
			int dx = adjust.X, dy = adjust.Y;
			for (int ay = 0; ay < grid[0].Length; ay++)
			{
				for (int ax = 0; ax < grid.Length; ax++)
				{
					for (int i = 0; i < grid[ax][ay].XLines.Length; i++)
						grid[ax][ay].XLines[i].translate(dx, dy);
					for (int j = 0; j < grid[ax][ay].YLines.Length; j++)
						grid[ax][ay].YLines[j].translate(dx, dy);
				}
			}
		}
	}
}