using System;
using Line = ThoughtWorks.QRCode.Geom.Line;
using Point = ThoughtWorks.QRCode.Geom.Point;

namespace ThoughtWorks.QRCode.Codec.Util
{
	/* 
	* This class must be a "edition independent" class for debug information controll.
	* I think it's good idea to modify this class with a adapter pattern
	*/
	public class DebugCanvasAdapter : DebugCanvas
	{
		public virtual void  println(String string_Renamed)
		{
		}
		
		public virtual void  drawPoint(Point point, int color)
		{
		}
		
		public virtual void  drawCross(Point point, int color)
		{
		}
		
		public virtual void  drawPoints(Point[] points, int color)
		{
		}
		
		public virtual void  drawLine(Line line, int color)
		{
		}
		
		public virtual void  drawLines(Line[] lines, int color)
		{
		}
		
		public virtual void  drawPolygon(Point[] points, int color)
		{
		}
		
		public virtual void  drawMatrix(bool[][] matrix)
		{
		}
		
	}
}