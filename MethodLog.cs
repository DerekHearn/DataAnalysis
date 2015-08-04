using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResultWrappers;

namespace DataAnalysis
{
	public class MethodLog
	{
		public BLResult<int> numberOfCalls()
		{
			try
			{
				var query = DBDataAccess.MethodLog
					.Query(log => log.MethodLogID > 0);

				var otherCount = query.Count();

				return new BLResult<int>(otherCount);
			}
			catch (Exception e)
			{
				return new BLResult<int>(e);
			}
		}

		public BLResult<int> numberOfCallsByDay(DateTime day)
		{
			try
			{
				DateTime start = day.AddHours(-12);
				DateTime end = day.AddHours(12);
                var query = getAllWithinDateRange(start, end);

				var otherCount = query.Count();

				return new BLResult<int>(otherCount);
			}
			catch (Exception e)
			{
				return new BLResult<int>(e);
			}
		}

		public BLResult<int[]> numberOfCalls(int daysBack)
		{
			try
			{
				var utcnow = DateTime.UtcNow;
				DateTime start = DateTime.Today.AddDays(-daysBack);
				DateTime end = utcnow;
                var query = getAllWithinDateRange(start, end);

				var list = new List<int>();

				for(int i = 0; i <= daysBack; i++)
				{
					var t_start = utcnow.AddDays(-(1+i));
					var t_end = t_start.AddDays(1);
					list.Add(query.Count(log=> 
						log.CallDate > t_start &&
						log.CallDate < t_end));
                }

				return new BLResult<int[]>(list.ToArray());
			}
			catch (Exception e)
			{
				return new BLResult<int[]>(e);
			}
		}

        public BLResult<KeyValuePair<string, double[]>[]> durationsByMethod(int daysBack)
        {
            try
            {
                var utcnow = DateTime.UtcNow;
                DateTime start = DateTime.Today.AddDays(-daysBack);
                DateTime end = utcnow;
                var query = getAllWithinDateRange(start, end);
                
                var MethodNames = query.Select(log=>
                    log.MethodLogName).Distinct().ToArray();

                var tasks = new Task<KeyValuePair<string, double[]>>[MethodNames.Length];

                var results = new KeyValuePair<string, double[]>[MethodNames.Length];

                for (int i = 0; i < MethodNames.Length; i++)
                {
                    Console.WriteLine("-{0}", MethodNames[i]);
                    var tempI = i;

                    var query2 = query.Where(log =>
                        log.MethodLogName == MethodNames[tempI]);

                    var averages = new double[daysBack + 1];

                    for (int j = 0; j <= daysBack; j++)
                    {
                        var t_start = utcnow.AddDays(-(1 + j));
                        var t_end = t_start.AddDays(1);
                        var q3 = query2
                            .Where(log => log.CallDate > t_start
                                && log.CallDate < t_end)
                            .Select(log => log.Duration)
                            .ToArray();

                        double averageCount = 0;
                        int count = 0;

                        for (int k = 0; k < q3.Length; k++)
                        {
                            if (q3[k].HasValue)
                            {
                                averageCount += q3[k].Value;
                                count++;
                            }
                        }

                        averages[j] = averageCount / count;
                        Console.WriteLine("--{0} - {1:0.00}ms", MethodNames[i], averages[j]);
                    }
                    results[i] = new KeyValuePair<string, double[]>(
                        MethodNames[tempI], averages);
                }
                
                for (int i = 0; i < tasks.Length; i++)
                    results[i] = tasks[i].Result;

                return new BLResult<KeyValuePair<string, double[]>[]>(results);
            }
            catch (Exception e)
            {
                return new BLResult<KeyValuePair<string, double[]>[]>(e);
            }
        }

        private IQueryable<DBDataAccess.MethodLog> getAllWithinDateRange(
            DateTime min, DateTime max)
        {
            return DBDataAccess.MethodLog.Query(log => 
                log.CallDate > min && log.CallDate < max);
        }
	}
}
