﻿using MoriAnonfilesChecker.Object_Class;
using System.Collections.Concurrent;
using System.Threading;

namespace MoriAnonfilesChecker.ProgramLogic
{
    static class DownloadLogic
    {
        static BlockingCollection<MoriDownload> queue = new BlockingCollection<MoriDownload>(new ConcurrentQueue<MoriDownload>());
        public static int CurrentThreads = 0;
        public static int ThreadsCount = 3;
        public static MainWindow mainWindow;
        public static bool AllDownloaded = true;
        public static void Core()
        {
            if (ThreadsCount != 0)
                if (queue.Count != 0)
                {
                    AllDownloaded = false;
                    if (CurrentThreads < ThreadsCount)
                    {
                        Interlocked.Increment(ref CurrentThreads);
                        Thread SubThread = new Thread(() =>
                        {
                            queue.Take().Download();
                        })
                        { IsBackground = true, Priority = ThreadPriority.BelowNormal };
                        SubThread.Start();
                    }
                }
        }

        public static void Check()
        {
            if (queue.Count == 0 && CurrentThreads == 0)
            {
                goto Flex;
            }

            if (CurrentThreads == 0 && ThreadsCount == 0)
            {
                goto Flex;
            }
            return;

        Flex:
            AllDownloaded = true;
            mainWindow.Stopped();
        }

        public static void AddQueue(MoriFile file)
        {
            queue.Add(new MoriDownload(file, mainWindow));
            Core();
        }
    }
}
