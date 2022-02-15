using UnityEngine;

public abstract class UISelectorMovementType : MonoBehaviour
{
	public abstract tk2dUIItem GetCurrentlyPointedAtItem();

	public abstract Vector2 GetCurrentScreenSpacePointerPosition();

	public abstract bool UseScreenSpacePointerPosition();

	public abstract void ForcePointAt(tk2dUIItem target);

	public abstract void ForcePointAt(GameObject go, int cameraLayer);

	public abstract void DoMovement(Vector2 absMove, Vector2 tickMove);

	public abstract void UpdateLogic();

	public abstract void SetUp(UISelectFinder finder);

	public abstract void DeSelect();

	public bool IsValid(tk2dUIItem item)
	{
		bool flag = item != null && item.enabled && item.gameObject.activeInHierarchy && item.GetComponent<Collider>() != null && item.GetComponent<Collider>().enabled;
		if (flag)
		{
			UISelectorPointingHint component = item.GetComponent<UISelectorPointingHint>();
			if (component != null && component.IsSelectBlocked())
			{
				flag = false;
			}
		}
		return flag;
	}
}
