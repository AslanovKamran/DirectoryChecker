using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using WpfDirectoryChechkerApp.Abstract;
using WpfDirectoryChechkerApp.Models;

namespace WpfDirectoryChechkerApp
{

	public partial class MainWindow : Window, INotifyPropertyChanged
	{


		private static readonly ConcurrentDictionary<string, byte> ProcessedFiles = new ConcurrentDictionary<string, byte>();
		private DispatcherTimer _timer = new();

		#region Notify Property Changed
		public event PropertyChangedEventHandler? PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		private ObservableCollection<string> entries = new();
		public ObservableCollection<string> Entries
		{
			get => entries;
			set
			{
				entries = value;
				OnPropertyChanged(nameof(Entries));
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			StartMonitoring();

		}


		static (string directoryPath, int frequency) ReadConfig(string configFilePath)
		{
			string directoryPath = string.Empty;
			int frequency = 5;

			try
			{
				if (File.Exists(configFilePath))
				{
					string[] lines = File.ReadAllLines(configFilePath);

					foreach (string line in lines)
					{
						if (line.StartsWith("DirectoryPath = "))
						{
							directoryPath = line.Substring("DirectoryPath = ".Length);
						}
						else if (line.StartsWith("Frequency = "))
						{
							string freqString = line.Substring("Frequency = ".Length);
							if (int.TryParse(freqString, out int freq))
							{
								frequency = freq;
							}
							else
							{
								MessageBox.Show("Invalid frequency value in config file. Using default value of 5 seconds.");
							}
						}
					}

					if (string.IsNullOrEmpty(directoryPath))
					{
						MessageBox.Show("Directory path not found in config file. Using default directory path.");
					}
				}
				else
				{
					MessageBox.Show("Config file not found. Using default values.");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error reading config file: {ex.Message}. Using default values.");
			}

			return (directoryPath, frequency);
		}

		private void StartMonitoring()
		{
			string configFilePath = @$"D:\VS\WpfDirectoryChechkerApp\config.txt";
			string directoryPath = string.Empty;
			int monitoringFrequency = 5;

			(directoryPath, monitoringFrequency) = ReadConfig(configFilePath);

			if (!Directory.Exists(directoryPath))
			{
				MessageBox.Show($"Directory does not exist: {directoryPath}");
				return;
			}

			var watcher = new FileSystemWatcher(directoryPath)
			{
				NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
				Filter = "*.*"
			};

			watcher.Created += async (sender, e) => await OnNewFileDetected(e.FullPath);
			watcher.EnableRaisingEvents = true;

			_timer = new DispatcherTimer();
			_timer.Interval = TimeSpan.FromSeconds(monitoringFrequency);
			_timer.Tick += async (sender, e) => await CheckDirectoryAsync(directoryPath);
			_timer.Start();
		}

		private async Task OnNewFileDetected(string filePath)
		{
			if (ProcessedFiles.ContainsKey(filePath))
			{
				return; // Skip already processed files
			}

			ProcessedFiles.TryAdd(filePath, 0); // Mark file as processed

			await Task.Run(() =>
			{
				string result = ProcessFile(filePath);
				Dispatcher.Invoke(() =>
				{
					Entries.Add(result);
				});
			});
		}

		private async Task CheckDirectoryAsync(string path)
		{
			try
			{
				if (Directory.Exists(path))
				{
					var files = await Task.Run(() =>
					{
						string[] xmlFiles = Directory.GetFiles(path, "*.xml", SearchOption.AllDirectories);
						string[] csvFiles = Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories);
						string[] txtFiles = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);
						return xmlFiles.Concat(csvFiles).Concat(txtFiles).ToArray();
					});

					foreach (var file in files)
					{
						if (!ProcessedFiles.ContainsKey(file))
						{
							await OnNewFileDetected(file);
						}
					}
				}
				else
				{
					MessageBox.Show($"Directory does not exist: {path}");
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				MessageBox.Show($"Error: {ex.Message}");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}");
			}
		}


		private string ProcessFile(string filePath)
		{
			var fileProcesser = new FileProcesser();
			try
			{
				string extension = Path.GetExtension(filePath).ToLower();
				switch (extension)
				{
					case ".xml":
						fileProcesser.ChangeFileType(new XmlFileModel(filePath));
						return fileProcesser.Process();
					case ".txt":
						fileProcesser.ChangeFileType(new TxtFileModel(filePath));
						return fileProcesser.Process();
					case ".csv":
						fileProcesser.ChangeFileType(new CsvFileModel(filePath));
						return fileProcesser.Process();
					default:
						return $"Unsupported file type: {filePath}\n";
				}
			}
			catch (Exception ex)
			{
				return $"Error processing file {filePath}: {ex.Message}\n";
			}
		}


	}
}
