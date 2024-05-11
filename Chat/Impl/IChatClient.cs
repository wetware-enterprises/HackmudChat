using HackmudChat.Data.Entities;

namespace HackmudChat.Chat.Impl;

public interface IChatClient : IDisposable {
	public bool IsAuthed { get; }
	
	public Task<string> ConnectPass(string pass);
	public Task<string> ConnectPass(string pass, CancellationToken cancelToken);
	
	public Task ConnectToken(string token);
	public Task ConnectToken(string token, CancellationToken cancelToken);
	
	public double LastPoll { get; set; }
	public double PollRate { get; set; }
	public bool IsPolling { get; set; }

	public IReadOnlyDictionary<string, ChatUser> GetAccountData();
	
	public Task<IReadOnlyDictionary<string, ChatUser>> RefreshAccountData();
	public Task<IReadOnlyDictionary<string, ChatUser>> RefreshAccountData(CancellationToken cancelToken);

	public Task SendChannel(string username, string channel, string msg);
	public Task SendChannel(string username, string channel, string msg, CancellationToken cancelToken);
	
	public Task SendTell(string username, string tell, string msg);
	public Task SendTell(string username, string tell, string msg, CancellationToken cancelToken);

	public void Reset();
}