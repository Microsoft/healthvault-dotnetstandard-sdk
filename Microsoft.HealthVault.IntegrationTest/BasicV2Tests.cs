﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.IntegrationTest
{
    [TestClass]
    public class BasicV2Tests
    {
        [TestMethod]
        public async Task BasicInformationFields()
        {
            IHealthVaultSodaConnection connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(Constants.Configuration);
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            HealthRecordInfo record = personInfo.SelectedRecord;

            IThingClient thingClient = connection.CreateThingClient();

            BasicV2 basicInfo = (await thingClient.GetThingsAsync<BasicV2>(record.Id)).First();
            Assert.AreEqual("Redmond", basicInfo.City);
            Assert.AreEqual(1985, basicInfo.BirthYear);
            Assert.AreEqual(Gender.Male, basicInfo.Gender);
            Assert.AreEqual("98052", basicInfo.PostalCode);
            Assert.AreEqual("United States", basicInfo.Country.Text);
            Assert.AreEqual("Washington", basicInfo.StateOrProvince.Text);
        }
    }
}
