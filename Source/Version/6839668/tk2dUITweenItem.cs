using System.Collections;
using UnityEngine;

[AddComponentMenu("2D Toolkit/UI/tk2dUITweenItem")]
public class tk2dUITweenItem : tk2dUIBaseItemControl
{
	private Vector3 onUpScale;

	public Vector3 onDownScale = new Vector3(0.9f, 0.9f, 0.9f);

	public float tweenDuration = 0.1f;

	public bool canButtonBeHeldDown = true;

	[SerializeField]
	private bool useOnReleaseInsteadOfOnUp;

	private bool internalTweenInProgress;

	private Vector3 tweenTargetScale = Vector3.one;

	private Vector3 tweenStartingScale = Vector3.one;

	private float tweenTimeElapsed;

	public bool UseOnReleaseInsteadOfOnUp => useOnReleaseInsteadOfOnUp;

	private void Awake()
	{
		onUpScale = base.transform.localScale;
	}

	private void OnEnable()
	{
		if ((bool)uiItem)
		{
			uiItem.OnDown += ButtonDown;
			if (canButtonBeHeldDown)
			{
				if (useOnReleaseInsteadOfOnUp)
				{
					uiItem.OnRelease += ButtonUp;
				}
				else
				{
					uiItem.OnUp += ButtonUp;
				}
			}
		}
		internalTweenInProgress = false;
		tweenTimeElapsed = 0f;
		base.transform.localScale = onUpScale;
	}

	private void OnDisable()
	{
		if (!uiItem)
		{
			return;
		}
		uiItem.OnDown -= ButtonDown;
		if (canButtonBeHeldDown)
		{
			if (useOnReleaseInsteadOfOnUp)
			{
				uiItem.OnRelease -= ButtonUp;
			}
			else
			{
				uiItem.OnUp -= ButtonUp;
			}
		}
	}

	private void ButtonDown()
	{
		if (tweenDuration <= 0f)
		{
			base.transform.localScale = onDownScale;
			return;
		}
		base.transform.localScale = onUpScale;
		tweenTargetScale = onDownScale;
		tweenStartingScale = base.transform.localScale;
		if (!internalTweenInProgress)
		{
			StartCoroutine(ScaleTween());
			internalTweenInProgress = true;
		}
	}

	private void ButtonUp()
	{
		if (tweenDuration <= 0f)
		{
			base.transform.localScale = onUpScale;
			return;
		}
		tweenTargetScale = onUpScale;
		tweenStartingScale = base.transform.localScale;
		if (!internalTweenInProgress)
		{
			StartCoroutine(ScaleTween());
			internalTweenInProgress = true;
		}
	}

	private IEnumerator ScaleTween()
	{
		for (tweenTimeElapsed = 0f; tweenTimeElapsed < tweenDuration; tweenTimeElapsed += tk2dUITime.deltaTime)
		{
			base.transform.localScale = Vector3.Lerp(tweenStartingScale, tweenTargetScale, tweenTimeElapsed / tweenDuration);
			yield return null;
		}
		base.transform.localScale = tweenTargetScale;
		internalTweenInProgress = false;
		if (!canButtonBeHeldDown)
		{
			if (tweenDuration <= 0f)
			{
				base.transform.localScale = onUpScale;
				yield break;
			}
			tweenTargetScale = onUpScale;
			tweenStartingScale = base.transform.localScale;
			StartCoroutine(ScaleTween());
			internalTweenInProgress = true;
		}
	}

	public void InternalSetUseOnReleaseInsteadOfOnUp(bool state)
	{
		useOnReleaseInsteadOfOnUp = state;
	}
}
