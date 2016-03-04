using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ChickenSoft.CSVReader.Interfaces;

namespace ChickenSoft.CSVReader
{
    /// <summary>
    /// The parser class does the donkey work of reading the CSV data and schema data
    /// </summary>
    public class Parser : ICsv
    {
        private FileInfo _csvSchemaFileInfo;
        private readonly char _seperatorChar = Convert.ToChar(",");
        private int _csvColumnCount;

        public FileInfo CsvFile { get; set; }
        public DataTable Data { get; set; }
        public IList<ICsvSchemaModel> Schema { get; set; }

        /// <summary>
        /// Just provides an easily readable representation of what columns are in the schema file.
        /// </summary>
        private enum SchemaMap
        {
            Index,
            Name,
            Type
        }

        internal Parser() //Using internal modifier to prevent construction of this class without parameters. It would be pointless.
        {
        }

        public Parser(string csvFileName)
        {
            Initialize(csvFileName, null);
        }

        public Parser(string csvFileName, string csvSchemaFileName)
        {
            Initialize(csvFileName, csvSchemaFileName);
        }

        /// <summary>
        /// Initialize method for the class constructor.
        /// </summary>
        /// <param name="csvFileName"></param>
        /// <param name="csvSchemaFileName"></param>
        private void Initialize(string csvFileName, string csvSchemaFileName)
        {
            if (string.IsNullOrEmpty(csvFileName))
                throw new ArgumentNullException(nameof(csvFileName));

            CsvFile = new FileInfo(csvFileName);

            if (!string.IsNullOrEmpty(csvSchemaFileName))
                _csvSchemaFileInfo = new FileInfo(csvSchemaFileName);

            Schema = new List<ICsvSchemaModel>();

            _csvColumnCount = GetCsvColumnCount();
            
            ParseSchema(); //Parse the schema file
            var useSchema = (Schema.Count == _csvColumnCount); //If the schema column count is the same as the csvcolumn count we can assume we should use the schema when we make the datatable
            CreateDataTableColumns(useSchema); //Create the datatable
            ReadCsvToDataTable();
        }

        /// <summary>
        /// Read the csv data into the datatable
        /// </summary>
        private void ReadCsvToDataTable()
        {
            using (var csvFile = new StreamReader(CsvFile.FullName))
            {
                do
                {
                    var csvLine = csvFile.ReadLine(); //Read the csv line

                    if (string.IsNullOrEmpty(csvLine) || !csvLine.Contains(_seperatorChar)) continue; //Make sure it is an actual csv line
                    var csvData = csvLine.Split(_seperatorChar); //Split the csv line into it's columns based on the seperator character

                    if (csvData.Length != _csvColumnCount) continue; //If the data length is not the same as the column count then ignore it (could be a header or a comment or something corrupt)
                    var dr = Data.NewRow(); //Create a new row object

                    foreach (DataColumn columnInfo in Data.Columns)
                    {
                        dr[columnInfo] = csvData[Data.Columns.IndexOf(columnInfo)]; //Popuate the data row with data from the csvdata, using the index of the column in the datatable as the index for the csvData array (it will match)
                    }

                    Data.Rows.Add(dr);
                } while (!csvFile.EndOfStream); //Keep going until we get to the end of the stream
            }
        }

        private void ParseSchema()
        {
            if (_csvSchemaFileInfo == null) return; //Eject out of the routine if we have no schema information

            using (var schemaFile = new StreamReader(_csvSchemaFileInfo.FullName))
            {
                var schemaData = new object[3]; //We expect three bits of data for the schema. The index, the column name, and the column type

                do
                {
                    var schemaLine = schemaFile.ReadLine(); //Read the line of the schema

                    if (!string.IsNullOrEmpty(schemaLine) && schemaLine.Contains(_seperatorChar)) //if it isn't null or empty and contains the seperator charactor, split it up.
                        schemaLine.Split(_seperatorChar).CopyTo(schemaData, 0); //Pour the split data into our schemaData array

                    var schemaItem = new CsvSchemaModel
                    {
                        ColumnIndex = Convert.ToInt32(schemaData[(int)SchemaMap.Index]),
                        ColumnName = Convert.ToString(schemaData[(int)SchemaMap.Name]),
                        ColumnType = GetColumnType(Convert.ToString(schemaData[(int)SchemaMap.Type])) //Uses GetColumnType to find the appropriate type for the datatable based on the schema
                    };

                    Schema.Add(schemaItem);
                } while (!schemaFile.EndOfStream); //Loop until end of stream
            }
        }

        /// <summary>
        /// Finds the maximum number of columns in the csv file. Useful for matching the number of columns vs the number of columns the schema says.
        /// </summary>
        /// <returns></returns>
        private int GetCsvColumnCount()
        {
            int maxColumnCount = 0;

            using (var csvFile = new StreamReader(CsvFile.FullName))
            {
                do
                {
                    var columnCount = csvFile.ReadLine()?.Split(_seperatorChar).Length;

                    if (columnCount > maxColumnCount)
                        maxColumnCount = (int) columnCount;

                } while (!csvFile.EndOfStream);
            }

            return maxColumnCount;
        }

        private void CreateDataTableColumns(bool useSchema)
        {
            Data = new DataTable {TableName = "CsvData"};

            if (useSchema) //We are using the schema
            {
                foreach (var schemaItem in Schema)
                {
                    Data.Columns.Add(new DataColumn(schemaItem.ColumnName, schemaItem.ColumnType)); //Create columns in the data table based on our schema
                }
            }
            else //We don't have a schema
            {
                for (int columnIndex = 0; columnIndex < GetCsvColumnCount(); columnIndex++) //Just find the maximum number of columns the csv file has and then add Column1, 2, 3 etc to the datatable
                {
                    Data.Columns.Add(new DataColumn($"Column{columnIndex}", typeof(string)));
                }
            }
        }

        /// <summary>
        /// Looks at the column type string and uses reflection to find the actual .NET type and returns it. If it can't find the type then it returns a string type instead
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Type GetColumnType(string type)
        {
            var returnType = Type.GetType(type);

            if (returnType == null)
            {
                returnType = typeof(string);
                Debug.WriteLine($"Could not retreive type {type}, defaulting to string");
            }

            return returnType;
        }
    }
}
