namespace HackmudChat.Data;

// ReSharper disable InconsistentNaming

[Serializable]
public record CreateChatRequest : AuthedRequestBase {
	public required string username { get; set; }
	public string? channel { get; set; }
	public string? tell { get; set; }
	public required string msg { get; set; }
}