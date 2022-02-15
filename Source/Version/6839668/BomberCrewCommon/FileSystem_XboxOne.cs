namespace BomberCrewCommon;

public class FileSystem_XboxOne : FileSystem_VirtualFS
{
	public override bool IsSupportedOnPlatform()
	{
		return false;
	}
}
