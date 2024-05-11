namespace HackmudChat.Data;

// ReSharper disable InconsistentNaming

[Serializable]
public record GetAccountDataResponse : ResponseBase {
	public Dictionary<string, User>? users { get; set; }
	
	public class User : Dictionary<string, string[]>;
}