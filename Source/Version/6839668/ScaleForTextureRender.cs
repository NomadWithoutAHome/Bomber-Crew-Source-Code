using UnityEngine;

public class ScaleForTextureRender : MonoBehaviour
{
	private void Update()
	{
		base.transform.localScale = new Vector3((float)Screen.width / (float)Screen.height, 1f, 1f);
	}
}
