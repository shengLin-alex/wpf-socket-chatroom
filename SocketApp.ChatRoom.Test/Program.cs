using System;
using System.Threading;

namespace SocketApp.ChatRoom.Test
{
    public class Program
    {
        private static volatile bool IsProgramActive;

        private static void Main(string[] args)
        {
            Runnable runnable = new Runnable();
            runnable.RequireStart();
            Thread outer = new Thread(() => 
            {
                try
                {
                    while (IsProgramActive)
                    {
                        Console.WriteLine("Outer Thread Running...");
                        Thread.Sleep(2000);

                        Thread inner = new Thread(() => runnable.Run())
                        {
                            IsBackground = true
                        };
                        inner.Start();
                    }
                }
                catch (ThreadInterruptedException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    Console.WriteLine("Outer Thread Stopped.");
                }
            })
            {
                IsBackground = true
            };
            outer.Start();

            string command = Console.ReadLine();
            if (command == "")
            {
                runnable.RequireStop();
            }

            Console.ReadLine();
        }

        private class Runnable
        {
            private volatile bool IsActive;

            public Runnable()
            {
            }

            public void RequireStart()
            {
                this.IsActive = true;
                IsProgramActive = true;
            }

            public void RequireStop()
            {
                this.IsActive = false;
                IsProgramActive = false;
            }

            public void Run()
            {
                try
                {
                    while (this.IsActive)
                    {
                        Console.WriteLine("Inner Thread running...");
                        Thread.Sleep(1000);
                    }
                }
                catch (ThreadInterruptedException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    Console.WriteLine("Inner Thread Stopped.");
                }
            }
        }
    }
}