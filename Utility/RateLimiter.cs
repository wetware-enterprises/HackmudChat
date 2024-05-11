namespace HackmudChat.Utility;

public class RateLimiter<TEnum> where TEnum : Enum {
	private readonly CancellationTokenSource _src = new();
	private readonly Dictionary<TEnum, Rate> _rates;
	
	public int Default { get; set; } = 1000;
    
	public RateLimiter(
		Dictionary<TEnum, int> rates
	) {
		this._rates = rates.ToDictionary(
			pair => pair.Key,
			pair => new Rate { Value = pair.Value }
		);
	}

	public async Task<IDisposable> Wait(TEnum type) {
		var rate = this.GetRate(type);

		var wait = 0;
		Handle handle = null!;

		var token = this._src.Token;
		
		var isUsing = true;
		while (isUsing && !token.IsCancellationRequested) {
			lock (rate) {
				isUsing = rate.IsUsing;
				if (!isUsing) {
					var next = rate.Previous.AddMilliseconds(rate.Value);
					var now = DateTime.Now;
					if (now < next)
						wait = (int)(next - now).TotalMilliseconds;
					handle = new Handle(rate);
				}
			}
			if (isUsing) await Task.Delay(20, token);
		}

		if (wait > 0) await Task.Delay(wait, token);
		
		token.ThrowIfCancellationRequested();

		return handle;
	}

	private Rate GetRate(TEnum type) {
		if (this._rates.TryGetValue(type, out var rate))
			return rate;
		return this._rates[type] = new Rate { Value = this.Default };
	}
	
	private class Rate {
		public required int Value;
		public DateTime Previous = DateTime.Now;
		public bool IsUsing;
	}

	private class Handle : IDisposable {
		private readonly Rate _rate;

		public Handle(Rate rate) {
			this._rate = rate;
			this._rate.IsUsing = true;
		}
		
		public void Dispose() {
			this._rate.Previous = DateTime.Now;
			this._rate.IsUsing = false;
		}
	}
}