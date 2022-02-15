using UnityEngine;

public class CrewmanBulkOutForPrinting : MonoBehaviour
{
	[SerializeField]
	private Transform[] m_transformsToSet;

	public void SetOnCrewman(GameObject go)
	{
		Transform[] transformsToSet = m_transformsToSet;
		foreach (Transform transform in transformsToSet)
		{
			string text = transform.name;
			Transform transform2 = go.transform.Find(text);
			transform.transform.SetPositionAndRotation(transform2.position, transform2.rotation);
		}
	}
}
