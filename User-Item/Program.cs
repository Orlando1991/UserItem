using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace UserItem
{
    class Program
    {
        private static string FILE_PATH = "C:\\Users\\Orlando\\Documents\\HRO\\INF3B\\OP4\\Data Science\\User-Item\\userItem.csv";
        static Dictionary<int, User> userpref = new Dictionary<int, User>();
        
        static void Main(string[] args)
        {
            readFile();
            
            foreach(var item in userpref)
            {
                Console.WriteLine("uid: " + item.Key);
                var user = userpref[item.Key];
                var userRatings = user.articleRating;
                foreach(var rating in userRatings)
                {
                    Console.WriteLine("articleId: " + rating.Key + " rating: " + rating.Value);
                }
                Console.WriteLine("---------------------");
            }
        }

        private static void readFile()
        {
            using(FileStream fs = File.OpenRead(FILE_PATH))
            using(var reader = new StreamReader(fs))
            {
                while(!reader.EndOfStream)
                {
                    Dictionary<int, double> articleRating = new Dictionary<int, double>();
                    var line = reader.ReadLine();
                    // index 0 = userid, index 1 = articleid, index 2= rating
                    var values = line.Split(',');
                    
                    int userId = int.Parse(values[0]);
                    int articleId = int.Parse(values[1]);
                    double rating = double.Parse(values[2], CultureInfo.InvariantCulture); //wtf c#
                    
                    initData(userId, articleId, rating);
                }
            }
        }

        private static void initData(int userId, int articleId, double rating)
        {
            if(userpref.ContainsKey(userId))
            {
                var user = userpref[userId];
                user.addArticle(articleId, rating);
            }
            else{
                Dictionary<int, double> articleRating = new Dictionary<int, double>{ {articleId, rating} };
                var newUser = new User(userId, articleRating);
                userpref.Add(userId, newUser);
            }            
        }
    }
}
