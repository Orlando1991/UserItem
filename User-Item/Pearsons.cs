using System;
using System.Collections.Generic;

namespace UserItem
{
    public class Pearsons
    {
        //if both article have a 0 rating we can exclude it. this comes in to play at part3 so n isnt higher than it should be
        List<int> excludeArticle = new List<int>();
        public double calculate(User user1, User user2, List<int> uniqueArticles)
        {
            var distance = 0.0 ;
            var allRatings1 = user1.articleRating;
            var allRatings2 = user2.articleRating;
            foreach(var article in uniqueArticles)
            {
                //check if the userratings have the key
                if(!allRatings1.ContainsKey(article))
                {
                    allRatings1.Add(article, 0);
                } 
                if(!allRatings2.ContainsKey(article))
                {
                    allRatings2.Add(article, 0);
                } 
            }

            //seperate it into chunks to avoid a mess
            double a = part1(allRatings1, allRatings2, uniqueArticles); //Sigma =X*Y
            double b = part2(allRatings1, allRatings2, uniqueArticles);//(Sigma x * Sigma y) / n (articles both rated)
            double c = part3(allRatings1, uniqueArticles);//sqrt(Sigma x2 - ( (sigma x)2 / n)
            double d = part3(allRatings2, uniqueArticles);//same as c but for the other list

            distance = (a - b) / (c * d);
            
            return distance;
        }

        private double part3(Dictionary<int, double> allRatings, List<int> uniqueArticles)
        {
            //sqrt(Sigma x2 - ( (sigma x)2 / n)
            double result = 0;
            double squared= 0; //Sigma x2
            double sigmaSquared = 0; //(sigma x)2
            var n = uniqueArticles.Count;
            foreach (var article in uniqueArticles)
            {
                if(!excludeArticle.Contains(article))
                {
                    var rating1 = allRatings[article];
                    squared += Math.Pow(rating1, 2);
                    sigmaSquared += rating1;
                } else{
                    n -= 1;
                }
                
            }
            sigmaSquared = Math.Pow(sigmaSquared, 2);
            result = squared - (sigmaSquared / n);
            return Math.Sqrt(result);
        }

        private double part2(Dictionary<int, double> allRatings1, Dictionary<int, double> allRatings2, List<int> uniqueArticles)
        {
            //(Sigma x * Sigma y) / n (articles both rated)
            double sigma1 = 0;
            double sigma2 = 0;
            int n = uniqueArticles.Count;
            foreach(var article in uniqueArticles)
            {
                var rating1 = allRatings1[article]; //x
                sigma1 += rating1;
                var rating2 = allRatings2[article]; //y
                sigma2 += rating2;
                //check if both have rating 0, so we dont have to count it
                if(rating1 == 0  && rating2 == 0)
                {
                    n -= 1;
                    excludeArticle.Add(article);
                }
            }
            return (sigma1*sigma2)/n;
            
        }

        private double part1(Dictionary<int, double> allRatings1, Dictionary<int, double> allRatings2, List<int> uniqueArticles)
        {
            //Sigma =X*Y
            double result = 0;
            foreach(var article in uniqueArticles)
            {
                var rating1 = allRatings1[article];
                var rating2 = allRatings2[article];

                var temp = rating1*rating2;
                result += temp;
            }

            return result;
        }
    }
}