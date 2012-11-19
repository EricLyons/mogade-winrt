using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Storage;

namespace Mogade.WindowsMetro
{
   public class MogadeClient : IMogadeClient
   {
      static MogadeClient()
      {
         DriverConfiguration.Configuration(c => c.NetworkAvailableCheck(NetworkInterface.GetIsNetworkAvailable));
      }

      private readonly IDriver _driver;
      private readonly IStorage _storage;
      
      public static IMogadeClient Initialize(string gameKey, string secret, StorageFolder storageFolder)
      {
         return new MogadeClient(gameKey, secret, storageFolder);
      }

      private MogadeClient(string gameKey, string secret, StorageFolder storageFolder)
      {
         _driver = new Driver(gameKey, secret);
         _storage = new Storage(storageFolder);
      }

      public IDriver Driver
      {
         get { return _driver; }
      }

      public string ApiVersion
      {
         get { return _driver.ApiVersion; }
      }

#if !ONLYASYNC

      public void SaveScore(string leaderboardId, Score score, Action<Response<SavedScore>> callback)
      {
         _driver.SaveScore(leaderboardId, score, GetUniqueIdentifier(), callback);
      }

      public void GetLeaderboard(string leaderboardId, LeaderboardScope scope, int page, Action<Response<LeaderboardScores>> callback)
      {
         _driver.GetLeaderboard(leaderboardId, scope, page, 10, callback);
      }

      public void GetLeaderboard(string leaderboardId, LeaderboardScope scope, int page, int records, Action<Response<LeaderboardScores>> callback)
      {
         _driver.GetLeaderboard(leaderboardId, scope, page, records, callback);
      }

      public void GetLeaderboardWithPlayerStats(string leaderboardId, LeaderboardScope scope, string userName, int page, int records, Action<Response<LeaderboardScoresWithPlayerStats>> callback)
      {
         _driver.GetLeaderboardWithPlayerStats(leaderboardId, scope, userName, GetUniqueIdentifier(), page, records, callback);
      }

      public void GetPlayerScore(string leaderboardId, LeaderboardScope scope, string userName, Action<Response<Score>> callback)
      {
         _driver.GetLeaderboard(leaderboardId, scope, userName, GetUniqueIdentifier(), callback);
      }

      public void GetLeaderboard(string leaderboardId, LeaderboardScope scope, string userName, int records, Action<Response<LeaderboardScores>> callback)
      {
         _driver.GetLeaderboard(leaderboardId, scope, userName, GetUniqueIdentifier(), records, callback);
      }

      public void GetLeaderboardCount(string leaderboardId, LeaderboardScope scope, Action<Response<int>> callback)
      {
         _driver.GetLeaderboardCount(leaderboardId, scope, callback);
      }

      public void GetRivals(string leaderboardId, LeaderboardScope scope, string userName, Action<Response<IList<Score>>> callback)
      {
         _driver.GetRivals(leaderboardId, scope, userName, GetUniqueIdentifier(), callback);
      }

      public void GetRanks(string leaderboardId, string userName, Action<Response<Ranks>> callback)
      {
         _driver.GetRanks(leaderboardId, userName, GetUniqueIdentifier(), callback);
      }

      public void GetRank(string leaderboardId, string userName, LeaderboardScope scope, Action<Response<int>> callback)
      {
         _driver.GetRank(leaderboardId, userName, GetUniqueIdentifier(), scope, callback);
      }

      public void GetRanks(string leaderboardId, string userName, LeaderboardScope[] scopes, Action<Response<Ranks>> callback)
      {
         _driver.GetRanks(leaderboardId, userName, GetUniqueIdentifier(), scopes, callback);
      }

      public void GetRanks(string leaderboardId, int score, Action<Response<Ranks>> callback)
      {
         _driver.GetRanks(leaderboardId, score, callback);
      }

      public void GetRank(string leaderboardId, int score, LeaderboardScope scope, Action<Response<int>> callback)
      {
         _driver.GetRank(leaderboardId, score, scope, callback);
      }

      public void GetRanks(string leaderboardId, int score, LeaderboardScope[] scopes, Action<Response<Ranks>> callback)
      {
         _driver.GetRanks(leaderboardId, score, scopes, callback);
      }

      public void GetAchievements(Action<Response<ICollection<Achievement>>> callback)
      {
         _driver.GetAchievements(callback);
      }

      public void GetEarnedAchievements(string userName, Action<Response<ICollection<string>>> callback)
      {
         _driver.GetEarnedAchievements(userName, GetUniqueIdentifier(), callback);
      }

      public void AchievementEarned(string achievementId, string userName, Action<Response<Achievement>> callback)
      {
         _driver.AchievementEarned(achievementId, userName, GetUniqueIdentifier(), callback);
      }

      public void LogApplicationStart()
      {
         _driver.LogApplicationStart(GetUniqueIdentifier(), null);
      }

      public void LogCustomStat(int index)
      {
         _driver.LogCustomStat(index, null);
      }

      public void LogError(string subject, string details)
      {
         _driver.LogError(subject, details, null);
      }

      public void GetAssets(Action<Response<IList<Asset>>> callback)
      {
         _driver.GetAssets(callback);
      }

      public void Rename(string currentUserName, string newUserName, Action<Response<bool>> callback)
      {
         _driver.Rename(GetUniqueIdentifier(), currentUserName, newUserName, callback);
      }

#endif

      public string GetUniqueIdentifier()
      {
         return _storage.GetUniqueIdentifier();
      }

      public ICollection<string> GetUserNames()
      {
         return _storage.GetUserNames();
      }

      public void SaveUserName(string userName)
      {
         _storage.SaveUserName(userName);
      }

      public void RemoveUserName(string userName)
      {
         _storage.RemoveUserName(userName);
      }


      #region Async and Await



      public Task<Response<SavedScore>> SaveScoreAsync(string leaderboardId, Score score)
      {
          return _driver.SaveScoreAsync(leaderboardId, score, GetUniqueIdentifier());
      }

      public Task<Response<LeaderboardScores>> GetLeaderboardAsync(string leaderboardId, LeaderboardScope scope, int page)
      {
          return _driver.GetLeaderboardAsync(leaderboardId, scope, page, 10);
      }

      public Task<Response<LeaderboardScores>> GetLeaderboardAsync(string leaderboardId, LeaderboardScope scope, int page, int records)
      {
          return _driver.GetLeaderboardAsync(leaderboardId, scope, page, records);
      }

      public Task<Response<LeaderboardScoresWithPlayerStats>> GetLeaderboardWithPlayerStatsAsync(string leaderboardId, LeaderboardScope scope, string userName, int page, int records)
      {
          return _driver.GetLeaderboardWithPlayerStatsAsync(leaderboardId, scope, userName, GetUniqueIdentifier(), page, records);
      }

      public Task<Response<Score>> GetPlayerScoreAsync(string leaderboardId, LeaderboardScope scope, string userName)
      {
          return _driver.GetLeaderboardAsync(leaderboardId, scope, userName, GetUniqueIdentifier());
      }

      public Task<Response<LeaderboardScores>> GetLeaderboardAsync(string leaderboardId, LeaderboardScope scope, string userName, int records)
      {
          return _driver.GetLeaderboardAsync(leaderboardId, scope, userName, GetUniqueIdentifier(), records);
      }

      public Task<Response<int>> GetLeaderboardCountAsync(string leaderboardId, LeaderboardScope scope)
      {
          return _driver.GetLeaderboardCountAsync(leaderboardId, scope);
      }

      public Task<Response<IList<Score>>> GetRivalsAsync(string leaderboardId, LeaderboardScope scope, string userName)
      {
          return _driver.GetRivalsAsync(leaderboardId, scope, userName, GetUniqueIdentifier());
      }

      public Task<Response<Ranks>> GetRanksAsync(string leaderboardId, string userName)
      {
          return _driver.GetRanksAsync(leaderboardId, userName, GetUniqueIdentifier());
      }

      public Task<Response<int>> GetRankAsync(string leaderboardId, string userName, LeaderboardScope scope)
      {
          return _driver.GetRankAsync(leaderboardId, userName, GetUniqueIdentifier(), scope);
      }

      public Task<Response<Ranks>> GetRanksAsync(string leaderboardId, string userName, LeaderboardScope[] scopes)
      {
          return _driver.GetRanksAsync(leaderboardId, userName, GetUniqueIdentifier(), scopes);
      }

      public Task<Response<Ranks>> GetRanksAsync(string leaderboardId, int score)
      {
          return _driver.GetRanksAsync(leaderboardId, score);
      }

      public Task<Response<int>> GetRankAsync(string leaderboardId, int score, LeaderboardScope scope)
      {
          return _driver.GetRankAsync(leaderboardId, score, scope);
      }

      public Task<Response<Ranks>> GetRanksAsync(string leaderboardId, int score, LeaderboardScope[] scopes)
      {
          return _driver.GetRanksAsync(leaderboardId, score, scopes);
      }

      public Task<Response<ICollection<Achievement>>> GetAchievementsAsync()
      {
          return _driver.GetAchievementsAsync();
      }

      public Task<Response<ICollection<string>>> GetEarnedAchievementsAsync(string userName)
      {
          return _driver.GetEarnedAchievementsAsync(userName, GetUniqueIdentifier());
      }

      public Task<Response<Achievement>> AchievementEarnedAsync(string achievementId, string userName)
      {
          return _driver.AchievementEarnedAsync(achievementId, userName, GetUniqueIdentifier());
      }

      public Task<Response> LogApplicationStartAsync()
      {
          return _driver.LogApplicationStartAsync(GetUniqueIdentifier());
      }

      public Task<Response> LogCustomStatAsync(int index)
      {
          return _driver.LogCustomStatAsync(index);
      }

      public Task<Response> LogErrorAsync(string subject, string details)
      {
          return _driver.LogErrorAsync(subject, details);
      }

      public Task<Response<IList<Asset>>> GetAssetsAsync()
      {
          return _driver.GetAssetsAsync();
      }

      public Task<Response<bool>> RenameAsync(string currentUserName, string newUserName)
      {
          return _driver.RenameAsync(GetUniqueIdentifier(), currentUserName, newUserName);
      }


      #endregion


   }
}