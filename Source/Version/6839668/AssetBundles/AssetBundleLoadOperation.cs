using System.Collections;

namespace AssetBundles;

public abstract class AssetBundleLoadOperation : IEnumerator
{
	public object Current => null;

	public bool MoveNext()
	{
		return !IsDone();
	}

	public void Reset()
	{
	}

	public abstract bool Update();

	public abstract bool IsDone();
}
