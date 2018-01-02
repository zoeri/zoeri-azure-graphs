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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Graphs;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zoeri.Azure.Graphs
{
    /// <summary>
    /// Provides extension methods for <see cref="DocumentClient" /> that streamline CRUD operations against a Cosmos DB graph
    /// using the Gremlin language.
    /// </summary>
    public static class DocumentClientExtensions
    {
        /// <summary>
        /// Deletes the vertex with the specified <paramref name="vertexId" />.
        /// </summary>
        /// <param name="client">The <see cref="DocumentClient" /> using to connect to the data source.</param>
        /// <param name="collection">The <see cref="DocumentCollection" /></param>
        /// against which to perform the delete.
        /// <param name="vertexId">The id of the vertex to delete.</param>
        /// <returns>A task yielding a reference to the specified <paramref name="client" />. This is useful for fluid use cases.</returns>
        public static async Task<DocumentClient> DeleteVertexAsync
        (
            this DocumentClient client,
            DocumentCollection collection,
            string vertexId
        )
        {
            await client.DeleteVertexAsync(collection, vertexId, CancellationToken.None);

            return client;
        }

        /// <summary>
        /// Deletes the vertex with the specified <paramref name="vertexId" />.
        /// </summary>
        /// <param name="client">The <see cref="DocumentClient" /> using to connect to the data source.</param>
        /// <param name="collection">The <see cref="DocumentCollection" /></param>
        /// against which to perform the delete.
        /// <param name="vertexId">The id of the vertex to delete.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> used to abort execution.</param>
        /// <returns>A task yielding a reference to the specified <paramref name="client" />. This is useful for fluid use cases.</returns>
        public static async Task<DocumentClient> DeleteVertexAsync
        (
            this DocumentClient client,
            DocumentCollection collection,
            string vertexId,
            CancellationToken cancellationToken
        )
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (string.IsNullOrWhiteSpace(vertexId)) throw new ArgumentNullException(nameof(vertexId));

            try
            {
                var gremlinScript = $"g.V('{vertexId}').drop()";
                await SubmitWithSingleResultAsync<Vertex>(client, collection, gremlinScript, cancellationToken,
                    allowNullResult: true);

                return client;
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to delete a vertex from the graph.", exception);
            }
        }

        /// <summary>
        /// Updates the specified <paramref name="vertex" />.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex.</typeparam>
        /// <param name="client">The <see cref="DocumentClient" /> using to connect to the data source.</param>
        /// <param name="collection">The <see cref="DocumentCollection" /></param>
        /// against which to perform the update.
        /// <param name="vertex">The <see cref="IVertex" /> to update.</param>
        /// <returns>A task yielding a reference to the specified <paramref name="client" />. This is useful for fluid use cases.</returns>
        public static async Task<TVertex> UpdateVertexAsync<TVertex>
        (
            this DocumentClient client,
            DocumentCollection collection,
            TVertex vertex
        )
            where TVertex : class, IVertex, new()
        {
            return await client.UpdateVertexAsync(collection, vertex, CancellationToken.None);
        }

        /// <summary>
        /// Updates the specified <paramref name="vertex" />.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex.</typeparam>
        /// <param name="client">The <see cref="DocumentClient" /> using to connect to the data source.</param>
        /// <param name="collection">The <see cref="DocumentCollection" /></param>
        /// against which to perform the update.
        /// <param name="vertex">The <see cref="IVertex" /> to update.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> used to abort execution.</param>
        /// <returns>A task yielding a reference to the specified <paramref name="client" />. This is useful for fluid use cases.</returns>
        public static async Task<TVertex> UpdateVertexAsync<TVertex>
        (
            this DocumentClient client,
            DocumentCollection collection,
            TVertex vertex,
            CancellationToken cancellationToken
        )
            where TVertex : class, IVertex, new()
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            try
            {
                var updateGremlinScript = $"{vertex.ToUpdateVertexCommand()}";

                var result = await client.SubmitWithSingleResultAsync<TVertex>(collection, updateGremlinScript,
                    cancellationToken, allowNullResult: false);

                return result;
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to update a vertex in the graph.", exception);
            }
        }

        /// <summary>
        /// Adds an edge with the specified <paramref name="edgeLabel" /> between the specified <paramref name="source" /> vertex
        /// to the specified <paramref name="target" /> vertex to the specified <paramref name="collection" />.
        /// </summary>
        /// <typeparam name="TSource">The type of the source vertex.</typeparam>
        /// <typeparam name="TTarget">The type of the target vertex.</typeparam>
        /// <param name="client">The <see cref="DocumentClient" /> using to connect to the data source.</param>
        /// <param name="collection">The <see cref="DocumentCollection" /></param>
        /// against which to perform the add.
        /// <param name="source">The source <see cref="IVertex" /> from which the edge will extend.</param>
        /// <param name="edgeLabel">The label of the edge to add.</param>
        /// <param name="target">The target <see cref="IVertex" /> to which the edge will extend.</param>
        /// <returns>A task yielding a reference to the specified <paramref name="client" />. This is useful for fluid use cases.</returns>
        public static async Task<DocumentClient> AddEdgeAsync<TSource, TTarget>
        (
            this DocumentClient client,
            DocumentCollection collection,
            TSource source,
            string edgeLabel,
            TTarget target
        )
            where TSource : class, IVertex, new()
            where TTarget : class, IVertex, new()
        {
            await client.AddEdgeAsync(collection, source, edgeLabel, target, CancellationToken.None);

            return client;
        }

        /// <summary>
        /// Adds an edge with the specified <paramref name="edgeLabel" /> between the specified <paramref name="source" /> vertex
        /// to the specified <paramref name="target" /> vertex to the specified <paramref name="collection" />.
        /// </summary>
        /// <typeparam name="TSource">The type of the source vertex.</typeparam>
        /// <typeparam name="TTarget">The type of the target vertex.</typeparam>
        /// <param name="client">The <see cref="DocumentClient" /> using to connect to the data source.</param>
        /// <param name="collection">The <see cref="DocumentCollection" /></param>
        /// against which to perform the add.
        /// <param name="source">The source <see cref="IVertex" /> from which the edge will extend.</param>
        /// <param name="edgeLabel">The label of the edge to add.</param>
        /// <param name="target">The target <see cref="IVertex" /> to which the edge will extend.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> used to abort execution.</param>
        /// <returns>A task yielding a reference to the specified <paramref name="client" />. This is useful for fluid use cases.</returns>
        public static async Task<DocumentClient> AddEdgeAsync<TSource, TTarget>
        (
            this DocumentClient client,
            DocumentCollection collection,
            TSource source,
            string edgeLabel,
            TTarget target,
            CancellationToken cancellationToken
        )
            where TSource : class, IVertex, new()
            where TTarget : class, IVertex, new()
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(edgeLabel)) throw new ArgumentNullException(nameof(edgeLabel));
            if (target == null) throw new ArgumentNullException(nameof(target));

            try
            {
                //TODO: Refactor to reuse the code in SubmitWithSingleResultAsync.
                var addGremlinScript = source.ToAddEdgeCommand(edgeLabel, target);

                var query = client.CreateGremlinQuery<Document>(collection, addGremlinScript, new FeedOptions());

                while (query.HasMoreResults)
                {
                    var addResponse = await query.ExecuteNextAsync(cancellationToken);
                    var createdDocument = addResponse.Single();
                    var serializedDocument = JsonConvert.SerializeObject(createdDocument);
                    Console.WriteLine(serializedDocument);

                    break;
                }

                return client;
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to add a vertex to the graph.", exception);
            }
        }

        /// <summary>
        /// Builds a Gremlin script that will add an edge with label <paramref name="edgeLabel" /> from the
        /// <paramref name="source" /> <see cref="IVertex" /> to the target <see cref="IVertex" />.
        /// </summary>
        /// <typeparam name="TSource">The type of the source vertex.</typeparam>
        /// <typeparam name="TTarget">The type of the target vertex.</typeparam>
        /// <param name="source">The source <see cref="IVertex" />.</param>
        /// <param name="edgeLabel">The label of the edge to add between the source and target.</param>
        /// <param name="target">The target <see cref="IVertex" />.</param>
        /// <returns>A gremlin script that will add the specified edge once executed.</returns>
        public static string ToAddEdgeCommand<TSource, TTarget>(this TSource source, string edgeLabel, TTarget target)
            where TSource : class, IVertex, new()
            where TTarget : class, IVertex, new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(edgeLabel)) throw new ArgumentNullException(nameof(edgeLabel));
            if (target == null) throw new ArgumentNullException(nameof(target));

            try
            {
                var addEdgeScript = $"g.V('{source.Id}').addE('{edgeLabel}').to(g.V('{target.Id}'))";

                return addEdgeScript;
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to construct a Gremlin add edge script.", exception);
            }
        }

        /// <summary>
        /// Gets a vertex with the specified <paramref name="vertexId" />.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex to get.</typeparam>
        /// <param name="client">The <see cref="DocumentClient" /> using to connect to the data source.</param>
        /// <param name="collection">The <see cref="DocumentCollection" /></param>
        /// against which to perform the lookup.
        /// <param name="vertexId">The id of the vertex to get.</param>
        /// <returns>A task yielding the retrieved instance of <typeparamref name="TVertex"></typeparamref>.</returns>
        public static async Task<TVertex> GetVertexAsync<TVertex>
        (
            this DocumentClient client,
            DocumentCollection collection,
            string vertexId
        )
            where TVertex : class, IVertex, new()
        {
            return await client.GetVertexAsync<TVertex>(collection, vertexId, CancellationToken.None);
        }

        /// <summary>
        /// Gets a vertex with the specified <paramref name="vertexId" />.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex to get.</typeparam>
        /// <param name="client">The <see cref="DocumentClient" /> using to connect to the data source.</param>
        /// <param name="collection">The <see cref="DocumentCollection" /></param>
        /// against which to perform the lookup.
        /// <param name="vertexId">The id of the vertex to get.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> used to abort execution.</param>
        /// <returns>A task yielding the retrieved instance of <typeparamref name="TVertex"></typeparamref>.</returns>
        public static async Task<TVertex> GetVertexAsync<TVertex>
        (
            this DocumentClient client,
            DocumentCollection collection,
            string vertexId,
            CancellationToken cancellationToken
        )
            where TVertex : class, IVertex, new()
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (string.IsNullOrWhiteSpace(vertexId)) throw new ArgumentNullException(nameof(vertexId));

            try
            {
                var gremlinScript = $"g.V('{vertexId}')";
                var vertex = await SubmitWithSingleResultAsync<TVertex>(client, collection, gremlinScript,
                    cancellationToken, allowNullResult: true);

                return vertex;
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to get a vertex.", exception);
            }
        }

        /// <summary>
        /// Adds the specified <paramref name="vertex" /> to the specified <paramref name="collection" />.
        /// </summary>
        /// <typeparam name="TVertex"></typeparam>
        /// <param name="client">The <see cref="DocumentClient" /> using to connect to the data source.</param>
        /// <param name="collection">The <see cref="DocumentCollection" /></param>
        /// against which to perform the add.
        /// <param name="vertex">The <see cref="IVertex" /> to add to the specified <paramref name="collection" />.</param>
        /// <returns>The added vertex.</returns>
        public static async Task<TVertex> AddVertexAsync<TVertex>
        (
            this DocumentClient client,
            DocumentCollection collection,
            TVertex vertex
        )
            where TVertex : class, IVertex, new()
        {
            return await client.AddVertexAsync(collection, vertex, CancellationToken.None);
        }

        /// <summary>
        /// Adds the specified <paramref name="vertex" /> to the specified <paramref name="collection" />.
        /// </summary>
        /// <typeparam name="TVertex"></typeparam>
        /// <param name="client">The <see cref="DocumentClient" /> using to connect to the data source.</param>
        /// <param name="collection">The <see cref="DocumentCollection" /></param>
        /// against which to perform the add.
        /// <param name="vertex">The <see cref="IVertex" /> to add to the specified <paramref name="collection" />.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> used to abort execution.</param>
        /// <returns>The added vertex.</returns>
        public static async Task<TVertex> AddVertexAsync<TVertex>
        (
            this DocumentClient client,
            DocumentCollection collection,
            TVertex vertex,
            CancellationToken cancellationToken
        )
            where TVertex : class, IVertex, new()
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            try
            {
                var gremlinScript = vertex.ToAddVertexCommand();

                var addedVertex =
                    await SubmitWithSingleResultAsync<TVertex>(client, collection, gremlinScript, cancellationToken);

                return addedVertex;
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to add a vertex to the graph.", exception);
            }
        }

        /// <summary>
        /// Builds a Gremlin script that will update the specified <paramref name="vertex" /> using the data provided therein.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex to add.</typeparam>
        /// <param name="vertex">The vertex whose properties will be used to construct the update command.</param>
        /// <returns>A Gremlin script that will update the specified <paramref name="vertex" /> when executed.</returns>
        public static string ToUpdateVertexCommand<TVertex>(this TVertex vertex)
            where TVertex : class, IVertex, new()
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            try
            {
                var jVertex = JObject.FromObject(vertex);
                var builder = new StringBuilder();
                builder.Append($"g.V('{vertex.Id}')");

                AppendProperties(jVertex, builder, false);

                return builder.ToString();
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to convert an instance of {vertex.GetType()} to a Gremlin add script.",
                    exception);
            }
        }

        /// <summary>
        /// Builds a Gremlin script that will update the specified <paramref name="vertex" /> using the data provided therein.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex to add.</typeparam>
        /// <param name="vertex">The vertex whose properties will be used to construct the add command.</param>
        /// <returns></returns>
        public static string ToAddVertexCommand<TVertex>(this TVertex vertex)
            where TVertex : class, IVertex, new()
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            try
            {
                var jVertex = JObject.FromObject(vertex);
                var builder = new StringBuilder();
                builder.Append($"g.addV('{vertex.Label}')");

                AppendProperties(jVertex, builder);

                return builder.ToString();
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to convert an instance of {vertex.GetType()} to a Gremlin add script.",
                    exception);
            }
        }

        private static void AppendProperties(JObject deserializedVertex, StringBuilder builder, bool includeId = true)
        {
            foreach (var property in deserializedVertex.Properties())
            {
                if (property.Name == Vertex.TypePropertyName
                    || property.Name == Vertex.LabelPropertyName
                    || property.Name == Vertex.PropertiesPropertyName
                    || property.Name == Vertex.IdPropertyName && !includeId
                )
                {
                    continue;
                }

                AppendProperty(builder, property);
            }
        }

        /// <summary>
        /// Adds a property to a vertex in the Gremlin language.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to which to write the serialized property.</param>
        /// <param name="property">THe <see cref="JProperty" /> whose name and value will be used to construct the Gremlin snippet.</param>
        public static void AppendProperty(StringBuilder builder, JProperty property)
        {
            if (property.Value.Type == JTokenType.Boolean
                || property.Value.Type == JTokenType.Float
                || property.Value.Type == JTokenType.Integer
                || property.Value.Type == JTokenType.Null
                || property.Value.Type == JTokenType.Array
                || property.Value.Type == JTokenType.None
                || property.Value.Type == JTokenType.Undefined)
            {
                var propertyValue = property.Value.Type == JTokenType.Boolean ? property.Value.ToString().ToLower()
                    : property.Value;

                builder.Append($".property('{property.Name}', {propertyValue})");
            }
            else
            {
                builder.Append($".property('{property.Name}', '{property.Value.ToString()}')");
            }
        }

        /// <summary>
        /// Executes the specified <paramref name="gremlinScript" /> against the specified <paramref name="collection" />.
        /// </summary>
        /// <typeparam name="TVertex">The type of <see cref="IVertex" /> to manipulate.</typeparam>
        /// <param name="client"></param>
        /// <param name="collection"></param>
        /// <param name="gremlinScript"></param>
        /// <param name="allowNullResult"></param>
        /// <returns></returns>
        public static async Task<TVertex> SubmitWithSingleResultAsync<TVertex>
        (
            this DocumentClient client,
            DocumentCollection collection,
            string gremlinScript,
            bool allowNullResult = false
        )
            where TVertex : class, IVertex, new()
        {
            return await client.SubmitWithSingleResultAsync<TVertex>(collection, gremlinScript, CancellationToken.None,
                allowNullResult);
        }

        /// <summary>
        /// Executes the specified <paramref name="gremlinScript" /> against the specified <paramref name="collection" />.
        /// </summary>
        /// <typeparam name="TVertex">The type of <see cref="IVertex" /> to manipulate.</typeparam>
        /// <param name="client"></param>
        /// <param name="collection"></param>
        /// <param name="gremlinScript"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="allowNullResult"></param>
        /// <returns></returns>
        public static async Task<TVertex> SubmitWithSingleResultAsync<TVertex>
        (
            this DocumentClient client,
            DocumentCollection collection,
            string gremlinScript,
            CancellationToken cancellationToken,
            bool allowNullResult = false
        )
            where TVertex : class, IVertex, new()
        {
            try
            {
                if (client == null) throw new ArgumentNullException(nameof(client));
                if (collection == null) throw new ArgumentNullException(nameof(collection));
                if (string.IsNullOrWhiteSpace(gremlinScript)) throw new ArgumentNullException(nameof(gremlinScript));

                var query = client.CreateGremlinQuery<Document>(collection, gremlinScript, new FeedOptions());

                if (!query.HasMoreResults)
                {
                    throw new InvalidOperationException(
                        $"Cannot execute single-vertex script because {nameof(query.HasMoreResults)} is false.");
                }

                var commandResponse = await query.ExecuteNextAsync(cancellationToken);
                var responseDocument = commandResponse.FirstOrDefault();
                if (responseDocument == null)
                {
                    if (!allowNullResult)
                    {
                        throw new InvalidOperationException(
                            $"The Gremlin script executed but did not return any results.");
                    }
                }
                else if (query.HasMoreResults)
                {
                    throw new InvalidOperationException($"More than one result was returned from the Gremlin script.");
                }

                var vertex = default(TVertex);

                if (responseDocument != null)
                {
                    var vertexJson = JsonConvert.SerializeObject(responseDocument);
                    vertex = new TVertex();
                    JsonConvert.PopulateObject(vertexJson, vertex);
                    if (vertex.Properties != null)
                    {
                        var propertiesJson = JsonConvert.SerializeObject(vertex.Properties);
                        JsonConvert.PopulateObject(propertiesJson, vertex);
                    }
                }

                return vertex;
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to execute a single-result Gremlin query.", exception);
            }
        }
    }
}