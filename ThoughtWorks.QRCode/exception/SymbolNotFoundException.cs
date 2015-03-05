using System;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[Serializable]
	public class SymbolNotFoundException:System.ArgumentException
	{
        internal String message = null;

		public override String Message
		{
			get
			{
				return message;
			}
			
		}
		
		public SymbolNotFoundException(String message)
		{
			this.message = message;
		}
	}
}