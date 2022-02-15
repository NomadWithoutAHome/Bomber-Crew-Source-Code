namespace GameAnalyticsSDK.Setup;

public class Game
{
	public string Name { get; private set; }

	public int ID { get; private set; }

	public string GameKey { get; private set; }

	public string SecretKey { get; private set; }

	public Game(string name, int id, string gameKey, string secretKey)
	{
		Name = name;
		ID = id;
		GameKey = gameKey;
		SecretKey = secretKey;
	}
}
