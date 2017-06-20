﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Grace.DependencyInjection;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Clients.Deserializers;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Services;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault
{
    internal static class Ioc
    {
        static Ioc()
        {
            Container = new DependencyInjectionContainer();

            Container.RegisterSingleton<IServiceLocator, ServiceLocator>();
            Container.RegisterSingleton<IMessageHandlerFactory, MessageHandlerFactory>();
            Container.RegisterSingleton<IHttpClientFactory, HttpClientFactory>();
            Container.RegisterSingleton<IDateTimeService, DateTimeService>();

            Container.RegisterSingleton<IRequestMessageCreator, RequestMessageCreator>();
            Container.RegisterSingleton<IHealthServiceResponseParser, HealthServiceResponseParser>();
            Container.RegisterSingleton<IThingDeserializer, ThingDeserializer>();

            Container.RegisterSingleton<IHealthWebRequestClient, HealthWebRequestClient>();
            Container.RegisterSingleton<ICryptographer, Cryptographer>();

            Container.RegisterSingleton<IThingTypeRegistrar, ThingTypeRegistrarInternal>();

            Container.RegisterTransient<IPersonClient, PersonClient>();
        }

        public static DependencyInjectionContainer Container { get; internal set; }

        public static T Get<T>()
        {
            return Container.Locate<T>();
        }
    }
}
