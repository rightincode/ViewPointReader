using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Core.Models;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Data.Models.Cloud;

namespace ViewPointReader.Data.Models
{
    public class ViewPointReaderCloudRepository : IViewPointReaderRepository
    {
        private readonly string _feedSubscriptionsTableName = "FeedSubscriptions";

        private readonly string _storageConnectionString;
        private CloudStorageAccount _cloudStorageAccount;
        private CloudTable _feedSubscriptionsTable;

        public ViewPointReaderCloudRepository(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString;
            CreateStorageAccountFromConnectionString();
        }

        public Task<int> DeleteFeedSubscriptionAsync(IFeedSubscription feedSubscription)
        {
            throw new NotImplementedException();
        }

        public Task<List<VprFeedItem>> GetFeedItemsForFeedAsync(int subscriptionId)
        {
            throw new NotImplementedException();
        }

        public Task<FeedSubscription> GetFeedSubscriptionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FeedSubscription>> GetFeedSubscriptionsAsync()
        {
            if (_feedSubscriptionsTable == null)
            {
                _feedSubscriptionsTable = await CreateTableAsync(_feedSubscriptionsTableName);
            }

            try
            {
                TableQuery<FeedSubscriptionEntity> query = new TableQuery<FeedSubscriptionEntity>();
                var feedSubscriptionsEntities = _feedSubscriptionsTable.ExecuteQuery(query);

                List<FeedSubscription> feedSubscriptions = new List<FeedSubscription>();

                var feedSubscriptionEntities = feedSubscriptionsEntities as FeedSubscriptionEntity[] ?? feedSubscriptionsEntities.ToArray();
                if (feedSubscriptionEntities.Any())
                {
                    feedSubscriptions = feedSubscriptionEntities.Select(TransformToFeedSubscription).ToList();
                }

                return feedSubscriptions;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<int> SaveFeedSubscriptionAsync(IFeedSubscription feedSubscription)
        {
            var newFeedSubscriptionEntity = TransformToFeedSubscriptionEntity(feedSubscription);

            if (_feedSubscriptionsTable == null)
            {
                _feedSubscriptionsTable = await CreateTableAsync(_feedSubscriptionsTableName);
            }

            try
            {
                TableOperation insertMergeOperation = TableOperation.InsertOrMerge(newFeedSubscriptionEntity);
                TableResult result = await _feedSubscriptionsTable.ExecuteAsync(insertMergeOperation);

                return result.HttpStatusCode;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private void CreateStorageAccountFromConnectionString()
        {
            try
            {
                _cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private async Task<CloudTable> CreateTableAsync(string tableName)
        {
            var tableClient = _cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(tableName);

            await table.CreateIfNotExistsAsync();
            return table;
        }

        private FeedSubscriptionEntity TransformToFeedSubscriptionEntity(IFeedSubscription feedSubscription)
        {
            var feedSubscriptionEntity = new FeedSubscriptionEntity
            {
                PartitionKey = feedSubscription.Id.ToString(),
                RowKey = feedSubscription.Title,
                Description = feedSubscription.Description,
                Url = feedSubscription.Url,
                ImageUrl = feedSubscription.ImageUrl,
                LastUpdated = feedSubscription.LastUpdated,
                KeyPhrases = BuildCommaDelimitedStringFromStringList(feedSubscription.KeyPhrases),
                SubscribedDate = feedSubscription.SubscribedDate,
            };

            return feedSubscriptionEntity;
        }

        private FeedSubscription TransformToFeedSubscription(FeedSubscriptionEntity feedSubscriptionEntity)
        {
            var feedSubscription = new FeedSubscription
            {
                Title = feedSubscriptionEntity.RowKey,
                Description = feedSubscriptionEntity.Description,
                Url = feedSubscriptionEntity.Url,
                ImageUrl = feedSubscriptionEntity.ImageUrl,
                LastUpdated = feedSubscriptionEntity.LastUpdated,
                KeyPhrases = BuildStringListFromCommaDelimitedString(feedSubscriptionEntity.KeyPhrases),
                SubscribedDate = feedSubscriptionEntity.SubscribedDate,
            };

            int.TryParse(feedSubscriptionEntity.PartitionKey, out var subscriptionId);
            feedSubscription.Id = (subscriptionId > 0) ? subscriptionId : 0;

            return feedSubscription;
        }
        
        private string BuildCommaDelimitedStringFromStringList(List<string> stringList)
        {
            var result = new StringBuilder();

            if (stringList?.Count > 0)
            {
                stringList.ForEach(x =>
                {
                    result.Append(x);
                    result.Append(",");
                });
            }

            return result.ToString();
        }

        private List<string> BuildStringListFromCommaDelimitedString(string commaDelimitedString)
        {
            var result = commaDelimitedString.Split(',').ToList();

            result.RemoveAll(string.IsNullOrEmpty);

            return result;
        }
    }
}
