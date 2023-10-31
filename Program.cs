using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;

using hw1;
using hw3;

var server = new HttpServer(49410, true);
server.Start();

public class HttpServer
{
    private readonly TcpListener _listener;
    private readonly bool _useThreadPool;
    private bool _isRunning = false;

    private LexemParser _lexemParser;
    private ReversePolishBuilder _reversePolishBuilder;
    private ReversePolishEvaluation _reversePolishEvaluation;

    public HttpServer(int port, bool useThreadPool)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _useThreadPool = useThreadPool;

        _lexemParser = new LexemParser();
        _reversePolishBuilder = new ReversePolishBuilder();
        _reversePolishEvaluation = new ReversePolishEvaluation();

        int MaxThreadsCount = Environment.ProcessorCount * 4;
        ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
        ThreadPool.SetMinThreads(2, 2);
    }

    ~HttpServer()
    {
        if (_isRunning)
        {
            _listener.Stop();
        }
    }

    public void Start()
    {
        _listener.Start();
        _isRunning = true;

        Console.WriteLine("Server started.");

        while (true)
        {
            TcpClient client = _listener.AcceptTcpClient();

            if (_useThreadPool)
            {
                ThreadPool.QueueUserWorkItem(ProcessClient, client);
            }
            else
            {
                ProcessClient(client);
            }
        }
    }

    private Uri extractUriFromMessage(string message)
    {
        var lines = message.Split('\n');
        var relative = lines.First().Split()[1];
        return new Uri("http://localhost:49410" + relative);
    }

    private byte[] getResponseWithUri(Uri uri)
    {
        Console.WriteLine($"abs path is {uri.AbsolutePath}");
        switch (uri.AbsolutePath)
        {
            case "/favicon.ico":
                return prependWithOK(File.ReadAllBytes("favicon.ico"), "image/x-icon");
            case "/":
                return prependWithOK(File.ReadAllBytes("response.html"), "text/html");
            case "/evaluate":
                var pairs = HttpUtility.ParseQueryString(uri.Query);
                var expression = pairs.Get("expression");
                if (expression == null)
                {
                    throw new Exception("No expression entered!");
                }

                List<string> lexems;
                try
                {
                    lexems = _lexemParser.parse_line(expression);
                }
                catch (Exception ex)
                {
                    return prependWithOK(new CalculationResult(ex.Message).toBytes(), "application/json");
                }

                List<string> polished;
                try
                {
                    polished = _reversePolishBuilder.parse_to_polish(ref lexems);
                }
                catch (Exception ex)
                {
                    return prependWithOK(new CalculationResult(ex.Message).toBytes(), "application/json");
                }

                try
                {
                    return prependWithOK(
                        new CalculationResult(_reversePolishEvaluation.apply_polish(ref polished).ToString()).toBytes(),
                        "application/json"
                    );
                }
                catch (Exception ex)
                {
                    return prependWithOK(new CalculationResult(ex.Message).toBytes(), "application/json");
                }
            default:
                throw new Exception("Something went wrong!");
        }
    }

    private byte[] prependWithOK(byte[] origin, string content_type)
    {
        var prefix = "HTTP/1.1 200 OK\r\n" +
                     $"Content-Type: {content_type}; charset=UTF-8\r\n" +
                     "Connection: close\r\n\r\n";
        var bytes = Encoding.ASCII.GetBytes(prefix);
        var res = bytes.Concat(origin).ToArray();
        return res;
    }

    private void ProcessClient(object obj)
    {
        TcpClient client = obj as TcpClient;

        if (client == null)
            return;

        using (NetworkStream stream = client.GetStream())
        {
            byte[] request = new byte[1024];
            int bytesRead = stream.Read(request, 0, request.Length);
            string message = Encoding.ASCII.GetString(request, 0, bytesRead);
            Console.WriteLine("Received message: " + message);

            Uri uri = extractUriFromMessage(message);
            byte[] buffer = getResponseWithUri(uri);

            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        client.Close();
    }
}

