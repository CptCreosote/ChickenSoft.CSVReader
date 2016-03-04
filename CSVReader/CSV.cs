namespace ChickenSoft.CSVReader
{
    public class Csv : Parser
    {
        public Csv(string csvFileName) : base(csvFileName)
        {   
        }

        public Csv(string csvFileName, string csvSchemaFileName) : base(csvFileName, csvSchemaFileName)
        {
        }
    }
}
