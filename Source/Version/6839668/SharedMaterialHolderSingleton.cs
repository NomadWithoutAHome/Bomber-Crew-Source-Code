using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class SharedMaterialHolderSingleton : Singleton<SharedMaterialHolderSingleton>
{
	private Dictionary<string, Material> m_cachedMaterials = new Dictionary<string, Material>();

	public Material GetFor(Material inMat)
	{
		Material value = null;
		m_cachedMaterials.TryGetValue(inMat.name, out value);
		if (value == null)
		{
			value = Object.Instantiate(inMat);
			value.name = inMat.name + "CACHED_MAT_" + m_cachedMaterials.Count;
			m_cachedMaterials[inMat.name] = value;
		}
		return value;
	}
}
