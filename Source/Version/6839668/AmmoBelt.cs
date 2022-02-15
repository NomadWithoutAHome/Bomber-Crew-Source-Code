using UnityEngine;

public class AmmoBelt : MonoBehaviour
{
	public void Use()
	{
		CrewmanAvatar carriedBy = GetComponent<CarryableItem>().GetCarriedBy();
		if (carriedBy != null)
		{
			carriedBy.DropItem();
			Object.Destroy(base.gameObject);
		}
	}
}
