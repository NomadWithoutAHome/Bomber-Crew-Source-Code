using System.Collections.Generic;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

public class TankController : MonoBehaviour
{
	public float TrailMaterialOffsetSpeed;

	public float MoveSpeed;

	public float MoveFriction;

	public float MoveAcceleration;

	public float RotateSpeed;

	public float RotateFriction;

	public float RotateAcceleration;

	public Material TrailMaterial;

	public Animator Animator;

	public List<Trail> TankTrackTrails;

	public TankWeaponController WeaponController;

	private float _moveSpeed;

	private float _rotateSpeed;

	public bool InControl;

	private void Update()
	{
		Animator.SetBool("InControl", InControl);
		if (InControl)
		{
			WeaponController.enabled = true;
			if (Input.GetKey(KeyCode.W))
			{
				Animator.SetBool("Forward", value: true);
				Animator.SetBool("Backward", value: false);
				_moveSpeed += MoveAcceleration * Time.deltaTime * 2f;
				if (_moveSpeed > MoveSpeed)
				{
					_moveSpeed = MoveSpeed;
				}
			}
			else if (Input.GetKey(KeyCode.S))
			{
				Animator.SetBool("Backward", value: true);
				Animator.SetBool("Forward", value: false);
				_moveSpeed -= MoveAcceleration * Time.deltaTime * 2f;
				if (_moveSpeed < 0f - MoveSpeed)
				{
					_moveSpeed = 0f - MoveSpeed;
				}
			}
			else
			{
				Animator.SetBool("Backward", value: false);
				Animator.SetBool("Forward", value: false);
			}
			if (Input.GetKey(KeyCode.D))
			{
				_rotateSpeed += RotateAcceleration * Time.deltaTime * 2f;
				if (_rotateSpeed > RotateSpeed)
				{
					_rotateSpeed = RotateSpeed;
				}
			}
			else if (Input.GetKey(KeyCode.A))
			{
				_rotateSpeed -= RotateAcceleration * Time.deltaTime * 2f;
				if (_rotateSpeed < 0f - RotateSpeed)
				{
					_rotateSpeed = 0f - RotateSpeed;
				}
			}
		}
		else
		{
			WeaponController.enabled = false;
		}
		if (Mathf.Abs(_moveSpeed) > 0f)
		{
			TankTrackTrails.ForEach(delegate(Trail trail)
			{
				trail.Emit = true;
			});
		}
		else
		{
			TankTrackTrails.ForEach(delegate(Trail trail)
			{
				trail.Emit = false;
			});
		}
		base.transform.position += base.transform.forward * _moveSpeed * Time.deltaTime;
		base.transform.RotateAround(base.transform.position, base.transform.up, _rotateSpeed);
		TrailMaterial.mainTextureOffset = new Vector2(TrailMaterial.mainTextureOffset.x + Mathf.Sign(_moveSpeed) * Mathf.Lerp(0f, TrailMaterialOffsetSpeed, Mathf.Abs(_moveSpeed / MoveSpeed) + Mathf.Abs(_rotateSpeed / RotateSpeed)), TrailMaterial.mainTextureOffset.y);
		_moveSpeed = Mathf.MoveTowards(_moveSpeed, 0f, MoveFriction * Time.deltaTime);
		_rotateSpeed = Mathf.MoveTowards(_rotateSpeed, 0f, RotateFriction * Time.deltaTime);
	}
}
