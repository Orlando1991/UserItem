using System;
using System.Collections.Generic;

namespace UserItem
{
    public class Cosine : StrategyInterface
    {
        double StrategyInterface.calculate(User user1, User user2, List<int> uniqueArticles)
        {
            var allRatings1 = user1.articleRating;
            var allRatings2 = user2.articleRating;
            double part1 = 0; //Sigma =X*Y
            double part2 = 0; // ||x||*||y||         sqrt(Sigma x2)
            double sigmaX2 = 0;
            double sigmaY2 = 0;
            foreach(var article in uniqueArticles)
            {
                var rating1 = allRatings1[article]; //x
                var rating2 = allRatings2[article]; //y

                part1 += rating1 * rating2;
                sigmaX2 += Math.Pow(rating1, 2);
                sigmaY2 += Math.Pow(rating2, 2);
            }
            part2 = Math.Sqrt(sigmaX2) * Math.Sqrt(sigmaY2);

            return part1/part2;
        }
    }
}