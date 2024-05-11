namespace HackmudChat.Data;

// ReSharper disable InconsistentNaming

[Serializable]
public record GetChatsRequest : AuthedRequestBase {
	public required string[] usernames { get; set; }
	public float? before { get; set; }
	public float? after { get; set; }
}