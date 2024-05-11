namespace HackmudChat.Data;

// ReSharper disable InconsistentNaming

[Serializable]
public record AuthedRequestBase {
	public required string chat_token { get; set; }
}