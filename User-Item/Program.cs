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
        static int amount_nearestNeighbours = 3;  
        static double simTreshold = 0.35;


        static void Main(string[] args)
        {
            readFile();
            //make sure all the values are filled in. -1 if the data didnt have the value
            foreach(var val in userpref){
                var user = val.Value;
                foreach(var article in uniqueArticles) {
                    if(!user.articleRating.ContainsKey(article)){
                        user.addArticle(article, 0);
                    }
                }
            }
            printUsers();

            //possible keys 1 .. 7
            var user1 = userpref[7];    
            var emptyRatings = user1.articleRating.Where(x => x.Value == 0).Select(y => y.Key).ToList();     
            var nearest_Neighbours =  nearestNeighbours(user1, emptyRatings, simTreshold, amount_nearestNeighbours);
            foreach(var item in emptyRatings)
            {
                predictedRatings(nearest_Neighbours, amount_nearestNeighbours, item);
            }
        }

        private static void predictedRatings(List<Tuple<double, User>> neighbours, int amount_nearestNeighbours, int articleId)
        {
            var sum = 0.0;
            var weightedAvg = 0.0;
            for(var i =0; i < amount_nearestNeighbours; i++)
            {
                var similarity = neighbours[i].Item1;
                var user = neighbours[i].Item2;
                if(user.articleRating[articleId] == 0)
                {
                    continue;
                }
                weightedAvg += similarity * user.articleRating[articleId];
                sum += similarity;
            }
            Console.WriteLine("Predicted Rating: {0}", weightedAvg/sum);
            
        } 
        private static List<Tuple<double, User>> nearestNeighbours(User selectedUser,  List<int> emptyRatings, double smt, int amount)
        {
            List<Tuple<double, User>> nn_Euclidian = new List<Tuple<double, User>>();
            var nn_Pearson = new List<Tuple<double, User>>();
            List<Tuple<double, User>> nn_Cosine = new List<Tuple<double, User>>();
            //loop through all the users and find the users that are most similar to selecteduser
            foreach(var user in userpref)
            {
                var user2 = userpref[user.Key];
                if(selectedUser.id != user.Key)
                {

                    var difference = 0;
                    foreach (var rating in emptyRatings)
                    {
                        if (user2.articleRating[rating] != 0) 
                        {
                            difference++;
                        }
                    }

                    if (difference <= 0) {
                        continue;
                    }
                    //TODO refactor this mess, only pearson has treshold atm
                    Console.WriteLine("Compare user: " + selectedUser.id + " with user: " + user.Key);
                    Console.WriteLine("---------------------");
                    
                    ed = new EuclidianDistance();
                    var euclidian_distance = ed.calculate(selectedUser, user2, uniqueArticles);
                    user2.euclidean_distance = euclidian_distance;
                    nn_Euclidian.Add(Tuple.Create(euclidian_distance, user2));
                    Console.WriteLine("Euclidian stuff: " +  euclidian_distance);

                    pe = new Pearsons();
                    var correlation = pe.calculate(selectedUser, user2, uniqueArticles);
                    if (correlation >= smt)
                    {
                        var neighbour = Tuple.Create(correlation, user2);
                        if (nn_Pearson.Count < amount) {
                            nn_Pearson.Add(neighbour);
                            if (nn_Pearson.Count() == amount) {
                                smt = nn_Pearson.Min(x => x.Item1);
                            }
                        }
                        else
                        {
                            nn_Pearson.Add(neighbour);
                            var lowestSimilarityUser = nn_Pearson.OrderBy(x => x.Item1).First();
                            nn_Pearson.Remove(lowestSimilarityUser);
                            smt = nn_Pearson.OrderBy(x => x.Item1).First().Item1;
                        }
                    }
                    Console.WriteLine("Pearsons corr: " + correlation);
                
                    co = new Cosine();
                    var cosine_similarity = co.calculate(selectedUser, user2, uniqueArticles);
                    user2.cosine = cosine_similarity;
                    nn_Cosine.Add(Tuple.Create(cosine_similarity, user2));

                    Console.WriteLine("Cosine similarity: " + cosine_similarity + "\n");
                } else
                {
                    continue;     
                }      
            }
            //maybe print the first 3 of this stuff
            List<Tuple<double, User>> nn_Euclidian_Sorted = nn_Euclidian.OrderByDescending(x => x.Item1).ToList();
            List<Tuple<double, User>> nn_Pearson_Sorted = nn_Pearson.OrderByDescending(x => x.Item1).ToList();
            List<Tuple<double, User>> nn_Cosine_Sorted = nn_Cosine.OrderByDescending(x => x.Item1).ToList();

            for(var i =0; i < amount_nearestNeighbours; i++)
            {
                Console.WriteLine("Nearest {0}", i + 1 + "\n----------------");                
                Console.WriteLine("Euclidian nn: "+ nn_Euclidian_Sorted[i].Item2.id);
                Console.WriteLine("Pearson nn: " + nn_Pearson_Sorted[i].Item2.id);
                Console.WriteLine("consine nn: " + nn_Cosine_Sorted[i].Item2.id + "\n");
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
