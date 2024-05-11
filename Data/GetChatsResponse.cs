using HackmudChat.Data.Entities;

namespace HackmudChat.Data;

// ReSharper disable InconsistentNaming

[Serializable]
public record GetChatsResponse : ResponseBase {
	public Dictionary<string, Chat[]>? chats { get; set; }
}