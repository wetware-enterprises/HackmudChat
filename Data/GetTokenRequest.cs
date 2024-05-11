namespace HackmudChat.Data;

// ReSharper disable InconsistentNaming

[Serializable]
public record GetTokenRequest {
	public required string pass { get; set; }
}