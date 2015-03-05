using System;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[Serializable]
	public class InvalidVersionException:VersionInformationException
	{
        internal String message;
		public override String Message
		{
			get
			{
				return message;
			}
			
		}
		
		public InvalidVersionException(String message)
		{
			this.message = message;
		}
	}
}