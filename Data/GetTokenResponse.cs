namespace HackmudChat.Data;

// ReSharper disable InconsistentNaming

[Serializable]
public record GetTokenResponse : ResponseBase {
	public string? chat_token { get; set; }
}