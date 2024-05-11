using HackmudChat.Impl;

namespace HackmudChat;

public class ChatClient : IChatClient {
	private readonly IChatApi _api;

	public ChatClient() {
		this._api = new ChatApi();
	}
	
	public ChatClient(IChatApi api) {
		this._api = api;
	}
}