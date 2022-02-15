using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	[SerializeField]
	private float m_orthoSizeMin = 10f;

	[SerializeField]
	private float m_orthoSizeMax = 40f;

	public Transform m_parallaxZoomNode01;

	public AnimationCurve m_parallaxCurve01;

	public Transform m_parallaxZoomNode02;

	public AnimationCurve m_parallaxCurve02;

	public tk2dSprite m_bomberExteriorSprite;

	public AnimationCurve m_bomberExteriorAlphaCurve;

	private void Update()
	{
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + Input.GetAxis("Mouse ScrollWheel"), m_orthoSizeMin, m_orthoSizeMax);
		float num = m_parallaxCurve01.Evaluate(Camera.main.orthographicSize);
		float num2 = m_parallaxCurve02.Evaluate(Camera.main.orthographicSize);
		m_parallaxZoomNode01.localScale = new Vector3(num, num, num);
		m_parallaxZoomNode02.localScale = new Vector3(num2, num2, num2);
		m_bomberExteriorSprite.color = new Color(1f, 1f, 1f, m_bomberExteriorAlphaCurve.Evaluate(Camera.main.orthographicSize));
	}
}
