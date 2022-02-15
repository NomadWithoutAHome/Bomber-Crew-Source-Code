using UnityEngine;

public class TankWeaponController : MonoBehaviour
{
	public TankProjectile ProjectilePrefab;

	public Transform Nozzle;

	private Animation _animation;

	private void Awake()
	{
		_animation = GetComponent<Animation>();
	}

	private void Update()
	{
		if (!_animation.isPlaying && Input.GetKeyDown(KeyCode.Space))
		{
			_animation.Play();
			Object.Instantiate(ProjectilePrefab, Nozzle.position, Nozzle.rotation);
		}
	}
}
