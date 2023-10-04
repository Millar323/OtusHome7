using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusHome7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] ints = { 100000, 1000000, 10000000 };

            foreach (var a in ints)
            {
                int[] array = randomizer(a);
                Console.WriteLine("Размер массива: " + a);

                var stopwatch = Stopwatch.StartNew();
                long sequentialSum = Sum(array);
                stopwatch.Stop();
                Console.WriteLine("Сумма элементов(обычная): " + sequentialSum);
                Console.WriteLine("Время выполнения: " + stopwatch.ElapsedMilliseconds + " ms");

                stopwatch.Reset();
                stopwatch.Start();
                long parallelSum = ParallelSum(array);
                stopwatch.Stop();
                Console.WriteLine("Сумма элементов(параллельная): " + parallelSum);
                Console.WriteLine("Время выполнения: " + stopwatch.ElapsedMilliseconds + " ms");

                stopwatch.Reset();
                stopwatch.Start();
                long linqSum = LinqSum(array);
                stopwatch.Stop();
                Console.WriteLine("Сумма элементов(LINQ): " + linqSum);
                Console.WriteLine("Время выполнения: " + stopwatch.ElapsedMilliseconds + " ms");

                Console.WriteLine();
                Console.ReadKey();
            }
        }

        static int[] randomizer(int mas)
        {
            int[] array = new int[mas];
            Random random = new Random();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = random.Next(100);
            }
            return array;
        }

        static long Sum(int[] array)
        {
            long sum = 0;
            foreach (int num in array)
            {
                sum += num;
            }
            return sum;
        }

        static long ParallelSum(int[] array)
        {
            long sum = 0;
            object lockObj = new object();

            var tasks = new List<Task>();
            int chunkSize = array.Length / Environment.ProcessorCount;

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                int startIndex = i * chunkSize;
                int endIndex = (i == Environment.ProcessorCount - 1) ? array.Length : (i + 1) * chunkSize;

                tasks.Add(Task.Run(() =>
                {
                    long partialSum = 0;
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        partialSum += array[j];
                    }

                    lock (lockObj)
                    {
                        sum += partialSum;
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            return sum;
        }

        static long LinqSum(int[] array)
        {
            return array.AsParallel().Sum();
        }
    }
}
