using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Core.Models;
using ViewPointReader.Data.Interfaces;

namespace ViewPointReader.Data.Models
{
    public class ViewPointReaderRepository: IViewPointReaderRepository
    {
        private SQLiteAsyncConnection _databaseConnection;

        public ViewPointReaderRepository(IFileHelper fileHelper)
        {
            _databaseConnection = new SQLiteAsyncConnection(fileHelper.GetLocalFilePath("ViewPointReaderRepository.db3"));

            try
            {
                _databaseConnection.CreateTablesAsync<FeedSubscriptionDo, VprFeedItemDo>().Wait();
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public Task<int> DeleteFeedSubscriptionAsync(IFeedSubscription feedSubscription)
        {
            return _databaseConnection.DeleteAsync(TransformToSubscriptionDo(feedSubscription));
        }

        public Task<FeedSubscription> GetFeedSubscriptionAsync(int id)
        {
            return Task.Run(async () =>
            {
                FeedSubscriptionDo feedSubscriptionDo =
                    await _databaseConnection.Table<FeedSubscriptionDo>().Where(i => i.Id == id).FirstOrDefaultAsync();

                return TransformToFeedSubscription(feedSubscriptionDo);
            });
        }

        public Task<List<FeedSubscription>> GetFeedSubscriptionsAsync()
        {
            return Task.Run(async () =>
            {
                List<FeedSubscriptionDo> feedSubscriptionDos =
                    await _databaseConnection.Table<FeedSubscriptionDo>().OrderByDescending(x => x.SubscribedDate).ToListAsync();

                List<FeedSubscription> feedSubscriptions = new List<FeedSubscription>();

                foreach (FeedSubscriptionDo feedSubscriptionDo in feedSubscriptionDos)
                {
                    feedSubscriptions.Add(TransformToFeedSubscription(feedSubscriptionDo));
                }

                return feedSubscriptions;
            });
        }

        public async Task<int> SaveFeedSubscriptionAsync(IFeedSubscription feedSubscription)
        {
            var feedSubscriptionDo = TransformToSubscriptionDo(feedSubscription);

            if (feedSubscription.Id > 0)
            {
                await _databaseConnection.UpdateAsync(feedSubscriptionDo);
            }
            else
            {
                await _databaseConnection.InsertAsync(feedSubscriptionDo);
            }

            if (feedSubscription.FeedItems.Any())
            {
                await SaveFeedItems(feedSubscription.FeedItems, feedSubscriptionDo.Id);
            }

            return feedSubscription.Id;
        }

        //TODO: Research bulk insert option, transaction?
        private Task SaveFeedItems(List<VprFeedItem> vprFeedItems, int subscriptionId)
        {
            return Task.Run(async () =>
            {
                foreach (var vprFeedItem in vprFeedItems)
                {
                    var vprFeedItemDo = new VprFeedItemDo
                    {
                        Id = vprFeedItem.Id,
                        Author = vprFeedItem.Author,
                        Content = vprFeedItem.Content,
                        Description = vprFeedItem.Description,
                        FeedSubscriptionDoId = subscriptionId,
                        Link = vprFeedItem.Link,
                        PublishingDate = vprFeedItem.PublishingDate,
                        PublishingDateString = vprFeedItem.PublishingDateString,
                        Title = vprFeedItem.Title
                    };

                    if (vprFeedItemDo.Id > 0)
                    {
                        await _databaseConnection.UpdateAsync(vprFeedItemDo);
                    }
                    else
                    {
                        await _databaseConnection.InsertAsync(vprFeedItemDo);
                    }
                }
            });
        }

        private FeedSubscriptionDo TransformToSubscriptionDo(IFeedSubscription feedSubscription)
        {
            var feedSubscriptionDo = new FeedSubscriptionDo
            {
                Id = feedSubscription.Id,
                Title = feedSubscription.Title,
                Description = feedSubscription.Description,
                Url = feedSubscription.Url,
                ImageUrl = feedSubscription.ImageUrl,
                LastUpdated = feedSubscription.LastUpdated,
                KeyPhrases = BuildCommaDelimitedStringFromStringList(feedSubscription.KeyPhrases),
                SubscribedDate = feedSubscription.SubscribedDate,
            };

            return feedSubscriptionDo;
        }

        private FeedSubscription TransformToFeedSubscription(IFeedSubscriptionDo feedSubscriptionDo)
        {
            var feedSubscription = new FeedSubscription
            {
                Id = feedSubscriptionDo.Id,
                Title = feedSubscriptionDo.Title,
                Description = feedSubscriptionDo.Description,
                Url = feedSubscriptionDo.Url,
                ImageUrl = feedSubscriptionDo.ImageUrl,
                LastUpdated = feedSubscriptionDo.LastUpdated,
                KeyPhrases = BuildStringListFromCommaDelimitedString(feedSubscriptionDo.KeyPhrases),
                SubscribedDate = feedSubscriptionDo.SubscribedDate,
                //FeedItems = new List<VprFeedItem>()
                
            };

            //if (feedSubscriptionDo.FeedItems == null) return feedSubscription;
            //foreach (var feedItem in feedSubscriptionDo.FeedItems)
            //{
            //    var vprFeedItem = new VprFeedItem
            //    {
            //        Id = feedItem.Id,
            //        Author = feedItem.Author,
            //        Categories = feedItem.Categories,
            //        Content = feedItem.Content,
            //        Description = feedItem.Description,
            //        FeedSubscriptionDoId = feedItem.FeedSubscriptionDoId,
            //        Link = feedItem.Link,
            //        PublishingDate = feedItem.PublishingDate,
            //        PublishingDateString = feedItem.PublishingDateString,
            //        Title = feedItem.Title
            //    };

            //    feedSubscription.FeedItems.Add(vprFeedItem);
            //}

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
