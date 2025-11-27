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

            var cursor = await collection.WatchAsync();

            await cursor.ForEachAsync(change =>
            {
                Console.WriteLine($"Received the following type of change: {change.BackingDocument}");
            });
        }
    }
}