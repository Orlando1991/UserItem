using System;
using System.Collections.Generic;

namespace UserItem
{

    public class EuclidianDistance : StrategyInterface
    {
        double StrategyInterface.calculate(User user1, User user2, List<int> uniqueArticles)
        {
            var distance = 0.0 ;
            var allRatings1 = user1.articleRating;
            var allRatings2 = user2.articleRating;
            foreach(var article in allRatings1)
            {
                var key = article.Key;
                if(allRatings1[key] == 0 || allRatings2[key] == 0)
                {
                    continue;
                }
                var i = allRatings1[key] - allRatings2[key];
                distance += Math.Pow(i, 2);
            }
            
            return 1 / (1 + Math.Sqrt(distance)); //1 / 1+ euclidian distance is similarity formula
        }
    }
}