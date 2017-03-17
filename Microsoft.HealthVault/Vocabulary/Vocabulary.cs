// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Vocabulary
{
    /// <summary>
    /// Vocabulary list
    /// </summary>
    ///
    public class Vocabulary : Dictionary<string, VocabularyItem>, IXmlSerializable
    {
        internal Vocabulary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="Vocabulary"/> class
        /// with the specified name
        /// </summary>
        /// <param name="name">The name of the vocabulary</param>
        public Vocabulary(string name)
            : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Create a new instance of the <see cref="Vocabulary"/> class
        /// with the specified name, family, and version
        /// </summary>
        /// <param name="name">The name of the vocabulary</param>
        /// <param name="family">The family of the vocabulary</param>
        /// <param name="version">The version of the vocabulary</param>
        public Vocabulary(string name, string family, string version)
            : this()
        {
            this.Name = name;
            this.Family = family;
            this.Version = version;
        }

        /// <summary>
        /// Gets or sets culture information containing the language in which the
        /// vocabulary items are represented.
        /// </summary>
        ///
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets the name of the <see cref="Vocabulary"/>.
        /// </summary>
        ///
        /// <value>
        /// A string representing the <see cref="Vocabulary"/> name.
        /// </value>
        ///
        public string Name { get; private set; }

        /// <summary>
        /// Gets the family of the vocabulary.
        /// </summary>
        ///
        /// <value>
        /// A string representing the <see cref="Vocabulary"/> family.
        /// </value>
        ///
        /// <remarks>
        /// The family indicates the source of the
        /// information in the vocabulary, including
        /// external standards such as ISO or
        /// internal standards such as HealthVault Vocabulary.
        /// </remarks>
        ///
        public string Family { get; private set; }

        /// <summary>
        /// Gets the version of the <see cref="Vocabulary"/>.
        /// </summary>
        ///
        /// <value>
        /// A string representing the <see cref="Vocabulary"/> version.
        /// </value>
        ///
        public string Version { get; private set; }

        /// <summary>
        /// Gets if the vocabulary contains none empty member.
        /// </summary>
        public bool IsNotEmpty
        {
            get
            {
                foreach (KeyValuePair<string, VocabularyItem> kvp in this)
                {
                    if ((!string.IsNullOrEmpty(kvp.Value.AbbreviationText))
                        || (!string.IsNullOrEmpty(kvp.Value.DisplayText)))
                    {
                        this.isNotEmpty = true;
                        break;
                    }
                }

                return this.isNotEmpty;
            }
        }

        private bool isNotEmpty;

        /// <summary>
        /// Gets if the set vocabulary items in the <see cref="Vocabulary"/> has been truncated i.e.
        /// there could be more <see cref="VocabularyItem"/>s in the <see cref="Vocabulary"/>.
        /// </summary>
        ///
        [Obsolete("Use Vocabulary.IsTruncated instead.")]
        public bool IsTruncted { get; private set; }

        /// <summary>
        /// Gets if the set vocabulary items in the <see cref="Vocabulary"/> has been truncated i.e.
        /// there could be more <see cref="VocabularyItem"/>s in the <see cref="Vocabulary"/>.
        /// </summary>
        ///
        public bool IsTruncated
        {
            get { return this.IsTruncted; }
            set { this.IsTruncted = value; }
        }

        /// <summary>
        /// Adds a vocabulary item to the vocabulary.
        /// </summary>
        /// <param name="item">The <see cref="VocabularyItem"/> instance to add.</param>
        public void Add(VocabularyItem item)
        {
            item.Vocabulary = this;
            this.Add(item.Value, item);
        }

        internal virtual void AddVocabularyItem(string key, VocabularyItem item)
        {
            item.Vocabulary = this;
            this.Add(key, item);
        }

        internal void PopulateFromXml(
            XPathNavigator vocabularyNav)
        {
            this.Name = vocabularyNav.SelectSingleNode("name").Value;
            this.Family = vocabularyNav.SelectSingleNode("family").Value;
            this.Version = vocabularyNav.SelectSingleNode("version").Value;
            this.Culture = new CultureInfo(vocabularyNav.XmlLang);

            XPathNodeIterator vocabularyItemsNav = vocabularyNav.Select("code-item");

            foreach (XPathNavigator vocabularyItemNav in vocabularyItemsNav)
            {
                // Check in case HealthVault returned invalid data.
                if (vocabularyItemNav == null)
                {
                    continue;
                }

                VocabularyItem code = new VocabularyItem();
                code.Vocabulary = this;
                code.ParseXml(vocabularyItemNav);
                this.AddVocabularyItem(code.Value, code);
            }

            XPathNavigator isTruncatedNav = vocabularyNav.SelectSingleNode("is-vocab-truncated");
            this.IsTruncted = isTruncatedNav != null ? isTruncatedNav.ValueAsBoolean : false;
        }

        private static XPathExpression infoPath = XPathExpression.Compile("/wc:info");

        internal static XPathExpression GetInfoXPathExpression(
            string methodNSSuffix, XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response." + methodNSSuffix);

            XPathExpression infoPathClone = null;
            lock (infoPath)
            {
                infoPathClone = infoPath.Clone();
            }

            infoPathClone.SetContext(infoXmlNamespaceManager);

            return infoPathClone;
        }

        #region IXmlSerializable Members

        /// <summary>
        /// GetSchema method for serialization
        /// </summary>
        /// <returns>Schema</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Serialization method to read the Xml from the reader and deserialize the instance.
        /// </summary>
        /// <param name="reader">The reader</param>
        public void ReadXml(XmlReader reader)
        {
            XPathDocument document = new XPathDocument(reader);
            XPathNavigator navigator = document.CreateNavigator();

            XPathNavigator vocabularyNode = navigator.SelectSingleNode("Vocabulary");

            this.Name = vocabularyNode.SelectSingleNode("name").Value;

            this.Family = XPathHelper.GetOptNavValue(vocabularyNode, "family");
            this.Version = XPathHelper.GetOptNavValue(vocabularyNode, "version");

            XPathNavigator itemsNode = vocabularyNode.SelectSingleNode("items");
            if (itemsNode != null)
            {
                foreach (XPathNavigator itemNode in itemsNode.SelectChildren(XPathNodeType.Element))
                {
                    VocabularyItem vocabItem = new VocabularyItem();
                    vocabItem.ParseXml(itemNode);
                    this.Add(vocabItem);
                }
            }
        }

        /// <summary>
        /// Write the serialized version of the data to the specified writer.
        /// </summary>
        /// <param name="writer">The writer</param>
        public void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            if (string.IsNullOrEmpty(this.Name))
            {
                throw new InvalidOperationException(Resources.VocabularyNameNullOrEmpty);
            }

            writer.WriteElementString("vocabulary-format-version", "1");

            writer.WriteElementString("name", this.Name);

            XmlWriterHelper.WriteOptString(writer, "family", this.Family);
            XmlWriterHelper.WriteOptString(writer, "version", this.Version);

            if (this.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("items");
            {
                foreach (KeyValuePair<string, VocabularyItem> item in this)
                {
                    item.Value.WriteXmlInternal("item", writer);
                }
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
