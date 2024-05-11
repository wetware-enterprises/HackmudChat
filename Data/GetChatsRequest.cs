namespace HackmudChat.Data;

// ReSharper disable InconsistentNaming

[Serializable]
public record GetChatsRequest : AuthedRequestBase {
	public required string[] usernames { get; set; }
	public double? before { get; set; }
	public double? after { get; set; }
}