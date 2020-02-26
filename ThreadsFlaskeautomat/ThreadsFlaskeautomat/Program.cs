using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadsFlaskeautomat
{
    class Program
    {
        static object _lock = new object();
        static Queue<Drink> mainBuffer = new Queue<Drink>();
        static Queue<Drink> sodaBuffer = new Queue<Drink>();
        static Queue<Drink> beerBuffer = new Queue<Drink>();
        static int maxCount = 10;
        static int numberOfProducers = 5;

        public static void Main()
        {
            for (int i = 0; i < numberOfProducers; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Producer));
            }
            new Thread(Splitter).Start();
            new Thread(SodaConsumer).Start();
            new Thread(BeerConsumer).Start();

        }

        public static void SodaConsumer()
        {
            while (true)
            {
                lock (sodaBuffer)
                {
                    while (sodaBuffer.Count == 0)
                    {
                        Console.WriteLine("                                         Soda Consumer waits");
                        Monitor.Wait(sodaBuffer);
                    }

                    Soda drink = (Soda)sodaBuffer.Dequeue();
                    Console.WriteLine("                                         Soda Consumer is taking " + drink.Name + " with ID:" + drink.RunnerNumber);
                    Monitor.Pulse(sodaBuffer);
                }
                Thread.Sleep(1000);
            }
        }

        public static void BeerConsumer()
        {
            while (true)
            {
                lock (beerBuffer)
                {
                    while (beerBuffer.Count == 0)
                    {
                        Console.WriteLine("                                         Beer Consumer waits");
                        Monitor.Wait(beerBuffer);
                    }

                    Beer drink = (Beer)beerBuffer.Dequeue();
                    Console.WriteLine("                                         Beer Consumer is taking " + drink.Name + " with ID:" + drink.RunnerNumber);
                    Monitor.Pulse(beerBuffer);
                }
                Thread.Sleep(1000);
            }
        }

        public static void Splitter()
        {
            while (true)
            {
                lock (_lock)
                {
                    while (mainBuffer.Count == 0)
                    {
                        Console.WriteLine("             Splitter waits");
                        Monitor.Wait(_lock);
                    }

                    Drink drink = mainBuffer.Dequeue();
                    if(drink is Soda)
                    {
                        Monitor.Enter(sodaBuffer);
                        try
                        {
                            sodaBuffer.Enqueue(drink);
                        }
                        finally
                        {
                            Monitor.Pulse(sodaBuffer);
                            Monitor.Exit(sodaBuffer);
                        }
                    }
                    else if(drink is Beer)
                    {
                        Monitor.Enter(beerBuffer);
                        try
                        {
                            beerBuffer.Enqueue(drink);
                        }
                        finally
                        {
                            Monitor.Pulse(beerBuffer);
                            Monitor.Exit(beerBuffer);
                        }
                    }

                    Console.WriteLine($"             Splitter is splitting {drink.Name} with ID:{drink.RunnerNumber} to {(drink is Soda ? "Soda" : "Beer")} queue.");
                    Monitor.Pulse(_lock);
                }
                Thread.Sleep(1000);
            }
        }
        public static void Producer(object obj)
        {
            Random r = new Random();
            while (true)
            {
                lock (_lock)
                {
                    for (int i = 0; i < maxCount; i++)
                    {
                    
                        while (mainBuffer.Count > maxCount)
                        {
                            Console.WriteLine("Producer waits");
                            Monitor.Wait(_lock);
                        }

                        Drink drink;
                        if (r.Next(0, 2) == 0)
                        {
                            drink = new Soda(i + 1);
                        }
                        else
                        {
                            drink = new Beer(i + 1);
                        }

                        mainBuffer.Enqueue(drink);
                        Console.WriteLine("Producer is adding " + drink.Name + " with ID:" + drink.RunnerNumber);
                        Monitor.Pulse(_lock);
                    }
                }
                Thread.Sleep(1000);

            }
        }


    }
}
