public interface LoadableSystem
{
	bool IsLoadComplete();

	void StartLoad();

	void ContinueLoad();

	string GetName();

	LoadableSystem[] GetDependencies();
}
