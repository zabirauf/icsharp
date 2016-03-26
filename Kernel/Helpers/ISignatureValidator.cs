
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

		/// <summary>
		/// Determines whether this instance is valid signature the specified message.
		/// </summary>
		/// <returns><c>true</c> if this instance is valid signature the specified message; otherwise, <c>false</c>.</returns>
		/// <param name="message">Message.</param>
		bool IsValidSignature(Message message);
	}
}

