using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace UserItem
{
    class Program
    {
        private static string FILE_PATH = "C:\\Users\\Orlando\\Documents\\HRO\\INF3B\\OP4\\Data Science\\UserItem\\User-Item\\userItem.csv";
        static Dictionary<int, User> userpref = new Dictionary<int, User>();
        static List<int> uniqueArticles = new List<int>();
        static EuclidianDistance ed;
        static Pearsons pe;
        static Cosine co;
        
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
            //possible keys 1 .. 7
            var user1 = userpref[1]; //[2,6] 1,58 en 0,95
            var user2 = userpref[7];
            ed = new EuclidianDistance();
            var euclidian_distance = ed.calculate(user1, user2, uniqueArticles);
            Console.WriteLine("Euclidian stuff:" + euclidian_distance);

            pe = new Pearsons();
            var correlation = pe.calculate(user1, user2, uniqueArticles);
            Console.WriteLine("Pearsons corr:" + correlation);
            
            co = new Cosine();
            var cosine_similarity = co.calculate(user1, user2, uniqueArticles);
            Console.WriteLine("Cosine similarity:" + cosine_similarity);

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
                    
                    initUniqueArticles(articleId);
                    initData(userId, articleId, rating);
                }
            }
        }

        private static void initUniqueArticles(int articleId)
        {
            if(!uniqueArticles.Contains(articleId))
            {
                uniqueArticles.Add(articleId);
                uniqueArticles.Sort();
            }
        }

        private static void initData(int userId, int articleId, double rating)
        {
            //user was already created
            if(userpref.ContainsKey(userId))
            {
                var user = userpref[userId];
                user.addArticle(articleId, rating);
            }
            //new user needs to be created
            else{
                Dictionary<int, double> articleRating = new Dictionary<int, double>{ {articleId, rating} };
                var newUser = new User(userId, articleRating);
                userpref.Add(userId, newUser);
            }            
        }
    }
}
