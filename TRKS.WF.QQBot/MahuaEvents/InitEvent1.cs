﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Newbe.Mahua.MahuaEvents;
using WTF;
using Timer = System.Timers.Timer;

namespace TRKS.WF.QQBot.MahuaEvents
{
    public class InitEvent1 : IInitializationMahuaEvent
    {
        internal static bool onlineBuild;
        internal static int localVersion;
        private static volatile bool updating;
        internal static Timer timer1;

        static InitEvent1()
        {
            Directory.CreateDirectory("WFBotLogs");
            var listener = new TextWriterTraceListener(File.Open($"WFBotLogs\\WFBot-{DateTime.Now:yy-MM-dd_HH.mm.ss}.log", FileMode.Append, FileAccess.Write, FileShare.ReadWrite)) { TraceOutputOptions = TraceOptions.Timestamp };
            Trace.Listeners.Add(listener);
            Trace.AutoFlush = true;
            Trace.WriteLine($"WFBot started.", "WFBot Core");
        }

        public InitEvent1()
        {
            onlineBuild = nameof(InitEvent1).Contains("_"); // trick
            if (onlineBuild)
            {
                localVersion = int.Parse(nameof(InitEvent1).Split(new[] { "_" }, StringSplitOptions.None)[1]);
                var delay = string.IsNullOrWhiteSpace(Config.Instance.GitHubOAuthKey) ? 60 : 600;
                var timer = new Timer(TimeSpan.FromSeconds(delay).TotalMilliseconds);
                timer1 = timer;
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
                
            }
            Initialized();
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (HotUpdateInfo.PreviousVersion) return;

            try
            {
                if (Config.Instance.UpdateLexion)
                {
                    WFResource.UpdateLexion();
                }
                if (Config.Instance.AutoUpdate)
                {
                    var releaseData = ReleaseGetter.Get();
                    var ver = new Version(releaseData.tag_name).Build;
                    if (ver != localVersion)
                    {
                        if (updating) return;
                        updating = true;

                        Messenger.SendDebugInfo($"开始自动更新。当前版本为v{localVersion}, 将会更新到v{ver}");
                        Messenger.Broadcast($"机器人开始了自动更新, 当前版本为v{localVersion}, 将会更新到v{ver}, 大约在1分钟内机器人不会回答你的问题.");
                        AutoUpdateRR.Execute();
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (WebException)
            {
                // 忽略
            }
            catch (Exception exception)
            {
                Messenger.SendDebugInfo(exception.ToString());

            }

        }

        public void Initialized(InitializedContext context)
        {
            if (HotUpdateInfo.PreviousVersion) return;

            Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(t =>
            {
                if (!onlineBuild)
                {
                    Messenger.SendDebugInfo("机器人已启动，你使用的是非官方构建，将不会启用自动更新功能。");
                }
                else
                {
                    Messenger.SendDebugInfo($"机器人已启动，你使用的是官方构建，自动更新功能{(Config.Instance.AutoUpdate ? "已经启用" : "已经被关闭")}。");
                }

                WFResource.WFTranslator.TranslateSearchWord("上辈子日了狗, 这辈子 OOP."); // ???????????????????????
            });

        }
        public void Initialized()
        {
            if (HotUpdateInfo.PreviousVersion) return;

            Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(t =>
            {
                if (!onlineBuild)
                {
                    Messenger.SendDebugInfo("机器人已启动，你使用的是非官方构建，将不会启用自动更新功能。");
                }
                else
                {
                    Messenger.SendDebugInfo($"机器人已启动，你使用的是官方构建，自动更新功能{(Config.Instance.AutoUpdate ? "已经启用" : "已经被关闭")}。");
                }

                WFResource.WFTranslator.TranslateSearchWord("上辈子日了狗, 这辈子 OOP."); // ???????????????????????
            });

        }
    }
}
