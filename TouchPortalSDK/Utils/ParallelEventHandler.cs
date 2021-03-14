using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalSDK.Messages.Events;

namespace TouchPortalSDK.Utils
{
    public class ParallelEventHandler : ITouchPortalEventHandler
    {
        public Func<InfoEvent, Task> OnInfoEventAsync { get; set; }

        public Func<ListChangeEvent, Task> OnListChangedEventAsync { get; set; }

        public Func<BroadcastEvent, Task> OnBroadcastEventAsync { get; set; }

        public Func<SettingsEvent, Task> OnSettingsEventAsync { get; set; }

        public Func<ActionEvent, Task> OnActionEventAsync { get; set; }

        public Func<string, Task> OnClosedEventAsync { get; set; }

        public Func<string, Task> OnUnhandledEventAsync { get; set; }

        private readonly CancellationToken _cancellationToken;
        public string PluginId { get; }

        private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        private readonly SemaphoreSlim _throttling;

        public ParallelEventHandler(string pluginId, int parallelism, CancellationToken cancellationToken = default)
        {
            _cancellationToken = cancellationToken;
            PluginId = pluginId;
            _throttling = new SemaphoreSlim(parallelism, parallelism);
        }

        private void CreateParallelTask(Func<Task> callback)
        {
            var task = Task.Run(async () =>
            {
                try
                {
                    await _throttling.WaitAsync(_cancellationToken);
                    if (_cancellationToken.IsCancellationRequested)
                        return;

                    await callback();
                }
                catch
                {
                    //ignore
                }
                finally
                {
                    if (!_cancellationToken.IsCancellationRequested)
                        _throttling.Release(1);
                }
            }, _cancellationToken);
            _tasks.Add(task, _cancellationToken);
        }

        void ITouchPortalEventHandler.OnInfoEvent(InfoEvent message)
        {
            var func = OnInfoEventAsync;
            if (func != null)
                CreateParallelTask(() => func(message));
        }

        void ITouchPortalEventHandler.OnListChangedEvent(ListChangeEvent message)
        {
            var func = OnListChangedEventAsync;
            if (func != null)
                CreateParallelTask(() => func(message));
        }

        void ITouchPortalEventHandler.OnBroadcastEvent(BroadcastEvent message)
        {
            var func = OnBroadcastEventAsync;
            if (func != null)
                CreateParallelTask(() => func(message));
        }

        void ITouchPortalEventHandler.OnSettingsEvent(SettingsEvent message)
        {
            var func = OnSettingsEventAsync;
            if (func != null)
                CreateParallelTask(() => func(message));
        }

        void ITouchPortalEventHandler.OnActionEvent(ActionEvent message)
        {
            var func = OnActionEventAsync;
            if (func != null)
                CreateParallelTask(() => func(message));
        }

        void ITouchPortalEventHandler.OnClosedEvent(string message)
        {
            var func = OnClosedEventAsync;
            if (func != null)
                CreateParallelTask(() => func(message));
        }

        void ITouchPortalEventHandler.OnUnhandledEvent(string jsonMessage)
        {
            var func = OnUnhandledEventAsync;
            if (func != null)
                CreateParallelTask(() => func(jsonMessage));
        }

        public void Close()
        {
            Task.WaitAll(_tasks.ToArray(), _cancellationToken);
        }
    }
}
