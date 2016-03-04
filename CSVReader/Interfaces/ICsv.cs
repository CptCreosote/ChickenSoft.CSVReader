using System.Collections.Generic;
using System.Data;
using System.IO;

namespace ChickenSoft.CSVReader.Interfaces
{
    /// <summary>
    /// Defines the minium requirements of a CSV file object.
    /// </summary>
    public interface ICsv
    {
        FileInfo CsvFile { get; set; }
        DataTable Data { get; set; }
        IList<ICsvSchemaModel> Schema { get; set; }
    }
}
