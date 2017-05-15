﻿using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Gets session credentials on the Client SDK.
    /// </summary>
    internal class ClientSessionCredentialClient : SessionCredentialClientBase, IClientSessionCredentialClient
    {
        public string AppSharedSecret { get; set; }

        public override void WriteInfoXml(XmlWriter writer)
        {
            byte[] hmacContentBytes = GetHmacContentBytes();
            CryptoData hmacResult = Cryptographer.Hmac(AppSharedSecret, hmacContentBytes);

            writer.WriteStartElement("appserver2");
            writer.WriteStartElement("hmacSig");
            writer.WriteAttributeString("algName", hmacResult.Algorithm);
            writer.WriteValue(hmacResult.Value); // HMAC of content
            writer.WriteEndElement(); // hmacSig
            writer.WriteRaw(Encoding.UTF8.GetString(hmacContentBytes));
            writer.WriteEndElement(); // appserver2
        }

        private byte[] GetHmacContentBytes()
        {
            XmlWriterSettings settings = SDKHelper.XmlUtf8WriterSettings;

            using (MemoryStream contentMemoryStream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(contentMemoryStream, settings))
                {
                    writer.WriteStartElement("content");
                    writer.WriteElementString("app-id", Connection.ApplicationId.ToString());
                    writer.WriteElementString("hmac", HealthVaultConstants.Cryptography.HmacAlgorithm);
                    writer.WriteElementString("signing-time", DateTimeOffset.UtcNow.ToString("o"));
                    writer.WriteEndElement(); // content
                }

                return contentMemoryStream.ToArray();
            }
        }
    }
}
