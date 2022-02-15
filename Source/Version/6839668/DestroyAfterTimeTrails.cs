using UnityEngine;

public class DestroyAfterTimeTrails : MonoBehaviour
{
	public float lifetime;

	private void Start()
	{
		Invoke("DestroyMe", lifetime);
	}

	private void DestroyMe()
	{
		Object.Destroy(base.gameObject);
	}
}
