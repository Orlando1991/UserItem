using System;
using System.Collections.Generic;

namespace UserItem
{

    public class EuclidianDistance
    {
        public double calculate(User user1, User user2, List<int> uniqueArticles)
        {
            var distance = 0.0 ;
            var allRatings1 = user1.articleRating;
            var allRatings2 = user2.articleRating;
            foreach(var article in uniqueArticles)
            {
                double rating1, rating2;
                //check if the userratings have the key
                if(allRatings1.ContainsKey(article))
                {
                    rating1 = allRatings1[article];
                } else {
                    rating1 = 0;
                }
                if(allRatings2.ContainsKey(article))
                {
                    rating2 = allRatings2[article];
                } else {
                    rating2 = 0;
                }
                var i = rating1 - rating2;
                var squared = Math.Pow(i, 2);
                //you need to square the value before summing everything up
                distance += squared;
            }

            return Math.Sqrt(distance);
        }
    }
}