﻿using SmartFormat.Core.Extensions;

namespace SmartFormat.Extensions
{
	public class DefaultSource : ISource
	{
		public DefaultSource(SmartFormatter formatter)
		{
			formatter.Parser.AddOperators(","); // This is for alignment.
			formatter.Parser.AddAdditionalSelectorChars("-"); // This is for alignment.
		}

		/// <summary>
		/// Performs the default index-based selector, same as String.Format.
		/// </summary>
		public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
		{
			var current = selectorInfo.CurrentValue;
			var selector = selectorInfo.Selector;
			var formatDetails = selectorInfo.FormatDetails;

			int selectorValue;
			if (int.TryParse(selector.Text, out selectorValue))
			{
				// Argument Index:
				// Just like String.Format, the arg index must be in-range,
				// should be the first item, and shouldn't have any operator:
				if (selector.SelectorIndex == 0
					&& selectorValue < formatDetails.OriginalArgs.Length
					&& selector.Operator == "")
				{
					// This selector is an argument index.
					selectorInfo.Result = formatDetails.OriginalArgs[selectorValue];
					return true;
				}

				// Alignment:
				// An alignment item should be preceded by a comma
				if (selector.Operator == ",")
				{
					// This selector is actually an Alignment modifier.
					selectorInfo.Placeholder.Alignment = selectorValue;
					selectorInfo.Result = current; // (don't change the current item)
					return true;
				}

			}

			return false;
		}
	}
}