using System;
using UnityEngine;

[AddComponentMenu("2D Toolkit/UI/tk2dUIProgressBar")]
public class tk2dUIProgressBar : MonoBehaviour
{
	public Transform scalableBar;

	public tk2dClippedSprite clippedSpriteBar;

	public bool vertical;

	public tk2dSlicedSprite slicedSpriteBar;

	private bool initializedSlicedSpriteDimensions;

	private Vector2 emptySlicedSpriteDimensions = Vector2.zero;

	private Vector2 fullSlicedSpriteDimensions = Vector2.zero;

	private Vector2 currentDimensions = Vector2.zero;

	[SerializeField]
	private float percent;

	private bool isProgressComplete;

	public GameObject sendMessageTarget;

	public string SendMessageOnProgressCompleteMethodName = string.Empty;

	private bool runTimeSetUp;

	private bool hasClippedBar;

	private bool hasScaleBar;

	private bool hasSlicedBar;

	private bool hasEverSetUp;

	public float Value
	{
		get
		{
			return percent;
		}
		set
		{
			float num = Mathf.Clamp(value, 0f, 1f);
			if (num == percent && hasEverSetUp)
			{
				return;
			}
			hasEverSetUp = true;
			percent = num;
			if (!Application.isPlaying)
			{
				return;
			}
			if (!runTimeSetUp)
			{
				hasClippedBar = clippedSpriteBar != null;
				hasScaleBar = scalableBar != null;
				hasSlicedBar = slicedSpriteBar != null;
				runTimeSetUp = true;
			}
			if (hasClippedBar)
			{
				if (vertical)
				{
					clippedSpriteBar.clipTopRight = new Vector2(1f, Value);
				}
				else
				{
					clippedSpriteBar.clipTopRight = new Vector2(Value, 1f);
				}
			}
			else if (hasScaleBar)
			{
				scalableBar.localScale = new Vector3(Value, scalableBar.localScale.y, scalableBar.localScale.z);
			}
			else if (hasSlicedBar)
			{
				InitializeSlicedSpriteDimensions();
				float newX = Mathf.Lerp(emptySlicedSpriteDimensions.x, fullSlicedSpriteDimensions.x, Value);
				currentDimensions.Set(newX, fullSlicedSpriteDimensions.y);
				slicedSpriteBar.dimensions = currentDimensions;
			}
			if (!isProgressComplete && Value == 1f)
			{
				isProgressComplete = true;
				if (this.OnProgressComplete != null)
				{
					this.OnProgressComplete();
				}
				if (sendMessageTarget != null && SendMessageOnProgressCompleteMethodName.Length > 0)
				{
					sendMessageTarget.SendMessage(SendMessageOnProgressCompleteMethodName, this, SendMessageOptions.RequireReceiver);
				}
			}
			else if (isProgressComplete && Value < 1f)
			{
				isProgressComplete = false;
			}
		}
	}

	public event Action OnProgressComplete;

	private void Start()
	{
		InitializeSlicedSpriteDimensions();
		Value = percent;
	}

	private void InitializeSlicedSpriteDimensions()
	{
		if (!initializedSlicedSpriteDimensions)
		{
			if (slicedSpriteBar != null)
			{
				tk2dSpriteDefinition currentSprite = slicedSpriteBar.CurrentSprite;
				Vector3 vector = currentSprite.boundsData[1];
				fullSlicedSpriteDimensions = slicedSpriteBar.dimensions;
				emptySlicedSpriteDimensions.Set((slicedSpriteBar.borderLeft + slicedSpriteBar.borderRight) * vector.x / currentSprite.texelSize.x, fullSlicedSpriteDimensions.y);
			}
			initializedSlicedSpriteDimensions = true;
		}
	}
}
