﻿//
// Copyright (C) axuno gGmbH, Scott Rippey, Bernhard Millauer and other contributors.
// Licensed under the MIT license.
//

using System;
using System.Threading;
using System.Threading.Tasks;
using SmartFormat.Core.Extensions;
using SmartFormat.Extensions;

namespace SmartFormat
{
    /// <summary>
    /// This class holds a Default instance of the SmartFormatter.
    /// The default instance has all extensions registered.
    /// </summary>
    public static class Smart
    {
        #region: Smart.Format :

        public static string Format(string format, params object[] args)
        {
            return Default.Format(format, args);
        }

		public static ValueTask<string> FormatAsync(string format, object[] args, CancellationToken cancellationToken)
		{
			return Default.FormatAsync(format, args, cancellationToken);
		}

		public static string Format(IFormatProvider provider, string format, params object[] args)
        {
            return Default.Format(provider, format, args);
        }

        #endregion

        #region: Overloads - Just to match the signature of String.Format, and allow support for programming languages that don't support "params" :

        public static string Format(string format, object arg0, object arg1, object arg2)
        {
            return Format(format, new[] {arg0, arg1, arg2});
        }

        public static string Format(string format, object arg0, object arg1)
        {
            return Format(format, new[] {arg0, arg1});
        }

        public static string Format(string format, object arg0)
        {
            return Default.Format(format, arg0); // call Default.Format in order to avoid infinite recursive method call
        }

        #endregion

        #region: Default formatter :

        public static SmartFormatter Default { get; set; } = CreateDefaultSmartFormat();

        public static SmartFormatter CreateDefaultSmartFormat()
        {
            // Register all default extensions here:
            var formatter = new SmartFormatter();
            // Add all extensions:
            // Note, the order is important; the extensions
            // will be executed in this order:

            var listFormatter = new ListFormatter(formatter);

            // sources for specific types must be in the list before ReflectionSource
            formatter.AddExtensions(
                (ISource) listFormatter, // ListFormatter MUST be first
                new DictionarySource(formatter),
                new ValueTupleSource(formatter),
#pragma warning disable CS0618 // Type or member is obsolete
                new SmartObjectsSource(formatter),
#pragma warning restore CS0618 // Type or member is obsolete
                new JsonSource(formatter),
                new XmlSource(formatter),
                new ReflectionSource(formatter),

                // The DefaultSource reproduces the string.Format behavior:
                new DefaultSource(formatter)
            );
            formatter.AddExtensions(
                (IFormatter) listFormatter,
                new PluralLocalizationFormatter("en"),
                new ConditionalFormatter(),
                new TimeFormatter("en"),
                new XElementFormatter(),
                new ChooseFormatter(),
                new SubStringFormatter(),
                new DefaultFormatter()
            );

            return formatter;
        }

        #endregion
    }
}