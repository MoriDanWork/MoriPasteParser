using MoriAnonfilesChecker.Object_Class;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static MoriAnonfilesChecker.ProgramLogic.ProxyParseLogic;

namespace MoriAnonfilesChecker.ProgramLogic
{
    public static class MainLogic
    {
        public static MainWindow mainWindow;
        private static int CurrentThreads = 0;
        public static BlockingCollection<int> pagesStack;

        public static void StartTesting()
        {
            var proxyQueue = new BlockingCollection<Proxy>(new ConcurrentQueue<Proxy>(SettingsLogic.Proxies));
            pagesStack = new BlockingCollection<int>(new ConcurrentStack<int>());

            for (int i = 1; i <= SettingsLogic.Deep; ++i) pagesStack.Add(i);

            Task.Run(() =>
            {
                Proxy proxy = proxyQueue.Take();
                while (pagesStack.Count > 0)
                {
                    SettingsLogic.ParseComplete = false;
                    if (SettingsLogic.StopWork) break;

                    if (CurrentThreads < SettingsLogic.ParseThreadsCount)
                    {
                        Thread SubThread = new Thread(() =>
                        {
                            if (proxyQueue.Count == 0) goto Exit;
                            int pageNumber = pagesStack.Take();
                        ProxyRepeat:
                            try
                            {
                                var LinkList = GetLogic.GoogleParse(pageNumber, proxy);
                                LinkList.ForEach(x =>
                                    {
                                        ++SettingsLogic.LinksCount;
                                        MoriFile file = GetLogic.WorkWithUrls(x);
                                        SimpleGood(file);
                                    });
                            }
                            catch (RankException)
                            {
                                SettingsLogic.RemoveProxy(proxy);
                                ++SettingsLogic.ErrorCount;
                                proxy = proxyQueue.Take();
                                if (!SettingsLogic.StopWork) goto ProxyRepeat; else goto Exit;
                            }
                            ++SettingsLogic.ProgressCount;
                        Exit:
                            Interlocked.Decrement(ref CurrentThreads);
                            Check();
                        })
                        { IsBackground = true, Priority = ThreadPriority.BelowNormal };
                        Interlocked.Increment(ref CurrentThreads);
                        SubThread.Start();
                    }
                }
            });
        }

        private static void Check()
        {
            if (pagesStack.Count == 0 && CurrentThreads == 0)
            {
                goto Flex;
            }

            if (CurrentThreads == 0 && SettingsLogic.StopWork)
            {
                goto Flex;
            }
            return;

        Flex:
            SettingsLogic.ParseComplete = true;
            mainWindow.Stopped();
        }


        public static void SecondStart(List<string> uid)
        {
            try
            {
                foreach (var x in uid)
                {
                    MoriFile file = GetLogic.WorkWithUrls(x);
                    SimpleGood(file);
                }
            }
            catch (Exception) { }
        }

        public static void SimpleGood(MoriFile file)
        {
            if (file.Name != null)
            {
                if (SettingsLogic.Extenstion.ToList().Contains(file.Extension))
                {
                    if (CheckSize(file))
                    {
                        if (SettingsLogic.ExistCheck)
                        {
                            if (ExistLogic.Check(file.Uid))
                            {
                                mainWindow.AddCollection(file);
                                mainWindow.FileCount();
                                return;
                            }
                            else
                            {
                                ++SettingsLogic.ExistFilter;
                            }
                        }
                        else
                        {
                            mainWindow.AddCollection(file);
                            mainWindow.FileCount();
                            return;
                        }
                    }
                    else
                    {
                        ++SettingsLogic.SizeFilter;
                    }
                }
                else
                {
                    ++SettingsLogic.ExtensionFilter;
                }
            }
            else
            {
                ++SettingsLogic.BadUrl;
            }
        }
        private static bool CheckSize(MoriFile file)
        {
            if (file.Size <= SettingsLogic.FileSize)
                return true;
            return false;
        }
    }
}
