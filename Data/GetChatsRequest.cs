namespace HackmudChat.Data;

// ReSharper disable InconsistentNaming

[Serializable]
public record GetChatsRequest : AuthedRequestBase {
	public required string[] usernames { get; set; }
	public string? before { get; set; }
	public string? after { get; set; }
}