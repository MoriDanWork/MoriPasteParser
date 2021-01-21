using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MoriAnonfilesChecker.ProgramLogic.ProxyParseLogic;

namespace MoriAnonfilesChecker.ProgramLogic
{
    public static class MainLogic
    {
        public static int Threads = 1;
        public static int Timeout = 5000;
        public static int Pages = 1;
        public static int CurrentPage = 0;
        public static string[] Extension;
        public static string Request = "Good";
        public static int CurrentThreads = 0;
        public static void StartTesting(MainWindow mw)
        {
            Pages = CurrentPage + Pages;
            ExistLogic.GetBase();

            Task.Run(() =>
            {

                while (Pages > CurrentPage)
                {
                    if (Threads == 0)
                    {
                        break;
                    }

                    if (CurrentThreads < Threads + 1)
                    {
                        try
                        {
                            Thread SubThread = new Thread(() =>
                            {
                                try
                                { 
                                    Interlocked.Increment(ref CurrentPage);
                                    var LinkList = GetLogic.GoogleParse(CurrentPage - 1, Request, new Proxy("202.142.159.204", 31026, ProxyType.Socks4));
                                    Debug.WriteLine("G - " + LinkList.Count); // Flex
                                    LinkList = ExistLogic.Check(LinkList);
                                    var FilteredList = GetLogic.WorkWithUrls(LinkList);
                                    FilteredList.RemoveAll(x => Extension.Any(y => x.Extension.Contains(y)));
                                    Debug.WriteLine("F - " + FilteredList.Count);
                                }
                                catch (RankException)
                                {

                                }
                                Interlocked.Decrement(ref CurrentThreads);

                                if (CurrentThreads == 0)
                                {
                                    return;//stop
                                }
                            })
                            { IsBackground = true, Priority = ThreadPriority.BelowNormal };
                            Interlocked.Increment(ref CurrentThreads);
                            SubThread.Start();
                        }
                        catch (Exception)
                        {
                        }

                    }
                }
            });
        }
    }
}
