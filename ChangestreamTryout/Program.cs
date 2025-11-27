using MongoDB.Bson;
using MongoDB.Driver;

namespace ChangestreamTryout
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("testest");
            var collection = database.GetCollection<BsonDocument>("testest");

            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                .Match("{operationType: { $in: ['insert', 'delete', 'update'] }}");

            var options = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
            };

            using var cursor = collection.Watch(pipeline, options);

            Console.WriteLine("Watching for changes...");

            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                await collection.InsertOneAsync(new BsonDocument("Name", "Jack"));
            });

            foreach (var change in cursor.ToEnumerable())
            {
                Console.WriteLine("Change detected!");
                Console.WriteLine(change.FullDocument);
            }
        }
    }
}
