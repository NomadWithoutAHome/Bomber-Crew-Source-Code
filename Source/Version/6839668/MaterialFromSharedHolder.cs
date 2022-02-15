using System.Collections.Generic;
using UnityEngine;

public class MaterialFromSharedHolder : MonoBehaviour
{
	[SerializeField]
	private SharedMaterialHolder m_sharedMaterialHolder;

	private bool m_initialised;

	private void Init()
	{
		if (!m_initialised)
		{
			Renderer component = GetComponent<Renderer>();
			List<Material> list = new List<Material>();
			for (int i = 0; i < component.sharedMaterials.Length; i++)
			{
				Material @for = m_sharedMaterialHolder.GetFor(component.sharedMaterials[i]);
				list.Add(@for);
			}
			component.sharedMaterials = list.ToArray();
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			materialPropertyBlock.SetColor("_FlashAddition", new Color(0f, 0f, 0f, 0f));
			component.SetPropertyBlock(materialPropertyBlock);
		}
	}

	private void Start()
	{
		Init();
	}
}
