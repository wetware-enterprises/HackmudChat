namespace HackmudChat.Utility;

public static class TimeUtils {
	public static float ConvertToRuby(DateTime date) => ((DateTimeOffset)date).ToUnixTimeMilliseconds() / 1000.0f;
	public static float? ConvertToRuby(DateTime? date) => date != null ? ConvertToRuby(date.Value) : null;

	public static DateTime ConvertFromRuby(float time) => DateTimeOffset.FromUnixTimeMilliseconds((long)Math.Floor(time * 1000)).DateTime;
}