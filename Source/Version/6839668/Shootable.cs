using UnityEngine;

public interface Shootable
{
	Vector3 GetVelocity();

	Transform GetRandomTargetableArea();

	Transform GetCentreTransform();

	ShootableType GetShootableType();

	GameObject GetMainObject();

	bool IsEvasive();

	bool IsLitUp();

	bool IsDestroyed();
}
