using StackExchange.Redis;

namespace RedisCache
{
    internal class Program
    {
        static string connectionString = "REDIS_CONNECTION_STRING";


        static async Task Main(string[] args)
        {
            using var cache = ConnectionMultiplexer.Connect(connectionString);
            IDatabase db = cache.GetDatabase();

            // Snippet below executes a PING to test the server connection
            var result = await db.ExecuteAsync("ping");
            Console.WriteLine($"PING = {result.Type} : {result}");

            //Get list of connected client
            var connectedClient = await db.ExecuteAsync("client", "list");
            Console.WriteLine($"Type = {connectedClient.Type}\r\nResult = {connectedClient}");

            // Check on database is key exist or not
            var isKeyExist = await db.KeyExistsAsync("test:key");
            Console.WriteLine($"Is key 'test:key' exist: {isKeyExist}");

            if(!isKeyExist)
            {
                // Call StringSetAsync on the IDatabase object to set the key "test:key" to the value "100"
                bool setValue = await db.StringSetAsync("test:key", "100");
                Console.WriteLine($"SET: {setValue}");
            }

            // Check key live time
            var keyTimeToLive = await db.KeyTimeToLiveAsync("test:key");
            Console.WriteLine($"Key 'test:key' live time: {keyTimeToLive}");

            if(keyTimeToLive is null)
            {
                //Keep key alive for one week        -Default time is 10 min
                var keyExpire = await db.KeyExpireAsync("test:key", DateTime.UtcNow.AddDays(7));
                Console.WriteLine($"SET Key Expiration: {keyExpire}");
            }

            //Rename cache Key name
            var keyRename = await db.KeyRenameAsync("test:key", "testNewKey");
            Console.WriteLine($"Rename Key: {keyRename}");

            // StringGetAsync retrieves the value for the "test" key
            string? getValue = await db.StringGetAsync("test:key");
            Console.WriteLine($"GET: {getValue}");
            
            //delete key value  -- key delete will dispose the instance
            var deleteKey = await db.KeyDeleteAsync("test:key");
            Console.WriteLine($"Is test key deleted: {deleteKey}");
        }
    }
}