using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TaskManager.Models
{
    public class ProcessInfo : INotifyPropertyChanged
    {
        private StateProcess _state;

        public bool checkState;

        public ProcessInfo(Process process)
        {
            Process = process;
            Id = process.Id;
            ProcessName = process.ProcessName;
            checkState = true;
            foreach (var item in process.Threads)
            {
                try
                {
                    if ((item as ProcessThread).WaitReason == ThreadWaitReason.Suspended)
                    {
                        checkState = false;
                    }
                }
                catch (System.Exception)
                {
                    State = StateProcess.Active;
                }

            }

            if(checkState)
            {
                State = StateProcess.Active;
            }
            else
            {
                State = StateProcess.White;
            }
        }

        public int Id { get; set; }

        public string ProcessName { get; set; }

        public Process Process { get; set; }

        public StateProcess State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged(nameof(State));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
