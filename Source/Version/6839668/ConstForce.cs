using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

public class ConstForce : MonoBehaviour
{
	public float speed;

	private void Start()
	{
	}

	private void Update()
	{
		SmokePlume[] components = GetComponents<SmokePlume>();
		foreach (SmokePlume smokePlume in components)
		{
			smokePlume.ConstantForce = base.transform.forward * speed;
		}
	}
}
