using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using HackmudChat.Data;
using HackmudChat.Chat.Impl;
using HackmudChat.Utility;

namespace HackmudChat.Chat;

public class ChatApi : IChatApi {
	private const string BaseAddress = "https://www.hackmud.com";
	
	private readonly RateLimiter<RateLimit> _rate;
	
	private readonly HttpClient _http;
	private readonly JsonSerializerOptions _options = new() {
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	public ChatApi() {
		this._rate = new RateLimiter<RateLimit>(RateLimits);
		this._http = new HttpClient { BaseAddress = new Uri(BaseAddress) };
		this._http.DefaultRequestHeaders.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json")
		);
	}
	
	// Rate limits

	private enum RateLimit {
		GetToken,
		AccountData,
		GetChats,
		CreateChat
	}

	private readonly static Dictionary<RateLimit, int> RateLimits = new() {
		{ RateLimit.GetToken, 100 },
		{ RateLimit.AccountData, 5000 },
		{ RateLimit.GetChats, 700 },
		{ RateLimit.CreateChat, 1000 }
	};
	
	// Endpoint: get_token.json
	
	private const string GetTokenEndpoint = "/mobile/get_token.json";

	public async Task<GetTokenResponse> GetToken(string pass)
		=> await this.GetToken(pass, CancellationToken.None);

	public async Task<GetTokenResponse> GetToken(string pass, CancellationToken cancelToken) {
		return await this.CallEndpointAsync<GetTokenResponse>(
			GetTokenEndpoint,
			new GetTokenRequest { pass = pass },
			RateLimit.GetToken,
			cancelToken
		);
	}
	
	// Endpoint: account_data.json
	
	private const string AccountDataEndpoint = "/mobile/account_data.json";

	public async Task<GetAccountDataResponse> GetAccountData(string token)
		=> await this.GetAccountData(token, CancellationToken.None);

	public async Task<GetAccountDataResponse> GetAccountData(string token, CancellationToken cancelToken) {
		return await this.CallEndpointAsync<GetAccountDataResponse>(
			AccountDataEndpoint,
			new AuthedRequestBase { chat_token = token },
			RateLimit.AccountData,
			cancelToken
		);
	}

	// Endpoint: chats.json
	
	private const string ChatsEndpoint = "/mobile/chats.json";

	public async Task<GetChatsResponse> GetChats(string token, string[] usernames, double? before = null, double? after = null)
		=> await this.GetChats(token, usernames, before, after, CancellationToken.None);

	public async Task<GetChatsResponse> GetChats(
		string token,
		string[] usernames,
		double? before,
		double? after,
		CancellationToken cancelToken
	) {
		if (before == null && after == null)
			throw new Exception("Before *or* after must be specified when polling. Please refer to the chat API documentation: https://www.hackmud.com/forums/general_discussion/chat_api_documentation");
		
		return await this.CallEndpointAsync<GetChatsResponse>(
			ChatsEndpoint,
			new GetChatsRequest {
				chat_token = token,
				usernames = usernames,
				before = before,
				after = after
			},
			RateLimit.GetChats,
			cancelToken
		);
	}

	public async Task<GetChatsResponse> GetChats(
		string token,
		string[] usernames,
		DateTime? before = null,
		DateTime? after = null
	) {
		return await this.GetChats(
			token,
			usernames,
			before: TimeUtils.ConvertToRuby(before),
			after: TimeUtils.ConvertToRuby(after),
			CancellationToken.None
		);
	}

	public async Task<GetChatsResponse> GetChats(
		string token,
		string[] usernames,
		DateTime? before,
		DateTime? after,
		CancellationToken cancelToken
	) {
		return await this.GetChats(
			token,
			usernames,
			before: TimeUtils.ConvertToRuby(before),
			after: TimeUtils.ConvertToRuby(after),
			cancelToken
		);
	}

	public async Task<GetChatsResponse> GetChatsBefore(string token, string[] usernames, double before)
		=> await this.GetChats(token, usernames, before: before);

	public async Task<GetChatsResponse> GetChatsBefore(string token, string[] usernames, double before, CancellationToken cancelToken)
		=> await this.GetChats(token, usernames, before: before, after: null, cancelToken);

	public async Task<GetChatsResponse> GetChatsBefore(string token, string[] usernames, DateTime before)
		=> await this.GetChats(token, usernames, before: before);

	public async Task<GetChatsResponse> GetChatsBefore(string token, string[] usernames, DateTime before, CancellationToken cancelToken)
		=> await this.GetChats(token, usernames, before: before, after: null, cancelToken);

	public async Task<GetChatsResponse> GetChatsAfter(string token, string[] usernames, double after)
		=> await this.GetChats(token, usernames, after: after);

	public async Task<GetChatsResponse> GetChatsAfter(string token, string[] usernames, double after, CancellationToken cancelToken)
		=> await this.GetChats(token, usernames, before: null, after: after, cancelToken);

	public async Task<GetChatsResponse> GetChatsAfter(string token, string[] usernames, DateTime after)
		=> await this.GetChats(token, usernames, after: after);

	public async Task<GetChatsResponse> GetChatsAfter(string token, string[] usernames, DateTime after, CancellationToken cancelToken)
		=> await this.GetChats(token, usernames, before: null, after: after, cancelToken);

	// Endpoint: create_chat.json
	
	private const string CreateChatEndpoint = "/mobile/create_chat.json";

	public async Task<ResponseBase> SendChannel(string token, string username, string channel, string msg)
		=> await this.SendChannel(token, username, channel, msg, CancellationToken.None);

	public async Task<ResponseBase> SendChannel(
		string token,
		string username,
		string channel,
		string msg,
		CancellationToken cancelToken
	) {
		return await this.CallEndpointAsync<ResponseBase>(
			CreateChatEndpoint,
			new CreateChatRequest {
				chat_token = token,
				username = username,
				channel = channel,
				msg = msg
			},
			RateLimit.CreateChat,
			cancelToken
		);
	}

	public async Task<ResponseBase> SendTell(string token, string username, string tell, string msg)
		=> await this.SendTell(token, username, tell, msg, CancellationToken.None);

	public async Task<ResponseBase> SendTell(
		string token,
		string username,
		string tell,
		string msg,
		CancellationToken cancelToken
	) {
		return await this.CallEndpointAsync<ResponseBase>(
			CreateChatEndpoint,
			new CreateChatRequest {
				chat_token = token,
				username = username,
				tell = tell,
				msg = msg
			},
			RateLimit.CreateChat,
			cancelToken
		);
	}

	// Request handler
	
	private async Task<T> CallEndpointAsync<T>(
		string endpoint,
		object content,
		RateLimit rate,
		CancellationToken cancelToken
	) where T : ResponseBase {
		using var _ = await this._rate.Wait(rate);
		var uri = new Uri(endpoint, UriKind.Relative);
		var result = await this._http.PostAsJsonAsync(uri, content, this._options, cancelToken);
		var response = await result.Content.ReadFromJsonAsync<T>(cancelToken);
		cancelToken.ThrowIfCancellationRequested();
		return response!;
	}
	
	// Disposal

	public void Dispose() {
		this._http.Dispose();
	}
}