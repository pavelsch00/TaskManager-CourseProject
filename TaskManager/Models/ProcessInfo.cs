using System.Diagnostics;

namespace TaskManager.Models
{
    public class ProcessInfo
    {
        public ProcessInfo(Process process)
        {
            Process = process;
            Id = process.Id;
            ProcessName = process.ProcessName;
        }

        public int Id { get; set; }

        public string ProcessName { get; set; }

        public Process Process { get; set; }
    }
}
