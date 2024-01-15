using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Manager
{
	public enum AlgorithmType
	{
		DES = 0,
		TripleDES = 1,
		AES = 2
	};

	public class SecretKey
	{
		#region Generate Secret Key

		/// <summary>
		/// Generate a symmetric key for the specific symmetric algorithm. IV is generated automatically.
		/// </summary>
		/// <param name="algorithmType"> type of symmetric algorith the key is generated for </param>
		/// <returns> string value representing a symmetric key </returns>
		public static byte[] GenerateKey(AlgorithmType algorithmType)
		{
			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
			{
				aes.GenerateKey();
				return aes.Key;
			}

		}

		#endregion

		#region Store Secret Key

		/// <summary>
		/// Store a secret key as string value in a specified file.
		/// </summary>
		/// <param name="secretKey"> a symmetric key value </param>
		/// <param name="outFile"> file location to store a secret key </param>
		public static void StoreKey(byte[] secretKey, string outFile)
		{
			
			File.WriteAllBytes(outFile, secretKey);
		}

		#endregion

		#region Load Secret Key

		/// <summary>
		/// Load a symmetric key value from a file
		/// </summary>
		/// <param name="inFile"> file location of a secret key </param>
		/// <returns> a secret key value </returns>
		public static byte[] LoadKey(string inFile)
		{
		
			return File.ReadAllBytes(inFile);
		}

		#endregion
	}
}
