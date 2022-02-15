using UnityEngine;

public class BulletWiggle : MonoBehaviour
{
	public float m_randomForce = 2.5f;

	private void FixedUpdate()
	{
		float num = m_randomForce * GetComponent<Rigidbody>().velocity.magnitude;
		GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0f - num, num), Random.Range(0f - num, num), 0f));
	}
}
