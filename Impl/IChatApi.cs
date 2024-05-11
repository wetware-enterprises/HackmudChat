using HackmudChat.Data;

namespace HackmudChat.Impl;

public interface IChatApi {
	public Task<GetTokenResponse> GetToken(string pass);
	public Task<GetAccountDataResponse> GetAccountData(string token);
	public Task<ResponseBase> CreateChatChannel(string token, string username, string channel, string msg);
	public Task<ResponseBase> CreateChatTell(string token, string username, string tell, string msg);
}