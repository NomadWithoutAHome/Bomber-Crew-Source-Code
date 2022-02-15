using UnityEngine;

public class DisableParticleMotionVectors : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Renderer>().motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
	}
}
