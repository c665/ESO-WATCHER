using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WatcherUI.Utils
{
    public class WinProcessWatcher
    {
        #region process handling
        private string _processName;
        private string _exe = string.Empty;

        private Process[] _processes;
        public Process[] Processes
        {
            get
            {
                _processes = Process.GetProcessesByName(_processName);
                return _processes;
            }
            set => _processes = value;
        }
        public bool IsActive
        {
            get => Processes.Any();
        }

        public WinProcessWatcher(string processName)
        {
            _processName = processName;
        }

        public WinProcessWatcher(string processName, string path)
        {
            _processName = processName;
            _exe = path;
        }

        public void Start()
        {
            if(!string.IsNullOrEmpty(_exe))
                Process.Start(_exe);
        }

        public void Kill()
        {
            foreach (var process in Processes)
                process.Kill();
        }
        #endregion

        #region watch handling
        private const int TICK = 1 * 1000;//ms
        private bool _isWatching = false;
        private object lockObject = false;

        public event Action OnProcessAlive;
        public event Action OnProcessDead;
        public event Action OnEveryTick;

        public void StartWatching()
        {
            Task.Run(Watch);
        }

        public void StopWatching()
        {
            setWatching(false);
        }

        private async Task Watch()
        {
            setWatching(true);

            while (await IsWatching())
            {
                bool state = IsActive;
                OnEveryTick?.Invoke();

                if (state)
                    OnProcessAlive?.Invoke();
                else
                    OnProcessDead?.Invoke();

                Thread.Sleep(TICK);
            }
        }

        private async Task<bool> IsWatching()
        {
            Func<bool> func = isWatching;
            return await Task.Run(func);
        }

        private void setWatching(bool isWatching)
        {
            lock (lockObject)
                _isWatching = isWatching;
        }

        private bool isWatching()
        {
            bool isWatching;
            lock (lockObject)
            {
                isWatching = _isWatching;
            }

            return isWatching;
        }
        #endregion
    }
}
