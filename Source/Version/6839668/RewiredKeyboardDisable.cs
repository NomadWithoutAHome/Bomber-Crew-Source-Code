using Rewired;

public static class RewiredKeyboardDisable
{
	private static int m_numDisables;

	public static void SetKeyboardDisable(bool disable)
	{
		if (ReInput.controllers.Keyboard == null)
		{
			return;
		}
		if (disable)
		{
			m_numDisables++;
			ReInput.controllers.Keyboard.enabled = false;
			return;
		}
		m_numDisables--;
		if (m_numDisables == 0)
		{
			ReInput.controllers.Keyboard.enabled = true;
		}
	}
}
