using System.Runtime.InteropServices;

namespace dbox;

public static class DboxSdkWrapper
{
	private const string DLLName = "dbxBridge";

	[DllImport("dbxBridge")]
	public static extern int InitializeDbox(string sAppKey, int nAppBuild);

	[DllImport("dbxBridge")]
	public static extern int TerminateDbox();

	[DllImport("dbxBridge")]
	public static extern int OpenDbox();

	[DllImport("dbxBridge")]
	public static extern int CloseDbox();

	[DllImport("dbxBridge")]
	public static extern int StartDbox();

	[DllImport("dbxBridge")]
	public static extern int StopDbox();

	[DllImport("dbxBridge")]
	public static extern int ResetStateDbox();

	[DllImport("dbxBridge")]
	public static extern bool IsInitializedDbox();

	[DllImport("dbxBridge")]
	public static extern bool IsOpenedDbox();

	[DllImport("dbxBridge")]
	public static extern bool IsStartedDbox();

	[DllImport("dbxBridge")]
	public static extern int PostCameraFrameUpdate([MarshalAs(UnmanagedType.Struct)] DboxStructs.CameraFrameUpdate oCameraFrameUpdate);

	[DllImport("dbxBridge")]
	public static extern int PostTarget();

	[DllImport("dbxBridge")]
	public static extern int PostTargetCancel();

	[DllImport("dbxBridge")]
	public static extern int PostFireGun();

	[DllImport("dbxBridge")]
	public static extern int PostLowerGear();

	[DllImport("dbxBridge")]
	public static extern int PostRaiseGear();

	[DllImport("dbxBridge")]
	public static extern int PostTakeDamage(float dOrientation, float dIntensity);

	[DllImport("dbxBridge")]
	public static extern int PostPlaneFailure();

	[DllImport("dbxBridge")]
	public static extern int PostTakeOff();

	[DllImport("dbxBridge")]
	public static extern int PostLand();

	[DllImport("dbxBridge")]
	public static extern int PostOpenBayDoors();

	[DllImport("dbxBridge")]
	public static extern int PostCloseBayDoors();

	[DllImport("dbxBridge")]
	public static extern int PostReleaseBombs();

	[DllImport("dbxBridge")]
	public static extern int PostExplosion(float dOrientation, float dDistance);

	[DllImport("dbxBridge")]
	public static extern int PostTakePhoto();

	[DllImport("dbxBridge")]
	public static extern int PostRepairStart();

	[DllImport("dbxBridge")]
	public static extern int PostRepairStop();

	[DllImport("dbxBridge")]
	public static extern int PostDive();

	[DllImport("dbxBridge")]
	public static extern int PostCorkscrew();

	[DllImport("dbxBridge")]
	public static extern int PostFlak(float dOrientation = 0f, float dDistance = 0f);

	[DllImport("dbxBridge")]
	public static extern int PostCustomEvent1();

	[DllImport("dbxBridge")]
	public static extern int PostCustomEvent2();

	[DllImport("dbxBridge")]
	public static extern int PostCustomEvent3();

	[DllImport("dbxBridge")]
	public static extern int PostCustomStart();

	[DllImport("dbxBridge")]
	public static extern int PostCustomStop();
}
