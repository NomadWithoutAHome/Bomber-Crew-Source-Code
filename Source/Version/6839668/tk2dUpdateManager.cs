using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
public class tk2dUpdateManager : MonoBehaviour
{
	private static tk2dUpdateManager inst;

	[SerializeField]
	private List<tk2dTextMesh> textMeshes = new List<tk2dTextMesh>(64);

	private static tk2dUpdateManager Instance
	{
		get
		{
			if (inst == null)
			{
				inst = Object.FindObjectOfType(typeof(tk2dUpdateManager)) as tk2dUpdateManager;
				if (inst == null)
				{
					GameObject gameObject = new GameObject("@tk2dUpdateManager");
					gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
					inst = gameObject.AddComponent<tk2dUpdateManager>();
					Object.DontDestroyOnLoad(gameObject);
				}
			}
			return inst;
		}
	}

	public static void QueueCommit(tk2dTextMesh textMesh)
	{
		Instance.QueueCommitInternal(textMesh);
	}

	public static void FlushQueues()
	{
		Instance.FlushQueuesInternal();
	}

	private void OnEnable()
	{
		StartCoroutine(coSuperLateUpdate());
	}

	private void LateUpdate()
	{
		FlushQueuesInternal();
	}

	private IEnumerator coSuperLateUpdate()
	{
		FlushQueuesInternal();
		yield break;
	}

	private void QueueCommitInternal(tk2dTextMesh textMesh)
	{
		textMeshes.Add(textMesh);
	}

	private void FlushQueuesInternal()
	{
		int count = textMeshes.Count;
		for (int i = 0; i < count; i++)
		{
			tk2dTextMesh tk2dTextMesh2 = textMeshes[i];
			if (tk2dTextMesh2 != null)
			{
				tk2dTextMesh2.DoNotUse__CommitInternal();
			}
		}
		textMeshes.Clear();
	}
}
