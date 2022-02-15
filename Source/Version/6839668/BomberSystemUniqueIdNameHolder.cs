using UnityEngine;

public class BomberSystemUniqueIdNameHolder : MonoBehaviour
{
	[SerializeField]
	private string m_bomberSystemUniqueId;

	public string GetUniqueId()
	{
		return m_bomberSystemUniqueId;
	}
}
