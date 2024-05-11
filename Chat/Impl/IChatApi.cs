using HackmudChat.Data;

namespace HackmudChat.Chat.Impl;

public interface IChatApi : IDisposable {
	public Task<GetTokenResponse> GetToken(string pass);
	public Task<GetTokenResponse> GetToken(string pass, CancellationToken cancelToken);
	
	public Task<GetAccountDataResponse> GetAccountData(string token);
	public Task<GetAccountDataResponse> GetAccountData(string token, CancellationToken cancelToken);
	
	public Task<GetChatsResponse> GetChats(string token, string[] usernames, double? before = null, double? after = null);
	public Task<GetChatsResponse> GetChats(string token, string[] usernames, double? before, double? after, CancellationToken cancelToken);
	public Task<GetChatsResponse> GetChats(string token, string[] usernames, DateTime? before = null, DateTime? after = null);
	public Task<GetChatsResponse> GetChats(string token, string[] usernames, DateTime? before, DateTime? after, CancellationToken cancelToken);
	
	public Task<GetChatsResponse> GetChatsBefore(string token, string[] usernames, double before);
	public Task<GetChatsResponse> GetChatsBefore(string token, string[] usernames, double before, CancellationToken cancelToken);
	public Task<GetChatsResponse> GetChatsBefore(string token, string[] usernames, DateTime before);
	public Task<GetChatsResponse> GetChatsBefore(string token, string[] usernames, DateTime before, CancellationToken cancelToken);
	
	
	public Task<GetChatsResponse> GetChatsAfter(string token, string[] usernames, double after);
	public Task<GetChatsResponse> GetChatsAfter(string token, string[] usernames, double after, CancellationToken cancelToken);
	public Task<GetChatsResponse> GetChatsAfter(string token, string[] usernames, DateTime after);
	public Task<GetChatsResponse> GetChatsAfter(string token, string[] usernames, DateTime after, CancellationToken cancelToken);
	
	public Task<ResponseBase> SendChannel(string token, string username, string channel, string msg);
	public Task<ResponseBase> SendChannel(string token, string username, string channel, string msg, CancellationToken cancelToken);
	public Task<ResponseBase> SendTell(string token, string username, string tell, string msg);
	public Task<ResponseBase> SendTell(string token, string username, string tell, string msg, CancellationToken cancelToken);
}