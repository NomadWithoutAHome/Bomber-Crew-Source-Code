using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("2D Toolkit/UI/tk2dUIDropDownMenu")]
public class tk2dUIDropDownMenu : MonoBehaviour
{
	public tk2dUIItem dropDownButton;

	public tk2dTextMesh selectedTextMesh;

	[HideInInspector]
	public float height;

	public tk2dUIDropDownItem dropDownItemTemplate;

	[SerializeField]
	private string[] startingItemList;

	[SerializeField]
	private int startingIndex;

	private List<string> itemList = new List<string>();

	public string SendMessageOnSelectedItemChangeMethodName = string.Empty;

	private int index;

	private List<tk2dUIDropDownItem> dropDownItems = new List<tk2dUIDropDownItem>();

	private bool isExpanded;

	[SerializeField]
	[HideInInspector]
	private tk2dUILayout menuLayoutItem;

	[SerializeField]
	[HideInInspector]
	private tk2dUILayout templateLayoutItem;

	public List<string> ItemList
	{
		get
		{
			return itemList;
		}
		set
		{
			itemList = value;
		}
	}

	public int Index
	{
		get
		{
			return index;
		}
		set
		{
			index = Mathf.Clamp(value, 0, ItemList.Count - 1);
			SetSelectedItem();
		}
	}

	public string SelectedItem
	{
		get
		{
			if (index >= 0 && index < itemList.Count)
			{
				return itemList[index];
			}
			return string.Empty;
		}
	}

	public GameObject SendMessageTarget
	{
		get
		{
			if (dropDownButton != null)
			{
				return dropDownButton.sendMessageTarget;
			}
			return null;
		}
		set
		{
			if (dropDownButton != null && dropDownButton.sendMessageTarget != value)
			{
				dropDownButton.sendMessageTarget = value;
			}
		}
	}

	public tk2dUILayout MenuLayoutItem
	{
		get
		{
			return menuLayoutItem;
		}
		set
		{
			menuLayoutItem = value;
		}
	}

	public tk2dUILayout TemplateLayoutItem
	{
		get
		{
			return templateLayoutItem;
		}
		set
		{
			templateLayoutItem = value;
		}
	}

	public event Action OnSelectedItemChange;

	private void Awake()
	{
		string[] array = startingItemList;
		foreach (string item in array)
		{
			itemList.Add(item);
		}
		index = startingIndex;
		dropDownItemTemplate.gameObject.SetActive(value: false);
		UpdateList();
	}

	private void OnEnable()
	{
		dropDownButton.OnDown += ExpandButtonPressed;
	}

	private void OnDisable()
	{
		dropDownButton.OnDown -= ExpandButtonPressed;
	}

	public void UpdateList()
	{
		if (dropDownItems.Count > ItemList.Count)
		{
			for (int i = ItemList.Count; i < dropDownItems.Count; i++)
			{
				dropDownItems[i].gameObject.SetActive(value: false);
			}
		}
		while (dropDownItems.Count < ItemList.Count)
		{
			dropDownItems.Add(CreateAnotherDropDownItem());
		}
		for (int j = 0; j < ItemList.Count; j++)
		{
			tk2dUIDropDownItem tk2dUIDropDownItem2 = dropDownItems[j];
			Vector3 localPosition = tk2dUIDropDownItem2.transform.localPosition;
			if (menuLayoutItem != null && templateLayoutItem != null)
			{
				localPosition.y = menuLayoutItem.bMin.y - (float)j * (templateLayoutItem.bMax.y - templateLayoutItem.bMin.y);
			}
			else
			{
				localPosition.y = 0f - height - (float)j * tk2dUIDropDownItem2.height;
			}
			tk2dUIDropDownItem2.transform.localPosition = localPosition;
			if (tk2dUIDropDownItem2.label != null)
			{
				tk2dUIDropDownItem2.LabelText = itemList[j];
			}
			tk2dUIDropDownItem2.Index = j;
		}
		SetSelectedItem();
	}

	public void SetSelectedItem()
	{
		if (index < 0 || index >= ItemList.Count)
		{
			index = 0;
		}
		if (index >= 0 && index < ItemList.Count)
		{
			selectedTextMesh.text = ItemList[index];
			selectedTextMesh.Commit();
		}
		else
		{
			selectedTextMesh.text = string.Empty;
			selectedTextMesh.Commit();
		}
		if (this.OnSelectedItemChange != null)
		{
			this.OnSelectedItemChange();
		}
		if (SendMessageTarget != null && SendMessageOnSelectedItemChangeMethodName.Length > 0)
		{
			SendMessageTarget.SendMessage(SendMessageOnSelectedItemChangeMethodName, this, SendMessageOptions.RequireReceiver);
		}
	}

	private tk2dUIDropDownItem CreateAnotherDropDownItem()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(dropDownItemTemplate.gameObject);
		gameObject.name = "DropDownItem";
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = dropDownItemTemplate.transform.localPosition;
		gameObject.transform.localRotation = dropDownItemTemplate.transform.localRotation;
		gameObject.transform.localScale = dropDownItemTemplate.transform.localScale;
		tk2dUIDropDownItem component = gameObject.GetComponent<tk2dUIDropDownItem>();
		component.OnItemSelected += ItemSelected;
		(component.upDownHoverBtn = gameObject.GetComponent<tk2dUIUpDownHoverButton>()).OnToggleOver += DropDownItemHoverBtnToggle;
		return component;
	}

	private void ItemSelected(tk2dUIDropDownItem item)
	{
		if (isExpanded)
		{
			CollapseList();
		}
		Index = item.Index;
	}

	private void ExpandButtonPressed()
	{
		if (isExpanded)
		{
			CollapseList();
		}
		else
		{
			ExpandList();
		}
	}

	private void ExpandList()
	{
		isExpanded = true;
		int num = Mathf.Min(ItemList.Count, dropDownItems.Count);
		for (int i = 0; i < num; i++)
		{
			dropDownItems[i].gameObject.SetActive(value: true);
		}
		tk2dUIDropDownItem tk2dUIDropDownItem2 = dropDownItems[index];
		if (tk2dUIDropDownItem2.upDownHoverBtn != null)
		{
			tk2dUIDropDownItem2.upDownHoverBtn.IsOver = true;
		}
	}

	private void CollapseList()
	{
		isExpanded = false;
		foreach (tk2dUIDropDownItem dropDownItem in dropDownItems)
		{
			dropDownItem.gameObject.SetActive(value: false);
		}
	}

	private void DropDownItemHoverBtnToggle(tk2dUIUpDownHoverButton upDownHoverButton)
	{
		if (!upDownHoverButton.IsOver)
		{
			return;
		}
		foreach (tk2dUIDropDownItem dropDownItem in dropDownItems)
		{
			if (dropDownItem.upDownHoverBtn != upDownHoverButton && dropDownItem.upDownHoverBtn != null)
			{
				dropDownItem.upDownHoverBtn.IsOver = false;
			}
		}
	}

	private void OnDestroy()
	{
		foreach (tk2dUIDropDownItem dropDownItem in dropDownItems)
		{
			dropDownItem.OnItemSelected -= ItemSelected;
			if (dropDownItem.upDownHoverBtn != null)
			{
				dropDownItem.upDownHoverBtn.OnToggleOver -= DropDownItemHoverBtnToggle;
			}
		}
	}
}
