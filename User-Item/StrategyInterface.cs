using System.Collections.Generic;

namespace UserItem
{
    interface StrategyInterface
    {
        double calculate(User user1, User user2, List<int> uniqueArticles);
    }
}