namespace NemoMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Core.Server("127.0.0.1", 62000);

            server.Start().Wait();
        }
    }
}
