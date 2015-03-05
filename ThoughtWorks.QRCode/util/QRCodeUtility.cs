using System;
using System.Text;

namespace ThoughtWorks.QRCode.Codec.Util
{
	
	/* 
	* This class must be modified as a adapter class for "edition dependent" methods
	*/
	
	public class QRCodeUtility
	{
		// Because CLDC1.0 does not support Math.sqrt(), we have to define it manually.
		// faster sqrt (GuoQing Hu's FIX)
		public static int sqrt(int val)
		{
			//		using estimate method from http://www.azillionmonkeys.com/qed/sqroot.html 
			//		Console.out.print(val + ", " + (int)Math.sqrt(val) + ", "); 
			int temp, g = 0, b = 0x8000, bshft = 15;
			do 
			{
				if (val >= (temp = (((g << 1) + b) << bshft--)))
				{
					g += b;
					val -= temp;
				}
			}
			while ((b >>= 1) > 0);
			
			return g;
		}
		
		// for au by KDDI Profile Phase 3.0
		//	public static int[][] parseImage(Image image) {
		//		int width = image.getWidth();
		//		int height = image.getHeight();
		//		Image mutable = Image.createImage(width, height);
		//		Graphics g = mutable.getGraphics();
		//		g.drawImage(image, 0, 0, Graphics.TOP|Graphics.LEFT);
		//		ExtensionGraphics eg = (ExtensionGraphics) g;
		//		int[][] result = new int[width][height];
		//		
		//		for (int x = 0; x < width; x++) {
		//			for (int y = 0; y < height; y++) {
		//				result[x][y] = eg.getPixel(x, y);
		//			}
		//		}
		//		return result;
		//	}
		//	
		//	public static int[][] parseImage(byte[] imageData) {
		//		return parseImage(Image.createImage(imageData, 0, imageData.length));
		//	}


        public static bool IsUniCode(String value)
        {
            byte[] ascii = AsciiStringToByteArray(value);
            byte[] unicode = UnicodeStringToByteArray(value);
            string value1 = FromASCIIByteArray(ascii);
            string value2 = FromUnicodeByteArray(unicode);
            if (value1 != value2)
                return true;
            return false;
        }

        public static bool IsUnicode(byte[] byteData)
        {
            string value1 = FromASCIIByteArray(byteData);
            string value2 = FromUnicodeByteArray(byteData);
            byte[] ascii = AsciiStringToByteArray(value1);
            byte[] unicode = UnicodeStringToByteArray(value2);
            if (ascii[0] != unicode[0])
                return true;
            return false;
        }

        public static String FromASCIIByteArray(byte[] characters)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            String constructedString = encoding.GetString(characters);
            return constructedString;
        }

        public static String FromUnicodeByteArray(byte[] characters)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            String constructedString = encoding.GetString(characters);
            return constructedString;
        }

        public static byte[] AsciiStringToByteArray(String str)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        public static byte[] UnicodeStringToByteArray(String str)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            return encoding.GetBytes(str);
        }
	}
}