using System;

namespace ChickenSoft.CSVReader.Interfaces
{
    public interface ICsvSchemaModel
    {
        int ColumnIndex { get; set; }
        string ColumnName { get; set; }
        Type ColumnType { get; set; }
    }
}
