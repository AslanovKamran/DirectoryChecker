
using System;

namespace WpfDirectoryChechkerApp.Abstract
{
	public abstract class BaseFile
	{
		public string FilePath { get; set; } = string.Empty;
		public abstract string ProcessFile();
	}

	public class FileProcesser
	{
		private BaseFile? _file;
		
		public void ChangeFileType(BaseFile NewFileType) => _file = NewFileType;
		public string Process() 
		{
			if (_file is null) throw new Exception("Cannot find the specific file type");
			return _file.ProcessFile();
		}

	}
}
