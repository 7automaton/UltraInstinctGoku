using Discord;
using Discord.WebSocket;
using UltraInstinctGoku;

public class Program
{
	private DiscordSocketClient _client;

	/// We make an object here so we don't have to reread the database constantly.
	/// If you need to update the list, please restart the bot.
	private MessageTrolls _messageReceiver;
	public static Task Main(string[] args) => new Program().MainAsync();

	/// <summary>
	/// The core of the Operation
	/// </summary>
	/// <returns>Laughter</returns>
	public async Task MainAsync()
	{

		_client = new DiscordSocketClient();
		MessageTrolls _messageReceiver = new();

		/// When a message is sent, check if the user should be epically trolled,
		/// and then commit to the trolling.
		_client.MessageReceived += _messageReceiver.MessageReceived;

		/// Simple secure token auth.
		/// To use this bot, make a new file called "token.txt"
		/// in the root directory of the project,
		/// then paste your bot's token on one line.
		string token = File.ReadAllText("token.txt");

		// ⬆️⬆️ START IT UP ⬆️⬆️
		await _client.LoginAsync(TokenType.Bot, token);
		await _client.StartAsync();

		// Block this task until the program is closed.
		await Task.Delay(-1);
	}
}