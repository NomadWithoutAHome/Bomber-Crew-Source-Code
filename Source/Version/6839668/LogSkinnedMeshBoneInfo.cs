using UnityEngine;

[ExecuteInEditMode]
public class LogSkinnedMeshBoneInfo : MonoBehaviour
{
	public bool m_log;

	private void Update()
	{
		if (!m_log)
		{
			return;
		}
		SkinnedMeshRenderer component = base.gameObject.GetComponent<SkinnedMeshRenderer>();
		string text = "Bones for " + component.name;
		text = text + "\nBone Count: " + component.bones.Length;
		if (component.bones.Length > 0)
		{
			for (int i = 0; i < component.bones.Length; i++)
			{
				text = text + "\n" + component.bones[i].gameObject.name;
			}
		}
		m_log = false;
	}
}
