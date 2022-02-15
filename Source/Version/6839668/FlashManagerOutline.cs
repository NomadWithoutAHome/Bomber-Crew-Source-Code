using UnityEngine;

public class FlashManagerOutline : FlashManager
{
	[SerializeField]
	private OutlineMesh[] m_outlinesToFlash;

	protected override void SetColor(Color c)
	{
		OutlineMesh[] outlinesToFlash = m_outlinesToFlash;
		foreach (OutlineMesh outlineMesh in outlinesToFlash)
		{
			if (c.r > 0f || c.g > 0f || c.b > 0f)
			{
				outlineMesh.GetClonedRenderer().enabled = true;
				Material[] sharedMaterials = outlineMesh.GetClonedRenderer().sharedMaterials;
				foreach (Material material in sharedMaterials)
				{
					material.SetColor("_Color", c);
				}
			}
			else
			{
				outlineMesh.GetClonedRenderer().enabled = false;
			}
		}
	}
}
