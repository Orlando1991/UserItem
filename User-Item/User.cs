using System;
using System.Collections.Generic;

namespace UserItem
{
    public class User
    {
        public int id { get; set; }
        public Dictionary<int, double> articleRating { get; set;}
        public double euclidean_distance {get; set;}
        public double cosine {get; set;}
        public double pearson_correlation {get; set;}

        public User(int id, Dictionary<int, double> articleRating)
        {
            this.id = id;
            this.articleRating = articleRating;
        }
        public void addArticle(int articleId, double rating)
        {
            articleRating.Add(articleId, rating);
        }
        
    }
}