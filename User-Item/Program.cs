using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace UserItem
{
    class Program
    {
        private static string FILE_PATH = "userItem.csv";
        static Dictionary<int, User> userpref = new Dictionary<int, User>();
        static List<int> uniqueArticles = new List<int>();
        static StrategyInterface ed, pe, co;


        static void Main(string[] args)
        {
            readFile();
            // printUsers();

            //possible keys 1 .. 7
            var user1 = userpref[4]; 
            var amount_nearestNeighbours = 3;          
            var nearest_Neighbours =  nearestNeighbours(user1);
            predictedRatings(nearest_Neighbours, amount_nearestNeighbours);
        }

        private static void predictedRatings(List<User> nearest_Neighbours, int amount_nearestNeighbours)
        {
            List<double> pearson_values = new List<double>();
            List<double> weight = new List<double>();
            double sum = 0;
            for(var i = 0; i< amount_nearestNeighbours; i++)
            {
                var user = nearest_Neighbours[i];
                var pearson_correlation = user.pearson_correlation;
                pearson_values.Add(pearson_correlation);
                sum += pearson_correlation;
            }

            foreach(var value in pearson_values)
            {
                var result = value/sum;
                weight.Add(result);
            }
            //list that containts the articles we want the ratings from from the other users
            List<int> articleRatings = new List<int>{101};            
            for(var i = 0; i < articleRatings.Count; i++)
            {
                var article = articleRatings[i];
                var predictedRating = 0.0;
                for(var j = 0; j < amount_nearestNeighbours; j++)
                {
                    var currentNeighbour = nearest_Neighbours[j];
                    var userRating  = currentNeighbour.articleRating[article];
                    var weighted_rating = weight[j] * userRating;
                    predictedRating += weighted_rating;
                } 
                Console.WriteLine("Predicted rating for article : " + article + " is: " + predictedRating);                         
            }
        } 
        private static List<User> nearestNeighbours(User selectedUser)
        {
            List<User> nn_Euclidian = new List<User>();
            List<User> nn_Pearson = new List<User>();
            List<User> nn_Cosine = new List<User>();
            //loop through all the users and find the users that are most similar to selecteduser
            foreach(var user in userpref)
            {
                if(selectedUser.id != user.Key)
                {
                    Console.WriteLine("Compare user: " + selectedUser.id + " with user: " + user.Key);
                    Console.WriteLine("---------------------");
                    var user2 = userpref[user.Key];

                    ed = new EuclidianDistance();
                    var euclidian_distance = ed.calculate(selectedUser, user2, uniqueArticles);
                    user2.euclidean_distance = euclidian_distance;
                    nn_Euclidian.Add(user2);
                    Console.WriteLine("Euclidian stuff: " + euclidian_distance);

                    pe = new Pearsons();
                    var correlation = pe.calculate(selectedUser, user2, uniqueArticles);
                    user2.pearson_correlation = correlation;
                    nn_Pearson.Add(user2);

                    Console.WriteLine("Pearsons corr: " + correlation);
                
                    co = new Cosine();
                    var cosine_similarity = co.calculate(selectedUser, user2, uniqueArticles);
                    user2.cosine = cosine_similarity;
                    nn_Cosine.Add(user2);

                    Console.WriteLine("Cosine similarity: " + cosine_similarity + "\n");
                } else
                {
                    continue;     
                }      
            }
            //maybe print the first 3 of this stuff
            List<User> nn_Euclidian_Sorted = nn_Euclidian.OrderBy(x => x.euclidean_distance).ToList();
            List<User> nn_Pearson_Sorted = nn_Pearson.OrderByDescending(x => x.pearson_correlation).ToList();
            List<User> nn_Cosine_Sorted = nn_Cosine.OrderByDescending(x => x.cosine).ToList();

            for(var i =0; i < nn_Euclidian_Sorted.Count; i++)
            {
                Console.WriteLine("Euclidian nn: " + nn_Euclidian_Sorted[i].id);
                Console.WriteLine("Pearson nn: " + nn_Pearson_Sorted[i].id);
                Console.WriteLine("consine nn: " + nn_Cosine_Sorted[i].id);
            }

            return nn_Pearson_Sorted;
        }
        private static void readFile()
        {
            using (FileStream fs = File.OpenRead(FILE_PATH))
            using (var reader = new StreamReader(fs))
            {
                while (!reader.EndOfStream)
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
            if (!uniqueArticles.Contains(articleId))
            {
                uniqueArticles.Add(articleId);
                uniqueArticles.Sort();
            }
        }

        private static void initData(int userId, int articleId, double rating)
        {
            //user was already created
            if (userpref.ContainsKey(userId))
            {
                var user = userpref[userId];
                user.addArticle(articleId, rating);
            }
            //new user needs to be created
            else
            {
                Dictionary<int, double> articleRating = new Dictionary<int, double> { { articleId, rating } };
                var newUser = new User(userId, articleRating);
                userpref.Add(userId, newUser);
            }
        }

        private static void printUsers()
        {
            foreach (var item in userpref)
            {
                Console.WriteLine("uid: " + item.Key);
                var user = userpref[item.Key];
                var userRatings = user.articleRating;
                foreach (var rating in userRatings)
                {
                    Console.WriteLine("articleId: " + rating.Key + " rating: " + rating.Value);
                }
                Console.WriteLine("---------------------");
            }
        }
    }
}
