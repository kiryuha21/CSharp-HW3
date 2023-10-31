using System.Text.Json;
using System.Text;

namespace hw3
{
    public class CalculationResult
    {
        public CalculationResult(string resultOrError)
        {
            result = resultOrError;
        }

        public byte[] toBytes()
        {
            return Encoding.ASCII.GetBytes(JsonSerializer.Serialize(this));
        }

        public string result { get; set; }
    }
}
