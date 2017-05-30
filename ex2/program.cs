using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.IO;
using System.Globalization;

namespace App 
{ 
    public class Program 
    { 
        private static string FILE_PATH = "userItem.csv";
        static SortedDictionary<int, User> userpref = new SortedDictionary<int, User>();
        static List<int> uniqueArticles = new List<int>();
        // * int = article ID 
        // * double[]
        //static SortedDictionary<int, Dictionary<int,double>> deviationMatrix = new SortedDictionary<int, Dictionary<int,double>>();
        static SortedDictionary<int, Dictionary<int,double>> deviationMatrix = new SortedDictionary<int, Dictionary<int,double>>();
        static SortedDictionary<int, Dictionary<int,double>> articleMatrix = new SortedDictionary<int, Dictionary<int,double>>();


        public static void Main(string[] args) 
        { 
            readFile();
            refillMatrix();
            //printUsers();
            var user1 = userpref[7]; // set user 7 as base
            var user2 = userpref[5]; // compare him with 5 and calculate the prediction for the base user based on this user
            User[] userDir = {user1,user2};
            initArticleMatrix();        // inverse userpref matrix thing, basicly [101: rating.. rating.. rating..] ,[102: rating.. rating.. rating..]

            initDeviationMatrix();      // initialises an empty deviation matrix
            setupDeviationMatrix();     // calculate the deviation based on user ratings
            printDeviationMatrix();     // prints the deviation matrix
            //printArticleMatrix();       // prints the article matrix
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
                user.setArticleRating(articleId, rating);
            }
            //new user needs to be created
            else{
                SortedDictionary<int, double> articleRating = new SortedDictionary<int, double>{ {articleId, rating} };
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
        /*
            Fills the matrix with 0.0 values where they are empty so we have points to recalculate and replace
         */
        private static void refillMatrix(){
            foreach(var user in userpref){
                SortedDictionary<int, double> userRatings = user.Value.articleRating;
                for(var i = 0; i < uniqueArticles.Count;i++){
                    if(!userRatings.ContainsKey(uniqueArticles[i])){
                        user.Value.addArticleRating(uniqueArticles[i],0.0);
                    }
                }
            }
        }
        
        private static void initDeviationMatrix(){
            for(var i = 0; i < uniqueArticles.Count;i++){
                Dictionary<int,double> tempDic = new Dictionary<int,double>();
                for(var j = 0; j < uniqueArticles.Count;j++){
                    tempDic.Add(j,0);                
                }
                deviationMatrix.Add(uniqueArticles[i],tempDic);
            };
        }
        private static void initArticleMatrix(){
            for(var i = 0; i< uniqueArticles.Count;i++){
                Dictionary<int,double> tempDic = new Dictionary<int,double>();
                foreach(var user in userpref){
                    foreach(var subItem in user.Value.articleRating){
                        if(subItem.Key == uniqueArticles[i]){
                            tempDic.Add(user.Key,subItem.Value);
                        }
                    }
                }
                articleMatrix.Add(uniqueArticles[i],tempDic);
            }
        }
        private static void setupDeviationMatrix(){
            Dictionary<int,double> tempDeviationHolder = new Dictionary<int,double>();
            for(var i = 0; i< articleMatrix.Count;i++){
                int articleListCounter = 0;
                Dictionary<int,double> articleList1 = articleMatrix.ElementAt(i).Value;
                articleListCounter = articleMatrix.ElementAt(i).Key;
                //Console.WriteLine("article 1: "+articleMatrix.ElementAt(i).Key);
                Dictionary<int,double> articleList2;
                if(i == articleMatrix.Count-1){
                    articleList2 = articleMatrix.ElementAt(0).Value;
                    //Console.WriteLine("article 2: "+articleMatrix.ElementAt(0).Key);
                }else{
                    articleList2 = articleMatrix.ElementAt(i+1).Value;
                    //Console.WriteLine("article 2: "+articleMatrix.ElementAt(i+1).Key);
                }
                double tempRating = 0;
                double userCount = 0;
                for(var j = 0; j< articleList1.Count;j++){
                    if(articleList1.ElementAt(j).Value != 0 && articleList2.ElementAt(j).Value != 0){
                        tempRating += articleList1.ElementAt(j).Value - articleList2.ElementAt(j).Value;
                        userCount++;
                    }
                }
                tempRating  = tempRating/userCount;
                //Console.WriteLine("rating: "+tempRating);  
                //Console.WriteLine("id: "+i);
                tempDeviationHolder.Add(articleListCounter,tempRating);
                deviationMatrix[articleMatrix.ElementAt(i).Key][i] = 0.0;

            }
            deviationMatrix[tempDeviationHolder.ElementAt(0).Key][1] = tempDeviationHolder.ElementAt(0).Value;
            deviationMatrix[tempDeviationHolder.ElementAt(1).Key][0] = tempDeviationHolder.ElementAt(0).Value;
            // uhhhhhhhhhhhhhhhhhhhhhhhhhgggg hoe the fuuuuuuuuuuuuuuuuuuuuuuuck ga ik dit loopen
            


        }
        private static void printDeviationMatrix(){
            Console.WriteLine("==Deviation Matrix==");
            Console.WriteLine("               A101   A102   A103   A104   A105   A106");
            foreach(var item in deviationMatrix){
                Console.Write("Article "+item.Key+":");
                foreach(var subItem in item.Value){
                    Console.Write("    ");
                    string temp = string.Format("{0:F1}",subItem.Value);
                    Console.Write(temp);

                }
                Console.WriteLine("");
            }
        }
        private static void printArticleMatrix(){
            Console.WriteLine("==Article Matrix==");
            Console.WriteLine("              U1    U2   U3   U4   U5   U6   U7");
            foreach(var item in articleMatrix){
                Console.Write("Article "+item.Key+":");
                foreach(var subItem in item.Value){
                    Console.Write("  ");
                    string temp = string.Format("{0:F1}",subItem.Value);
                    Console.Write(temp);
                }
                Console.WriteLine("");
            }
        }
   }
}