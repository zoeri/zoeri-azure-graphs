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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zoeri.Azure.Graphs
{
    /// <summary>
    /// An convenient implementation of <see cref="IVertex" /> from which entity types can derive. This is an optional base
    /// class only.
    /// </summary>
    public class Vertex
        : IVertex
    {
        /// <summary>
        /// The name of the id property on a vertex.
        /// </summary>
        public const string IdPropertyName = "id";
        /// <summary>
        /// The name of the id property on a vertex.
        /// </summary>
        public const string LabelPropertyName = "label";
        /// <summary>
        /// The name of the type property on a vertex.
        /// </summary>
        public const string TypePropertyName = "type";
        /// <summary>
        /// The name of the properties property on a vertex.
        /// </summary>
        public const string PropertiesPropertyName = "properties";
        /// <summary>
        /// The name of the type property on a vertex.
        /// </summary>
        public const string TypePropertyValue = "vertex";

        /// <inheritdoc />
        [JsonProperty(IdPropertyName)]
        public string Id
        {
            get;
            set;
        }

        /// <inheritdoc />
        [JsonProperty(LabelPropertyName)]
        public string Label
        {
            get;
            protected set;
        }

        /// <inheritdoc />
        [JsonProperty(TypePropertyName)]
        public string Type
        {
            get;
            protected set;
        } = TypePropertyValue;

        /// <inheritdoc />
        [JsonProperty(PropertiesPropertyName)]
        public JObject Properties
        {
            get;
            protected set;
        }
    }
}