using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Manager;

namespace SymmetricAlgorithms
{
	public class AES_Symm_Algorithm
	{
		/// <summary>
		/// Function that encrypts the plaintext from inFile and stores cipher text to outFile
		/// </summary>
		/// <param name="inFile"> filepath where plaintext is stored </param>
		/// <param name="outFile"> filepath where cipher text is expected to be stored </param>
		/// <param name="secretKey"> symmetric encryption key </param>

		public static string EncryptMessage(string message, byte[] secretKey)
		{
			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
			{
				aes.Key = secretKey;
				aes.Mode = CipherMode.ECB;

				ICryptoTransform encryptor = aes.CreateEncryptor();
				byte[] plainBytes = Encoding.UTF8.GetBytes(message);
				byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

				return Convert.ToBase64String(encryptedBytes);
			}
		}

		public static string DecryptMessage(string encryptedMessage, byte[] secretKey)
		{
			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
			{
				aes.Key =secretKey;
				aes.Mode = CipherMode.ECB;

				ICryptoTransform decryptor = aes.CreateDecryptor();
				byte[] encryptedBytes = Convert.FromBase64String(encryptedMessage);
				byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

				return Encoding.UTF8.GetString(decryptedBytes);
			}
		}


	}
}
