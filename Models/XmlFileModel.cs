using System;
using System.Text;
using System.Xml.Linq;
using WpfDirectoryChechkerApp.Abstract;

namespace WpfDirectoryChechkerApp.Models
{
	public class XmlFileModel : BaseFile
	{
		public XmlFileModel(string filePath)
		{
			this.FilePath = filePath;
		}
		public override string ProcessFile()
		{
            try
            {
                XDocument xdoc = XDocument.Load(FilePath);
                StringBuilder result = new StringBuilder();
                result.Append ( $"Processed XML file: {FilePath}\n");

                // Example: Display root element name and its children
                XElement root = xdoc.Root!;
                if (root != null)
                {
                    result.Append($"Root element: {root.Name}\n");
                    foreach (XElement child in root.Elements())
                    {
                        result.Append($"Child element: {child.Name}, Value: {child.Value}\n");
                    }
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                return $"Error processing XML file {FilePath}: {ex.Message}\n";
            }
        }
	}
}
