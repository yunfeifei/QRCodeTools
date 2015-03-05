using System;
using System.Text;

using QRCodeImage = ThoughtWorks.QRCode.Codec.Data.QRCodeImage;
using QRCodeSymbol = ThoughtWorks.QRCode.Codec.Data.QRCodeSymbol;
using ReedSolomon = ThoughtWorks.QRCode.Codec.Ecc.ReedSolomon;
using DecodingFailedException = ThoughtWorks.QRCode.ExceptionHandler.DecodingFailedException;
using InvalidDataBlockException = ThoughtWorks.QRCode.ExceptionHandler.InvalidDataBlockException;
using SymbolNotFoundException = ThoughtWorks.QRCode.ExceptionHandler.SymbolNotFoundException;
using Point = ThoughtWorks.QRCode.Geom.Point;
using QRCodeDataBlockReader = ThoughtWorks.QRCode.Codec.Reader.QRCodeDataBlockReader;
using QRCodeImageReader = ThoughtWorks.QRCode.Codec.Reader.QRCodeImageReader;
using DebugCanvas = ThoughtWorks.QRCode.Codec.Util.DebugCanvas;
using DebugCanvasAdapter = ThoughtWorks.QRCode.Codec.Util.DebugCanvasAdapter;
using QRCodeUtility = ThoughtWorks.QRCode.Codec.Util.QRCodeUtility;

namespace ThoughtWorks.QRCode.Codec
{
	
	public class QRCodeDecoder
	{
        internal QRCodeSymbol qrCodeSymbol;
        internal int numTryDecode;
        internal System.Collections.ArrayList results;
        internal System.Collections.ArrayList lastResults = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
        internal static DebugCanvas canvas;
        internal QRCodeImageReader imageReader;
        internal int numLastCorrections;
        internal bool correctionSucceeded;

		public static DebugCanvas Canvas
		{
			get
			{
				return QRCodeDecoder.canvas;
			}
			
			set
			{
				QRCodeDecoder.canvas = value;
			}
			
		}
		virtual internal Point[] AdjustPoints
		{
			get
			{
				// note that adjusts affect dependently
				// i.e. below means (0,0), (2,3), (3,4), (1,2), (2,1), (1,1), (-1,-1)
				
				
				//		Point[] adjusts = {new Point(0,0), new Point(2,3), new Point(1,1), 
				//				new Point(-2,-2), new Point(1,-1), new Point(-1,0), new Point(-2,-2)};
				System.Collections.ArrayList adjustPoints = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
				for (int d = 0; d < 4; d++)
					adjustPoints.Add(new Point(1, 1));
				int lastX = 0, lastY = 0;
				for (int y = 0; y > - 4; y--)
				{
					for (int x = 0; x > - 4; x--)
					{
						if (x != y && ((x + y) % 2 == 0))
						{
							adjustPoints.Add(new Point(x - lastX, y - lastY));
							lastX = x;
							lastY = y;
						}
					}
				}
				Point[] adjusts = new Point[adjustPoints.Count];
				for (int i = 0; i < adjusts.Length; i++)
					adjusts[i] = (Point) adjustPoints[i];
				return adjusts;
			}			
		}		
				
		internal class DecodeResult
		{
            internal int numCorrections;
            internal bool correctionSucceeded;
            internal sbyte[] decodedBytes;
            private QRCodeDecoder enclosingInstance;

            public DecodeResult(QRCodeDecoder enclosingInstance, sbyte[] decodedBytes, int numErrors, bool correctionSucceeded)
            {
                InitBlock(enclosingInstance);
                this.decodedBytes = decodedBytes;
                this.numCorrections = numErrors;
                this.correctionSucceeded = correctionSucceeded;
            }

			private void  InitBlock(QRCodeDecoder enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			virtual public sbyte[] DecodedBytes
			{
				get
				{
					return decodedBytes;
				}
				
			}
			virtual public int NumErrors
			{
				get
				{
					return numCorrections;
				}
				
			}
			virtual public bool CorrectionSucceeded
			{
				get
				{
					return correctionSucceeded;
				}
				
			}
			public QRCodeDecoder Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			
		}
		
		public QRCodeDecoder()
		{
			numTryDecode = 0;
			results = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			QRCodeDecoder.canvas = new DebugCanvasAdapter();
		}
		
		/*	public byte[] decode(QRCodeImage qrCodeImage) throws DecodingFailedException{
		    canvas.println("Decoding started.");
		    int[][] intImage = imageToIntArray(qrCodeImage);
		    try {
		    QRCodeImageReader reader = new QRCodeImageReader();
		    qrCodeSymbol = reader.getQRCodeSymbol(intImage);
		    } catch (SymbolNotFoundException e) {
		    throw new DecodingFailedException(e.getMessage());
		    }
		    canvas.println("Created QRCode symbol.");
		    canvas.println("Reading symbol.");
		    canvas.println("Version: " + qrCodeSymbol.getVersionReference());		
		    canvas.println("Mask pattern: " + qrCodeSymbol.getMaskPatternRefererAsString());
		    int[] blocks = qrCodeSymbol.getBlocks();
		    canvas.println("Correcting data errors.");
		    int[] dataBlocks = correctDataBlocks(blocks);
		    try {
		    byte[] decodedByteArray = 
		    getDecodedByteArray(dataBlocks, qrCodeSymbol.getVersion());
		    canvas.println("Decoding finished.");
		    return decodedByteArray;
		    } catch (InvalidDataBlockException e) {
		    throw new DecodingFailedException(e.getMessage());
		    }
		}*/
		
		public virtual sbyte[] decodeBytes(QRCodeImage qrCodeImage)
		{
			Point[] adjusts = AdjustPoints;
			System.Collections.ArrayList results = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			while (numTryDecode < adjusts.Length)
			{
				try
				{
					DecodeResult result = decode(qrCodeImage, adjusts[numTryDecode]);
					if (result.CorrectionSucceeded)
					{
						return result.DecodedBytes;
					}
					else
					{
						results.Add(result);
						canvas.println("Decoding succeeded but could not correct");
						canvas.println("all errors. Retrying..");
					}
				}
				catch (DecodingFailedException dfe)
				{
					if (dfe.Message.IndexOf("Finder Pattern") >= 0)
					throw dfe;
				}
				finally
				{
					numTryDecode += 1;
				}
			}
			
			if (results.Count == 0)
				throw new DecodingFailedException("Give up decoding");
			
			int lowestErrorIndex = - 1;
			int lowestError = System.Int32.MaxValue;
			for (int i = 0; i < results.Count; i++)
			{
				DecodeResult result = (DecodeResult) results[i];
				if (result.NumErrors < lowestError)
				{
					lowestError = result.NumErrors;
					lowestErrorIndex = i;
				}
			}
			canvas.println("All trials need for correct error");
			canvas.println("Reporting #" + (lowestErrorIndex) + " that,");
			canvas.println("corrected minimum errors (" + lowestError + ")");
			
			canvas.println("Decoding finished.");
			return ((DecodeResult) results[lowestErrorIndex]).DecodedBytes;
		}

        public virtual String decode(QRCodeImage qrCodeImage, Encoding encoding)
        {
            sbyte[] data = decodeBytes(qrCodeImage);
            byte[] byteData = new byte[data.Length];

            Buffer.BlockCopy(data, 0, byteData, 0, byteData.Length); 
            /*
            char[] decodedData = new char[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                decodedData[i] = Convert.to(data[i]);

            }
            return new String(decodedData);
            */
            String decodedData;            
            decodedData = encoding.GetString(byteData);
            return decodedData;
        }

        public virtual String decode(QRCodeImage qrCodeImage)
        {
            sbyte[] data = decodeBytes(qrCodeImage);
            byte[] byteData = new byte[data.Length];
            Buffer.BlockCopy(data, 0, byteData, 0, byteData.Length);

            Encoding encoding;
            if (QRCodeUtility.IsUnicode(byteData))
            {
                encoding = Encoding.Unicode;
            }
            else
            {
                encoding = Encoding.ASCII;
            }
            String decodedData;
            decodedData = encoding.GetString(byteData);
            return decodedData;
        }

		internal virtual DecodeResult decode(QRCodeImage qrCodeImage, Point adjust)
		{
			try
			{
				if (numTryDecode == 0)
				{
					canvas.println("Decoding started");
					int[][] intImage = imageToIntArray(qrCodeImage);
					imageReader = new QRCodeImageReader();
					qrCodeSymbol = imageReader.getQRCodeSymbol(intImage);
				}
				else
				{
					canvas.println("--");
					canvas.println("Decoding restarted #" + (numTryDecode));
					qrCodeSymbol = imageReader.getQRCodeSymbolWithAdjustedGrid(adjust);
				}
			}
			catch (SymbolNotFoundException e)
			{
				throw new DecodingFailedException(e.Message);
			}
			canvas.println("Created QRCode symbol.");
			canvas.println("Reading symbol.");
			canvas.println("Version: " + qrCodeSymbol.VersionReference);
			canvas.println("Mask pattern: " + qrCodeSymbol.MaskPatternRefererAsString);
			// blocks contains all (data and RS) blocks in QR Code symbol
			int[] blocks = qrCodeSymbol.Blocks;
			canvas.println("Correcting data errors.");
			// now blocks turn to data blocks (corrected and extracted from original blocks)
			blocks = correctDataBlocks(blocks);
			try
			{
				sbyte[] decodedByteArray = getDecodedByteArray(blocks, qrCodeSymbol.Version, qrCodeSymbol.NumErrorCollectionCode);
				return new DecodeResult(this, decodedByteArray, numLastCorrections, correctionSucceeded);
			}
			catch (InvalidDataBlockException e)
			{
				canvas.println(e.Message);
				throw new DecodingFailedException(e.Message);
			}
		}
		
		
		internal virtual int[][] imageToIntArray(QRCodeImage image)
		{
			int width = image.Width;
			int height = image.Height;
			int[][] intImage = new int[width][];
			for (int i = 0; i < width; i++)
			{
				intImage[i] = new int[height];
			}
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					intImage[x][y] = image.getPixel(x, y);
				}
			}
			return intImage;
		}
		
		internal virtual int[] correctDataBlocks(int[] blocks)
		{
			int numCorrections = 0;
			int dataCapacity = qrCodeSymbol.DataCapacity;
			int[] dataBlocks = new int[dataCapacity];
			int numErrorCollectionCode = qrCodeSymbol.NumErrorCollectionCode;
			int numRSBlocks = qrCodeSymbol.NumRSBlocks;
			int eccPerRSBlock = numErrorCollectionCode / numRSBlocks;
			if (numRSBlocks == 1)
			{
				ReedSolomon corrector = new ReedSolomon(blocks, eccPerRSBlock);
				corrector.correct();
				numCorrections += corrector.NumCorrectedErrors;
				if (numCorrections > 0)
					canvas.println(System.Convert.ToString(numCorrections) + " data errors corrected.");
				else
					canvas.println("No errors found.");
				numLastCorrections = numCorrections;
				correctionSucceeded = corrector.CorrectionSucceeded;
				return blocks;
			}
			else
			{
				//we have to interleave data blocks because symbol has 2 or more RS blocks
				int numLongerRSBlocks = dataCapacity % numRSBlocks;
				if (numLongerRSBlocks == 0)
				{
					//symbol has only 1 type of RS block
					int lengthRSBlock = dataCapacity / numRSBlocks;
					int[][] tmpArray = new int[numRSBlocks][];
					for (int i = 0; i < numRSBlocks; i++)
					{
						tmpArray[i] = new int[lengthRSBlock];
					}
					int[][] RSBlocks = tmpArray;
					//obtain RS blocks
					for (int i = 0; i < numRSBlocks; i++)
					{
						for (int j = 0; j < lengthRSBlock; j++)
						{
							RSBlocks[i][j] = blocks[j * numRSBlocks + i];
						}
						ReedSolomon corrector = new ReedSolomon(RSBlocks[i], eccPerRSBlock);
						corrector.correct();
						numCorrections += corrector.NumCorrectedErrors;
						correctionSucceeded = corrector.CorrectionSucceeded;
					}
					//obtain only data part
					int p = 0;
					for (int i = 0; i < numRSBlocks; i++)
					{
						for (int j = 0; j < lengthRSBlock - eccPerRSBlock; j++)
						{
							dataBlocks[p++] = RSBlocks[i][j];
						}
					}
				}
				else
				{
					//symbol has 2 types of RS blocks
					int lengthShorterRSBlock = dataCapacity / numRSBlocks;
					int lengthLongerRSBlock = dataCapacity / numRSBlocks + 1;
					int numShorterRSBlocks = numRSBlocks - numLongerRSBlocks;
					int[][] tmpArray2 = new int[numShorterRSBlocks][];
					for (int i2 = 0; i2 < numShorterRSBlocks; i2++)
					{
						tmpArray2[i2] = new int[lengthShorterRSBlock];
					}
					int[][] shorterRSBlocks = tmpArray2;
					int[][] tmpArray3 = new int[numLongerRSBlocks][];
					for (int i3 = 0; i3 < numLongerRSBlocks; i3++)
					{
						tmpArray3[i3] = new int[lengthLongerRSBlock];
					}
					int[][] longerRSBlocks = tmpArray3;
					for (int i = 0; i < numRSBlocks; i++)
					{
						if (i < numShorterRSBlocks)
						{
							//get shorter RS Block(s)
							int mod = 0;
							for (int j = 0; j < lengthShorterRSBlock; j++)
							{
								if (j == lengthShorterRSBlock - eccPerRSBlock)
									mod = numLongerRSBlocks;
								shorterRSBlocks[i][j] = blocks[j * numRSBlocks + i + mod];
							}
							ReedSolomon corrector = new ReedSolomon(shorterRSBlocks[i], eccPerRSBlock);
							corrector.correct();
							numCorrections += corrector.NumCorrectedErrors;
							correctionSucceeded = corrector.CorrectionSucceeded;
						}
						else
						{
							//get longer RS Blocks
							int mod = 0;
							for (int j = 0; j < lengthLongerRSBlock; j++)
							{
								if (j == lengthShorterRSBlock - eccPerRSBlock)
									mod = numShorterRSBlocks;
								longerRSBlocks[i - numShorterRSBlocks][j] = blocks[j * numRSBlocks + i - mod];
							}
							
							ReedSolomon corrector = new ReedSolomon(longerRSBlocks[i - numShorterRSBlocks], eccPerRSBlock);
							corrector.correct();
							numCorrections += corrector.NumCorrectedErrors;
							correctionSucceeded = corrector.CorrectionSucceeded;
						}
					}
					int p = 0;
					for (int i = 0; i < numRSBlocks; i++)
					{
						if (i < numShorterRSBlocks)
						{
							for (int j = 0; j < lengthShorterRSBlock - eccPerRSBlock; j++)
							{
								dataBlocks[p++] = shorterRSBlocks[i][j];
							}
						}
						else
						{
							for (int j = 0; j < lengthLongerRSBlock - eccPerRSBlock; j++)
							{
								dataBlocks[p++] = longerRSBlocks[i - numShorterRSBlocks][j];
							}
						}
					}
				}
				if (numCorrections > 0)
					canvas.println(System.Convert.ToString(numCorrections) + " data errors corrected.");
				else
					canvas.println("No errors found.");
				numLastCorrections = numCorrections;
				return dataBlocks;
			}
		}
		
		internal virtual sbyte[] getDecodedByteArray(int[] blocks, int version, int numErrorCorrectionCode)
		{
			sbyte[] byteArray;
			QRCodeDataBlockReader reader = new QRCodeDataBlockReader(blocks, version, numErrorCorrectionCode);
			try
			{
				byteArray = reader.DataByte;
			}
			catch (InvalidDataBlockException e)
			{
				throw e;
			}
			return byteArray;
		}
		
		internal virtual String getDecodedString(int[] blocks, int version, int numErrorCorrectionCode)
		{
			String dataString = null;
			QRCodeDataBlockReader reader = new QRCodeDataBlockReader(blocks, version, numErrorCorrectionCode);
			try
			{
				dataString = reader.DataString;
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new InvalidDataBlockException(e.Message);
			}
			return dataString;
		}

       
	}
}