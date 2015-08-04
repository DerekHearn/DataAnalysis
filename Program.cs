using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis
{
	class Program
	{
		static void Main(string[] args)
		{
			var ml = new MethodLog();

			var today = DateTime.Today.AddHours(12);
			var numOfDays = 5;

            //durationsByMethod

            var result = ml.durationsByMethod(numOfDays);
            if (result.wasSuccess())
            {
                for (int i = 0; i < result.Item.Length; i++)
                {
                    Console.WriteLine(String.Format("Method Name: {0}",
                        result.Item[i].Key));

                    var averageDur = result.Item[i].Value;

                    for (int j = 0; j < averageDur.Length; j++)
                    {
                        if (i + 1 < result.Item.Length)
                        {
                            Console.WriteLine(String.Format("{0} days ago - {0}", j, averageDur[j]));

                            var delta = averageDur[i] - averageDur[i + 1];
                            var change = "SAME";
                            if (delta > 0)
                                change = "UP";
                            if (delta < 0)
                                change = "DOWN";
                            Console.WriteLine(String.Format(" - {1}: {0}", delta, change));
                        }
                    }
                }
            }
            else
                Console.WriteLine(result.MBResult.developerErrorMsg);

            //var result = ml.numberOfCalls(numOfDays);
            //if (result.wasSuccess())
            //{
            //    int averageChangeCount = 0;
            //    int averageCallCount = 0;
            //    int[] array = new int[result.Item.Length - 1];
            //    for (int i = 0; i < result.Item.Length; i++)
            //    {
            //        averageCallCount += result.Item[i];
            //        Console.Write(String.Format("{0} days ago - Total calls: {1}", 
            //            i ,result.Item[i]));
            //        if (i + 1 < result.Item.Length)
            //        {
            //            var delta = result.Item[i] - result.Item[i+1];
            //            array[i] = delta;
            //            averageChangeCount += delta;
            //            var change = "SAME";
            //            if(delta > 0)
            //                change = "UP";
            //            if(delta < 0)
            //                change = "DOWN";
            //            Console.WriteLine(String.Format(" - {1}: {0}",delta, change));
            //        }
            //    }

            //    var avgCalls = averageCallCount / result.Item.Length;
            //    var avgDelta = averageChangeCount / array.Length;
            //    Console.WriteLine(String.Format("\nTrend: {0}\tAverage Calls: {1}", avgDelta, avgCalls));
            //}
            //else
            //    Console.WriteLine(result.MBResult.developerErrorMsg);

			Console.ReadLine();
		}
	}
}
