using System;
using ChickenSoft.CSVReader.Interfaces;

namespace ChickenSoft.CSVReader
{
    /// <summary>
    /// Defines a CSV schema
    /// </summary>
    public class CsvSchemaModel : ICsvSchemaModel
    {
        public int ColumnIndex { get; set; }
        public string ColumnName { get; set; }
        public Type ColumnType { get; set; }
    }
}
