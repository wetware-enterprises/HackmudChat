﻿namespace HackmudChat.Data;

// ReSharper disable InconsistentNaming

[Serializable]
public record ResponseBase {
	public bool ok { get; set; }
}