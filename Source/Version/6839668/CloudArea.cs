using System.Globalization;
using BomberCrewCommon;

public class CloudArea : MissionPlaceableObject
{
	public void Start()
	{
		float density = float.Parse(GetParameter("density"), CultureInfo.InvariantCulture);
		float num = float.Parse(GetParameter("radius"), CultureInfo.InvariantCulture);
		bool rain = GetParameter("rain") == "true";
		bool snow = GetParameter("snow") == "true";
		if (num == 0f)
		{
			Singleton<CloudGrid>.Instance.RegisterOverall(density, rain, snow);
		}
		else
		{
			Singleton<CloudGrid>.Instance.RegisterArea(base.transform.position, num, density, rain, snow);
		}
	}
}
