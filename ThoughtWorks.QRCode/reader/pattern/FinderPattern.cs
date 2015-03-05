using System;
using QRCodeDecoder = ThoughtWorks.QRCode.Codec.QRCodeDecoder;
using ThoughtWorks.QRCode.Codec.Reader;
using FinderPatternNotFoundException = ThoughtWorks.QRCode.ExceptionHandler.FinderPatternNotFoundException;
using InvalidVersionInfoException = ThoughtWorks.QRCode.ExceptionHandler.InvalidVersionInfoException;
using InvalidVersionException = ThoughtWorks.QRCode.ExceptionHandler.InvalidVersionException;
using VersionInformationException = ThoughtWorks.QRCode.ExceptionHandler.VersionInformationException;
using ThoughtWorks.QRCode.Geom;
using ThoughtWorks.QRCode.Codec.Util;

namespace ThoughtWorks.QRCode.Codec.Reader.Pattern
{
	
	public class FinderPattern
	{
        public const int UL = 0;
        public const int UR = 1;
        public const int DL = 2;

        internal static readonly int[] VersionInfoBit = new int[] { 0x07C94, 0x085BC, 0x09A99, 0x0A4D3, 0x0BBF6, 0x0C762, 0x0D847, 0x0E60D, 0x0F928, 0x10B78, 0x1145D, 0x12A17, 0x13532, 0x149A6, 0x15683, 0x168C9, 0x177EC, 0x18EC4, 0x191E1, 0x1AFAB, 0x1B08E, 0x1CC1A, 0x1D33F, 0x1ED75, 0x1F250, 0x209D5, 0x216F0, 0x228BA, 0x2379F, 0x24B0B, 0x2542E, 0x26A64, 0x27541, 0x28C69 };

        internal static DebugCanvas canvas;
        internal Point[] center;
        internal int version;
        internal int[] sincos;
        internal int[] width;
        internal int[] moduleSize;

		virtual public int Version
		{
			get
			{
				return version;
			}
			
		}
		virtual public int SqrtNumModules
		{
			get
			{
				return 17 + 4 * version;
			}
			
		}
		
		public static FinderPattern findFinderPattern(bool[][] image)
		{
			Line[] lineAcross = findLineAcross(image);
			Line[] lineCross = findLineCross(lineAcross);
			Point[] center = null;
			try
			{
				center = getCenter(lineCross);
			}
			catch (FinderPatternNotFoundException e)
			{
				throw e;
			}
			int[] sincos = getAngle(center);
			center = sort(center, sincos);
			int[] width = getWidth(image, center, sincos);
			// moduleSize for version recognition
			int[] moduleSize = new int[]{(width[UL] << QRCodeImageReader.DECIMAL_POINT) / 7, (width[UR] << QRCodeImageReader.DECIMAL_POINT) / 7, (width[DL] << QRCodeImageReader.DECIMAL_POINT) / 7};
			int version = calcRoughVersion(center, width);
			if (version > 6)
			{
				try
				{
					version = calcExactVersion(center, sincos, moduleSize, image);
				}
				catch (VersionInformationException e)
				{
					//use rough version data
					// throw e;
				}
			}
			return new FinderPattern(center, version, sincos, width, moduleSize);
		}
		
		internal FinderPattern(Point[] center, int version, int[] sincos, int[] width, int[] moduleSize)
		{
			this.center = center;
			this.version = version;
			this.sincos = sincos;
			this.width = width;
			this.moduleSize = moduleSize;
		}
		
		public virtual Point[] getCenter()
		{
			return center;
		}
		
		public virtual Point getCenter(int position)
		{
			if (position >= UL && position <= DL)
				return center[position];
			else
				return null;
		}
		
		public virtual int getWidth(int position)
		{
			return width[position];
		}
		
		public virtual int[] getAngle()
		{
			return sincos;
		}
		
		public virtual int getModuleSize()
		{
			return moduleSize[UL];
		}
		public virtual int getModuleSize(int place)
		{
			return moduleSize[place];
		}
				
		internal static Line[] findLineAcross(bool[][] image)
		{
			int READ_HORIZONTAL = 0;
			int READ_VERTICAL = 1;
			
			int imageWidth = image.Length;
			int imageHeight = image[0].Length;
			
			//int currentX = 0, currentY = 0;
			Point current = new Point();
			System.Collections.ArrayList lineAcross = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			
			//buffer contains recent length of modules which has same brightness
			int[] lengthBuffer = new int[5];
			int bufferPointer = 0;
			
			int direction = READ_HORIZONTAL; //start to read horizontally
			bool lastElement = QRCodeImageReader.POINT_LIGHT;
			
			while (true)
			{
				//check points in image
				bool currentElement = image[current.X][current.Y];
				if (currentElement == lastElement)
				{
					//target point has same brightness with last point
					lengthBuffer[bufferPointer]++;
				}
				else
				{
					//target point has different brightness with last point
					if (currentElement == QRCodeImageReader.POINT_LIGHT)
					{
						if (checkPattern(lengthBuffer, bufferPointer))
						{
							//detected pattern
							int x1, y1, x2, y2;
							if (direction == READ_HORIZONTAL)
							{
								//obtain X coordinates of both side of the detected horizontal pattern
								x1 = current.X;
								for (int j = 0; j < 5; j++)
								{
									x1 -= lengthBuffer[j];
								}
								x2 = current.X - 1; //right side is last X coordinate
								y1 = y2 = current.Y;
							}
							else
							{
								x1 = x2 = current.X;
								//obtain Y coordinates of both side of the detected vertical pattern
								// upper side is sum of length of buffer
								y1 = current.Y;
								for (int j = 0; j < 5; j++)
								{
									y1 -= lengthBuffer[j];
								}
								y2 = current.Y - 1; // bottom side is last Y coordinate
							}
							lineAcross.Add(new Line(x1, y1, x2, y2));
						}
					}
					bufferPointer = (bufferPointer + 1) % 5;
					lengthBuffer[bufferPointer] = 1;
					lastElement = !lastElement;
				}
				
				// determine if read next, change read direction or terminate this loop
				if (direction == READ_HORIZONTAL)
				{
					if (current.X < imageWidth - 1)
					{
						current.translate(1, 0);
					}
					else if (current.Y < imageHeight - 1)
					{
						current.set_Renamed(0, current.Y + 1);
						lengthBuffer = new int[5];
					}
					else
					{
						current.set_Renamed(0, 0); //reset target point
						lengthBuffer = new int[5];
						direction = READ_VERTICAL; //start to read vertically
					}
				}
				else
				{
					//reading vertically
					if (current.Y < imageHeight - 1)
						current.translate(0, 1);
					else if (current.X < imageWidth - 1)
					{
						current.set_Renamed(current.X + 1, 0);
						lengthBuffer = new int[5];
					}
					else
					{
						break;
					}
				}
			}
			
			Line[] foundLines = new Line[lineAcross.Count];
			
			for (int i = 0; i < foundLines.Length; i++)
				foundLines[i] = (Line) lineAcross[i];
			
			canvas.drawLines(foundLines, ThoughtWorks.QRCode.Codec.Util.Color_Fields.LIGHTGREEN);
			return foundLines;
		}
		
		internal static bool checkPattern(int[] buffer, int pointer)
		{
			int[] modelRatio = new int[]{1, 1, 3, 1, 1};
			
			int baselength = 0;
			for (int i = 0; i < 5; i++)
			{
				baselength += buffer[i];
			}
			// pseudo fixed point calculation. I think it needs smarter code
			baselength <<= QRCodeImageReader.DECIMAL_POINT;
			baselength /= 7;
			int i2;
			for (i2 = 0; i2 < 5; i2++)
			{
				int leastlength = baselength * modelRatio[i2] - baselength / 2;
				int mostlength = baselength * modelRatio[i2] + baselength / 2;
				
				//TODO rough finder pattern detection
				
				int targetlength = buffer[(pointer + i2 + 1) % 5] << QRCodeImageReader.DECIMAL_POINT;
				if (targetlength < leastlength || targetlength > mostlength)
				{
					return false;
				}
			}
			return true;
		}
		
		
		//obtain lines cross at the center of Finder Patterns
		
		internal static Line[] findLineCross(Line[] lineAcross)
		{
			System.Collections.ArrayList crossLines = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			System.Collections.ArrayList lineNeighbor = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			System.Collections.ArrayList lineCandidate = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			Line compareLine;
			for (int i = 0; i < lineAcross.Length; i++)
				lineCandidate.Add(lineAcross[i]);
			
			for (int i = 0; i < lineCandidate.Count - 1; i++)
			{
				lineNeighbor.Clear();
				lineNeighbor.Add(lineCandidate[i]);
				for (int j = i + 1; j < lineCandidate.Count; j++)
				{
					if (Line.isNeighbor((Line) lineNeighbor[lineNeighbor.Count - 1], (Line) lineCandidate[j]))
					{
						lineNeighbor.Add(lineCandidate[j]);
						compareLine = (Line) lineNeighbor[lineNeighbor.Count - 1];
						if (lineNeighbor.Count * 5 > compareLine.Length && j == lineCandidate.Count - 1)
						{
							crossLines.Add(lineNeighbor[lineNeighbor.Count / 2]);
							for (int k = 0; k < lineNeighbor.Count; k++)
								lineCandidate.Remove(lineNeighbor[k]);
						}
					}
					//terminate comparison if there are no possibility for found neighbour lines
					else if (cantNeighbor((Line) lineNeighbor[lineNeighbor.Count - 1], (Line) lineCandidate[j]) || (j == lineCandidate.Count - 1))
					{
						compareLine = (Line) lineNeighbor[lineNeighbor.Count - 1];
						/*
						* determine lines across Finder Patterns when number of neighbour lines are 
						* bigger than 1/6 length of theirselves
						*/
						if (lineNeighbor.Count * 6 > compareLine.Length)
						{
							crossLines.Add(lineNeighbor[lineNeighbor.Count / 2]);
							for (int k = 0; k < lineNeighbor.Count; k++)
							{
								lineCandidate.Remove(lineNeighbor[k]);
							}
						}
						break;
					}
				}
			}
			
			Line[] foundLines = new Line[crossLines.Count];
			for (int i = 0; i < foundLines.Length; i++)
			{
				foundLines[i] = (Line) crossLines[i];
			}
			return foundLines;
		}
		
		internal static bool cantNeighbor(Line line1, Line line2)
		{
			if (Line.isCross(line1, line2))
				return true;
			
			if (line1.Horizontal)
			{
				if (System.Math.Abs(line1.getP1().Y - line2.getP1().Y) > 1)
					return true;
				else
					return false;
			}
			else
			{
				if (System.Math.Abs(line1.getP1().X - line2.getP1().X) > 1)
					return true;
				else
					return false;
			}
		}
		
		//obtain slope of symbol
		internal static int[] getAngle(Point[] centers)
		{
			
			Line[] additionalLine = new Line[3];
			
			for (int i = 0; i < additionalLine.Length; i++)
			{
				additionalLine[i] = new Line(centers[i], centers[(i + 1) % additionalLine.Length]);
			}
			// remoteLine - does not contain UL center
			Line remoteLine = Line.getLongest(additionalLine);
			Point originPoint = new Point();
			for (int i = 0; i < centers.Length; i++)
			{
				if (!remoteLine.getP1().equals(centers[i]) && !remoteLine.getP2().equals(centers[i]))
				{
					originPoint = centers[i];
					break;
				}
			}
			canvas.println("originPoint is: " + originPoint);
			Point remotePoint = new Point();
			
			//with origin that the center of Left-Up Finder Pattern, determine other two patterns center.
			//then calculate symbols angle
			if (originPoint.Y <= remoteLine.getP1().Y & originPoint.Y <= remoteLine.getP2().Y)
				if (remoteLine.getP1().X < remoteLine.getP2().X)
					remotePoint = remoteLine.getP2();
				else
					remotePoint = remoteLine.getP1();
			else if (originPoint.X >= remoteLine.getP1().X & originPoint.X >= remoteLine.getP2().X)
				if (remoteLine.getP1().Y < remoteLine.getP2().Y)
					remotePoint = remoteLine.getP2();
				else
					remotePoint = remoteLine.getP1();
			else if (originPoint.Y >= remoteLine.getP1().Y & originPoint.Y >= remoteLine.getP2().Y)
				if (remoteLine.getP1().X < remoteLine.getP2().X)
					remotePoint = remoteLine.getP1();
				else
					remotePoint = remoteLine.getP2();
			//1st or 4th quadrant
			else if (remoteLine.getP1().Y < remoteLine.getP2().Y)
				remotePoint = remoteLine.getP1();
			else
				remotePoint = remoteLine.getP2();
			
			int r = new Line(originPoint, remotePoint).Length;
			//canvas.println(Integer.toString(((remotePoint.getX() - originPoint.getX()) << QRCodeImageReader.DECIMAL_POINT)));
			int[] angle = new int[2];
			angle[0] = ((remotePoint.Y - originPoint.Y) << QRCodeImageReader.DECIMAL_POINT) / r; //Sin
			angle[1] = ((remotePoint.X - originPoint.X) << (QRCodeImageReader.DECIMAL_POINT)) / r; //Cos
			
			return angle;
		}
		
		internal static Point[] getCenter(Line[] crossLines)
		{
			System.Collections.ArrayList centers = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			for (int i = 0; i < crossLines.Length - 1; i++)
			{
				Line compareLine = crossLines[i];
				for (int j = i + 1; j < crossLines.Length; j++)
				{
					Line comparedLine = crossLines[j];
					if (Line.isCross(compareLine, comparedLine))
					{
						int x = 0;
						int y = 0;
						if (compareLine.Horizontal)
						{
							x = compareLine.Center.X;
							y = comparedLine.Center.Y;
						}
						else
						{
							x = comparedLine.Center.X;
							y = compareLine.Center.Y;
						}
						centers.Add(new Point(x, y));
					}
				}
			}
			
			Point[] foundPoints = new Point[centers.Count];
			
			for (int i = 0; i < foundPoints.Length; i++)
			{
				foundPoints[i] = (Point) centers[i];
				//Console.out.println(foundPoints[i]);
			}
			//Console.out.println(foundPoints.length);
			
			if (foundPoints.Length == 3)
			{
				canvas.drawPolygon(foundPoints, ThoughtWorks.QRCode.Codec.Util.Color_Fields.RED);
				return foundPoints;
			}
			else
				throw new FinderPatternNotFoundException("Invalid number of Finder Pattern detected");
		}
		
		//sort center of finder patterns as Left-Up: points[0], Right-Up: points[1], Left-Down: points[2].
		internal static Point[] sort(Point[] centers, int[] angle)
		{
			
			Point[] sortedCenters = new Point[3];
			
			int quadant = getURQuadant(angle);
			switch (quadant)
			{
				
				case 1: 
					sortedCenters[1] = getPointAtSide(centers, Point.RIGHT, Point.BOTTOM);
					sortedCenters[2] = getPointAtSide(centers, Point.BOTTOM, Point.LEFT);
					break;
				
				case 2: 
					sortedCenters[1] = getPointAtSide(centers, Point.BOTTOM, Point.LEFT);
					sortedCenters[2] = getPointAtSide(centers, Point.TOP, Point.LEFT);
					break;
				
				case 3: 
					sortedCenters[1] = getPointAtSide(centers, Point.LEFT, Point.TOP);
					sortedCenters[2] = getPointAtSide(centers, Point.RIGHT, Point.TOP);
					break;
				
				case 4: 
					sortedCenters[1] = getPointAtSide(centers, Point.TOP, Point.RIGHT);
					sortedCenters[2] = getPointAtSide(centers, Point.BOTTOM, Point.RIGHT);
					break;
				}
			
			//last of centers is Left-Up patterns one
			for (int i = 0; i < centers.Length; i++)
			{
				if (!centers[i].equals(sortedCenters[1]) && !centers[i].equals(sortedCenters[2]))
				{
					sortedCenters[0] = centers[i];
				}
			}
			
			return sortedCenters;
		}
		
		internal static int getURQuadant(int[] angle)
		{
			int sin = angle[0];
			int cos = angle[1];
			if (sin >= 0 && cos > 0)
				return 1;
			else if (sin > 0 && cos <= 0)
				return 2;
			else if (sin <= 0 && cos < 0)
				return 3;
			else if (sin < 0 && cos >= 0)
				return 4;
			
			return 0;
		}
		
		internal static Point getPointAtSide(Point[] points, int side1, int side2)
		{
			Point sidePoint = new Point();
			int x = ((side1 == Point.RIGHT || side2 == Point.RIGHT)?0:System.Int32.MaxValue);
			int y = ((side1 == Point.BOTTOM || side2 == Point.BOTTOM)?0:System.Int32.MaxValue);
			sidePoint = new Point(x, y);
			
			for (int i = 0; i < points.Length; i++)
			{
				switch (side1)
				{
					
					case Point.RIGHT: 
						if (sidePoint.X < points[i].X)
						{
							sidePoint = points[i];
						}
						else if (sidePoint.X == points[i].X)
						{
							if (side2 == Point.BOTTOM)
							{
								if (sidePoint.Y < points[i].Y)
								{
									sidePoint = points[i];
								}
							}
							else
							{
								if (sidePoint.Y > points[i].Y)
								{
									sidePoint = points[i];
								}
							}
						}
						break;
					
					case Point.BOTTOM: 
						if (sidePoint.Y < points[i].Y)
						{
							sidePoint = points[i];
						}
						else if (sidePoint.Y == points[i].Y)
						{
							if (side2 == Point.RIGHT)
							{
								if (sidePoint.X < points[i].X)
								{
									sidePoint = points[i];
								}
							}
							else
							{
								if (sidePoint.X > points[i].X)
								{
									sidePoint = points[i];
								}
							}
						}
						break;
					
					case Point.LEFT: 
						if (sidePoint.X > points[i].X)
						{
							sidePoint = points[i];
						}
						else if (sidePoint.X == points[i].X)
						{
							if (side2 == Point.BOTTOM)
							{
								if (sidePoint.Y < points[i].Y)
								{
									sidePoint = points[i];
								}
							}
							else
							{
								if (sidePoint.Y > points[i].Y)
								{
									sidePoint = points[i];
								}
							}
						}
						break;
					
					case Point.TOP: 
						if (sidePoint.Y > points[i].Y)
						{
							sidePoint = points[i];
						}
						else if (sidePoint.Y == points[i].Y)
						{
							if (side2 == Point.RIGHT)
							{
								if (sidePoint.X < points[i].X)
								{
									sidePoint = points[i];
								}
							}
							else
							{
								if (sidePoint.X > points[i].X)
								{
									sidePoint = points[i];
								}
							}
						}
						break;
					}
			}
			return sidePoint;
		}
		
		internal static int[] getWidth(bool[][] image, Point[] centers, int[] sincos)
		{
			
			int[] width = new int[3];
			
			for (int i = 0; i < 3; i++)
			{
				bool flag = false;
				int lx, rx;
				int y = centers[i].Y;
				for (lx = centers[i].X; lx > 0; lx--)
				{
					if (image[lx][y] == QRCodeImageReader.POINT_DARK && image[lx - 1][y] == QRCodeImageReader.POINT_LIGHT)
					{
						if (flag == false)
							flag = true;
						else
							break;
					}
				}
				flag = false;
				for (rx = centers[i].X; rx < image.Length; rx++)
				{
					if (image[rx][y] == QRCodeImageReader.POINT_DARK && image[rx + 1][y] == QRCodeImageReader.POINT_LIGHT)
					{
						if (flag == false)
							flag = true;
						else
							break;
					}
				}
				width[i] = (rx - lx + 1);
			}
			return width;
		}
		
		internal static int calcRoughVersion(Point[] center, int[] width)
		{
			int dp = QRCodeImageReader.DECIMAL_POINT;
			int lengthAdditionalLine = (new Line(center[UL], center[UR]).Length) << dp;
			int avarageWidth = ((width[UL] + width[UR]) << dp) / 14;
			int roughVersion = ((lengthAdditionalLine / avarageWidth) - 10) / 4;
			if (((lengthAdditionalLine / avarageWidth) - 10) % 4 >= 2)
			{
				roughVersion++;
			}
			
			return roughVersion;
		}
		
		internal static int calcExactVersion(Point[] centers, int[] angle, int[] moduleSize, bool[][] image)
		{
			bool[] versionInformation = new bool[18];
			Point[] points = new Point[18];
			Point target;
			Axis axis = new Axis(angle, moduleSize[UR]); //UR
			axis.Origin = centers[UR];
			
			for (int y = 0; y < 6; y++)
			{
				for (int x = 0; x < 3; x++)
				{
					target = axis.translate(x - 7, y - 3);
					versionInformation[x + y * 3] = image[target.X][target.Y];
					points[x + y * 3] = target;
				}
			}
			canvas.drawPoints(points, ThoughtWorks.QRCode.Codec.Util.Color_Fields.RED);
			
			int exactVersion = 0;
			try
			{
				exactVersion = checkVersionInfo(versionInformation);
			}
			catch (InvalidVersionInfoException e)
			{
				canvas.println("Version info error. now retry with other place one.");
				axis.Origin = centers[DL];
				axis.ModulePitch = moduleSize[DL]; //DL
				
				for (int x = 0; x < 6; x++)
				{
					for (int y = 0; y < 3; y++)
					{
						target = axis.translate(x - 3, y - 7);
						versionInformation[y + x * 3] = image[target.X][target.Y];
						points[x + y * 3] = target;
					}
				}
				canvas.drawPoints(points, ThoughtWorks.QRCode.Codec.Util.Color_Fields.RED);
				
				try
				{
					exactVersion = checkVersionInfo(versionInformation);
				}
				catch (VersionInformationException e2)
				{
					throw e2;
				}
			}
			return exactVersion;
		}
		
		internal static int checkVersionInfo(bool[] target)
		{
			// note that this method includes BCH 18-6 Error Correction
			// see page 67 on JIS-X-0510(2004) 
			int errorCount = 0, versionBase;
			for (versionBase = 0; versionBase < VersionInfoBit.Length; versionBase++)
			{
				errorCount = 0;
				for (int j = 0; j < 18; j++)
				{
					if (target[j] ^ (VersionInfoBit[versionBase] >> j) % 2 == 1)
						errorCount++;
				}
				if (errorCount <= 3)
					break;
			}
			if (errorCount <= 3)
				return 7 + versionBase;
			else
				throw new InvalidVersionInfoException("Too many errors in version information");
		}
		static FinderPattern()
		{
			canvas = QRCodeDecoder.Canvas;
		}
	}
}