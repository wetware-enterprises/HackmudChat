using Timer = System.Timers.Timer;

using HackmudChat.Chat.Impl;
using HackmudChat.Data.Entities;
using HackmudChat.Utility;

namespace HackmudChat.Chat;

public delegate void ClientReadyEvent(ChatClient sender);
public delegate void ChatsReceivedEvent(ChatClient sender, Dictionary<string, List<ChatMessage>> data);

public class ChatClient : IChatClient {
	private const string NoAuthError = "Client is not authenticated.";
	
	private readonly IChatApi _api;
	
	private readonly Timer _pollTimer;

	private string? _token;

	private DateTime _lastRefresh = DateTime.UnixEpoch;
	private readonly Dictionary<string, ChatUser> _users = new();

	private readonly HashSet<string> _prevIds = new();

	// Events

	public event ClientReadyEvent? OnReady;
	public event ChatsReceivedEvent? OnReceived;

	// Constructors
	
	public ChatClient() : this(new ChatApi()) { }
	
	public ChatClient(IChatApi api) {
		this._api = api;
		
		this._pollTimer = new Timer();
		this._pollTimer.Elapsed += this.Poll;
		this._pollTimer.Interval = 3000;
	}
	
	// Authentication
	
	public bool IsAuthed { get; private set; }

	public async Task<string> ConnectPass(string pass) {
		string? token;
		
		var getToken = await this._api.GetToken(pass);
		if (getToken.ok != true || (token = getToken.chat_token) == null)
			throw new Exception($"Failed to get token: {getToken.msg ?? "Unknown"}");

		await this.ConnectToken(token);
		return token;
	}

	public async Task ConnectToken(string token) {
		this._token = token;
		
		if (this.LastPoll == 0.0)
			this.LastPoll = TimeUtils.ConvertToRuby(DateTime.Now);
		
		await this.RefreshAccountData();

		this.IsAuthed = true;
		this.IsPolling = true;
		this.OnReady?.Invoke(this);
	}
	
	// Polling
	
	public double LastPoll { get; set; }
	
	public double PollRate {
		get => this._pollTimer.Interval;
		set {
			if (value < 2000)
				throw new Exception("Cannot set poll rate below 2000ms.");
			this._pollTimer.Interval = value;
		}
	}

	public bool IsPolling {
		get => this._pollTimer.Enabled;
		set {
			if (this._token == null)
				throw new Exception(NoAuthError);
			this._pollTimer.Enabled = value;
		}
	}

	private async void Poll(object? sender, object? _) {
		if (!this.IsPolling || this._token == null) {
			this._pollTimer.Stop();
			return;
		}

		var users = this._users.Keys.ToArray();
		var data = await this._api.GetChatsAfter(this._token, users, this.LastPoll - 1);
		if (data.ok != true || data.chats == null)
			throw new Exception($"Failed to poll chat messages: {data.msg ?? "Unknown"}");

		var ids = new List<string>();
		var timeMax = this.LastPoll;

		var chats = new Dictionary<string, List<ChatMessage>>();
		foreach (var (user, messages) in data.chats) {
			ids.AddRange(messages.Select(msg => msg.id));
			
			var notify = messages
				.Where(msg => !this._prevIds.Contains(msg.id))
				.ToList();
			
			chats[user] = notify;
			
			timeMax = messages
				.Select(msg => msg.t)
				.Aggregate(timeMax, Math.Max);
		}
		this.OnReceived?.Invoke(this, chats);
		
		this._prevIds.Clear();
		foreach (var id in ids.Distinct())
			this._prevIds.Add(id);

		var timeNow = TimeUtils.ConvertToRuby(DateTime.Now);
		if (timeMax + 300000 < timeNow) timeMax = timeNow;
		this.LastPoll = timeMax;

		if ((DateTime.Now - this._lastRefresh).TotalMinutes >= 60)
			await this.RefreshAccountData();
	}
	
	// Account data

	public IReadOnlyDictionary<string, ChatUser> GetAccountData() {
		lock (this._users) {
			return this._users.AsReadOnly();
		}
	}

	public async Task<IReadOnlyDictionary<string, ChatUser>> RefreshAccountData() {
		if (this._token == null)
			throw new Exception(NoAuthError);

		this._lastRefresh = DateTime.Now;
		
		var data = await this._api.GetAccountData(this._token);
		if (data.ok != true || data.users == null)
			throw new Exception($"Failed to get account data: {data.msg ?? "Unknown"}");
		
		lock (this._users) {
			foreach (var (user, channels) in data.users)
				this._users[user] = channels;
			return this._users;
		}
	}
	
	// Chat messages

	public async Task SendChannel(string username, string channel, string msg) {
		if (this._token == null)
			throw new Exception(NoAuthError);
		
		var result = await this._api.SendTell(this._token, username, channel, msg);
		if (result.ok != true)
			throw new Exception(result.msg ?? "Unknown error");
	}

	public async Task SendTell(string username, string tell, string msg) {
		if (this._token == null)
			throw new Exception(NoAuthError);
		
		var result = await this._api.SendTell(this._token, username, tell, msg);
		if (result.ok != true)
			throw new Exception(result.msg ?? "Unknown error");
	}
}