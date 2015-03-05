using System;
namespace ThoughtWorks.QRCode.Codec.Util
{
	
	public struct Color_Fields{
		public readonly static int GRAY = 0xAAAAAA;
		public readonly static int LIGHTGRAY = 0xBBBBBB;
		public readonly static int DARKGRAY = 0x444444;
		public readonly static int BLACK = 0x000000;
		public readonly static int WHITE = 0xFFFFFF;
		public readonly static int BLUE = 0x8888FF;
		public readonly static int GREEN = 0x88FF88;
		public readonly static int LIGHTBLUE = 0xBBBBFF;
		public readonly static int LIGHTGREEN = 0xBBFFBB;
		public readonly static int RED = 0xFF88888;
		public readonly static int ORANGE = 0xFFFF88;
		public readonly static int LIGHTRED = 0xFFBBBB;
	}
	public interface Color
	{
		
	}
}