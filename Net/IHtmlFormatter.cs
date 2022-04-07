﻿namespace Ecng.Net
{
	using System.Threading;
	using System.Threading.Tasks;

	public interface IHtmlFormatter
	{
		ValueTask<string> ToHtmlAsync(string text, object context, CancellationToken cancellationToken = default);
		ValueTask<string> CleanAsync(string text, CancellationToken cancellationToken = default);
		ValueTask<string> ActivateRuleAsync(string text, object rule, object context, CancellationToken cancellationToken = default);
	}
}