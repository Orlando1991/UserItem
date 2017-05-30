using System;
using System.Collections.Generic;

namespace App
{
    public class User
    {
        public int id { get; set; }
        public SortedDictionary<int, double> articleRating { get; set;}

        public User(int id, SortedDictionary<int, double> articleRating)
        {
            this.id = id;
            this.articleRating = articleRating;
        }
        public void addArticleRating(int articleId, double rating) {
            articleRating.Add(articleId, rating);
        }
        public void setArticleRating(int index,double content){
            articleRating[index] = content;
        }

    }
}