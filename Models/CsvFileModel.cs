using System;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using WpfDirectoryChechkerApp.Abstract;

namespace WpfDirectoryChechkerApp.Models
{
    public class CsvFileModel : BaseFile
    {
        public CsvFileModel(string filePath)
        {
            this.FilePath = filePath;
        }
        public override string ProcessFile()
        {
            try
            {
                using (TextFieldParser parser = new TextFieldParser(FilePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    StringBuilder result = new StringBuilder();
                    result.Append($"Processed CSV file: {FilePath}\n");

                    // Read header fields
                    if (!parser.EndOfData)
                    {
                        string[] headers = parser.ReadFields()!;
                        result.Append($"Headers: {string.Join(", ", headers)}\n");
                    }

                    // Read all lines
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields()!;
                        result.Append($"{string.Join(", ", fields)}\n");
                    }

                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"Error processing CSV file {FilePath}: {ex.Message}\n";
            }
        }
    }
}
