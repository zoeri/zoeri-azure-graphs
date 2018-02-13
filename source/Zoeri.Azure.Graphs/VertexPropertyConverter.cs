#region License

// The MIT License (MIT)
// 
// Copyright © 2018 Zoeri
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Threading;

using Newtonsoft.Json;

using Zoeri.Azure.Graphs.Threading;

namespace Zoeri.Azure.Graphs
{
    /// <inheritdoc />
    /// <summary>
    /// Implements a strongly-typed JSON serialization and deserialization functionality to properties on any object decorated
    /// with the <see cref="T:Newtonsoft.Json.JsonPropertyAttribute" />.
    /// </summary>
    public class VertexPropertyConverter
        : JsonConverter
    {
        #region Properties

        private static Dictionary<Type, IIdValueConverter> ConverterDictionary
        {
            get;
        } = new Dictionary<Type, IIdValueConverter>();

        private static ReaderWriterLockSlim ConverterDictionaryLock
        {
            get;
        } = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        #endregion

        #region Methods

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            var canConvert = typeof(VertexProperty<>).IsAssignableFrom(objectType.GetGenericTypeDefinition());

            return canConvert;
        }

        /// <inheritdoc />
        public override object ReadJson
            (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var unboundType = typeof(IdValueConverter<>);
            var boundType = unboundType.MakeGenericType(objectType);
            IIdValueConverter internalConverter;

            using (var readerLock = new UpgradeableReaderLock(ConverterDictionaryLock))
            {
                if (!ConverterDictionary.TryGetValue(boundType, out internalConverter))
                {
                    using (readerLock.GetWriterLock())
                    {
                        internalConverter = (IIdValueConverter) Activator.CreateInstance(boundType);
                        ConverterDictionary.Add(boundType, internalConverter);
                    }
                }
            }

            var result = internalConverter.ReadJson(reader, objectType, existingValue, serializer);

            return result;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        #endregion
    }
}