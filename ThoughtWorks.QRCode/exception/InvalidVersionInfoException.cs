using System;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[Serializable]
	public class InvalidVersionInfoException:VersionInformationException
	{
        internal String message = null;
		public override String Message
		{
			get
			{
				return message;
			}
			
		}
		
		public InvalidVersionInfoException(String message)
		{
			this.message = message;
		}
	}
}