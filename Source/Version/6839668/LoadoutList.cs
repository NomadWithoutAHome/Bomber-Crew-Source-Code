using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class LoadoutList
{
	[JsonObject(MemberSerialization.OptIn)]
	public class LoadoutItem
	{
		[JsonProperty]
		public string m_loadoutTypeReference;

		[JsonProperty]
		public float m_condition;
	}

	[JsonProperty]
	public List<LoadoutItem> m_loadoutItems = new List<LoadoutItem>();
}
