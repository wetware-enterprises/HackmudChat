namespace HackmudChat.Data;

// ReSharper disable InconsistentNaming

[Serializable]
public record GetChatsResponse : ResponseBase {
	public Dictionary<string, Chat[]>? chats { get; set; }
	
	[Serializable]
	public record Chat {
		public required string id { get; set; }
		public required float t { get; set; }
		public required string from_user { get; set; }
		public required string msg { get; set; }
		public bool is_join { get; set; } = false;
		public bool is_leave { get; set; } = false;
		public string? channel { get; set; }
	}
}