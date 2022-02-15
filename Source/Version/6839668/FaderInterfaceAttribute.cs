using UnityEngine;

public class FaderInterfaceAttribute : PropertyAttribute
{
	private bool m_horizontal;

	public FaderInterfaceAttribute()
	{
	}

	public FaderInterfaceAttribute(bool horizontalLayout)
	{
		m_horizontal = horizontalLayout;
	}

	public bool IsHorizontal()
	{
		return m_horizontal;
	}
}
