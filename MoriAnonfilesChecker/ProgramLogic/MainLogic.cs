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
        public static int Pages;
        public static string[] Extension;
        public static string Request;
        public static long MaxSize;
        public static MainWindow mainWindow;
        private static int CurrentThreads = 0;
        public static int ThreadsCount = 3;
        public static bool AllParsed = false;
        public static bool StopParse = false;
        public static BlockingCollection<int> pagesStack;

        public static void StartTesting()
        {
            var queue = new BlockingCollection<Proxy>(new ConcurrentQueue<Proxy>(mainWindow.proxies));
            pagesStack = new BlockingCollection<int>(new ConcurrentStack<int>());

            while (Pages != 0)
            {
                pagesStack.Add(Pages);
                --Pages;
            }


            Task.Run(() =>
            {
                Proxy proxy = queue.Take();
                while (pagesStack.Count > 0)
                {

                    if (ThreadsCount == 0) break;

                    if (CurrentThreads < ThreadsCount)
                    {
                        Interlocked.Increment(ref CurrentThreads);

                        Thread SubThread = new Thread(() =>
                        {
                            int pageNumber = pagesStack.Take();
                        Flex:
                            try
                            {
                                var LinkList = GetLogic.GoogleParse(pageNumber, Request, proxy);
                                LinkList.ForEach(x =>
                                    {
                                        mainWindow.AddLinksCount();
                                        MoriFile file = GetLogic.WorkWithUrls(x);
                                        SimpleGood(file);
                                    });
                            }
                            catch (RankException)
                            {
                                mainWindow.BadProxy();
                                mainWindow.AddError();
                                proxy = queue.Take();
                                if(!StopParse) goto Flex;
                            }
                            mainWindow.AddProgress();
                            Interlocked.Decrement(ref CurrentThreads);
                            Check();
                        })
                        { IsBackground = true, Priority = ThreadPriority.BelowNormal };
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

            if (CurrentThreads == 0 && ThreadsCount == 0)
            {
                goto Flex;
            }
            return;

        Flex:
            AllParsed = true;
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
                if (Extension.ToList().Contains(file.Extension))
                    if (CheckSize(file))
                    //if (ExistLogic.Check(file.Uid))
                    {
                        mainWindow.AddCollection(file);
                        mainWindow.FileCount();
                        return;
                    }
            mainWindow.AddFilterCount();
        }
        private static bool CheckSize(MoriFile file)
        {
            if (file.Size < MaxSize)
                return true;
            return false;
        }
    }
}
