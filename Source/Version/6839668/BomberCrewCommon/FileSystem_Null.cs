namespace BomberCrewCommon;

public class FileSystem_Null : FileSystemBase
{
	public override bool IsSupportedOnPlatform()
	{
		return true;
	}

	public override bool Exists(string filename)
	{
		return false;
	}

	public override byte[] ReadAllBytes(string filename)
	{
		return null;
	}

	public override string ReadAllText(string filename)
	{
		return null;
	}

	public override void WriteAllBytes(string filename, byte[] bytes)
	{
	}

	public override void WriteAllText(string filename, string contents)
	{
	}
}
