﻿namespace Ecng.Net.BBCodes
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

    using Ecng.Common;

	/// <summary>
	/// Not regular expression, just a simple replace
	/// </summary>
	public class SimpleReplaceRule<TContext, TDomain> : BaseReplaceRule<TContext>
		where TContext : BBCodesContext<TDomain>
  {
    #region Constants and Fields

    /// <summary>
    ///   The _find.
    /// </summary>
    private readonly string _find;

    /// <summary>
    ///   The _replace.
    /// </summary>
    private readonly Func<TDomain, string> _replace;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleReplaceRule"/> class.
    /// </summary>
    /// <param name="find">
    /// The find.
    /// </param>
    /// <param name="replace">
    /// The replace.
    /// </param>
    public SimpleReplaceRule(string find, Func<TDomain, string> replace)
    {
      _find = find;
      _replace = replace;

      // lower the rank by default
      RuleRank = 100;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets RuleDescription.
    /// </summary>
    public override string RuleDescription
    {
      get
      {
        return $"Find = \"{_find}\"";
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    public override Task<string> ReplaceAsync(TContext context, string text, IReplaceBlocks replacement, CancellationToken cancellationToken)
    {
      int index;

      do
      {
        cancellationToken.ThrowIfCancellationRequested();

        index = text.FastIndexOf(_find);

        if (index >= 0)
        {
          // replace it...
          int replaceIndex = replacement.Add(_replace(context.Domain));
          text = text.Substring(0, index) + replacement.Get(replaceIndex) +
                 text.Substring(index + _find.Length);
        }
      }
      while (index >= 0);

	  return Task.FromResult(text);
    }

    #endregion
  }
}