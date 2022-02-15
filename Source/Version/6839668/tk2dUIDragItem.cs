using UnityEngine;

[AddComponentMenu("2D Toolkit/UI/tk2dUIDragItem")]
public class tk2dUIDragItem : tk2dUIBaseItemControl
{
	public tk2dUIManager uiManager;

	private Vector3 offset = Vector3.zero;

	private bool isBtnActive;

	private void OnEnable()
	{
		if ((bool)uiItem)
		{
			uiItem.OnDown += ButtonDown;
			uiItem.OnRelease += ButtonRelease;
		}
	}

	private void OnDisable()
	{
		if ((bool)uiItem)
		{
			uiItem.OnDown -= ButtonDown;
			uiItem.OnRelease -= ButtonRelease;
		}
		if (isBtnActive)
		{
			if (tk2dUIManager.Instance__NoCreate != null)
			{
				tk2dUIManager.Instance.OnInputUpdate -= UpdateBtnPosition;
			}
			isBtnActive = false;
		}
	}

	private void UpdateBtnPosition()
	{
		base.transform.position = CalculateNewPos();
	}

	private Vector3 CalculateNewPos()
	{
		Vector2 position = uiItem.Touch.position;
		Camera uICameraForControl = tk2dUIManager.Instance.GetUICameraForControl(base.gameObject);
		Vector3 vector = uICameraForControl.ScreenToWorldPoint(new Vector3(position.x, position.y, base.transform.position.z - uICameraForControl.transform.position.z));
		vector.z = base.transform.position.z;
		return vector + offset;
	}

	public void ButtonDown()
	{
		if (!isBtnActive)
		{
			tk2dUIManager.Instance.OnInputUpdate += UpdateBtnPosition;
		}
		isBtnActive = true;
		offset = Vector3.zero;
		Vector3 vector = CalculateNewPos();
		offset = base.transform.position - vector;
	}

	public void ButtonRelease()
	{
		if (isBtnActive)
		{
			tk2dUIManager.Instance.OnInputUpdate -= UpdateBtnPosition;
		}
		isBtnActive = false;
	}
}
