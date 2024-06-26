﻿//
// Copyright (C) axuno gGmbH, Scott Rippey, Bernhard Millauer and other contributors.
// Licensed under the MIT license.
//

using System;
using Newtonsoft.Json.Linq;
using SmartFormat.Core.Extensions;

namespace SmartFormat.Extensions
{
    public class JsonSource : ISource
	{
        public JsonSource(SmartFormatter formatter)
        {
            // Note: We only have ONE parser at a time.
            // These settings will affect all extensions loaded at the same time

            // Escaped JSON property names and special characters are not supported in
            // order to avoid interference with other extensions
            formatter.Parser.AddAlphanumericSelectors(); // (A-Z + a-z)
            formatter.Parser.AddAdditionalSelectorChars("_");
            // For JsonSource it would be optimal not to have any operators in place,
            // but we have a workaround, if they are set by other extensions
            formatter.Parser.AddOperators("."); 
        }

		public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
        {
            // Note: Operators are processed by ListFormatter
            return selectorInfo.CurrentValue switch
            {
                JObject _ => NewtonSoftJson.TryEvaluateSelector(selectorInfo),
                _ => false
            };
        }

        /// <summary>
        /// Evaluation class for <see cref="Newtonsoft.Json"/>.
        /// </summary>
        private static class NewtonSoftJson
        {
            // Note: Operators are processed by ListFormatter
            public static bool TryEvaluateSelector(ISelectorInfo selectorInfo)
            {
                var jsonObject = selectorInfo.CurrentValue as JObject;

                var result = jsonObject?.GetValue(selectorInfo.SelectorText,
                    selectorInfo.FormatDetails.Settings.GetCaseSensitivityComparison());

                selectorInfo.Result = result ?? throw new FormatException($"'{selectorInfo.SelectorText}'");
                return true;
            }
        }
    }
}