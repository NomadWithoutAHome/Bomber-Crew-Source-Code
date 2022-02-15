using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.Demos;

[AddComponentMenu("")]
public class SimpleControlRemapping : MonoBehaviour
{
	private class Row
	{
		public InputAction action;

		public AxisRange actionRange;

		public Button button;

		public Text text;
	}

	private const string category = "Default";

	private const string layout = "Default";

	private InputMapper inputMapper = new InputMapper();

	public GameObject buttonPrefab;

	public GameObject textPrefab;

	public RectTransform fieldGroupTransform;

	public RectTransform actionGroupTransform;

	public Text controllerNameUIText;

	public Text statusUIText;

	private ControllerType selectedControllerType;

	private int selectedControllerId;

	private List<Row> rows = new List<Row>();

	private Player player => ReInput.players.GetPlayer(0);

	private ControllerMap controllerMap
	{
		get
		{
			if (controller == null)
			{
				return null;
			}
			return player.controllers.maps.GetMap(controller.type, controller.id, "Default", "Default");
		}
	}

	private Controller controller => player.controllers.GetController(selectedControllerType, selectedControllerId);

	private void OnEnable()
	{
		if (ReInput.isReady)
		{
			inputMapper.options.timeout = 5f;
			inputMapper.options.ignoreMouseXAxis = true;
			inputMapper.options.ignoreMouseYAxis = true;
			ReInput.ControllerConnectedEvent += OnControllerChanged;
			ReInput.ControllerDisconnectedEvent += OnControllerChanged;
			inputMapper.InputMappedEvent += OnInputMapped;
			inputMapper.StoppedEvent += OnStopped;
			InitializeUI();
		}
	}

	private void OnDisable()
	{
		inputMapper.Stop();
		inputMapper.RemoveAllEventListeners();
		ReInput.ControllerConnectedEvent -= OnControllerChanged;
		ReInput.ControllerDisconnectedEvent -= OnControllerChanged;
	}

	private void RedrawUI()
	{
		if (controller == null)
		{
			ClearUI();
			return;
		}
		controllerNameUIText.text = controller.name;
		for (int i = 0; i < rows.Count; i++)
		{
			Row row = rows[i];
			InputAction action = rows[i].action;
			string text = string.Empty;
			int actionElementMapId = -1;
			foreach (ActionElementMap item in controllerMap.ElementMapsWithAction(action.id))
			{
				if (item.ShowInField(row.actionRange))
				{
					text = item.elementIdentifierName;
					actionElementMapId = item.id;
					break;
				}
			}
			row.text.text = text;
			row.button.onClick.RemoveAllListeners();
			int index = i;
			row.button.onClick.AddListener(delegate
			{
				OnInputFieldClicked(index, actionElementMapId);
			});
		}
	}

	private void ClearUI()
	{
		if (selectedControllerType == ControllerType.Joystick)
		{
			controllerNameUIText.text = "No joysticks attached";
		}
		else
		{
			controllerNameUIText.text = string.Empty;
		}
		for (int i = 0; i < rows.Count; i++)
		{
			rows[i].text.text = string.Empty;
		}
	}

	private void InitializeUI()
	{
		foreach (Transform item in actionGroupTransform)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in fieldGroupTransform)
		{
			Object.Destroy(item2.gameObject);
		}
		foreach (InputAction action in ReInput.mapping.Actions)
		{
			if (action.type == InputActionType.Axis)
			{
				CreateUIRow(action, AxisRange.Full, action.descriptiveName);
				CreateUIRow(action, AxisRange.Positive, string.IsNullOrEmpty(action.positiveDescriptiveName) ? (action.descriptiveName + " +") : action.positiveDescriptiveName);
				CreateUIRow(action, AxisRange.Negative, string.IsNullOrEmpty(action.negativeDescriptiveName) ? (action.descriptiveName + " -") : action.negativeDescriptiveName);
			}
			else if (action.type == InputActionType.Button)
			{
				CreateUIRow(action, AxisRange.Positive, action.descriptiveName);
			}
		}
		RedrawUI();
	}

	private void CreateUIRow(InputAction action, AxisRange actionRange, string label)
	{
		GameObject gameObject = Object.Instantiate(textPrefab);
		gameObject.transform.SetParent(actionGroupTransform);
		gameObject.transform.SetAsLastSibling();
		gameObject.GetComponent<Text>().text = label;
		GameObject gameObject2 = Object.Instantiate(buttonPrefab);
		gameObject2.transform.SetParent(fieldGroupTransform);
		gameObject2.transform.SetAsLastSibling();
		rows.Add(new Row
		{
			action = action,
			actionRange = actionRange,
			button = gameObject2.GetComponent<Button>(),
			text = gameObject2.GetComponentInChildren<Text>()
		});
	}

	private void SetSelectedController(ControllerType controllerType)
	{
		bool flag = false;
		if (controllerType != selectedControllerType)
		{
			selectedControllerType = controllerType;
			flag = true;
		}
		int num = selectedControllerId;
		if (selectedControllerType == ControllerType.Joystick)
		{
			if (player.controllers.joystickCount > 0)
			{
				selectedControllerId = player.controllers.Joysticks[0].id;
			}
			else
			{
				selectedControllerId = -1;
			}
		}
		else
		{
			selectedControllerId = 0;
		}
		if (selectedControllerId != num)
		{
			flag = true;
		}
		if (flag)
		{
			inputMapper.Stop();
			RedrawUI();
		}
	}

	public void OnControllerSelected(int controllerType)
	{
		SetSelectedController((ControllerType)controllerType);
	}

	private void OnInputFieldClicked(int index, int actionElementMapToReplaceId)
	{
		if (index >= 0 && index < rows.Count && controller != null)
		{
			inputMapper.Start(new InputMapper.Context
			{
				actionId = rows[index].action.id,
				controllerMap = controllerMap,
				actionRange = rows[index].actionRange,
				actionElementMapToReplace = controllerMap.GetElementMap(actionElementMapToReplaceId)
			});
			statusUIText.text = "Listening...";
		}
	}

	private void OnControllerChanged(ControllerStatusChangedEventArgs args)
	{
		SetSelectedController(selectedControllerType);
	}

	private void OnInputMapped(InputMapper.InputMappedEventData data)
	{
		RedrawUI();
	}

	private void OnStopped(InputMapper.StoppedEventData data)
	{
		statusUIText.text = string.Empty;
	}
}
