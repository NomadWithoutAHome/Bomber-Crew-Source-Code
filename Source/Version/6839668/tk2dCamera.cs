using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Camera/tk2dCamera")]
[ExecuteInEditMode]
public class tk2dCamera : MonoBehaviour
{
	private static int CURRENT_VERSION = 1;

	public int version;

	[SerializeField]
	private tk2dCameraSettings cameraSettings = new tk2dCameraSettings();

	public tk2dCameraResolutionOverride[] resolutionOverride = new tk2dCameraResolutionOverride[1] { tk2dCameraResolutionOverride.DefaultOverride };

	[SerializeField]
	private tk2dCamera inheritSettings;

	public int nativeResolutionWidth = 960;

	public int nativeResolutionHeight = 640;

	[SerializeField]
	private Camera _unityCamera;

	private static tk2dCamera inst;

	private static List<tk2dCamera> allCameras = new List<tk2dCamera>();

	public bool viewportClippingEnabled;

	public Vector4 viewportRegion = new Vector4(0f, 0f, 100f, 100f);

	private Vector2 _targetResolution = Vector2.zero;

	[SerializeField]
	private float zoomFactor = 1f;

	[HideInInspector]
	public bool forceResolutionInEditor;

	[HideInInspector]
	public Vector2 forceResolution = new Vector2(960f, 640f);

	private Rect _screenExtents;

	private Rect _nativeScreenExtents;

	private Rect unitRect = new Rect(0f, 0f, 1f, 1f);

	private tk2dCamera _settingsRoot;

	public tk2dCameraSettings CameraSettings => cameraSettings;

	public tk2dCameraResolutionOverride CurrentResolutionOverride
	{
		get
		{
			tk2dCamera settingsRoot = SettingsRoot;
			Camera screenCamera = ScreenCamera;
			float num = screenCamera.pixelWidth;
			float num2 = screenCamera.pixelHeight;
			tk2dCameraResolutionOverride tk2dCameraResolutionOverride2 = null;
			if (tk2dCameraResolutionOverride2 == null || (tk2dCameraResolutionOverride2 != null && ((float)tk2dCameraResolutionOverride2.width != num || (float)tk2dCameraResolutionOverride2.height != num2)))
			{
				tk2dCameraResolutionOverride2 = null;
				if (settingsRoot.resolutionOverride != null)
				{
					tk2dCameraResolutionOverride[] array = settingsRoot.resolutionOverride;
					foreach (tk2dCameraResolutionOverride tk2dCameraResolutionOverride3 in array)
					{
						if (tk2dCameraResolutionOverride3.Match((int)num, (int)num2))
						{
							tk2dCameraResolutionOverride2 = tk2dCameraResolutionOverride3;
							break;
						}
					}
				}
			}
			return tk2dCameraResolutionOverride2;
		}
	}

	public tk2dCamera InheritConfig
	{
		get
		{
			return inheritSettings;
		}
		set
		{
			if (inheritSettings != value)
			{
				inheritSettings = value;
				_settingsRoot = null;
			}
		}
	}

	private Camera UnityCamera
	{
		get
		{
			if (_unityCamera == null)
			{
				_unityCamera = GetComponent<Camera>();
				if (_unityCamera == null)
				{
					DebugLogWrapper.LogError("A unity camera must be attached to the tk2dCamera script");
				}
			}
			return _unityCamera;
		}
	}

	public static tk2dCamera Instance => inst;

	public Rect ScreenExtents => _screenExtents;

	public Rect NativeScreenExtents => _nativeScreenExtents;

	public Vector2 TargetResolution => _targetResolution;

	public Vector2 NativeResolution => new Vector2(nativeResolutionWidth, nativeResolutionHeight);

	[Obsolete]
	public Vector2 ScreenOffset => new Vector2(ScreenExtents.xMin - NativeScreenExtents.xMin, ScreenExtents.yMin - NativeScreenExtents.yMin);

	[Obsolete]
	public Vector2 resolution => new Vector2(ScreenExtents.xMax, ScreenExtents.yMax);

	[Obsolete]
	public Vector2 ScreenResolution => new Vector2(ScreenExtents.xMax, ScreenExtents.yMax);

	[Obsolete]
	public Vector2 ScaledResolution => new Vector2(ScreenExtents.width, ScreenExtents.height);

	public float ZoomFactor
	{
		get
		{
			return zoomFactor;
		}
		set
		{
			zoomFactor = Mathf.Max(0.01f, value);
		}
	}

	[Obsolete]
	public float zoomScale
	{
		get
		{
			return 1f / Mathf.Max(0.001f, zoomFactor);
		}
		set
		{
			ZoomFactor = 1f / Mathf.Max(0.001f, value);
		}
	}

	public Camera ScreenCamera => (!viewportClippingEnabled || !(inheritSettings != null) || !(inheritSettings.UnityCamera.rect == unitRect)) ? UnityCamera : inheritSettings.UnityCamera;

	public tk2dCamera SettingsRoot
	{
		get
		{
			if (_settingsRoot == null)
			{
				_settingsRoot = ((!(inheritSettings == null) && !(inheritSettings == this)) ? inheritSettings.SettingsRoot : this);
			}
			return _settingsRoot;
		}
	}

	public static tk2dCamera CameraForLayer(int layer)
	{
		int num = 1 << layer;
		int count = allCameras.Count;
		for (int i = 0; i < count; i++)
		{
			tk2dCamera tk2dCamera2 = allCameras[i];
			if ((tk2dCamera2.UnityCamera.cullingMask & num) == num)
			{
				return tk2dCamera2;
			}
		}
		return null;
	}

	private void Awake()
	{
		Upgrade();
		if (allCameras.IndexOf(this) == -1)
		{
			allCameras.Add(this);
		}
		tk2dCamera settingsRoot = SettingsRoot;
		tk2dCameraSettings tk2dCameraSettings2 = settingsRoot.CameraSettings;
		if (tk2dCameraSettings2.projection == tk2dCameraSettings.ProjectionType.Perspective)
		{
			UnityCamera.transparencySortMode = tk2dCameraSettings2.transparencySortMode;
		}
	}

	private void OnEnable()
	{
		if (UnityCamera != null)
		{
			UpdateCameraMatrix();
		}
		else
		{
			GetComponent<Camera>().enabled = false;
		}
		if (!viewportClippingEnabled)
		{
			inst = this;
		}
		if (allCameras.IndexOf(this) == -1)
		{
			allCameras.Add(this);
		}
	}

	private void OnDestroy()
	{
		int num = allCameras.IndexOf(this);
		if (num != -1)
		{
			allCameras.RemoveAt(num);
		}
	}

	private void OnPreCull()
	{
		tk2dUpdateManager.FlushQueues();
		UpdateCameraMatrix();
	}

	public float GetSizeAtDistance(float distance)
	{
		tk2dCameraSettings tk2dCameraSettings2 = SettingsRoot.CameraSettings;
		switch (tk2dCameraSettings2.projection)
		{
		case tk2dCameraSettings.ProjectionType.Orthographic:
			if (tk2dCameraSettings2.orthographicType == tk2dCameraSettings.OrthographicType.PixelsPerMeter)
			{
				return 1f / tk2dCameraSettings2.orthographicPixelsPerMeter;
			}
			return 2f * tk2dCameraSettings2.orthographicSize / (float)SettingsRoot.nativeResolutionHeight;
		case tk2dCameraSettings.ProjectionType.Perspective:
			return Mathf.Tan(CameraSettings.fieldOfView * ((float)Math.PI / 180f) * 0.5f) * distance * 2f / (float)SettingsRoot.nativeResolutionHeight;
		default:
			return 1f;
		}
	}

	public Matrix4x4 OrthoOffCenter(Vector2 scale, float left, float right, float bottom, float top, float near, float far)
	{
		float value = 2f / (right - left) * scale.x;
		float value2 = 2f / (top - bottom) * scale.y;
		float value3 = -2f / (far - near);
		float value4 = (0f - (right + left)) / (right - left);
		float value5 = (0f - (bottom + top)) / (top - bottom);
		float value6 = (0f - (far + near)) / (far - near);
		Matrix4x4 result = default(Matrix4x4);
		result[0, 0] = value;
		result[0, 1] = 0f;
		result[0, 2] = 0f;
		result[0, 3] = value4;
		result[1, 0] = 0f;
		result[1, 1] = value2;
		result[1, 2] = 0f;
		result[1, 3] = value5;
		result[2, 0] = 0f;
		result[2, 1] = 0f;
		result[2, 2] = value3;
		result[2, 3] = value6;
		result[3, 0] = 0f;
		result[3, 1] = 0f;
		result[3, 2] = 0f;
		result[3, 3] = 1f;
		return result;
	}

	private Vector2 GetScaleForOverride(tk2dCamera settings, tk2dCameraResolutionOverride currentOverride, float width, float height)
	{
		Vector2 one = Vector2.one;
		float num = 1f;
		if (currentOverride == null)
		{
			return one;
		}
		switch (currentOverride.autoScaleMode)
		{
		case tk2dCameraResolutionOverride.AutoScaleMode.PixelPerfect:
			num = 1f;
			one.Set(num, num);
			break;
		case tk2dCameraResolutionOverride.AutoScaleMode.FitHeight:
			num = height / (float)settings.nativeResolutionHeight;
			one.Set(num, num);
			break;
		case tk2dCameraResolutionOverride.AutoScaleMode.FitWidth:
			num = width / (float)settings.nativeResolutionWidth;
			one.Set(num, num);
			break;
		case tk2dCameraResolutionOverride.AutoScaleMode.FitVisible:
		case tk2dCameraResolutionOverride.AutoScaleMode.ClosestMultipleOfTwo:
		{
			float num2 = (float)settings.nativeResolutionWidth / (float)settings.nativeResolutionHeight;
			float num3 = width / height;
			num = ((!(num3 < num2)) ? (height / (float)settings.nativeResolutionHeight) : (width / (float)settings.nativeResolutionWidth));
			if (currentOverride.autoScaleMode == tk2dCameraResolutionOverride.AutoScaleMode.ClosestMultipleOfTwo)
			{
				num = ((!(num > 1f)) ? Mathf.Pow(2f, Mathf.Floor(Mathf.Log(num, 2f))) : Mathf.Floor(num));
			}
			one.Set(num, num);
			break;
		}
		case tk2dCameraResolutionOverride.AutoScaleMode.StretchToFit:
			one.Set(width / (float)settings.nativeResolutionWidth, height / (float)settings.nativeResolutionHeight);
			break;
		case tk2dCameraResolutionOverride.AutoScaleMode.Fill:
			num = Mathf.Max(width / (float)settings.nativeResolutionWidth, height / (float)settings.nativeResolutionHeight);
			one.Set(num, num);
			break;
		default:
			num = currentOverride.scale;
			one.Set(num, num);
			break;
		}
		return one;
	}

	private Vector2 GetOffsetForOverride(tk2dCamera settings, tk2dCameraResolutionOverride currentOverride, Vector2 scale, float width, float height)
	{
		Vector2 result = Vector2.zero;
		if (currentOverride == null)
		{
			return result;
		}
		switch (currentOverride.fitMode)
		{
		case tk2dCameraResolutionOverride.FitMode.Center:
			if (settings.cameraSettings.orthographicOrigin == tk2dCameraSettings.OrthographicOrigin.BottomLeft)
			{
				result = new Vector2(Mathf.Round(((float)settings.nativeResolutionWidth * scale.x - width) / 2f), Mathf.Round(((float)settings.nativeResolutionHeight * scale.y - height) / 2f));
			}
			break;
		default:
			result = -currentOverride.offsetPixels;
			break;
		}
		return result;
	}

	private Matrix4x4 GetProjectionMatrixForOverride(tk2dCamera settings, tk2dCameraResolutionOverride currentOverride, float pixelWidth, float pixelHeight, bool halfTexelOffset, out Rect screenExtents, out Rect unscaledScreenExtents)
	{
		Vector2 scaleForOverride = GetScaleForOverride(settings, currentOverride, pixelWidth, pixelHeight);
		Vector2 offsetForOverride = GetOffsetForOverride(settings, currentOverride, scaleForOverride, pixelWidth, pixelHeight);
		float num = offsetForOverride.x;
		float num2 = offsetForOverride.y;
		float num3 = pixelWidth + offsetForOverride.x;
		float num4 = pixelHeight + offsetForOverride.y;
		Vector2 zero = Vector2.zero;
		bool flag = false;
		if (viewportClippingEnabled && InheritConfig != null)
		{
			float num5 = (num3 - num) / scaleForOverride.x;
			float num6 = (num4 - num2) / scaleForOverride.y;
			Vector4 vector = new Vector4((int)viewportRegion.x, (int)viewportRegion.y, (int)viewportRegion.z, (int)viewportRegion.w);
			flag = true;
			float num7 = (0f - offsetForOverride.x) / pixelWidth + vector.x / num5;
			float num8 = (0f - offsetForOverride.y) / pixelHeight + vector.y / num6;
			float num9 = vector.z / num5;
			float num10 = vector.w / num6;
			if (settings.cameraSettings.orthographicOrigin == tk2dCameraSettings.OrthographicOrigin.Center)
			{
				num7 += (pixelWidth - (float)settings.nativeResolutionWidth * scaleForOverride.x) / pixelWidth / 2f;
				num8 += (pixelHeight - (float)settings.nativeResolutionHeight * scaleForOverride.y) / pixelHeight / 2f;
			}
			Rect rect = new Rect(num7, num8, num9, num10);
			if (UnityCamera.rect.x != num7 || UnityCamera.rect.y != num8 || UnityCamera.rect.width != num9 || UnityCamera.rect.height != num10)
			{
				UnityCamera.rect = rect;
			}
			float num11 = Mathf.Min(1f - rect.x, rect.width);
			float num12 = Mathf.Min(1f - rect.y, rect.height);
			float num13 = vector.x * scaleForOverride.x - offsetForOverride.x;
			float num14 = vector.y * scaleForOverride.y - offsetForOverride.y;
			if (settings.cameraSettings.orthographicOrigin == tk2dCameraSettings.OrthographicOrigin.Center)
			{
				num13 -= (float)settings.nativeResolutionWidth * 0.5f * scaleForOverride.x;
				num14 -= (float)settings.nativeResolutionHeight * 0.5f * scaleForOverride.y;
			}
			if (rect.x < 0f)
			{
				num13 += (0f - rect.x) * pixelWidth;
				num11 = rect.x + rect.width;
			}
			if (rect.y < 0f)
			{
				num14 += (0f - rect.y) * pixelHeight;
				num12 = rect.y + rect.height;
			}
			num += num13;
			num2 += num14;
			num3 = pixelWidth * num11 + offsetForOverride.x + num13;
			num4 = pixelHeight * num12 + offsetForOverride.y + num14;
		}
		else
		{
			if (UnityCamera.rect != CameraSettings.rect)
			{
				UnityCamera.rect = CameraSettings.rect;
			}
			if (settings.cameraSettings.orthographicOrigin == tk2dCameraSettings.OrthographicOrigin.Center)
			{
				float num15 = (num3 - num) * 0.5f;
				num -= num15;
				num3 -= num15;
				float num16 = (num4 - num2) * 0.5f;
				num4 -= num16;
				num2 -= num16;
				zero.Set((float)(-nativeResolutionWidth) / 2f, (float)(-nativeResolutionHeight) / 2f);
			}
		}
		float num17 = 1f / ZoomFactor;
		bool flag2 = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.WindowsEditor;
		float num18 = ((!halfTexelOffset || !flag2 || SystemInfo.graphicsShaderLevel >= 40) ? 0f : 0.5f);
		float num19 = settings.cameraSettings.orthographicSize;
		switch (settings.cameraSettings.orthographicType)
		{
		case tk2dCameraSettings.OrthographicType.OrthographicSize:
			num19 = 2f * settings.cameraSettings.orthographicSize / (float)settings.nativeResolutionHeight;
			break;
		case tk2dCameraSettings.OrthographicType.PixelsPerMeter:
			num19 = 1f / settings.cameraSettings.orthographicPixelsPerMeter;
			break;
		}
		if (!flag)
		{
			float num20 = Mathf.Min(UnityCamera.rect.width, 1f - UnityCamera.rect.x);
			float num21 = Mathf.Min(UnityCamera.rect.height, 1f - UnityCamera.rect.y);
			if (num20 > 0f && num21 > 0f)
			{
				scaleForOverride.x /= num20;
				scaleForOverride.y /= num21;
			}
		}
		float num22 = num19 * num17;
		screenExtents = new Rect(num * num22 / scaleForOverride.x, num2 * num22 / scaleForOverride.y, (num3 - num) * num22 / scaleForOverride.x, (num4 - num2) * num22 / scaleForOverride.y);
		unscaledScreenExtents = new Rect(zero.x * num22, zero.y * num22, (float)nativeResolutionWidth * num22, (float)nativeResolutionHeight * num22);
		return OrthoOffCenter(scaleForOverride, num19 * (num + num18) * num17, num19 * (num3 + num18) * num17, num19 * (num2 - num18) * num17, num19 * (num4 - num18) * num17, UnityCamera.nearClipPlane, UnityCamera.farClipPlane);
	}

	private Vector2 GetScreenPixelDimensions(tk2dCamera settings)
	{
		return new Vector2(ScreenCamera.pixelWidth, ScreenCamera.pixelHeight);
	}

	private void Upgrade()
	{
		if (version == CURRENT_VERSION)
		{
			return;
		}
		if (version == 0)
		{
			cameraSettings.orthographicPixelsPerMeter = 1f;
			cameraSettings.orthographicType = tk2dCameraSettings.OrthographicType.PixelsPerMeter;
			cameraSettings.orthographicOrigin = tk2dCameraSettings.OrthographicOrigin.BottomLeft;
			cameraSettings.projection = tk2dCameraSettings.ProjectionType.Orthographic;
			tk2dCameraResolutionOverride[] array = resolutionOverride;
			foreach (tk2dCameraResolutionOverride tk2dCameraResolutionOverride2 in array)
			{
				tk2dCameraResolutionOverride2.Upgrade(version);
			}
			Camera component = GetComponent<Camera>();
			if (component != null)
			{
				cameraSettings.rect = component.rect;
				if (!component.orthographic)
				{
					cameraSettings.projection = tk2dCameraSettings.ProjectionType.Perspective;
					cameraSettings.fieldOfView = component.fieldOfView * ZoomFactor;
				}
				component.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
			}
		}
		version = CURRENT_VERSION;
	}

	public void UpdateCameraMatrix()
	{
		Upgrade();
		if (!viewportClippingEnabled)
		{
			inst = this;
		}
		Camera unityCamera = UnityCamera;
		tk2dCamera settingsRoot = SettingsRoot;
		tk2dCameraSettings tk2dCameraSettings2 = settingsRoot.CameraSettings;
		if (unityCamera.rect != cameraSettings.rect)
		{
			unityCamera.rect = cameraSettings.rect;
		}
		_targetResolution = GetScreenPixelDimensions(settingsRoot);
		if (tk2dCameraSettings2.projection == tk2dCameraSettings.ProjectionType.Perspective)
		{
			if (unityCamera.orthographic)
			{
				unityCamera.orthographic = false;
			}
			float num = Mathf.Min(179.9f, tk2dCameraSettings2.fieldOfView / Mathf.Max(0.001f, ZoomFactor));
			if (unityCamera.fieldOfView != num)
			{
				unityCamera.fieldOfView = num;
			}
			_screenExtents.Set(0f - unityCamera.aspect, -1f, unityCamera.aspect * 2f, 2f);
			_nativeScreenExtents = _screenExtents;
			unityCamera.ResetProjectionMatrix();
			return;
		}
		if (!unityCamera.orthographic)
		{
			unityCamera.orthographic = true;
		}
		Matrix4x4 matrix4x = GetProjectionMatrixForOverride(settingsRoot, settingsRoot.CurrentResolutionOverride, _targetResolution.x, _targetResolution.y, halfTexelOffset: true, out _screenExtents, out _nativeScreenExtents);
		if (Application.platform == RuntimePlatform.WP8Player && (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight))
		{
			float z = ((Screen.orientation != ScreenOrientation.LandscapeRight) ? (-90f) : 90f);
			Matrix4x4 matrix4x2 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, z), Vector3.one);
			matrix4x = matrix4x2 * matrix4x;
		}
		if (unityCamera.projectionMatrix != matrix4x)
		{
			unityCamera.projectionMatrix = matrix4x;
		}
	}
}
