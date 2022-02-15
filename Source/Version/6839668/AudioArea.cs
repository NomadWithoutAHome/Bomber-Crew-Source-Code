using UnityEngine;

public class AudioArea : MonoBehaviour
{
	[SerializeField]
	private Vector3 m_centreOffset;

	[SerializeField]
	private Vector3 m_size;

	public Vector3 GetListeningPosition(Vector3 audioCtrPos, Vector3 myRelativePos)
	{
		Vector3 vector = myRelativePos + m_centreOffset + m_size * 0.5f;
		Vector3 vector2 = myRelativePos + m_centreOffset - m_size * 0.5f;
		Vector3 result = audioCtrPos;
		if (audioCtrPos.x < vector2.x)
		{
			result.x = vector2.x;
		}
		else if (audioCtrPos.x > vector.x)
		{
			result.x = vector.x;
		}
		if (audioCtrPos.y < vector2.y)
		{
			result.y = vector2.y;
		}
		else if (audioCtrPos.y > vector.y)
		{
			result.y = vector.y;
		}
		if (audioCtrPos.z < vector2.z)
		{
			result.z = vector2.z;
		}
		else if (audioCtrPos.z > vector.z)
		{
			result.z = vector.z;
		}
		return result;
	}

	public void SetSize(Vector3 size)
	{
		m_size = size;
	}

	public void SetCentreOffset(Vector3 cOff)
	{
		m_centreOffset = cOff;
	}

	private void OnDrawGizmosSelected()
	{
		Color color = Gizmos.color;
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(base.transform.position + m_centreOffset, m_size);
		Gizmos.color = color;
	}
}
