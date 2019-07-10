using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.ML;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Core.Models;
using ViewPointReader.Data.Interfaces;

namespace ViewPointReader.ModelBuilder
{
    public class ModelBuilder
    {
        private readonly string _storageAccountConnectionString =
            "DefaultEndpointsProtocol=https;AccountName=viewpointreaderstorage;AccountKey=RY44OOYQwseuLJn3o3JAvqyf7qWnkQH1fFJlxtpQNnWHBJm9lBIbU2dO7Pv07TBu36EN5bOBOmcW485xV1xwQQ==;EndpointSuffix=core.windows.net";

        private readonly string _viewPointReaderContainerName = "viewpointreader";
        private CloudStorageAccount _cloudStorageAccount;
        private CloudBlobClient _cloudBlobClient;
        private CloudBlobContainer _cloudBlobContainer;


        private readonly IViewPointReaderRepository _feedRepository;
        private ITransformer _trainedModel;

        public ModelBuilder()
        {
        }

        public ModelBuilder(IViewPointReaderRepository feedRepository)
        {
            _feedRepository = feedRepository;
            CreateCloudStorageAccountFromConnectionString();
        }

        public async Task BuildModel()
        {
            var createBlobContainerTask = CreateBlobContainer();

            MLContext mlContext = new MLContext();

            try
            {
                var sourceData = await _feedRepository.GetFeedSubscriptionsAsync();
                IDataView trainingData = mlContext.Data.LoadFromEnumerable<FeedData>(FormatFeedData(sourceData));
                //trainingData = mlContext.Data.Cache(trainingData);

                #region not used

                //var pipeline = mlContext.Transforms.Text.FeaturizeText("idFeaturized", nameof(FeedData.Id))
                //        .Append(mlContext.Transforms.Text.FeaturizeText("keyPhrasesFeaturized", nameof(FeedData.KeyPhrases))
                //        .Append(mlContext.Transforms.Concatenate("Features", "idFeaturized", "keyPhrasesFeaturized"))
                //        .Append(mlContext.BinaryClassification.Trainers.FieldAwareFactorizationMachine(new string[]
                //            {"Features"}))); 

                #endregion

                var pipeline = mlContext.Transforms.Text
                    .FeaturizeText("keyPhrasesFeaturized", nameof(FeedData.KeyPhrases))
                    .Append(mlContext.Transforms.Concatenate("Features", "keyPhrasesFeaturized"))
                    .Append(mlContext.BinaryClassification.Trainers.FieldAwareFactorizationMachine(new string[]
                        {"Features"}));

                //train model
                var model = pipeline.Fit(trainingData);

                #region manual testing of model

                //test model

                //var predictionEngine = mlContext.Model.CreatePredictionEngine<FeedData, FeedRecommendation>(model);

                //var testFeedData = new FeedData
                //{
                //    KeyPhrases = new string[] {"skating"}
                //};

                //var testPrediction = predictionEngine.Predict(testFeedData);

                #endregion

                //save model
                var modelMemoryStream = new MemoryStream();
                mlContext.Model.Save(model, trainingData.Schema, modelMemoryStream);
                modelMemoryStream.Seek(0, SeekOrigin.Begin);

                await createBlobContainerTask;
                var cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference("vprrecommendationmodel-mdl");
                await cloudBlockBlob.UploadFromStreamAsync(modelMemoryStream);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task LoadModel()
        {
            MLContext mlContext = new MLContext();

            var modelMemoryStream = new MemoryStream();
            await CreateBlobContainer();
            var cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference("vprrecommendationmodel-mdl");
            await cloudBlockBlob.DownloadToStreamAsync(modelMemoryStream);

            try
            {

                _trainedModel = mlContext.Model.Load(modelMemoryStream, out var modelInputSchema);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<float> ScoreFeed(IFeedSubscription feedSubscription)
        {
            if (!feedSubscription.KeyPhrases.Any()) return 0;

            await LoadModel();

            var testFeedData = new FeedData
            {
                KeyPhrases = feedSubscription.KeyPhrases.ToArray()
            };

            try
            {
                MLContext mlContext = new MLContext();
                var predictionEngine =
                    mlContext.Model.CreatePredictionEngine<FeedData, FeedRecommendation>(_trainedModel);

                var prediction = predictionEngine.Predict(testFeedData);
                return prediction.Score;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return 0;
        }

        private FeedData[] FormatFeedData(List<FeedSubscription> sourceData)
        {
            var results = new List<FeedData>();

            foreach (var feedSubscription in sourceData)
            {
                results.Add(new FeedData
                {
                    //Id = feedSubscription.Id,
                    //Title = feedSubscription.Title,
                    //Description = feedSubscription.Description,
                    KeyPhrases = feedSubscription.KeyPhrases.ToArray(),
                    //SubscribedDate = feedSubscription.SubscribedDate,
                    //Url = feedSubscription.Url,
                    //ImageUrl = feedSubscription.ImageUrl
                    //LastUpdated = feedSubscription.LastUpdated
                });
            }

            return results.ToArray();
        }

        private void CreateCloudStorageAccountFromConnectionString()
        {
            try
            {
                _cloudStorageAccount = CloudStorageAccount.Parse(_storageAccountConnectionString);
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

        private async Task CreateBlobContainer()
        {
            _cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();
            _cloudBlobContainer = _cloudBlobClient.GetContainerReference(_viewPointReaderContainerName);

            await _cloudBlobContainer.CreateIfNotExistsAsync();

            // Set the permissions so the blobs are public.
            var permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await _cloudBlobContainer.SetPermissionsAsync(permissions);
        }

    }
}