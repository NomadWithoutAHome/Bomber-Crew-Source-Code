using UnityEngine;

namespace Rewired.Demos;

[AddComponentMenu("")]
[RequireComponent(typeof(CharacterController))]
public class PressStartToJoinExample_GamePlayer : MonoBehaviour
{
	public int gamePlayerId;

	public float moveSpeed = 3f;

	public float bulletSpeed = 15f;

	public GameObject bulletPrefab;

	private CharacterController cc;

	private Vector3 moveVector;

	private bool fire;

	private Player player => PressStartToJoinExample_Assigner.GetRewiredPlayer(gamePlayerId);

	private void OnEnable()
	{
		cc = GetComponent<CharacterController>();
	}

	private void Update()
	{
		if (ReInput.isReady && player != null)
		{
			GetInput();
			ProcessInput();
		}
	}

	private void GetInput()
	{
		moveVector.x = player.GetAxis("Move Horizontal");
		moveVector.y = player.GetAxis("Move Vertical");
		fire = player.GetButtonDown("Fire");
	}

	private void ProcessInput()
	{
		if (moveVector.x != 0f || moveVector.y != 0f)
		{
			cc.Move(moveVector * moveSpeed * Time.deltaTime);
		}
		if (fire)
		{
			GameObject gameObject = Object.Instantiate(bulletPrefab, base.transform.position + base.transform.right, base.transform.rotation);
			gameObject.GetComponent<Rigidbody>().AddForce(base.transform.right * bulletSpeed, ForceMode.VelocityChange);
		}
	}
}
