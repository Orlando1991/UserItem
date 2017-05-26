using System;
using System.Collections.Generic;

namespace UserItem
{
    public class Pearsons : StrategyInterface
    {
        List<int> excludeArticle = new List<int>();
        double StrategyInterface.calculate(User user1, User user2, List<int> uniqueArticles)
        {
            double sum1, sum2, sum3, sum4, sum5;
            sum1=sum2=sum3=sum4=sum5= .0;

            var n = 0;
            var allRatings1 = user1.articleRating;
            var allRatings2 = user2.articleRating;
            foreach(var article in allRatings1)
            {
                var key = article.Key;
                var x = allRatings1[key];
                var y = allRatings2[key];
                if(x == 0 || y == 0)
                {
                    continue;
                }

                sum1 += x * y;
                sum2 += x;
                sum3 += y;
                sum4 += Math.Pow(x, 2);
                sum5 += Math.Pow(y, 2);
                n++;
            }
            return (sum1 - (sum2 * sum3) / n) / ((Math.Sqrt(sum4 - ((Math.Pow(sum2, 2) / n)))) * Math.Sqrt(sum5 - ((Math.Pow(sum3, 2) / n))));
        }
    }
}