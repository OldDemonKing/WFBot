﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Newbe.Mahua.MahuaEvents;

namespace TRKS.WF.QQBot.MahuaEvents
{
    public class InitEvent1 : IInitializationMahuaEvent
    {
        private bool onlineBuild;
        private int localVersion;
        internal static Timer timer1;

        public InitEvent1()
        {
            onlineBuild = nameof(InitEvent1).Contains("_"); // trick
            if (onlineBuild)
            {
                localVersion = int.Parse(nameof(InitEvent1).Split(new[] { "_" }, StringSplitOptions.None)[1]);
                var timer = new Timer(TimeSpan.FromSeconds(55).TotalMilliseconds);
                timer1 = timer;
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }


        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var releaseData = ReleaseGetter.Get();
            var ver = new Version(releaseData.tag_name).Build;
            if (ver != localVersion)
            {
                Messenger.SendDebugInfo($"开始自动更新。当前版本为v{localVersion}, 将会更新到v{ver}");
                Process.Start("AutoUpdater.exe");
                timer1.Stop();
            }

        }

        public void Initialized(InitializedContext context)
        {
            Task.Delay(TimeSpan.FromSeconds(20)).ContinueWith(t =>
            {
                if (!onlineBuild)
                {
                    Messenger.SendDebugInfo("机器人已启动，你使用的是非官方构建，将不会启用自动更新功能。");
                }
                else
                {
                    Messenger.SendDebugInfo("机器人已启动，你使用的是官方构建，自动更新功能已经启用。");
                }
            });

        }
    }
}