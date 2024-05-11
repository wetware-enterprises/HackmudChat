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
	
	private readonly HttpClient _http;
	
	private readonly JsonSerializerOptions _options = new() {
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	public ChatApi() {
		this._http = new HttpClient { BaseAddress = new Uri(BaseAddress) };
		this._http.DefaultRequestHeaders.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json")
		);
	}
	
	// Endpoint: get_token.json
	
	private const string GetTokenEndpoint = "/mobile/get_token.json";

	public async Task<GetTokenResponse> GetToken(string pass) {
		var body = new GetTokenRequest { pass = pass };
		return await this.CallEndpointAsync<GetTokenResponse>(GetTokenEndpoint, body);
	}
	
	// Endpoint: account_data.json
	
	private const string AccountDataEndpoint = "/mobile/account_data.json";

	public async Task<GetAccountDataResponse> GetAccountData(string token) {
		return await this.CallEndpointAsync<GetAccountDataResponse>(
			AccountDataEndpoint,
			new AuthedRequestBase { chat_token = token }
		);
	}

	// Endpoint: chats.json
	
	private const string ChatsEndpoint = "/mobile/chats.json";

	public async Task<GetChatsResponse> GetChats(
		string token,
		string[] usernames,
		double? before = null,
		double? after = null
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
			}
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
			after: TimeUtils.ConvertToRuby(after)
		);
	}

	public async Task<GetChatsResponse> GetChatsBefore(string token, string[] usernames, double before)
		=> await this.GetChats(token, usernames, before: before);
	
	public async Task<GetChatsResponse> GetChatsBefore(string token, string[] usernames, DateTime before)
		=> await this.GetChats(token, usernames, before: before);
	
	public async Task<GetChatsResponse> GetChatsAfter(string token, string[] usernames, double after)
		=> await this.GetChats(token, usernames, after: after);
	
	public async Task<GetChatsResponse> GetChatsAfter(string token, string[] usernames, DateTime after)
		=> await this.GetChats(token, usernames, after: after);
	
	// Endpoint: create_chat.json
	
	private const string CreateChatEndpoint = "/mobile/create_chat.json";

	public async Task<ResponseBase> SendChannel(string token, string username, string channel, string msg) {
		return await this.CallEndpointAsync<ResponseBase>(
			CreateChatEndpoint,
			new CreateChatRequest {
				chat_token = token,
				username = username,
				channel = channel,
				msg = msg
			}
		);
	}
	
	public async Task<ResponseBase> SendTell(string token, string username, string tell, string msg) {
		return await this.CallEndpointAsync<ResponseBase>(
			CreateChatEndpoint,
			new CreateChatRequest {
				chat_token = token,
				username = username,
				tell = tell,
				msg = msg
			}
		);
	}
	
	// Request handler
	
	private async Task<T> CallEndpointAsync<T>(string endpoint, object content) where T : ResponseBase {
		var uri = new Uri(endpoint, UriKind.Relative);
		var result = await this._http.PostAsJsonAsync(uri, content, this._options);
		var response = await result.Content.ReadFromJsonAsync<T>();
		return response!;
	}
}