using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TaskManager.Models
{
    public class SelectProcessInfo : INotifyPropertyChanged
    {
        public SelectProcessInfo(Process process)
        {
            Process = process;
            Id = process.Id;
            Name = process.ProcessName;
            WorkingSet64 = ((double)process.WorkingSet64 / 1048576).ToString("0.00");
            PagedMemorySize64 = ((double)process.PagedMemorySize64 / 1048576).ToString("0.00");
            VirtualMemorySize64 = ((double)process.VirtualMemorySize64 / 1048576).ToString("0.00");
            StartTime = process.StartTime;
            ThredsCount = process.Threads.Count;

            try
            {
                Modules = process.Modules;
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message, "Acces denied!", MessageBoxButton.OK);
            }

            try
            {
                PriorityClass = process.PriorityClass;
            }
            catch (Exception)
            {
            }

            Threads = process.Threads;
        }

        public Process Process { get; set; }

        public ProcessModuleCollection Modules { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string WorkingSet64 { get; set; }

        public string PagedMemorySize64 { get; set; }

        public string VirtualMemorySize64 { get; set; }

        public DateTime StartTime { get; set; }

        public int ThredsCount { get; set; }

        public ProcessThreadCollection Threads { get; set; }

        public ProcessPriorityClass PriorityClass
        {
            get => Process.PriorityClass;
            set
            {
                try
                {
                    Process.PriorityClass = value;
                    OnPropertyChanged(nameof(Process.PriorityClass));
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Acces denied!", MessageBoxButton.OK);
                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
