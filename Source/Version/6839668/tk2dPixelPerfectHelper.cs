using System;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Deprecated/Extra/tk2dPixelPerfectHelper")]
public class tk2dPixelPerfectHelper : MonoBehaviour
{
	private static tk2dPixelPerfectHelper _inst;

	[NonSerialized]
	public Camera cam;

	public int collectionTargetHeight = 640;

	public float collectionOrthoSize = 1f;

	public float targetResolutionHeight = 640f;

	[NonSerialized]
	public float scaleD;

	[NonSerialized]
	public float scaleK;

	public static tk2dPixelPerfectHelper inst
	{
		get
		{
			if (_inst == null)
			{
				_inst = UnityEngine.Object.FindObjectOfType(typeof(tk2dPixelPerfectHelper)) as tk2dPixelPerfectHelper;
				if (_inst == null)
				{
					return null;
				}
				inst.Setup();
			}
			return _inst;
		}
	}

	public bool CameraIsOrtho => cam.orthographic;

	private void Awake()
	{
		Setup();
		_inst = this;
	}

	public virtual void Setup()
	{
		float num = (float)collectionTargetHeight / targetResolutionHeight;
		if (GetComponent<Camera>() != null)
		{
			cam = GetComponent<Camera>();
		}
		if (cam == null)
		{
			cam = Camera.main;
		}
		if (cam.orthographic)
		{
			scaleK = num * cam.orthographicSize / collectionOrthoSize;
			scaleD = 0f;
		}
		else
		{
			float num2 = num * Mathf.Tan((float)Math.PI / 180f * cam.fieldOfView * 0.5f) / collectionOrthoSize;
			scaleK = num2 * (0f - cam.transform.position.z);
			scaleD = num2;
		}
	}

	public static float CalculateScaleForPerspectiveCamera(float fov, float zdist)
	{
		return Mathf.Abs(Mathf.Tan((float)Math.PI / 180f * fov * 0.5f) * zdist);
	}
}
