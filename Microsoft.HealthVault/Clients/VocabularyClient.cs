﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Vocabulary;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// Imlementation of <seealso cref="IVocabularyClient"/>
    /// </summary>
    internal class VocabularyClient : IVocabularyClient
    {
        private readonly IHealthVaultConnection _connection;

        public VocabularyClient(IHealthVaultConnection connection)
        {
            _connection = connection;
        }

        public Guid? CorrelationId { get; set; }

        public async Task<IReadOnlyCollection<VocabularyKey>> GetVocabularyKeysAsync()
        {
            return await HealthVaultPlatformVocabulary.Current.GetVocabularyKeysAsync(_connection);
        }

        public async Task<Vocabulary.Vocabulary> GetVocabularyAsync(VocabularyKey key, bool cultureIsFixed = false)
        {
            return (await GetVocabulariesAsync(new[] { key }, cultureIsFixed).ConfigureAwait(false)).FirstOrDefault();
        }

        public async Task<IList<Vocabulary.Vocabulary>> GetVocabulariesAsync(IList<VocabularyKey> vocabularyKeys, bool cultureIsFixed = false)
        {
            return await HealthVaultPlatformVocabulary.Current.GetVocabularyAsync(
                _connection,
                vocabularyKeys,
                cultureIsFixed).ConfigureAwait(false);
        }

        public async Task<ReadOnlyCollection<VocabularyKey>> SearchVocabularyAsync(string searchValue, VocabularySearchType searchType, int? maxResults)
        {
            return (await HealthVaultPlatformVocabulary.Current.SearchVocabularyAsync(_connection, null, searchValue, searchType, maxResults).ConfigureAwait(false)).MatchingKeys;
        }
    }
}