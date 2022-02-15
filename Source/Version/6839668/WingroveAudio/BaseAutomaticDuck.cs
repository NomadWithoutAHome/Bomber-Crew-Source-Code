namespace WingroveAudio;

public abstract class BaseAutomaticDuck : BaseEventReceiveAction
{
	public abstract string GetGroupName();

	public abstract float GetDuckVol();
}
