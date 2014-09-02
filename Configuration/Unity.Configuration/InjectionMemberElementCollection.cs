﻿//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Ecng.Configuration.Properties;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A polymorphic collection of <see cref="InjectionMemberElement"/>s.
    /// </summary>
    [ConfigurationCollection(typeof(InjectionMemberElement))]
    public class InjectionMemberElementCollection : DeserializableConfigurationElementCollectionBase<InjectionMemberElement>
    {
        private readonly Dictionary<string, Type> elementTypeMap = new Dictionary<string, Type>
            {
                {"constructor", typeof (ConstructorElement)},
                {"property", typeof(PropertyElement)},
                {"method", typeof(MethodElement)}
            };

        /// <summary>
        /// Indexer that lets you access elements by their key.
        /// </summary>
        /// <param name="key">Key to retrieve element with.</param>
        /// <returns>The element.</returns>
        public new InjectionMemberElement this[string key]
        {
            get { return (InjectionMemberElement) BaseGet(key); }
        }

        /// <summary>
        /// Causes the configuration system to throw an exception.
        /// </summary>
        /// <returns>
        /// true if the unrecognized element was deserialized successfully; otherwise, false. The default is false.
        /// </returns>
        /// <param name="elementName">The name of the unrecognized element. 
        ///                 </param><param name="reader">An input stream that reads XML from the configuration file. 
        ///                 </param><exception cref="T:System.Configuration.ConfigurationErrorsException">The element specified in <paramref name="elementName"/> is the &lt;clear&gt; element.
        ///                 </exception><exception cref="T:System.ArgumentException"><paramref name="elementName"/> starts with the reserved prefix "config" or "lock".
        ///                 </exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            return DeserializeRegisteredElement(elementName, reader) ||
                base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            throw new InvalidOperationException(Resources.CannotCreateInjectionMemberElement);
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. 
        ///                 </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((InjectionMemberElement) element).Key;
        }

        private Type GetKnownElementType(string elementName)
        {
            return elementTypeMap.GetOrNull(elementName);
        }

        private static Type GetExtensionElementType(string elementName)
        {
            return ExtensionElementMap.GetInjectionMemberElementType(elementName);
        }

        private bool DeserializeRegisteredElement(string elementName, XmlReader reader)
        {
            Type elementType = GetKnownElementType(elementName) ?? GetExtensionElementType(elementName);
            if(elementType == null) return false;
            this.ReadElementByType(reader, elementType, this);
            return true;
        }
    }
}
