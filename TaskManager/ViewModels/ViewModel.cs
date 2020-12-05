using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TaskManager.Models;
using Microsoft.Win32;

namespace TaskManager.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        private SelectProcessInfo _selectedProcess;

        private ProcessInfo _selectedProcessInfo;

        private RelayCommand _openCommand;

        private RelayCommand _openProcess;

        private RelayCommand _killCommand;

        private RelayCommand _chengePriorityCommand;

        public ObservableCollection<SelectProcessInfo> _selectProcessInfo;

        private ObservableCollection<ProcessInfo> _processes;

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += UpdateProcesses;
            timer.Start();
            Processes = new ObservableCollection<ProcessInfo>();
            SelectProcessInfo = new ObservableCollection<SelectProcessInfo>();
            ProcessProperties = Enum.GetValues(typeof(ProcessPriorityClass)).Cast<ProcessPriorityClass>();
        }

        public RelayCommand OpenCommand => _openCommand ?? (_openCommand = new RelayCommand(OpenFileLocationImpl));

        public RelayCommand OpenProcess => _openProcess ?? (_openProcess = new RelayCommand(OpenProcessExecute));

        public RelayCommand KillProcess => _killCommand ?? (_killCommand = new RelayCommand(KillSelectedProcess));

        public RelayCommand ChengePriority => _chengePriorityCommand ?? (_chengePriorityCommand = new RelayCommand(ChangePriority));

        public IEnumerable<ProcessPriorityClass> ProcessProperties { get; set; }

        public ProcessPriorityClass SelectedProcessProperty { get; set; }

        public ProcessInfo SelectedProcessInfo
        {
            get => _selectedProcessInfo;
            set
            {
                _selectedProcessInfo = value;
                SelectedProcess = new SelectProcessInfo(_selectedProcessInfo.Process);
            }
        }

        public SelectProcessInfo SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged(nameof(SelectedProcess));
            }
        }

        public ObservableCollection<SelectProcessInfo> SelectProcessInfo
        {
            get => _selectProcessInfo;
            set
            {
                _selectProcessInfo = value;
                OnPropertyChanged(nameof(SelectProcessInfo));
            }
        }

        public ObservableCollection<ProcessInfo> Processes
        {
            get => _processes;
            set
            {
                _processes = value;
                OnPropertyChanged(nameof(Processes));
            }
        }

        public void UpdateProcesses(object sender, EventArgs e)
        {
            var currentIds = Processes.Select(p => p.Id).ToList();

            foreach (var p in Process.GetProcesses())
            {
                if (!currentIds.Remove(p.Id))
                {
                    Processes.Add(new ProcessInfo(p));
                }
            }
        }

        public void ChangePriority(object obj)
        {
            SelectedProcess.PriorityClass = SelectedProcessProperty;
        }

        public void KillSelectedProcess(object o)
        {
            Processes.Remove(Processes.First(obj => obj.Id == SelectedProcess.Id));
            SelectedProcess.Process.Kill();
        }

        public void StartProcess()
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                ReadOnlyChecked = true
            };
            fileDialog.ShowDialog();
            string path = fileDialog.FileName;

            try
            {
                Process.Start(path);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, " Ooops", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void OpenFileLocationImpl(object o)
        {
            await Task.Run(() =>
            {
                var process = Process.GetProcessById(SelectedProcess.Id);
                try
                {
                    string fullPath = process.MainModule.FileName;
                    Process.Start("explorer.exe", fullPath.Remove(fullPath.LastIndexOf('\\')));
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        private void OpenProcessExecute(object obj)
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                ReadOnlyChecked = true
            };
            fileDialog.ShowDialog();
            string path = fileDialog.FileName;

            try
            {
                Process.Start(path);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error file not selected!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

