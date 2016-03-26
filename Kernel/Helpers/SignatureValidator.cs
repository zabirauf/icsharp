
namespace iCSharp.Kernel.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Security.Cryptography;
	using System.Text;
	using Common.Logging;
	using Common.Serializer;
	using iCSharp.Messages;


	public class SignatureValidator : ISignatureValidator
	{
		private readonly ILog _logger;

		private readonly HMAC _signatureGenerator;

		private readonly Encoding _encoder;
		/// <summary>
		/// Initializes a new instance of the <see cref="iCSharp.Kernel.Helpers.Sha256SignatureValidator"/> class.
		/// </summary>
		/// <param name="key">Shared key used to initialize the digest.</param>
		public SignatureValidator (ILog logger, string key, string algorithm)
		{
			this._logger = logger;
			this._encoder = new UTF8Encoding ();

			this._signatureGenerator = HMAC.Create (algorithm);
			this._signatureGenerator.Key = this._encoder.GetBytes (key);
		}

		#region ISignatureValidator implementation

		/// <summary>
		/// Creates the signature.
		/// </summary>
		/// <returns>The signature.</returns>
		/// <param name="message">Message.</param>
		public string CreateSignature (Message message)
		{
			this._signatureGenerator.Initialize ();

			List<string> messages = this.GetMessagesToAddForDigest (message);

			// For all items update the signature
			foreach (string item in messages) {
				byte[] sourceBytes = this._encoder.GetBytes (item);
				this._signatureGenerator.TransformBlock (sourceBytes, 0, sourceBytes.Length, null, 0);
			}

			this._signatureGenerator.TransformFinalBlock (new byte[0], 0, 0);

			// Calculate the digest and remove -
			return BitConverter.ToString(this._signatureGenerator.Hash).Replace("-","").ToLower();
		}

		/// <summary>
		/// Determines whether this instance is valid signature the specified message.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="message">Message.</param>
		public bool IsValidSignature (Message message)
		{
			string calculatedSignature = this.CreateSignature (message);
			this._logger.Info (string.Format ("Expected Signature: {0}", calculatedSignature));
			return string.Equals (message.HMac, calculatedSignature, StringComparison.OrdinalIgnoreCase);
		}

		#endregion

		/// <summary>
		/// Gets the messages to add for digest.
		/// </summary>
		/// <returns>The messages to add for digest.</returns>
		/// <param name="message">Message.</param>
		private List<string> GetMessagesToAddForDigest(Message message)
		{
			if (message == null) 
			{
				return new List<string> ();
			}

			return new List<string>(){
				JsonSerializer.Serialize(message.Header),
				JsonSerializer.Serialize(message.ParentHeader),
				JsonSerializer.Serialize(message.MetaData),
				message.Content
			};
		}
	}
}

