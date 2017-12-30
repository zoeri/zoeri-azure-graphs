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

namespace Zoeri.Azure.Graphs.Sample.Model
{
    [JsonObject(JsonContainerId)]
    public class Bill
        : Vertex
    {
        public const string JsonContainerId = "bill";

        public Bill()
        {
            Label = JsonContainerId;
        }

        [JsonConverter(typeof(VertexPropertyConverter))]
        [JsonProperty("amount")]
        public double Amount
        {
            get;
            set;
        }

        [JsonConverter(typeof(VertexPropertyConverter))]
        [JsonProperty("dueDate")]
        public string DueDate
        {
            get;
            set;
        }

        [JsonConverter(typeof(VertexPropertyConverter))]
        [JsonProperty("displayName")]
        public string DisplayName
        {
            get;
            set;
        }

        [JsonConverter(typeof(VertexPropertyConverter))]
        [JsonProperty("paymentRequested")]
        public bool PaymentRequested
        {
            get;
            set;
        }

        [JsonConverter(typeof(VertexPropertyConverter))]
        [JsonProperty("payorSalutation")]
        public string PayorSalutation
        {
            get;
            set;
        }

        [JsonConverter(typeof(VertexPropertyConverter))]
        [JsonProperty("payorEmail")]
        public string PayorEmail
        {
            get;
            set;
        }

        [JsonConverter(typeof(VertexPropertyConverter))]
        [JsonProperty("payorFullName")]
        public string PayorFullName
        {
            get;
            set;
        }
    }
}