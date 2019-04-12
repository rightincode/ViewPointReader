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
                _databaseConnection.CreateTableAsync<FeedSubscriptionDo>().Wait();
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

        public Task<int> SaveFeedSubscriptionAsync(IFeedSubscription feedSubscription)
        {
            if (feedSubscription.Id > 0)
            {
                return _databaseConnection.UpdateAsync(TransformToSubscriptionDo(feedSubscription));
            }
            else
            {
                return _databaseConnection.InsertAsync(TransformToSubscriptionDo(feedSubscription));
            }
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
                SubscribedDate = feedSubscription.SubscribedDate
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
                SubscribedDate = feedSubscriptionDo.SubscribedDate
            };

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
            return commaDelimitedString.Split(',').ToList();
        }
    }
}
