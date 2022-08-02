using StressTestZminingAPi;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace WebStressConsoleApp
{

    public class AttackWithEverything
    {
        public long RequestsPerSeconds { set; get; } = 0;
        public long RequestsDroppedPerSecond { set; get; } = 0;
        public long RequestsSuccessPerSecond { set; get; } = 0;
        public long TotalSentPerSecond { set; get; } = 0;
        public long RequestsDropped { set; get; } = 0;
        public long TotalRequestsSent { set; get; } = 0;
        public long TotalBytesRecieved { set; get; } = 0;
        public int NumberThreads { set; get; } = 1;
        public bool ShowFurther { set; get; } = true;
        public string Url { set; get; }
        public string AccessToken { get; set; }
        public long RequestSucess { set; get; } = 0;
        public System.Timers.Timer aTimer;

        CancellationTokenSource cts = new CancellationTokenSource();
        ParallelOptions po = new ParallelOptions();

        public void Attack()
        {
            //Cancellation Token Config
            cts = new CancellationTokenSource();
            po = new ParallelOptions
            {
                CancellationToken = cts.Token
            };
            po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 2;

            HttpClient client = new HttpClient();
            aTimer = new System.Timers.Timer(10000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            try
            {
                Parallel.For(0, NumberThreads, async ctr =>
                {
                    while (true)
                    {
                        Task<bool> dwnld = ProcessUrl(client);
                        bool z = await dwnld;
                    }
                });
            }

            catch (OperationCanceledException)
            {
                //Console.WriteLine(e.Message);
            }
            finally
            {
                //cts.Dispose();
            }
        }

        private async Task<bool> ProcessUrl(HttpClient client)
        {
            try
            {
                RequestsPerSeconds++;
                TotalRequestsSent++;
                client.DefaultRequestHeaders.Accept.Clear();
                if (!string.IsNullOrEmpty(AccessToken))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                }
                var res = await client.GetAsync(Url);
                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsStringAsync();
                    TotalBytesRecieved += data.Length;
                }
                else
                {
                    RequestsDropped++;
                    RequestsDroppedPerSecond++;

                }
                RequestSucess = TotalRequestsSent - RequestsDropped;
                RequestsSuccessPerSecond = RequestsPerSeconds - RequestsDroppedPerSecond;
                return true;
            }
            catch (Exception)
            {
                RequestsDropped++;
                return false;
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {

            if (ShowFurther)
            {
                RequestsSuccessPerSecond = RequestsPerSeconds - RequestsDroppedPerSecond;
                Console.WriteLine("> {0} : Sent {1} requests", DateTime.Now.ToLongTimeString(), RequestsPerSeconds);
                Console.WriteLine("Total success " + RequestsSuccessPerSecond);
                Console.WriteLine("Total dropped " + RequestsDroppedPerSecond);
            }
            RequestsPerSeconds = 0;
            RequestsSuccessPerSecond = 0;
            RequestsDroppedPerSecond = 0;
        }
    }

    public class Program
    {

        public static async Task Main(string[] args)
        {
            DateTime StartTime;
            DateTime EndTime;
            double TimeTaken = 0;
            long TotalRequestsSent = 0;
            long RequestsDropped = 0;
            long TotalBytesRecieved = 0;
            long RequestSuccess = 0;

            if (TimeTaken == 0)
            {
                AttackWithEverything attackWithEverything = new AttackWithEverything();

                Console.WriteLine("\n......Welcome to the Web Stress Analysis Tool......\n");

                Console.Write("Enter complete url to analyze: ");
                string InputUrl = Console.ReadLine().ToLower();
                Regex.Replace(InputUrl, @"\s+", "");
                bool result = Uri.TryCreate(InputUrl, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                while (!result)
                {
                    Console.WriteLine("\nERROR: URL syntax is not valid!");
                    Console.WriteLine("URL syntax should be: http://FullDomainName or https://FullDomainName");
                    Console.Write("Enter complete url to analyze: ");
                    InputUrl = Console.ReadLine().ToLower();
                    Regex.Replace(InputUrl, @"\s+", "");
                    result = Uri.TryCreate(InputUrl, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                }
                attackWithEverything.Url = InputUrl;

                Console.Write("\nEnter number of threads to use (1-5000): ");
                result = int.TryParse(Console.ReadLine(), out int NumberThreads);

                while (!result || NumberThreads < 1 || NumberThreads > 5000)
                {
                    Console.WriteLine("\nERROR: Number is not valid!");
                    Console.Write("Enter number of threads to use (1-5000): ");
                    result = int.TryParse(Console.ReadLine(), out NumberThreads);
                }
                attackWithEverything.NumberThreads = NumberThreads;

                Console.WriteLine("\nEnter access token to authenticate: ");
                string accessToken = Console.ReadLine();
                attackWithEverything.AccessToken = accessToken;

                StartTime = DateTime.Now;
                attackWithEverything.Attack();

                Console.ReadKey();

                EndTime = DateTime.Now;
                TimeTaken = (EndTime.Subtract(StartTime)).TotalSeconds;
                TotalRequestsSent = attackWithEverything.TotalRequestsSent;
                RequestsDropped = attackWithEverything.RequestsDropped;
                TotalBytesRecieved = attackWithEverything.TotalBytesRecieved;
                RequestSuccess = attackWithEverything.RequestSucess;
                attackWithEverything.ShowFurther = false;
            }

            Console.WriteLine("\n\nTest Results...");
            Console.WriteLine("Total Requests Sent: " + TotalRequestsSent);
            Console.WriteLine("Total Time Taken: " + $"{TimeTaken:0.#}" + " seconds");
            Console.WriteLine("Total Requests Dropped By Server: " + RequestsDropped);
            Console.WriteLine("Total Requests Success By Server: " + RequestSuccess);
            Console.WriteLine($"Total Data Recieved: {(TotalBytesRecieved / 1048576.0):0.##} MB");
            Console.WriteLine($"Average Speed of Transmission: {((TotalBytesRecieved / 1048576.0) / TimeTaken):0.##} MB/s");
            Console.WriteLine($"Average Requests sent per second: {(TotalRequestsSent / TimeTaken):0.##}\n\n");

            Console.WriteLine("\nPress Any Key to Exit...");
            Console.ReadKey();
            return;
        }
    }
}
