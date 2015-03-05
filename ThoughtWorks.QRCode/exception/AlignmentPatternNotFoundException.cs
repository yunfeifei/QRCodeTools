using System;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[Serializable]
	public class AlignmentPatternNotFoundException:System.ArgumentException
	{
        internal String message = null;

		public override String Message
		{
			get
			{
				return message;
			}
			
		}		
		public AlignmentPatternNotFoundException(String message)
		{
			this.message = message;
		}
	}
}