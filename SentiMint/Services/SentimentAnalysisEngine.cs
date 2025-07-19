using Microsoft.ML;
using SentiMint.Models;

namespace SentiMint.Services
{
    public class SentimentAnalysisEngine
    {
        private readonly string _modelPath  = @"D:\Codebase\Repos\SentiMint\SentiMint\Adhoc\Model\SdcaLogisticRegression";
        private readonly MLContext _mlContext;
        private ITransformer _trainedModel;
        private PredictionEngine<SentimentData, SentimentPredictionInternal> _predictionEngine;

        // Class used for training.
        public class SentimentData
        {
            public bool Label { get; set; }
            public string ReviewText { get; set; }
        }

        /// <summary>
        /// Class used for prediction results.
        /// </summary>
        private class SentimentPredictionInternal
        {
            public bool PredictedLabel { get; set; }
            public float Score { get; set; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SentimentAnalysisEngine()
        {
            _mlContext = new MLContext();
            LoadOrTrainModel();
        }

        /// <summary>
        /// Load or train the sentiment analysis model.
        /// </summary>
        private void LoadOrTrainModel()
        {
            if (File.Exists(_modelPath))
            {
                // Load the pre-trained model from dists if it exists
                _trainedModel = _mlContext.Model.Load(_modelPath, out _);
            }
            else
            {
                // If not found, train a model with training data
                TrainModel();
            }

            // Create a prediction engine for making predictions
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPredictionInternal>(_trainedModel);
        }

        /// <summary>
        /// Trains the machine learning model using the configured dataset and parameters.
        /// </summary>
        /// <remarks>This method initializes the training process for the model. Ensure that all required 
        /// data and configurations are properly set before calling this method. The training  process may take
        /// significant time depending on the size of the dataset and complexity of the model.</remarks>
        public void TrainModel()
        {
            // Load training data from the specified path
            string trainingDataPath = @"C:\Users\RP-21\Downloads\imdb_reviews\test";
            List<SentimentData> trainingData = new List<SentimentData>();
            foreach (var type in new string[]{ "neg", "pos" })
            { 
                foreach (var file in Directory.GetFiles(Path.Combine(trainingDataPath, type), "*.txt"))
                {
                    var label = type == "pos" ? true : false;
                    var reviewText = File.ReadAllText(file);
                    trainingData.Add(new SentimentData { Label = label, ReviewText = reviewText });
                }
            }

            var trainingDataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            // Defin the pipeline: Convert text to features and then use a binary classifier
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.ReviewText));
            var trainer = _mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: nameof(SentimentData.Label), 
                maximumNumberOfIterations: 10);
            var trainingPipeline = pipeline.Append(trainer);

            // Train the model
            _trainedModel = trainingPipeline.Fit(trainingDataView);

            // Save model for future use
            using (var stream = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                _mlContext.Model.Save(_trainedModel, trainingDataView.Schema, stream);
            }
        }

        public SentimentPrediction Predict(string reviewText)
        {
            if (_trainedModel == null)
            {
                throw new InvalidOperationException("Model is not trained or loaded.");
            }

            // Create a new instance of SentimentData for prediction
            var input = new SentimentData { ReviewText = reviewText };

            // Predict the sentiment
            var prediction = _predictionEngine.Predict(input);

            // Map the internal prediction to the public SentimentPrediction model
            return new SentimentPrediction
            {
                IsPositive = prediction.PredictedLabel,
                Score = prediction.Score
            };
        }
    }
}
