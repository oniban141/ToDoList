using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DailyPlanner.Models;
using DailyPlanner.Services;
using LiveCharts;
using Newtonsoft.Json;

namespace DailyPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _dbService;
        private readonly User _currentUser;
        private ObservableCollection<Task> _tasks;
        private ObservableCollection<Event> _events;
        private ObservableCollection<Note> _notes;
        private ObservableCollection<Tag> _tags;
        private ObservableCollection<Reminder> _reminders;
        private Task _selectedTask;
        private Event _selectedEvent;
        private Note _selectedNote;
        private string _searchQuery;
        private DateTime? _filterDueDate;
        private string _filterStatus;
        private string _filterPriority;
        private bool _isDarkTheme;
        private SeriesCollection _taskStatusChartSeries;
        private SeriesCollection _taskPriorityChartSeries;
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        public ObservableCollection<Task> Tasks
        {
            get => _tasks;
            set { _tasks = value; OnPropertyChanged(); }
        }
        
        public ObservableCollection<Event> Events
        {
            get => _events;
            set { _events = value; OnPropertyChanged(); }
        }
        
        public ObservableCollection<Note> Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(); }
        }
        
        public ObservableCollection<Tag> Tags
        {
            get => _tags;
            set { _tags = value; OnPropertyChanged(); }
        }
        
        public ObservableCollection<Reminder> Reminders
        {
            get => _reminders;
            set { _reminders = value; OnPropertyChanged(); }
        }
        
        public Task SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); }
        }
        
        public Event SelectedEvent
        {
            get => _selectedEvent;
            set { _selectedEvent = value; OnPropertyChanged(); }
        }
        
        public Note SelectedNote
        {
            get => _selectedNote;
            set { _selectedNote = value; OnPropertyChanged(); }
        }
        
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                FilterTasks();
            }
        }
        
        public DateTime? FilterDueDate
        {
            get => _filterDueDate;
            set
            {
                _filterDueDate = value;
                OnPropertyChanged();
                FilterTasks();
            }
        }
        
        public string FilterStatus
        {
            get => _filterStatus;
            set
            {
                _filterStatus = value;
                OnPropertyChanged();
                FilterTasks();
            }
        }
        
        public string FilterPriority
        {
            get => _filterPriority;
            set
            {
                _filterPriority = value;
                OnPropertyChanged();
                FilterTasks();
            }
        }
        
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                _isDarkTheme = value;
                OnPropertyChanged();
            }
        }
        
        public SeriesCollection TaskStatusChartSeries
        {
            get => _taskStatusChartSeries;
            set { _taskStatusChartSeries = value; OnPropertyChanged(); }
        }
        
        public SeriesCollection TaskPriorityChartSeries
        {
            get => _taskPriorityChartSeries;
            set { _taskPriorityChartSeries = value; OnPropertyChanged(); }
        }
        
        public ICommand AddTaskCommand { get; }
        public ICommand UpdateTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand AddEventCommand { get; }
        public ICommand UpdateEventCommand { get; }
        public ICommand DeleteEventCommand { get; }
        public ICommand AddNoteCommand { get; }
        public ICommand UpdateNoteCommand { get; }
        public ICommand DeleteNoteCommand { get; }
        public ICommand AddTagCommand { get; }
        public ICommand AddReminderCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand ExportToJsonCommand { get; }
        public ICommand ImportFromJsonCommand { get; }
        public ICommand ExportToCsvCommand { get; }
        public ICommand ImportFromCsvCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        
        public MainViewModel(User user, DatabaseService dbService)
        {
            _currentUser = user;
            _dbService = dbService;
            
            // Initialize commands
            AddTaskCommand = new RelayCommand(AddTask);
            UpdateTaskCommand = new RelayCommand(UpdateTask, CanExecuteTaskCommand);
            DeleteTaskCommand = new RelayCommand(DeleteTask, CanExecuteTaskCommand);
            AddEventCommand = new RelayCommand(AddEvent);
            UpdateEventCommand = new RelayCommand(UpdateEvent, CanExecuteEventCommand);
            DeleteEventCommand = new RelayCommand(DeleteEvent, CanExecuteEventCommand);
            AddNoteCommand = new RelayCommand(AddNote);
            UpdateNoteCommand = new RelayCommand(UpdateNote, CanExecuteNoteCommand);
            DeleteNoteCommand = new RelayCommand(DeleteNote, CanExecuteNoteCommand);
            AddTagCommand = new RelayCommand(AddTag);
            AddReminderCommand = new RelayCommand(AddReminder, CanExecuteTaskCommand);
            ToggleThemeCommand = new RelayCommand(ToggleTheme);
            ExportToJsonCommand = new RelayCommand(ExportToJson);
            ImportFromJsonCommand = new RelayCommand(ImportFromJson);
            ExportToCsvCommand = new RelayCommand(ExportToCsv);
            ImportFromCsvCommand = new RelayCommand(ImportFromCsv);
            SearchCommand = new RelayCommand(FilterTasks);
            RefreshCommand = new RelayCommand(RefreshData);
            
            // Load data
            RefreshData(null);
            
            // Initialize charts
            UpdateCharts();
        }
        
        private void RefreshData(object parameter)
        {
            Tasks = new ObservableCollection<Task>(_dbService.GetAllTasks(_currentUser.Id));
            Events = new ObservableCollection<Event>(_dbService.GetAllEvents(_currentUser.Id));
            Notes = new ObservableCollection<Note>(_dbService.GetAllNotes(_currentUser.Id));
            Tags = new ObservableCollection<Tag>(_dbService.GetAllTags(_currentUser.Id));
            Reminders = new ObservableCollection<Reminder>(_dbService.GetRemindersByUser(_currentUser.Id));
            UpdateCharts();
        }
        
        private void UpdateCharts()
        {
            // Task Status Chart (Pie Chart)
            var statusGroups = Tasks.GroupBy(t => t.Status).Select(g => new { Status = g.Key, Count = g.Count() });
            TaskStatusChartSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Todo",
                    Values = new ChartValues<int> { statusGroups.FirstOrDefault(g => g.Status == "Todo")?.Count ?? 0 },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "In Progress",
                    Values = new ChartValues<int> { statusGroups.FirstOrDefault(g => g.Status == "InProgress")?.Count ?? 0 },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Completed",
                    Values = new ChartValues<int> { statusGroups.FirstOrDefault(g => g.Status == "Completed")?.Count ?? 0 },
                    DataLabels = true
                }
            };
            
            // Task Priority Chart (Column Chart)
            var priorityGroups = Tasks.GroupBy(t => t.Priority).Select(g => new { Priority = g.Key, Count = g.Count() });
            TaskPriorityChartSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Low",
                    Values = new ChartValues<int> { priorityGroups.FirstOrDefault(g => g.Priority == "Low")?.Count ?? 0 }
                },
                new ColumnSeries
                {
                    Title = "Medium",
                    Values = new ChartValues<int> { priorityGroups.FirstOrDefault(g => g.Priority == "Medium")?.Count ?? 0 }
                },
                new ColumnSeries
                {
                    Title = "High",
                    Values = new ChartValues<int> { priorityGroups.FirstOrDefault(g => g.Priority == "High")?.Count ?? 0 }
                }
            };
        }
        
        private void FilterTasks(object parameter = null)
        {
            var filteredTasks = _dbService.SearchTasks(_currentUser.Id, SearchQuery);
            
            if (!string.IsNullOrEmpty(FilterStatus) || !string.IsNullOrEmpty(FilterPriority) || FilterDueDate.HasValue)
            {
                filteredTasks = _dbService.FilterTasks(_currentUser.Id, FilterStatus, FilterPriority, FilterDueDate);
            }
            
            Tasks = new ObservableCollection<Task>(filteredTasks);
            UpdateCharts();
        }
        
        #region Task Operations
        
        private void AddTask(object parameter)
        {
            var newTask = new Task
            {
                Title = "Новая задача",
                Description = string.Empty,
                DueDate = DateTime.Now.AddDays(7),
                Priority = "Medium",
                Status = "Todo",
                UserId = _currentUser.Id,
                CreatedAt = DateTime.Now
            };
            
            int taskId = _dbService.AddTask(newTask);
            if (taskId > 0)
            {
                newTask.Id = taskId;
                Tasks.Add(newTask);
                UpdateCharts();
            }
        }
        
        private void UpdateTask(object parameter)
        {
            if (SelectedTask != null)
            {
                if (_dbService.UpdateTask(SelectedTask))
                {
                    RefreshData(null);
                }
            }
        }
        
        private void DeleteTask(object parameter)
        {
            if (SelectedTask != null)
            {
                if (_dbService.DeleteTask(SelectedTask.Id))
                {
                    Tasks.Remove(SelectedTask);
                    UpdateCharts();
                }
            }
        }
        
        private bool CanExecuteTaskCommand(object parameter) => SelectedTask != null;
        
        #endregion
        
        #region Event Operations
        
        private void AddEvent(object parameter)
        {
            var newEvent = new Event
            {
                Title = "Новое событие",
                Description = string.Empty,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(2),
                Location = string.Empty,
                UserId = _currentUser.Id,
                CreatedAt = DateTime.Now
            };
            
            int eventId = _dbService.AddEvent(newEvent);
            if (eventId > 0)
            {
                newEvent.Id = eventId;
                Events.Add(newEvent);
            }
        }
        
        private void UpdateEvent(object parameter)
        {
            if (SelectedEvent != null)
            {
                if (_dbService.UpdateEvent(SelectedEvent))
                {
                    RefreshData(null);
                }
            }
        }
        
        private void DeleteEvent(object parameter)
        {
            if (SelectedEvent != null)
            {
                if (_dbService.DeleteEvent(SelectedEvent.Id))
                {
                    Events.Remove(SelectedEvent);
                }
            }
        }
        
        private bool CanExecuteEventCommand(object parameter) => SelectedEvent != null;
        
        #endregion
        
        #region Note Operations
        
        private void AddNote(object parameter)
        {
            var newNote = new Note
            {
                Title = "Новая заметка",
                Content = string.Empty,
                UserId = _currentUser.Id,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            
            int noteId = _dbService.AddNote(newNote);
            if (noteId > 0)
            {
                newNote.Id = noteId;
                Notes.Add(newNote);
            }
        }
        
        private void UpdateNote(object parameter)
        {
            if (SelectedNote != null)
            {
                SelectedNote.UpdatedAt = DateTime.Now;
                if (_dbService.UpdateNote(SelectedNote))
                {
                    RefreshData(null);
                }
            }
        }
        
        private void DeleteNote(object parameter)
        {
            if (SelectedNote != null)
            {
                if (_dbService.DeleteNote(SelectedNote.Id))
                {
                    Notes.Remove(SelectedNote);
                }
            }
        }
        
        private bool CanExecuteNoteCommand(object parameter) => SelectedNote != null;
        
        #endregion
        
        #region Tag Operations
        
        private void AddTag(object parameter)
        {
            var newTag = new Tag
            {
                Name = "Новый тег",
                Color = "#D12128",
                UserId = _currentUser.Id,
                CreatedAt = DateTime.Now
            };
            
            int tagId = _dbService.AddTag(newTag);
            if (tagId > 0)
            {
                newTag.Id = tagId;
                Tags.Add(newTag);
            }
        }
        
        #endregion
        
        #region Reminder Operations
        
        private void AddReminder(object parameter)
        {
            if (SelectedTask != null)
            {
                var newReminder = new Reminder
                {
                    TaskId = SelectedTask.Id,
                    ReminderTime = DateTime.Now.AddHours(1),
                    Message = "Напоминание о задаче: " + SelectedTask.Title,
                    IsActive = true,
                    UserId = _currentUser.Id,
                    CreatedAt = DateTime.Now,
                    Task = SelectedTask
                };
                
                int reminderId = _dbService.AddReminder(newReminder);
                if (reminderId > 0)
                {
                    newReminder.Id = reminderId;
                    Reminders.Add(newReminder);
                }
            }
        }
        
        #endregion
        
        #region Theme Operations
        
        private void ToggleTheme(object parameter)
        {
            IsDarkTheme = !IsDarkTheme;
        }
        
        #endregion
        
        #region Export/Import Operations
        
        private void ExportToJson(object parameter)
        {
            try
            {
                var tasksToExport = Tasks.Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.DueDate,
                    t.Priority,
                    t.Status,
                    t.UserId,
                    t.CreatedAt
                }).ToList();
                
                string json = JsonConvert.SerializeObject(tasksToExport, Formatting.Indented);
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "tasks.json");
                File.WriteAllText(filePath, json);
                
                // Show success message
                System.Windows.MessageBox.Show("Задачи экспортированы в JSON файл", "Экспорт");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка при экспорте: " + ex.Message, "Ошибка");
            }
        }
        
        private void ImportFromJson(object parameter)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "tasks.json");
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var importedTasks = JsonConvert.DeserializeObject<List<Task>>(json);
                    
                    foreach (var task in importedTasks)
                    {
                        task.UserId = _currentUser.Id;
                        _dbService.AddTask(task);
                    }
                    
                    RefreshData(null);
                    System.Windows.MessageBox.Show("Задачи импортированы из JSON файла", "Импорт");
                }
                else
                {
                    System.Windows.MessageBox.Show("Файл tasks.json не найден", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка при импорте: " + ex.Message, "Ошибка");
            }
        }
        
        private void ExportToCsv(object parameter)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "tasks.csv");
                var csvLines = new List<string>();
                
                // Header
                csvLines.Add("Id,Title,Description,DueDate,Priority,Status,UserId,CreatedAt");
                
                // Data
                foreach (var task in Tasks)
                {
                    csvLines.Add($"{task.Id},{EscapeCsv(task.Title)},{EscapeCsv(task.Description)},{task.DueDate},{task.Priority},{task.Status},{task.UserId},{task.CreatedAt}");
                }
                
                File.WriteAllLines(filePath, csvLines);
                System.Windows.MessageBox.Show("Задачи экспортированы в CSV файл", "Экспорт");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка при экспорте: " + ex.Message, "Ошибка");
            }
        }
        
        private void ImportFromCsv(object parameter)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "tasks.csv");
                if (File.Exists(filePath))
                {
                    var csvLines = File.ReadAllLines(filePath).Skip(1); // Skip header
                    
                    foreach (var line in csvLines)
                    {
                        var values = line.Split(',');
                        if (values.Length >= 8)
                        {
                            var task = new Task
                            {
                                Title = values[1],
                                Description = values[2],
                                Priority = values[4],
                                Status = values[5],
                                UserId = _currentUser.Id,
                                CreatedAt = DateTime.Now
                            };
                            
                            if (DateTime.TryParse(values[3], out DateTime dueDate))
                            {
                                task.DueDate = dueDate;
                            }
                            
                            _dbService.AddTask(task);
                        }
                    }
                    
                    RefreshData(null);
                    System.Windows.MessageBox.Show("Задачи импортированы из CSV файла", "Импорт");
                }
                else
                {
                    System.Windows.MessageBox.Show("Файл tasks.csv не найден", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка при импорте: " + ex.Message, "Ошибка");
            }
        }
        
        private string EscapeCsv(string value)
        {
            if (value == null) return string.Empty;
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }
        
        #endregion
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
