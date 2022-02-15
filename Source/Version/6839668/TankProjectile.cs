using UnityEngine;

public class TankProjectile : MonoBehaviour
{
	public float Speed;

	public float Lifetime;

	private void Start()
	{
		Invoke("DestroySelf", Lifetime);
	}

	private void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		base.transform.position = base.transform.position + base.transform.forward * Speed * Time.deltaTime;
	}
}
