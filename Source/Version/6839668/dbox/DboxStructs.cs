namespace dbox;

public class DboxStructs
{
	public struct XyzFloat
	{
		public float X;

		public float Y;

		public float Z;
	}

	public struct CameraFrameUpdate
	{
		public XyzFloat angularVelocity;

		public float distance;
	}
}
