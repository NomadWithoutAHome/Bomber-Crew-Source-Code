using UnityEngine;

public class TankAlwaysForward : MonoBehaviour
{
	public Material TrailMaterial;

	public float Speed;

	public float TrailSpeed;

	private void FixedUpdate()
	{
		base.transform.position = base.transform.position + base.transform.forward * Speed;
		TrailMaterial.mainTextureOffset = new Vector2(TrailMaterial.mainTextureOffset.x + TrailSpeed, TrailMaterial.mainTextureOffset.y);
	}
}
