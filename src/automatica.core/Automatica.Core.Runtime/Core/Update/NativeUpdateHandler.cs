﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Automatica.Core.Base.Common;
using Automatica.Core.EF.Models;
using Automatica.Core.Internals;
using Automatica.Core.Internals.Cloud;
using Automatica.Core.Runtime.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Automatica.Core.Runtime.Core.Update
{
    public class NativeUpdateHandler : IUpdateHandler
    {
        private readonly IConfiguration _config;
        private readonly ICloudApi _api;

        private readonly Timer _timer = new Timer();


        public NativeUpdateHandler(IConfiguration config, ICloudApi api)
        {
            _config = config;
            _api = api;

            _timer.Elapsed += _timer_Elapsed;
        }

        private async void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SystemLogger.Instance.LogInformation("Check for update");
            _timer.Stop();
            var update = await _api.CheckForUpdates();

            if (update != null)
            {

                SystemLogger.Instance.LogInformation("Download update");
                await _api.DownloadUpdate(update);
                SystemLogger.Instance.LogInformation("Install update");
            }

            await Init();
        }

        public Task Update()
        {
            Environment.Exit(ServerInfo.ExitCodeUpdateInstall);
            return Task.CompletedTask;
        }

        public Task Init()
        {
            _timer.Stop();
            using (var context = new AutomaticaContext(_config))
            {
                var autoUpdate = context.Settings.SingleOrDefault(a => a.ValueKey == "autoUpdate");
                var autoUpdateTime = context.Settings.SingleOrDefault(a => a.ValueKey == "autoUpdateTime");

                if (autoUpdate != null && (bool) autoUpdate.Value && autoUpdateTime != null && autoUpdateTime.Value is DateTime updateTime)
                {
                    var now = DateTime.Now;
                    var updateTimeToday = new DateTime(now.Year, now.Month, now.Day, updateTime.Hour, updateTime.Minute,
                        updateTime.Second);

                    if (now > updateTimeToday)
                    {
                        updateTimeToday = updateTimeToday.AddDays(1);
                    }
                    var diff = (updateTimeToday - now).TotalSeconds;

                    _timer.Interval = diff * 1000;
                    SystemLogger.Instance.LogInformation($"Next check for update is in {diff} seconds at {updateTimeToday}");
                    _timer.Start();
                }
                else
                {
                    SystemLogger.Instance.LogInformation("Auto update seems to be disabled");
                }
            }
            return Task.CompletedTask;
        }

        public Task ReInitialize()
        {
            return Init();
        }
    }
}
