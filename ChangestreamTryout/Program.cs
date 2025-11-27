using MongoDB.Bson;
using MongoDB.Driver;

namespace ChangestreamTryout
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            MongoClient client = new MongoClient("mongodb://localhost:40131/?authSource=admin"); //the mongodb refuses to connect. Idk how to solve that
            var database = client.GetDatabase("foo");
            var collection = database.GetCollection<BsonDocument>("test");

            var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>().Match("{operationType: { $in: [ 'insert', 'delete', 'update' ] } }");

            var cursor = collection.Watch<ChangeStreamDocument<BsonDocument>>(pipeline, options);

            await collection.InsertOneAsync(new BsonDocument("Name", "Jack"));

            var enumerator = cursor.ToEnumerable().GetEnumerator();
            while (enumerator.MoveNext())
            {
                ChangeStreamDocument<BsonDocument> doc = enumerator.Current;
                Console.WriteLine("It works");
                Console.WriteLine(doc.DocumentKey);
            }
        }
    }
}