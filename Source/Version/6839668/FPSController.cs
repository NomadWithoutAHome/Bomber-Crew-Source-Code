using UnityEngine;

public class FPSController : MonoBehaviour
{
	public Animator CamAnimator;

	public Animator WeaponAnimator;

	public float moveSpeed;

	private void Start()
	{
	}

	private void Update()
	{
		CamAnimator.SetBool("Running", Input.GetKey(KeyCode.W));
		WeaponAnimator.SetBool("Fire", Input.GetKey(KeyCode.Space));
		if (Input.GetKey(KeyCode.W))
		{
			base.transform.position = base.transform.position + base.transform.forward * moveSpeed * Time.deltaTime;
		}
	}
}
