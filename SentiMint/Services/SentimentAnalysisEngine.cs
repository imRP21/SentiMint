using Microsoft.ML;
using Microsoft.ML.Data;

namespace SentiMint.Services
{
    public class SentimentAnalysisEngine
    {
        private readonly string _modelPath  = "Path to local models";
        private readonly MLContext _mlContext;
        private ITransformer _trainedModel;

        // Classes used for training and prediction
        public class SentimentData
        {
            public bool Label { get; set; }
            public string ReviewText { get; set; }
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

            // Defin the pipeline: Convert text to features and then use a binay classifier
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.ReviewText));
            var trainer = _mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(SentimentData.Label), maximumNumberOfIterations: 100);
            var trainingPipeline = pipeline.Append(trainer);

            // Train the model
            _trainedModel = trainingPipeline.Fit(trainingDataView);

            // 
        }
    }
}
