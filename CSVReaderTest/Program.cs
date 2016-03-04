using System;

namespace CSVReaderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Read a csv file with a schema definition
            var csv = new ChickenSoft.CSVReader.Csv(@"csvdata.txt", @"csvschema.txt");

            csv.Data.WriteXml(@"data_withschema.xml");
            csv.Data.WriteXmlSchema(@"data_withschema.xsd");
            Console.WriteLine(@"CSV file read successfully. An XML version has been saved to data_withschema.xml");
            
            //Read a csv file without a schema definition
            csv = new ChickenSoft.CSVReader.Csv(@"csvdata.txt");

            csv.Data.WriteXml(@"data_withoutschema.xml");
            Console.WriteLine(@"CSV file read successfully. An XML version has been saved to data_withoutschema.xml");
        }
    }
}
