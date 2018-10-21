namespace BFT.AzureFuncApp.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;

    public class DatabaseService : IDisposable
    {
        public readonly string DatabaseId;

        private readonly DocumentClient Client;

        public DatabaseService(string endpointUrl, string accountKey, string databaseId)
        {
            if (string.IsNullOrWhiteSpace(endpointUrl))
            {
                throw new ArgumentNullException(nameof(endpointUrl));
            }

            if (string.IsNullOrWhiteSpace(accountKey))
            {
                throw new ArgumentNullException(nameof(accountKey));
            }

            DatabaseId = databaseId ?? throw new ArgumentNullException(nameof(databaseId));

            var uri = new Uri(endpointUrl);
            Client = new DocumentClient(uri, accountKey);
        }

        public async Task<string> InsertAsync(string collectionId, object entity)
        {
            if (string.IsNullOrWhiteSpace(collectionId))
            {
                throw new ArgumentNullException(nameof(collectionId));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await EnsureCreatedAsync(collectionId);

            var uri = GetDocumentCollectionUri(collectionId);

            var response = await Client.CreateDocumentAsync(uri, entity).ConfigureAwait(false);

            return response.Resource.Id;
        }

        private async Task EnsureCreatedAsync(string collectionId)
        {
            await CreateDatabaseIfNotExistsAsync();
            await CreateCollectionIfNotExistsAsync(collectionId);
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            var database = new Database() { Id = DatabaseId };

            await Client.CreateDatabaseIfNotExistsAsync(database).ConfigureAwait(false);
        }

        private async Task CreateCollectionIfNotExistsAsync(string collectionId)
        {
            var uri = GetDatabaseUri();
            var collection = new DocumentCollection() { Id = collectionId };
            var options = new RequestOptions() { OfferThroughput = 2500 };

            await Client.CreateDocumentCollectionIfNotExistsAsync(uri, collection, options).ConfigureAwait(false);
        }

        private Uri GetDatabaseUri()
        {
            return UriFactory.CreateDatabaseUri(DatabaseId);
        }

        private Uri GetDocumentCollectionUri(string collectionId)
        {
            return UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionId);
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}