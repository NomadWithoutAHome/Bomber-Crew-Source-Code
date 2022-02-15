using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BomberCrewCommon;

public static class Cryptography
{
	private static int _iterations = 2;

	private static int _keySize = 256;

	private static string _hash = "SHA1";

	private static string _salt = "kjanvejkna34714j";

	private static string _vector = "fe8ja3lfkjnv823f";

	public static byte[] Encrypt(byte[] value, string password)
	{
		return Encrypt<AesManaged>(value, password);
	}

	public static byte[] Encrypt<T>(byte[] valueBytes, string password) where T : SymmetricAlgorithm, new()
	{
		byte[] bytes = Encoding.ASCII.GetBytes(_vector);
		byte[] bytes2 = Encoding.ASCII.GetBytes(_salt);
		T val = new T();
		try
		{
			PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(password, bytes2, _hash, _iterations);
			byte[] bytes3 = passwordDeriveBytes.GetBytes(_keySize / 8);
			val.Mode = CipherMode.CBC;
			byte[] result;
			using (ICryptoTransform transform = val.CreateEncryptor(bytes3, bytes))
			{
				using MemoryStream memoryStream = new MemoryStream();
				using CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
				cryptoStream.Write(valueBytes, 0, valueBytes.Length);
				cryptoStream.FlushFinalBlock();
				result = memoryStream.ToArray();
			}
			val.Clear();
			return result;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static string Decrypt(byte[] value, string password)
	{
		return Decrypt<AesManaged>(value, password);
	}

	public static string Decrypt<T>(byte[] valueBytes, string password) where T : SymmetricAlgorithm, new()
	{
		byte[] bytes = Encoding.ASCII.GetBytes(_vector);
		byte[] bytes2 = Encoding.ASCII.GetBytes(_salt);
		int count = 0;
		T val = new T();
		byte[] array;
		try
		{
			PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(password, bytes2, _hash, _iterations);
			byte[] bytes3 = passwordDeriveBytes.GetBytes(_keySize / 8);
			val.Mode = CipherMode.CBC;
			try
			{
				using ICryptoTransform transform = val.CreateDecryptor(bytes3, bytes);
				using MemoryStream stream = new MemoryStream(valueBytes);
				using CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
				array = new byte[valueBytes.Length];
				count = cryptoStream.Read(array, 0, array.Length);
			}
			catch (Exception)
			{
				return string.Empty;
			}
			val.Clear();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return Encoding.UTF8.GetString(array, 0, count);
	}
}
