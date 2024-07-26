using System;
using System.Diagnostics;
using System.IO;
using WpfDirectoryChechkerApp.Abstract;

namespace WpfDirectoryChechkerApp.Models
{
	public class TxtFileModel : BaseFile
    {
        public TxtFileModel(string filePath)
        {
            this.FilePath = filePath;
        }
        public override string ProcessFile()
        {
			try
			{
                
               string[] lines = File.ReadAllLines(this.FilePath);
                return $"Processed Txt file: {FilePath}\n" + " " + string.Join("", lines); 
            }
            catch (Exception ex)
			{
                throw new Exception($"Error processing CSV file {FilePath}: {ex.Message}\n");
            }
        }
    }
}
