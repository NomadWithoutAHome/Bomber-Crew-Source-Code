using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

public class TankTranslator : MonoBehaviour
{
	public float TranslateDistance;

	public bool TrailTranslationEnabled;

	private void Update()
	{
		Vector3 vector = Vector3.zero;
		if (Input.GetKeyDown(KeyCode.A))
		{
			vector = base.transform.right * TranslateDistance;
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			vector = -base.transform.right * TranslateDistance;
		}
		if (!(vector != Vector3.zero))
		{
			return;
		}
		base.transform.Translate(vector);
		if (TrailTranslationEnabled)
		{
			TrailRenderer_Base[] componentsInChildren = GetComponentsInChildren<TrailRenderer_Base>();
			foreach (TrailRenderer_Base trailRenderer_Base in componentsInChildren)
			{
				trailRenderer_Base.Translate(vector);
			}
		}
	}
}
