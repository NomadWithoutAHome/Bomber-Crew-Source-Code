using UnityEngine;

public class UISelectorMovementTypeNone : UISelectorMovementType
{
	private void Start()
	{
	}

	private void OnDestroy()
	{
	}

	public override void SetUp(UISelectFinder finder)
	{
	}

	public override void ForcePointAt(GameObject go, int cameraLayer)
	{
	}

	public override void DeSelect()
	{
	}

	public override void ForcePointAt(tk2dUIItem target)
	{
	}

	public override void DoMovement(Vector2 absMove, Vector2 tickMove)
	{
	}

	public override void UpdateLogic()
	{
	}

	public override tk2dUIItem GetCurrentlyPointedAtItem()
	{
		return null;
	}

	public override Vector2 GetCurrentScreenSpacePointerPosition()
	{
		return Vector3.zero;
	}

	public override bool UseScreenSpacePointerPosition()
	{
		return false;
	}
}
