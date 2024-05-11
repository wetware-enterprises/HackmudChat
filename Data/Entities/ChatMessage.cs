using HackmudChat.Utility;

namespace HackmudChat.Data.Entities;

// ReSharper disable InconsistentNaming

[Serializable]
public record ChatMessage {
	public required string id { get; set; }
	public required double t { get; set; }
	public required string from_user { get; set; }
	public required string msg { get; set; }
	public bool is_join { get; set; } = false;
	public bool is_leave { get; set; } = false;
	public string? channel { get; set; }

	public bool IsTell => this.channel == null;
	
	public DateTime GetDateTime() => TimeUtils.ConvertFromRuby(this.t);
}