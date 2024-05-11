namespace HackmudChat.Utility;

public static class TimeUtils {
	public static double ConvertToRuby(DateTime date) => ((DateTimeOffset)date).ToUnixTimeSeconds();
	public static double? ConvertToRuby(DateTime? date) => date != null ? ConvertToRuby(date.Value) : null;

	public static DateTime ConvertFromRuby(double time) => DateTimeOffset.FromUnixTimeMilliseconds((long)Math.Floor(time * 1000)).DateTime;
}