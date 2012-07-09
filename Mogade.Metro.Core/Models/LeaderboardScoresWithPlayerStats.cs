using Newtonsoft.Json;
namespace Mogade
{
   public class LeaderboardScoresWithPlayerStats
   {
      [JsonProperty("scores")]
      public LeaderboardScores Scores { get; set; }
      public int Rank { get; set; }
      public Score Player { get; set; }
   }
}