using System;

namespace ThoughtWorks.QRCode.Codec.Data
{
	public interface QRCodeImage
	{
        int Width
        {
            get;

        }
        int Height
        {
            get;

        }
        int getPixel(int x, int y);
	}
}