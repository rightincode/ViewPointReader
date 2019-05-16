using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Core.Models;
using ViewPointReader.Data;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Data.Models;

namespace ViewPointReader.ModelBuilder
{
    public class ModelBuilder
    {
        private readonly ViewPointReaderRepository _feedRepository;
        private ITransformer _trainedModel;
        private IFileHelper _fileHelper;

        public ModelBuilder()
        {
            _feedRepository = new ViewPointReaderRepository(new FileHelper());
            _fileHelper = new FileHelper();
            LoadModel();
        }

        public ModelBuilder(IFileHelper fileHelper)
        {
            _feedRepository = new ViewPointReaderRepository(fileHelper);
            _fileHelper = fileHelper;
            LoadModel();
        }

        public async Task BuildModel()
        {
            MLContext mlContext = new MLContext();

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
            
            var pipeline = mlContext.Transforms.Text.FeaturizeText("keyPhrasesFeaturized", nameof(FeedData.KeyPhrases))
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
            mlContext.Model.Save(model, trainingData.Schema, _fileHelper.GetLocalFilePath("VprRecommendationModel.mdl"));

        }

        private void LoadModel()
        {
            MLContext mlContext = new MLContext();
            var modelPath = _fileHelper.GetLocalFilePath("VprRecommendationModel.mdl");

            try
            {
                using (FileStream stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    _trainedModel = mlContext.Model.Load(stream, out var modelInputSchema);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public float ScoreFeed(IFeedSubscription feedSubscription)
        {
            MLContext mlContext = new MLContext();
            var predictionEngine = mlContext.Model.CreatePredictionEngine<FeedData, FeedRecommendation>(_trainedModel);

            if (!feedSubscription.KeyPhrases.Any()) return 0;
            var testFeedData = new FeedData
            {
                KeyPhrases = feedSubscription.KeyPhrases.ToArray()
            };

            var prediction = predictionEngine.Predict(testFeedData);
            return prediction.Score;
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

    }


    //*******  This is only for local testing!!! *******
    class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {

            //TODO: Replace this path with a local path to your machine if you would like to perform local testing....
            return
                "C:\\Users\\rtaylor\\AppData\\Local\\Packages\\1c992eaa-0a62-41b2-bc8b-71d92909cd6f_x11ngb3ehp7m2\\LocalState\\" + filename;
        }
    }
}
