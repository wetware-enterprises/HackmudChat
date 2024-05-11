using HackmudChat.Data;

namespace HackmudChat.Impl;

public interface IChatApi {
	public Task<GetTokenResponse> GetToken(string pass);
	public Task<GetAccountDataResponse> GetAccountData(string token);
	
	public Task<GetChatsResponse> GetChats(string token, string[] usernames, float? before = null, float? after = null);
	public Task<GetChatsResponse> GetChats(string token, string[] usernames, DateTime? before = null, DateTime? after = null);
	
	public Task<GetChatsResponse> GetChatsBefore(string token, string[] usernames, float before);
	public Task<GetChatsResponse> GetChatsBefore(string token, string[] usernames, DateTime before);
	
	public Task<GetChatsResponse> GetChatsAfter(string token, string[] usernames, float after);
	public Task<GetChatsResponse> GetChatsAfter(string token, string[] usernames, DateTime after);
	
	public Task<ResponseBase> CreateChatChannel(string token, string username, string channel, string msg);
	public Task<ResponseBase> CreateChatTell(string token, string username, string tell, string msg);
}