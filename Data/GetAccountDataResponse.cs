using HackmudChat.Data.Entities;

namespace HackmudChat.Data;

// ReSharper disable InconsistentNaming

[Serializable]
public record GetAccountDataResponse : ResponseBase {
	public Dictionary<string, ChatUser>? users { get; set; }
}