using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public Transform camTransform;

	public float shake;

	public float shakeAmount = 0.7f;

	public float decreaseFactor = 1f;

	private Vector3 originalPos;

	private void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}

	private void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	private void Update()
	{
		if (shake > 0f)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
			shake -= Time.deltaTime * decreaseFactor;
			shakeAmount -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shake = 0f;
			camTransform.localPosition = originalPos;
		}
	}
}
