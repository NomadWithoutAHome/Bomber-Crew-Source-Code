using UnityEngine;

public class BomberCameraTrackNode : MonoBehaviour
{
	[SerializeField]
	private Transform[] m_nodesToTrackTo;

	[SerializeField]
	private Transform m_movementNode;

	[SerializeField]
	private float m_lerpSpeed = 8f;

	private Vector3 m_targetPosition = Vector3.zero;

	private Quaternion m_targetRotation = Quaternion.identity;

	public void SetT(float t)
	{
		t = Mathf.Clamp01(t);
		t *= (float)(m_nodesToTrackTo.Length - 1);
		int num = Mathf.FloorToInt(t);
		int num2 = num + 1;
		if (num == m_nodesToTrackTo.Length - 1)
		{
			m_targetPosition = m_nodesToTrackTo[m_nodesToTrackTo.Length - 1].localPosition;
			m_targetRotation = m_nodesToTrackTo[m_nodesToTrackTo.Length - 1].rotation;
		}
		else
		{
			float t2 = t - (float)num;
			m_targetPosition = Vector3.Lerp(m_nodesToTrackTo[num].localPosition, m_nodesToTrackTo[num2].localPosition, t2);
			m_targetRotation = Quaternion.Lerp(m_nodesToTrackTo[num].rotation, m_nodesToTrackTo[num2].rotation, t2);
		}
	}

	public float GetPos(Vector3 worldPos)
	{
		int num = 0;
		float num2 = 0f;
		for (int i = 0; i < m_nodesToTrackTo.Length - 1; i++)
		{
			Vector3 position = m_nodesToTrackTo[i].position;
			Vector3 vector = m_nodesToTrackTo[i + 1].position - position;
			float magnitude = vector.magnitude;
			Vector3 rhs = vector / magnitude;
			Vector3 vector2 = worldPos - position;
			float num3 = Vector3.Dot(vector2.normalized, rhs);
			if (num3 > 0f)
			{
				float value = num3 * vector2.magnitude / magnitude;
				num = i;
				num2 = Mathf.Clamp01(value);
			}
		}
		return Mathf.Clamp01(((float)num + num2) / (float)m_nodesToTrackTo.Length);
	}

	private void FixedUpdate()
	{
		m_movementNode.localPosition = Vector3.Lerp(m_movementNode.localPosition, m_targetPosition, m_lerpSpeed * Time.deltaTime);
		m_movementNode.rotation = Quaternion.Lerp(m_movementNode.rotation, m_targetRotation, m_lerpSpeed * Time.deltaTime);
	}
}
