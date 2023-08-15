using MoriPattern.Data;
using MoriPattern.Model;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoriPattern.Controller
{
    public static partial class StartController
    {
        enum Result
        {
            Good,
            Bad,
            Error
        }

        public static async Task Start()
        {
            CheckIfPrepare();

            

            SemaphoreSlim semaphore = new(GlobalData.Instance.ThreadsCount);
            SemaphoreSlim currentSourceCountLock = new(1);
            SemaphoreSlim threadsCountSemaphore = new(1);
            GlobalData.Instance.CurrentSourceCount = 0;
            List<Task> subTasks = new();
            GlobalData.Instance.IsWork = true;

            try
            {
                await Task.Run(() =>
                {
                    BlockingCollection<string> source = new(new ConcurrentQueue<string>(GlobalData.Instance.Source));
                    BlockingCollection<Proxy> proxies = new(new ConcurrentQueue<Proxy>(GlobalData.Instance.ProxyList));
                    source.CompleteAdding();

                    while (!source.IsCompleted)
                    {
                        if (GlobalData.Instance.StopWork)
                        {
                            break;
                        }

                        if (GlobalData.Instance.CurrentThreads < GlobalData.Instance.ThreadsCount && !source.IsCompleted)
                        {
                            try
                            {
                                semaphore.Wait();

                                Thread SubThread = new(() =>
                                {
                                    string sourceItem = null;
                                    Proxy proxyItem = new();

                                    lock (source)
                                    {
                                        if (!source.IsCompleted)
                                        {
                                            sourceItem = source.Take();

                                            GlobalData.Instance.CurrentSourceCount += 1;

                                            if (GlobalData.Instance.UseProxy)
                                            {
                                                proxyItem = proxies.Take();
                                                proxies.Add(proxyItem);
                                            }
                                        }
                                    }

                                    if (sourceItem != null)
                                    {
                                        try
                                        {
                                            MainLogic(sourceItem, proxyItem);
                                        }
                                        catch (RankException)
                                        {
                                            lock (proxies)
                                            {
                                                if (proxies.TryTake(out Proxy removedProxy))
                                                {
                                                    if (proxyItem.Try < 5)
                                                    {
                                                        proxyItem.Try += 1;
                                                        proxies.Add(proxyItem);
                                                    }
                                                }
                                            }
                                        }


                                    }

                                    lock (currentSourceCountLock)
                                    {
                                        --GlobalData.Instance.CurrentThreads;
                                    }

                                    semaphore.Release();
                                })
                                { IsBackground = true, Priority = ThreadPriority.BelowNormal };

                                lock (currentSourceCountLock)
                                {
                                    ++GlobalData.Instance.CurrentThreads;
                                }

                                SubThread.Start();
                            }
                            catch (Exception ex) { Debug.WriteLine(ex); }
                        }
                    }

                    threadsCountSemaphore.Wait();
                    if (GlobalData.Instance.CurrentThreads == 0)
                    {
                        GlobalData.Instance.StopWork = false;
                    }
                    threadsCountSemaphore.Release();
                });
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }

            await Task.WhenAll(subTasks);

            GlobalData.Instance.IsWork = false;
            GlobalData.Instance.StopWork = false;
        }

        private static void MainLogic(string sourceItem, Proxy proxy)
        {
            Result res = Result.Error;

            switch (res) {
                case Result.Good:
                    ++GlobalData.Instance.GoodCount;
                    break;
                case Result.Error:
                    ++GlobalData.Instance.ErrorCount;
                    break;
                case Result.Bad:
                    ++GlobalData.Instance.BadCount;
                    break;

            }
        }

        public static void CheckIfPrepare()
        {
            if (GlobalData.Instance.UseProxy && GlobalData.Instance.ProxyList.Count == 0) throw new RankException("You need to load proxy.");
            if (GlobalData.Instance.Source.Count == 0) throw new RankException("You need to load source.");

        }
    }
}
