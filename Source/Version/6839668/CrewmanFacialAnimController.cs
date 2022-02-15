using System;
using System.Collections;
using UnityEngine;

public class CrewmanFacialAnimController : MonoBehaviour
{
	[Serializable]
	private struct Vector2Int
	{
		public int x;

		public int y;

		public Vector2Int(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	[SerializeField]
	private Renderer m_renderer;

	[SerializeField]
	private bool m_useRealtimeWaitForSeconds;

	[SerializeField]
	private int m_eyesMaterialIndex;

	[SerializeField]
	private Vector2Int m_eyesTextureLayout = new Vector2Int(8, 16);

	[SerializeField]
	private int m_blinkIndex = 1;

	private Vector2 m_blinkTime = new Vector2(0.15f, 0.5f);

	private Vector2 m_blinkIntervalTime = new Vector2(1f, 5f);

	[SerializeField]
	private int m_lookLeftIndex = 3;

	[SerializeField]
	private int m_lookRightIndex = 2;

	[SerializeField]
	private int m_lookUpIndex = 5;

	[SerializeField]
	private int m_eyesWideIndex = 4;

	[SerializeField]
	private int m_eyesClenchedIndex = 7;

	[SerializeField]
	private int m_eyesFrownIndex = 6;

	[SerializeField]
	private int m_mouthMaterialIndex;

	[SerializeField]
	private Vector2Int m_mouthTextureLayout = new Vector2Int(8, 16);

	[SerializeField]
	private int m_mouthTalkIndex = 1;

	[SerializeField]
	private int m_mouthSmileIndex = 2;

	[SerializeField]
	private int m_mouthFrownIndex = 3;

	[SerializeField]
	private int m_mouthShockIndex = 4;

	[SerializeField]
	private int m_mouthClenchTeethIndex = 7;

	private void Start()
	{
		StartCoroutine("PlayBaseBlink");
	}

	public void SetUseRealtimeWaitForSeconds(bool useRealtime)
	{
		m_useRealtimeWaitForSeconds = useRealtime;
	}

	private IEnumerator PlayBaseBlink()
	{
		while (true)
		{
			yield return DoWaitForSeconds(UnityEngine.Random.Range(m_blinkIntervalTime.x, m_blinkIntervalTime.y));
			SetEyesTextureOffsetToIndex(m_blinkIndex);
			yield return DoWaitForSeconds(UnityEngine.Random.Range(m_blinkTime.x, m_blinkTime.y));
			SetEyesTextureOffsetToIndex(0);
		}
	}

	public void Blink(float time)
	{
		StopAllCoroutines();
		StartCoroutine(PlayExpressionEyes(m_blinkIndex, time));
	}

	public void LookLeft(float time)
	{
		StopAllCoroutines();
		StartCoroutine(PlayExpressionWithBlinks(m_lookLeftIndex, time));
	}

	public void LookRight(float time)
	{
		StopAllCoroutines();
		StartCoroutine(PlayExpressionWithBlinks(m_lookRightIndex, time));
	}

	public void LookUp(float time)
	{
		StopAllCoroutines();
		StartCoroutine(PlayExpressionWithBlinks(m_lookUpIndex, time));
	}

	public void Grimace(float time)
	{
		StopAllCoroutines();
		StartCoroutine(PlayGrimace(time));
	}

	public void Frown(float time)
	{
		StopAllCoroutines();
		StartCoroutine(PlayFrown(time));
	}

	public void EyesWide(float time)
	{
		StopAllCoroutines();
		StartCoroutine(PlayEyesWide(time));
	}

	public void Talk(float time)
	{
		StopAllCoroutines();
		StartCoroutine(PlayTalk(time));
	}

	public void Smile(float time)
	{
		StopAllCoroutines();
		StartCoroutine(PlaySmile(time));
	}

	public void Scowl(float time)
	{
		StopAllCoroutines();
		StartCoroutine(PlayScowl(time));
	}

	private IEnumerator DoWaitForSeconds(float time)
	{
		if (m_useRealtimeWaitForSeconds)
		{
			yield return new WaitForSecondsRealtime(time);
		}
		else
		{
			yield return new WaitForSeconds(time);
		}
		yield return null;
	}

	private IEnumerator PlayExpressionEyes(int index, float time)
	{
		SetEyesTextureOffsetToIndex(index);
		yield return DoWaitForSeconds(time);
		SetEyesTextureOffsetToIndex(0);
		StartCoroutine("PlayBaseBlink");
		yield return null;
	}

	private IEnumerator PlayExpressionWithBlinks(int index, float time)
	{
		SetEyesTextureOffsetToIndex(m_blinkIndex);
		yield return DoWaitForSeconds(0.15f);
		SetEyesTextureOffsetToIndex(index);
		yield return DoWaitForSeconds(time);
		SetEyesTextureOffsetToIndex(m_blinkIndex);
		yield return DoWaitForSeconds(0.1f);
		SetEyesTextureOffsetToIndex(0);
		StartCoroutine("PlayBaseBlink");
		yield return null;
	}

	public void SetDead()
	{
		StopAllCoroutines();
		SetEyesTextureOffsetToIndex(m_blinkIndex);
	}

	private IEnumerator PlayGrimace(float time)
	{
		SetEyesTextureOffsetToIndex(m_eyesClenchedIndex);
		SetMouthTextureOffsetToIndex(m_mouthClenchTeethIndex);
		yield return DoWaitForSeconds(time);
		SetEyesTextureOffsetToIndex(m_eyesFrownIndex);
		yield return DoWaitForSeconds(0.033f);
		SetEyesTextureOffsetToIndex(0);
		SetMouthTextureOffsetToIndex(0);
		StartCoroutine("PlayBaseBlink");
		yield return null;
	}

	private IEnumerator PlayFrown(float time)
	{
		SetEyesTextureOffsetToIndex(m_eyesFrownIndex);
		SetMouthTextureOffsetToIndex(m_mouthFrownIndex);
		yield return DoWaitForSeconds(time);
		SetEyesTextureOffsetToIndex(0);
		SetMouthTextureOffsetToIndex(0);
		StartCoroutine("PlayBaseBlink");
		yield return null;
	}

	private IEnumerator PlayEyesWide(float time)
	{
		SetEyesTextureOffsetToIndex(m_eyesWideIndex);
		yield return DoWaitForSeconds(time);
		SetEyesTextureOffsetToIndex(0);
		StartCoroutine("PlayBaseBlink");
		yield return null;
	}

	private IEnumerator PlayTalk(float time)
	{
		SetEyesTextureOffsetToIndex(0);
		float counter = time;
		bool mouthOpen = false;
		float mouthTime2 = 0f;
		while (counter > 0f)
		{
			if (!mouthOpen)
			{
				SetMouthTextureOffsetToIndex(m_mouthTalkIndex);
				mouthOpen = true;
			}
			else
			{
				SetMouthTextureOffsetToIndex(0);
				mouthOpen = false;
			}
			mouthTime2 = UnityEngine.Random.Range(0.05f, 0.2f);
			yield return DoWaitForSeconds(mouthTime2);
			counter -= mouthTime2;
			yield return null;
		}
		SetMouthTextureOffsetToIndex(0);
		StartCoroutine("PlayBaseBlink");
		yield return null;
	}

	private IEnumerator PlaySmile(float time)
	{
		SetEyesTextureOffsetToIndex(m_eyesWideIndex);
		SetMouthTextureOffsetToIndex(m_mouthSmileIndex);
		yield return DoWaitForSeconds(time);
		SetMouthTextureOffsetToIndex(0);
		SetEyesTextureOffsetToIndex(0);
		StartCoroutine("PlayBaseBlink");
		yield return null;
	}

	private IEnumerator PlayScowl(float time)
	{
		SetEyesTextureOffsetToIndex(m_eyesFrownIndex);
		SetMouthTextureOffsetToIndex(m_mouthFrownIndex);
		yield return DoWaitForSeconds(time);
		SetMouthTextureOffsetToIndex(0);
		SetEyesTextureOffsetToIndex(0);
		StartCoroutine("PlayBaseBlink");
		yield return null;
	}

	private void SetEyesTextureOffsetToIndex(int index)
	{
		Vector2 textureOffset = m_renderer.materials[m_eyesMaterialIndex].GetTextureOffset("_MainTex");
		textureOffset.x = 1f / (float)m_eyesTextureLayout.x * (float)index;
		m_renderer.materials[m_eyesMaterialIndex].SetTextureOffset("_MainTex", textureOffset);
	}

	private void SetMouthTextureOffsetToIndex(int index)
	{
		Vector2 textureOffset = m_renderer.materials[m_mouthMaterialIndex].GetTextureOffset("_MainTex");
		textureOffset.x = 1f / (float)m_mouthTextureLayout.x * (float)index;
		m_renderer.materials[m_mouthMaterialIndex].SetTextureOffset("_MainTex", textureOffset);
	}

	public void SetStatue()
	{
		StopCoroutine("PlayBaseBlink");
		base.enabled = false;
	}

	public void SetRenderer(Renderer r)
	{
		m_renderer = r;
	}
}
