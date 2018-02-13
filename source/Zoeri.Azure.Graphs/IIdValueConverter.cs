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

using Newtonsoft.Json;

namespace Zoeri.Azure.Graphs
{
    /// <summary>
    /// Reads and writes JSON containing id and value properties as structured for properties in a graph node.
    /// </summary>
    public interface IIdValueConverter
    {
        /// <summary>
        /// Reads from the specified <paramref name="reader" /> a JSON snippet into a <see cref="VertexProperty{T}" />.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader" /> from which to read.</param>
        /// <param name="objectType">The expected type of the property.</param>
        /// <param name="existingValue">Unused.</param>
        /// <param name="serializer">The <see cref="JsonSerializer" /> to use for deserialization.</param>
        /// <returns>
        /// An instance of <see cref="VertexProperty{T}" /> containing the id and value properties read from the JSON or
        /// null if any part of the JSON snippet could not be read as expected.
        /// </returns>
        object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer);

        /// <summary>
        /// Writes the specified <paramref name="value" /> to the specified <paramref name="writer" /> as a JSON snippet.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter" /> to which to right the property JSON.</param>
        /// <param name="value">An instance of <see cref="VertexProperty{T}" /> to write as JSON.</param>
        /// <param name="serializer">The <see cref="JsonSerializer" /> to use for serialization.</param>
        void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);
    }
}