using System;
using QRCodeDecoder = ThoughtWorks.QRCode.Codec.QRCodeDecoder;
using BCH15_5 = ThoughtWorks.QRCode.Codec.Ecc.BCH15_5;
using ThoughtWorks.QRCode.Geom;
using LogicalSeed = ThoughtWorks.QRCode.Codec.Reader.Pattern.LogicalSeed;
using DebugCanvas = ThoughtWorks.QRCode.Codec.Util.DebugCanvas;
using SystemUtils = ThoughtWorks.QRCode.Codec.Util.SystemUtils;

namespace ThoughtWorks.QRCode.Codec.Data
{
	
	public class QRCodeSymbol
	{
        internal int version;
        internal int errorCollectionLevel;
        internal int maskPattern;
        internal int dataCapacity;
        internal bool[][] moduleMatrix;
        internal int width, height;
        internal Point[][] alignmentPattern;
        internal int[][] numErrorCollectionCode = new int[][] { new int[] { 7, 10, 13, 17 }, new int[] { 10, 16, 22, 28 }, new int[] { 15, 26, 36, 44 }, new int[] { 20, 36, 52, 64 }, new int[] { 26, 48, 72, 88 }, new int[] { 36, 64, 96, 112 }, new int[] { 40, 72, 108, 130 }, new int[] { 48, 88, 132, 156 }, new int[] { 60, 110, 160, 192 }, new int[] { 72, 130, 192, 224 }, new int[] { 80, 150, 224, 264 }, new int[] { 96, 176, 260, 308 }, new int[] { 104, 198, 288, 352 }, new int[] { 120, 216, 320, 384 }, new int[] { 132, 240, 360, 432 }, new int[] { 144, 280, 408, 480 }, new int[] { 168, 308, 448, 532 }, new int[] { 180, 338, 504, 588 }, new int[] { 196, 364, 546, 650 }, new int[] { 224, 416, 600, 700 }, new int[] { 224, 442, 644, 750 }, new int[] { 252, 476, 690, 816 }, new int[] { 270, 504, 750, 900 }, new int[] { 300, 560, 810, 960 }, new int[] { 312, 588, 870, 1050 }, new int[] { 336, 644, 952, 1110 }, new int[] { 360, 700, 1020, 1200 }, new int[] { 390, 728, 1050, 1260 }, new int[] { 420, 784, 1140, 1350 }, new int[] { 450, 812, 1200, 1440 }, new int[] { 480, 868, 1290, 1530 }, new int[] { 510, 924, 1350, 1620 }, new int[] { 540, 980, 1440, 1710 }, new int[] { 570, 1036, 1530, 1800 }, new int[] { 570, 1064, 1590, 1890 }, new int[] { 600, 1120, 1680, 1980 }, new int[] { 630, 1204, 1770, 2100 }, new int[] { 660, 1260, 1860, 2220 }, new int[] { 720, 1316, 1950, 2310 }, new int[] { 750, 1372, 2040, 2430 } };
        internal int[][] numRSBlocks = new int[][] { new int[] { 1, 1, 1, 1 }, new int[] { 1, 1, 1, 1 }, new int[] { 1, 1, 2, 2 }, new int[] { 1, 2, 2, 4 }, new int[] { 1, 2, 4, 4 }, new int[] { 2, 4, 4, 4 }, new int[] { 2, 4, 6, 5 }, new int[] { 2, 4, 6, 6 }, new int[] { 2, 5, 8, 8 }, new int[] { 4, 5, 8, 8 }, new int[] { 4, 5, 8, 11 }, new int[] { 4, 8, 10, 11 }, new int[] { 4, 9, 12, 16 }, new int[] { 4, 9, 16, 16 }, new int[] { 6, 10, 12, 18 }, new int[] { 6, 10, 17, 16 }, new int[] { 6, 11, 16, 19 }, new int[] { 6, 13, 18, 21 }, new int[] { 7, 14, 21, 25 }, new int[] { 8, 16, 20, 25 }, new int[] { 8, 17, 23, 25 }, new int[] { 9, 17, 23, 34 }, new int[] { 9, 18, 25, 30 }, new int[] { 10, 20, 27, 32 }, new int[] { 12, 21, 29, 35 }, new int[] { 12, 23, 34, 37 }, new int[] { 12, 25, 34, 40 }, new int[] { 13, 26, 35, 42 }, new int[] { 14, 28, 38, 45 }, new int[] { 15, 29, 40, 48 }, new int[] { 16, 31, 43, 51 }, new int[] { 17, 33, 45, 54 }, new int[] { 18, 35, 48, 57 }, new int[] { 19, 37, 51, 60 }, new int[] { 19, 38, 53, 63 }, new int[] { 20, 40, 56, 66 }, new int[] { 21, 43, 59, 70 }, new int[] { 22, 45, 62, 74 }, new int[] { 24, 47, 65, 77 }, new int[] { 25, 49, 68, 81 } };

		virtual public int NumErrorCollectionCode
		{
			get
			{
				return numErrorCollectionCode[version - 1][errorCollectionLevel];
			}
			
		}
		virtual public int NumRSBlocks
		{
			get
			{
				return numRSBlocks[version - 1][errorCollectionLevel];
			}
			
		}
		virtual public int Version
		{
			get
			{
				return version;
			}
			
		}
		virtual public String VersionReference
		{
			get
			{
				char[] versionReferenceCharacter = new char[]{'L', 'M', 'Q', 'H'};				
				return System.Convert.ToString(version) + "-" + versionReferenceCharacter[errorCollectionLevel];
			}
			
		}
		virtual public Point[][] AlignmentPattern
		{
			get
			{
				return alignmentPattern;
			}
			
		}
		virtual public int DataCapacity
		{
			get
			{
				return this.dataCapacity;
			}
			
		}
		virtual public int ErrorCollectionLevel
		{
			get
			{
				return errorCollectionLevel;
			}
			
		}
		virtual public int MaskPatternReferer
		{
			get
			{
				return maskPattern;
			}
			
		}
		virtual public String MaskPatternRefererAsString
		{
			// for debug			
			get
			{
				String maskPattern = System.Convert.ToString(MaskPatternReferer, 2);
				int length = maskPattern.Length;
				for (int i = 0; i < 3 - length; i++)
					maskPattern = "0" + maskPattern;
				return maskPattern;
			}
			
		}
		virtual public int Width
		{
			get
			{
				return width;
			}
			
		}
		virtual public int Height
		{
			get
			{
				return height;
			}
			
		}
		virtual public int[] Blocks
		{
			get
			{
				int width = Width;
				int height = Height;
				int x = width - 1;
				int y = height - 1;
				System.Collections.ArrayList codeBits = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
				System.Collections.ArrayList codeWords = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
				int tempWord = 0;
				int figure = 7;
				int isNearFinish = 0;
				bool READ_UP = true;
				bool READ_DOWN = false;
				bool direction = READ_UP;
				do 
				{
					codeBits.Add(getElement(x, y));
				    if (getElement(x, y) == true)
					{
						tempWord += (1 << figure);
					}
					figure--;
					if (figure == - 1)
					{
						codeWords.Add((System.Int32) tempWord);
						figure = 7;
						tempWord = 0;
					}
					// determine module that read next
					do 
					{
						if (direction == READ_UP)
						{
							if ((x + isNearFinish) % 2 == 0)
							//if right side of two column
								x--;
							// to left
							else
							{
								if (y > 0)
								{
									//be able to move upper side
									x++;
									y--;
								}
								else
								{
									//can't move upper side
									x--; //change direction
									if (x == 6)
									{
										x--;
										isNearFinish = 1; // after through horizontal Timing Pattern, move pattern is changed
									}
									direction = READ_DOWN;
								}
							}
						}
						else
						{
							if ((x + isNearFinish) % 2 == 0)
							//if left side of two column
								x--;
							else
							{
								if (y < height - 1)
								{
									x++;
									y++;
								}
								else
								{
									x--;
									if (x == 6)
									{
										x--;
										isNearFinish = 1;
									}
									direction = READ_UP;
								}
							}
						}
					}
					while (isInFunctionPattern(x, y));
				}
				while (x != - 1);
				
				int[] gotWords = new int[codeWords.Count];
				for (int i = 0; i < codeWords.Count; i++)
				{
					System.Int32 temp = (System.Int32) codeWords[i];
					gotWords[i] = temp;
				}
				return gotWords;
			}
			
		}
		
		
		public virtual bool getElement(int x, int y)
		{
			return moduleMatrix[x][y];
		}
		
		public QRCodeSymbol(bool[][] moduleMatrix)
		{
			this.moduleMatrix = moduleMatrix;
			width = moduleMatrix.Length;
			height = moduleMatrix[0].Length;
			initialize();
		}
		
		
		
		internal virtual void  initialize()
		{
			//calculate version by number of side modules
			version = (width - 17) / 4;
			Point[][] alignmentPattern = new Point[1][];
			for (int i = 0; i < 1; i++)
			{
				alignmentPattern[i] = new Point[1];
			}
			
			int[] logicalSeeds = new int[1];
			if (version >= 2 && version <= 40)
			{
				logicalSeeds = LogicalSeed.getSeed(version);
				Point[][] tmpArray = new Point[logicalSeeds.Length][];
				for (int i2 = 0; i2 < logicalSeeds.Length; i2++)
				{
					tmpArray[i2] = new Point[logicalSeeds.Length];
				}
				alignmentPattern = tmpArray;
			}
			
			//obtain alignment pattern's center coodintates by logical seeds
			for (int col = 0; col < logicalSeeds.Length; col++)
			{
				for (int row = 0; row < logicalSeeds.Length; row++)
				{
					alignmentPattern[row][col] = new Point(logicalSeeds[row], logicalSeeds[col]);
				}
			}
			this.alignmentPattern = alignmentPattern;			
			dataCapacity = calcDataCapacity();			
			bool[] formatInformation = readFormatInformation();
			decodeFormatInformation(formatInformation);			
			unmask();
		}
		
		internal virtual bool[] readFormatInformation()
		{
			bool[] modules = new bool[15];
			
			//obtain format information from symbol
			for (int i = 0; i <= 5; i++)
				modules[i] = getElement(8, i);
			
			modules[6] = getElement(8, 7);
			modules[7] = getElement(8, 8);
			modules[8] = getElement(7, 8);
			
			for (int i = 9; i <= 14; i++)
				modules[i] = getElement(14 - i, 8);
			
			//unmask Format Information's with given mask pattern. (JIS-X-0510(2004), p65)
			int maskPattern = 0x5412;
			
			for (int i = 0; i <= 14; i++)
			{
				bool xorBit = false;
				if (((SystemUtils.URShift(maskPattern, i)) & 1) == 1)
					xorBit = true;
				else
					xorBit = false;
				
				// get unmasked format information with bit shift
				if (modules[i] == xorBit)
					modules[i] = false;
				else
					modules[i] = true;
			}
			
			BCH15_5 corrector = new BCH15_5(modules);
			bool[] output = corrector.correct();
			bool[] formatInformation = new bool[5];
			for (int i = 0; i < 5; i++)
				formatInformation[i] = output[10 + i];
			
			return formatInformation;
		}
		
		internal virtual void  unmask()
		{
			bool[][] maskPattern = generateMaskPattern();
			
			int size = Width;
			
			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					if (maskPattern[x][y] == true)
					{
						reverseElement(x, y);
					}
				}
			}
		}
		
		
		internal virtual bool[][] generateMaskPattern()
		{
			int maskPatternReferer = MaskPatternReferer;
			
			int width = Width;
			int height = Height;
			bool[][] maskPattern = new bool[width][];
			for (int i = 0; i < width; i++)
			{
				maskPattern[i] = new bool[height];
			}
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					if (isInFunctionPattern(x, y))
						continue;
					switch (maskPatternReferer)
					{
						
						case 0:  // 000
							if ((y + x) % 2 == 0)
								maskPattern[x][y] = true;
							break;
						
						case 1:  // 001
							if (y % 2 == 0)
								maskPattern[x][y] = true;
							break;
						
						case 2:  // 010
							if (x % 3 == 0)
								maskPattern[x][y] = true;
							break;
						
						case 3:  // 011
							if ((y + x) % 3 == 0)
								maskPattern[x][y] = true;
							break;
						
						case 4:  // 100
							if ((y / 2 + x / 3) % 2 == 0)
								maskPattern[x][y] = true;
							break;
						
						case 5:  // 101
							if ((y * x) % 2 + (y * x) % 3 == 0)
								maskPattern[x][y] = true;
							break;
						
						case 6:  // 110
							if (((y * x) % 2 + (y * x) % 3) % 2 == 0)
								maskPattern[x][y] = true;
							break;
						
						case 7:  // 111
							if (((y * x) % 3 + (y + x) % 2) % 2 == 0)
								maskPattern[x][y] = true;
							break;
						}
				}
			}
			return maskPattern;
		}
		
		private int calcDataCapacity()
		{
			int numFunctionPatternModule = 0;
			int numFormatAndVersionInfoModule = 0;
			int version = this.Version;
			//Console.out.println("Version:" + String.valueOf(version));
			
			if (version <= 6)
				numFormatAndVersionInfoModule = 31;
			else
				numFormatAndVersionInfoModule = 67;
			
			
			// the number of finter patterns :
			int sqrtCenters = (version / 7) + 2;
			// the number of modules left when we remove the patterns modules
			// 3*64 for the 3 big ones,
			// sqrtCenters*sqrtCenters)-3)*25 for the small ones
			int modulesLeft = (version == 1?192:192 + ((sqrtCenters * sqrtCenters) - 3) * 25);
			// Don't ask me how I found that one...
			//
			numFunctionPatternModule = modulesLeft + 8 * version + 2 - (sqrtCenters - 2) * 10;			
			int dataCapacity = (width * width - numFunctionPatternModule - numFormatAndVersionInfoModule) / 8;
			return dataCapacity;
		}
		
		internal virtual void  decodeFormatInformation(bool[] formatInformation)
		{
			if (formatInformation[4] == false)
				if (formatInformation[3] == true)
					errorCollectionLevel = 0;
				else
					errorCollectionLevel = 1;
			else if (formatInformation[3] == true)
				errorCollectionLevel = 2;
			else
				errorCollectionLevel = 3;
			
			for (int i = 2; i >= 0; i--)
				if (formatInformation[i] == true)
					maskPattern += (1 << i);
		}
		
		public virtual void  reverseElement(int x, int y)
		{
			moduleMatrix[x][y] = !moduleMatrix[x][y];
		}

		public virtual bool isInFunctionPattern(int targetX, int targetY)
		{
			if (targetX < 9 && targetY < 9)
			//in Left-Up Finder Pattern or function patterns around it
				return true;
			if (targetX > Width - 9 && targetY < 9)
			//in Right-up Finder Pattern or function patterns around it
				return true;
			if (targetX < 9 && targetY > Height - 9)
			//in Left-bottom Finder Pattern or function patterns around it
				return true;
			
			if (version >= 7)
			{
				if (targetX > Width - 12 && targetY < 6)
					return true;
				if (targetX < 6 && targetY > Height - 12)
					return true;
			}
			// in timing pattern
			if (targetX == 6 || targetY == 6)
				return true;
			
			// in alignment pattern. 		
			Point[][] alignmentPattern = AlignmentPattern;
			int sideLength = alignmentPattern.Length;
			
			for (int y = 0; y < sideLength; y++)
			{
				for (int x = 0; x < sideLength; x++)
				{
					if (!(x == 0 && y == 0) && !(x == sideLength - 1 && y == 0) && !(x == 0 && y == sideLength - 1))
						if (System.Math.Abs(alignmentPattern[x][y].X - targetX) < 3 && System.Math.Abs(alignmentPattern[x][y].Y - targetY) < 3)
							return true;
				}
			}			
			return false;
		}
	}
}