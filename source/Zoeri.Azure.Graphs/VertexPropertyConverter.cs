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
    /// <summary>
    /// Implements a strongly-typed JSON serialization and deserialization functionality to properties on any object decorated
    /// with the <see cref="JsonPropertyAttribute" />.
    /// </summary>
    public class VertexPropertyConverter
        : JsonConverter
    {
        private static Dictionary<Type, IInternalGenericConverter> ConverterDictionary
        {
            get;
        } = new Dictionary<Type, IInternalGenericConverter>();

        private static ReaderWriterLockSlim ConverterDictionaryLock
        {
            get;
        } = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

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
            var unboundType = typeof(InternalGenericConverter<>);
            var boundType = unboundType.MakeGenericType(objectType);
            IInternalGenericConverter internalConverter;

            using (var readerLock = new UpgradeableReaderLock(ConverterDictionaryLock))
            {
                if (!ConverterDictionary.TryGetValue(boundType, out internalConverter))
                {
                    using (readerLock.GetWriterLock())
                    {
                        internalConverter = (IInternalGenericConverter) Activator.CreateInstance(boundType);
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

        private interface IInternalGenericConverter
        {
            object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer);

            void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);
        }

        private class InternalGenericConverter<T>
            : IInternalGenericConverter
        {
            private const string IdPropertyName = "id";
            private const string ValuePropertyName = "value";

            #region Implementation of IInternalGenericConverter

            public void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteStartArray();
                writer.WriteStartObject();
                var typedValue = (VertexProperty<T>) value;
                writer.WritePropertyName(IdPropertyName);
                writer.WriteValue(typedValue.Id);
                writer.WritePropertyName(ValuePropertyName);
                writer.WriteValue(typedValue.Value);
                writer.WriteEndObject();
                writer.WriteEndArray();
            }

            public object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.StartArray)
                {
                    return null;
                }

                var nestedObjectRead = reader.Read();
                if (!nestedObjectRead)
                {
                    throw new Exception($"Could not read a nested object.");
                }

                if (reader.TokenType != JsonToken.StartObject)
                {
                    return null;
                }

                nestedObjectRead = reader.Read();

                if (!nestedObjectRead)
                {
                    return null;
                }

                var vertexProperty = new VertexProperty<T>();

                if ((string) reader.Value == IdPropertyName)
                {
                    vertexProperty.Id = reader.ReadAsString();
                }

                nestedObjectRead = reader.Read();

                if (!nestedObjectRead)
                {
                    return vertexProperty;
                }

                var result = default(T);

                if ((string) reader.Value == ValuePropertyName)
                {
                    nestedObjectRead = reader.Read();
                    result = (T) Convert.ChangeType(reader.Value, typeof(T));
                }

                //EndObject
                nestedObjectRead = reader.Read();
                //EndArray
                nestedObjectRead = reader.Read();

                return result;
            }

            #endregion
        }
    }
}