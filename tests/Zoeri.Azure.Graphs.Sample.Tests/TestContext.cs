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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

namespace Zoeri.Azure.Graphs.Sample.Tests
{
    public class TestContext
        : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="TestContext" /> with the default options.
        /// </summary>
        /// <remarks>
        /// Uses the two fields 1) <see cref="CosmosDbEndpointAppSettingsKey" /> and 2)
        /// <see cref="CosmosDbAuthKeyAppSettingsKey" /> to connect to the data source.
        /// Connects directly to the datasource using TCP.
        /// </remarks>
        public TestContext(string databaseName = null, string collectionId = null, bool isOffline = false)
        {
            //TODO: Configure the test environment to connect to a live graph database. Use appSettings.local.json as a template.
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    //.AddJsonFile("appsettings.json")
                    .AddJsonFile("appsettings.local.json")
                ;

            Configuration = builder.Build();
            var endpoint = Configuration[CosmosDbEndpointAppSettingsKey];
            if (string.IsNullOrWhiteSpace(endpoint)) throw new ArgumentException($"No endpoint was specified.");
            var authKey = Configuration[CosmosDbAuthKeyAppSettingsKey];
            if (string.IsNullOrWhiteSpace(authKey)) throw new ArgumentException($"No authKey was specified.");
            databaseName = databaseName ?? Configuration[CosmosDbDatabaseAppSettingsKey];
            if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException($"No database was specified.");
            collectionId = collectionId ?? Configuration[CosmosDbCollectionIdAppSettingsKey];
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentException($"No collectionId was specified.");

            IsOffline = isOffline;
            CancellationTokenSource = new CancellationTokenSource();

            try
            {
                Client = new DocumentClient(
                    new Uri(endpoint),
                    authKey,
                    new ConnectionPolicy
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol = Protocol.Tcp
                    });

                InitializeAsync(databaseName, collectionId).Wait(CancellationTokenSource.Token);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to initialize the {nameof(TestContext)}.", exception).LogException();
            }
        }

        #endregion Constructors

        #region Fields

        /// <summary>
        /// The key for the name of the configuration settings key the contains the URI to the Cosmos DB endpoint.
        /// </summary>
        public const string CosmosDbEndpointAppSettingsKey = "CosmosDbEndpoint";
        /// <summary>
        /// The key for the configuration setting containing the auth key for the target Cosmos DB service.
        /// </summary>
        public const string CosmosDbAuthKeyAppSettingsKey = "CosmosDbAuthKey";

        /// <summary>
        /// The key for the name of the default graph database with which to connect.
        /// </summary>
        public const string CosmosDbDatabaseAppSettingsKey = "CosmosDbDatabaseName";

        /// <summary>
        /// The key for the name of the default collection against which to execute operations.
        /// </summary>
        public const string CosmosDbCollectionIdAppSettingsKey = "CosmosDbCollectionId";

        /// <summary>
        /// The key for the format string based upon which test email addresses are based.
        /// </summary>
        public const string TestUserEmailFormatAppSettingsKey = "TestUserEmailFormat";

        /// <summary>
        /// They key for the phone number to which test messages are sent.
        /// </summary>
        public const string TestUserPhoneNumberKey = "TestUserPhoneNumber";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the <see cref="IConfigurationRoot" /> holding application settings.
        /// </summary>
        public IConfigurationRoot Configuration
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="DocumentClient" /> used to execute graph operations.
        /// </summary>
        public DocumentClient Client
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="Database" /> used to execute graph operations.
        /// </summary>
        public Database Database
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="DocumentCollection" /> used to execute graph operations.
        /// </summary>
        public DocumentCollection Collection
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="CancellationTokenSource" /> used to cancel any pending operations.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource
        {
            get;
        }

        public bool IsOffline
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Conducts Cosmos DB connection setup.
        /// </summary>
        /// <param name="databaseName">The name of the database to connect to.</param>
        /// <param name="collectionId">The name of the collection/graph against which operations will execute.</param>
        /// <returns>A <see cref="Task" /> that tracks the initialization process.</returns>
        private async Task InitializeAsync(string databaseName, string collectionId)
        {
            if (!IsOffline)
            {
                await Client.OpenAsync(CancellationTokenSource.Token);
            }

            try
            {
                Database = Client.CreateDatabaseQuery().Where(db => db.Id == databaseName).ToList().First();
                Console.WriteLine($"Got database {Database}");
            }
            catch (Exception exception)
            {
                throw new Exception($"Could not find a database named {databaseName}.", exception);
            }

            try
            {
                Collection =
                    await Client.ReadDocumentCollectionAsync(
                        UriFactory.CreateDocumentCollectionUri(databaseName, collectionId));
                Console.WriteLine($"Got collection {Collection}");
            }
            catch (Exception exception)
            {
                throw new Exception($"Could not find a collection with id {collectionId}.", exception);
            }
        }

        /// <inheritdoc />
        /// <summary>Disposes the <see cref="DocumentClient" /> used to create the context.</summary>
        public void Dispose()
        {
            Client.Dispose();
        }

        /// <summary>
        /// Gets an email address that varies based on the specified <paramref name="variable" />.
        /// </summary>
        /// <param name="variable">A user-defined string to insert into the template email address to make it unique to the user.</param>
        /// <returns>A fully constructed email address.</returns>
        public string GetTestEmailAddress(string variable)
        {
            var address = string.Format(Configuration[TestUserEmailFormatAppSettingsKey], variable);

            return address;
        }

        /// <summary>
        /// Gets a phone number for the user.
        /// </summary>
        /// <returns>A telephone number.</returns>
        public string GetTestPhoneNumber()
        {
            var number = Configuration[TestUserPhoneNumberKey];

            return number;
        }

        public Model.User GenerateTestUser()
        {
            var userId = Guid.NewGuid().ToString().ToLower();

            var originalUser = new Model.User
            {
                Id = userId,
                DisplayName = "Test User One",
                Salutation = "Test User",
                Balance = 185,
                Email = GetTestEmailAddress(userId),
                MonthlyPayment = 100,
                LastPaymentAmount = 70,
                LastPaymentDate = "12/01/2017",
                MobilePhone = GetTestPhoneNumber()
            };

            return originalUser;
        }

        #endregion Methods
    }
}