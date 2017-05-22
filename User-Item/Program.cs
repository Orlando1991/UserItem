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
            var user1 = userpref[7]; //[2,6] 1,58 en 0,95
            var amount_nearestNeighbours = 3;
            
            //will return a list of size n with neirest neighbours eventually
            nearestNeighbours(user1, amount_nearestNeighbours);
        }
        //how to take all the similarities into consideration and have method return only one list????
        private static void nearestNeighbours(User selectedUser, int amount_nearestNeighbours)
        {
            //loop through all the users and find the users that are most similar to selecteduser
            foreach(var user in userpref)
            {
                Dictionary<int, double> nn_Euclidian = new Dictionary<int, double>();
                Dictionary<int, double> nn_Pearson = new Dictionary<int, double>();
                Dictionary<int, double> nn_Cosine = new Dictionary<int, double>();

                if(selectedUser.id != user.Key)
                {
                    Console.WriteLine("Compare user: " + selectedUser.id + " with user: " + user.Key);
                    Console.WriteLine("---------------------");
                    var user2 = userpref[user.Key];

                    ed = new EuclidianDistance();
                    var euclidian_distance = ed.calculate(selectedUser, user2, uniqueArticles);
                    nn_Euclidian = sortEuclidianNeighbours(user2.id, euclidian_distance, nn_Euclidian, amount_nearestNeighbours);
                    Console.WriteLine("Euclidian stuff: " + euclidian_distance);

                    pe = new Pearsons();
                    var correlation = pe.calculate(selectedUser, user2, uniqueArticles);
                    Console.WriteLine("Pearsons corr: " + correlation);
                
                    co = new Cosine();
                    var cosine_similarity = co.calculate(selectedUser, user2, uniqueArticles);
                    Console.WriteLine("Cosine similarity: " + cosine_similarity + "\n");
                } else
                {
                    continue;     
                }
                
            }
            
        }

        private static Dictionary<int, double> sortEuclidianNeighbours(int id, double euclidian_distance, Dictionary<int, double> nearestNeighbours, int amount_nearestNeighbours)
        {
            if(nearestNeighbours.Count < amount_nearestNeighbours)
            {
                nearestNeighbours.Add(id, euclidian_distance);
                //sort dictionary here
            } else
            {
                //the first value has the biggest distance, so only this will need to be replaced
                var first = nearestNeighbours.First();
                double firstValue = first.Value;
                if(euclidian_distance < firstValue)
                {
                    //remove firstvalue here
                    //add euclidian_distance here
                    //sort nearestneighbours
                }
            }
            return nearestNeighbours;           
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

        private static void printUsers()
        {
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
    }
}
