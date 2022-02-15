using System.Collections.Generic;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
	public List<TrailRenderer_Base> Trails;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			Trails.ForEach(delegate(TrailRenderer_Base a)
			{
				a.Emit = true;
			});
			base.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 0.01f));
		}
		else
		{
			Trails.ForEach(delegate(TrailRenderer_Base a)
			{
				a.Emit = false;
			});
		}
	}
}
