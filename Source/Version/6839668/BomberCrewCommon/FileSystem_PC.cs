using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace BomberCrewCommon;

public class FileSystem_PC : FileSystem_Null
{
	private string ConvertFilename(string filename)
	{
		return Application.persistentDataPath + "/" + filename;
	}

	public override bool IsSupportedOnPlatform()
	{
		return true;
	}

	public override bool Exists(string filename)
	{
		return File.Exists(ConvertFilename(filename));
	}

	public override byte[] ReadAllBytes(string filename)
	{
		return File.ReadAllBytes(ConvertFilename(filename));
	}

	public override string ReadAllText(string filename)
	{
		return File.ReadAllText(ConvertFilename(filename));
	}

	public static void WriteAllTextWithBackup(string path, string contents)
	{
		try
		{
			if (File.Exists(path))
			{
				string tempFileName = Path.GetTempFileName();
				string text = path + ".backup";
				byte[] bytes = Encoding.UTF8.GetBytes(contents);
				using (FileStream fileStream = File.Create(tempFileName, 4096))
				{
					fileStream.Write(bytes, 0, bytes.Length);
					fileStream.Flush();
					fileStream.Close();
				}
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				File.Replace(tempFileName, path, text);
			}
			else
			{
				byte[] bytes2 = Encoding.UTF8.GetBytes(contents);
				FileStream fileStream2 = File.Create(path, 4096);
				fileStream2.Write(bytes2, 0, bytes2.Length);
				fileStream2.Flush();
				fileStream2.Close();
			}
		}
		catch (Exception ex)
		{
			try
			{
				if (ex != null)
				{
					DebugLogWrapper.LogError("[FILESYSTEM] Couldn't save through backup method... falling back once: " + ex);
				}
				else
				{
					DebugLogWrapper.LogError("[FILESYSTEM] Couldn't save through backup method... falling back once: null");
				}
				byte[] bytes3 = Encoding.UTF8.GetBytes(contents);
				FileStream fileStream3 = File.Create(path, 4096);
				fileStream3.Write(bytes3, 0, bytes3.Length);
				fileStream3.Flush();
				fileStream3.Close();
			}
			catch
			{
				if (ex != null)
				{
					DebugLogWrapper.LogError("[FILESYSTEM] Couldn't save through backup method... falling back twice: " + ex);
				}
				else
				{
					DebugLogWrapper.LogError("[FILESYSTEM] Couldn't save through backup method... falling back twice: null");
				}
				File.WriteAllText(path, contents);
			}
		}
	}

	public static void WriteAllDataWithBackup(string path, byte[] data)
	{
		try
		{
			string tempFileName = Path.GetTempFileName();
			string text = path + ".backup";
			using (FileStream fileStream = File.Create(tempFileName, 4096))
			{
				fileStream.Write(data, 0, data.Length);
				fileStream.Flush();
				fileStream.Close();
			}
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			File.Replace(tempFileName, path, text);
		}
		catch (Exception)
		{
			DebugLogWrapper.LogError("[FILESYSTEM] Couldn't save through backup method... falling back");
			File.WriteAllBytes(path, data);
		}
	}

	public override void WriteAllBytes(string filename, byte[] bytes)
	{
		SaveStart();
		WriteAllDataWithBackup(ConvertFilename(filename), bytes);
		SaveFinish();
	}

	public override void WriteAllText(string filename, string contents)
	{
		SaveStart();
		WriteAllTextWithBackup(ConvertFilename(filename), contents);
		SaveFinish();
	}
}
