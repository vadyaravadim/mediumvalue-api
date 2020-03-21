using HtmlAgilityPack;
using mediumvalue_api.Interface;
using mediumvalue_api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mediumvalue_api.Helper
{
    public class HandlerRequest
    {
        public void HandlerStartRequest(string method)
        {
            Console.WriteLine("**********************");
            Console.WriteLine($"START PROCESSING {method}");
            Console.WriteLine($"START TIME {DateTime.Now}");
            Console.WriteLine("**********************");
        }

        public void HandlerEndRequest(string method)
        {
            Console.WriteLine("**********************");
            Console.WriteLine($"END PROCESSING {method}");
            Console.WriteLine($"END TIME {DateTime.Now}");
            Console.WriteLine("**********************");
        }
    }

    public static class MathStatistic
    {
        internal static (int mediana, bool isEven) CalcMediana(List<int> arr, bool forPages = false)
        {
            arr.Sort();

            if (forPages)
            {
                bool validatingSequence = ValidatingSequence(arr);
                if (!validatingSequence)
                {
                    arr = DetermineSequence(arr[arr.Count - 1]);
                }
            }

            int middleArray = Convert.ToInt32(Math.Floor((decimal)arr.Count() / 2));

            if (arr.Count() % 2 != 0)
            {
                return (arr[middleArray], false);
            }

            if (arr.Count() % 2 == 0)
            {
                return ((arr[middleArray - 1] + arr[middleArray]) / 2, true);
            }

            throw new ArgumentException("Value is not processed");
        }

        private static List<int> DetermineSequence(int quantity)
        {

            List<int> validArr = new List<int>();

            for (int i = 1; i <= quantity; i++)
            {
                validArr.Add(i);
            }

            return validArr;
        }

        private static bool ValidatingSequence(List<int> arr)
        {
            if (arr is null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            for (int i = 1; i <= arr.Count; i++)
            {
                if (arr[i - 1] != i)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public static class HelperDatabaseRequest
    {
        internal static async Task<PricesStatistic> SelectExistDatePriceAsync(string query, RequestDatabase database)
        {
            PricesStatistic[] pricesStatistic = await database.PricesStatisticExistDate(query);
            if (pricesStatistic != null && pricesStatistic.Any())
            {
                return pricesStatistic[0];
            }

            return null;
        }
    }
}
