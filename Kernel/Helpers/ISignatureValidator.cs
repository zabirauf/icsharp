
namespace iCSharp.Kernel.Helpers
{
	using System;
	using iCSharp.Messages;

	public interface ISignatureValidator
	{
		/// <summary>
		/// Creates the signature.
		/// </summary>
		/// <returns>The signature.</returns>
		/// <param name="message">Message.</param>
		string CreateSignature(Message message);

	}
}

