using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Mogade
{
    public class Driver : IDriver, IRequestContext
    {
        public const string VERSION = "gamma";
        public Driver(string gameKey, string secret)
        {
            ValidationHelper.AssertNotNullOrEmpty(gameKey, "gameKey");
            ValidationHelper.AssertNotNullOrEmpty(secret, "secret");
            Key = gameKey;
            Secret = secret;
        }

        public string ApiVersion
        {
            get { return VERSION; }
        }

        public string Key { get; private set; }
        public string Secret { get; private set; }


#if !ONLYASYNC

        public void SaveScore(string leaderboardId, Score score, string uniqueIdentifier, Action<Response<SavedScore>> callback)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "username", score.UserName }, { "userkey", uniqueIdentifier }, { "points", score.Points }, { "data", score.Data } };
            var communicator = new Communicator(this);
            communicator.SendPayload<SavedScore>(Communicator.Post, "scores", payload, r =>
            {
                if (r.Success) { r.Data = JsonConvert.DeserializeObject<SavedScore>(r.Raw); }
                if (callback != null) { callback(r); }
            });
        }

        public void GetLeaderboard(string leaderboardId, LeaderboardScope scope, int page, int records, Action<Response<LeaderboardScores>> callback)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "page", page }, { "records", records }, { "scope", (int)scope } };
            GetLeaderboard(payload, callback);
        }

        public void GetLeaderboardWithPlayerStats(string leaderboardId, LeaderboardScope scope, string userName, string uniqueIdentifier, int page, int records, Action<Response<LeaderboardScoresWithPlayerStats>> callback)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "with_player", "true" }, { "username", userName }, { "userkey", uniqueIdentifier }, { "page", page }, { "records", records }, { "scope", (int)scope } };
            var communicator = new Communicator(this);
            communicator.SendPayload<LeaderboardScoresWithPlayerStats>(Communicator.Get, "scores", payload, r =>
            {
                if (r.Success) { r.Data = JsonConvert.DeserializeObject<LeaderboardScoresWithPlayerStats>(r.Raw); }
                callback(r);
            });
        }

        public void GetLeaderboard(string leaderboardId, LeaderboardScope scope, string userName, string uniqueIdentifier, int records, Action<Response<LeaderboardScores>> callback)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "username", userName }, { "userkey", uniqueIdentifier }, { "records", records }, { "scope", (int)scope } };
            GetLeaderboard(payload, callback);
        }

        public void GetLeaderboard(string leaderboardId, LeaderboardScope scope, string userName, string uniqueIdentifier, Action<Response<Score>> callback)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "username", userName }, { "userkey", uniqueIdentifier }, { "records", 1 }, { "scope", (int)scope } };
            var communicator = new Communicator(this);
            communicator.SendPayload<Score>(Communicator.Get, "scores", payload, r =>
            {
                if (r.Success) { r.Data = JsonConvert.DeserializeObject<Score>(r.Raw); }
                callback(r);
            });
        }

        public void GetLeaderboardCount(string leaderboardId, LeaderboardScope scope, Action<Response<int>> callback)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "scope", (int)scope } };
            var communicator = new Communicator(this);
            communicator.SendPayload<int>(Communicator.Get, "scores/count", payload, r =>
            {
                if (r.Success) { r.Data = JsonConvert.DeserializeObject<int>(r.Raw); }
                callback(r);
            });
        }

        public void GetRivals(string leaderboardId, LeaderboardScope scope, string userName, string uniqueIdentifier, Action<Response<IList<Score>>> callback)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "username", userName }, { "userkey", uniqueIdentifier }, { "scope", (int)scope } };
            var communicator = new Communicator(this);
            communicator.SendPayload<IList<Score>>(Communicator.Get, "scores/rivals", payload, r =>
            {
                if (r.Success) { r.Data = string.IsNullOrEmpty(r.Raw) ? new List<Score>(0) : JsonConvert.DeserializeObject<IList<Score>>(r.Raw); }
                callback(r);
            });
        }

        private void GetLeaderboard(IDictionary<string, object> payload, Action<Response<LeaderboardScores>> callback)
        {
            var communicator = new Communicator(this);
            communicator.SendPayload<LeaderboardScores>(Communicator.Get, "scores", payload, r =>
            {
                if (r.Success) { r.Data = JsonConvert.DeserializeObject<LeaderboardScores>(r.Raw); }
                callback(r);
            });
        }

        public void GetRank(string leaderboardId, string userName, string uniqueIdentifier, LeaderboardScope scope, Action<Response<int>> callback)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "username", userName }, { "userkey", uniqueIdentifier }, { "scopes", (int)scope } };
            var communicator = new Communicator(this);
            communicator.SendPayload<int>(Communicator.Get, "ranks", payload, r =>
            {
                if (r.Success)
                {
                    r.Data = JsonConvert.DeserializeObject<int>(r.Raw);
                }
                callback(r);
            });
        }

        public void GetRanks(string leaderboardId, string userName, string uniqueIdentifier, Action<Response<Ranks>> callback)
        {
            var allScopes = new[] { LeaderboardScope.Daily, LeaderboardScope.Weekly, LeaderboardScope.Overall, LeaderboardScope.Yesterday };
            GetRanks(leaderboardId, userName, uniqueIdentifier, allScopes, callback);
        }

        public void GetRanks(string leaderboardId, string userName, string uniqueIdentifier, LeaderboardScope[] scopes, Action<Response<Ranks>> callback)
        {
            var realScopes = new int[scopes.Length];
            for (var i = 0; i < scopes.Length; ++i)
            {
                realScopes[i] = (int)scopes[i];
            }
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "username", userName }, { "userkey", uniqueIdentifier }, { "scopes", realScopes } };
            var communicator = new Communicator(this);
            communicator.SendPayload<Ranks>(Communicator.Get, "ranks", payload, r =>
            {
                if (r.Success) { r.Data = JsonConvert.DeserializeObject<Ranks>(r.Raw); }
                callback(r);
            });
        }

        public void GetRanks(string leaderboardId, int score, Action<Response<Ranks>> callback)
        {
            var allScopes = new[] { LeaderboardScope.Daily, LeaderboardScope.Weekly, LeaderboardScope.Overall, LeaderboardScope.Yesterday };
            GetRanks(leaderboardId, score, allScopes, callback);
        }

        public void GetRank(string leaderboardId, int score, LeaderboardScope scope, Action<Response<int>> callback)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "score", score }, { "scopes", (int)scope } };
            var communicator = new Communicator(this);
            communicator.SendPayload<int>(Communicator.Get, "ranks", payload, r =>
            {
                if (r.Success)
                {
                    r.Data = JsonConvert.DeserializeObject<int>(r.Raw);
                }
                callback(r);
            });
        }

        public void GetRanks(string leaderboardId, int score, LeaderboardScope[] scopes, Action<Response<Ranks>> callback)
        {
            var realScopes = new int[scopes.Length];
            for (var i = 0; i < scopes.Length; ++i)
            {
                realScopes[i] = (int)scopes[i];
            }
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "score", score }, { "scopes", realScopes } };
            var communicator = new Communicator(this);
            communicator.SendPayload<Ranks>(Communicator.Get, "ranks", payload, r =>
            {
                if (r.Success) { r.Data = JsonConvert.DeserializeObject<Ranks>(r.Raw); }
                callback(r);
            });
        }

        public void GetAchievements(Action<Response<ICollection<Achievement>>> callback)
        {
            var payload = new Dictionary<string, object>() { { "key", Key } };
            var communicator = new Communicator(this);
            communicator.SendPayload<ICollection<Achievement>>(Communicator.Get, "achievements", payload, r =>
            {
                if (r.Success) { r.Data = JsonConvert.DeserializeObject<ICollection<Achievement>>(r.Raw); }
                if (callback != null) { callback(r); }
            });
        }

        public void GetEarnedAchievements(string userName, string uniqueIdentifier, Action<Response<ICollection<string>>> callback)
        {
            //unlike most GET operation, this actually requies the game's key
            //though it still doesn't require signing
            var payload = new Dictionary<string, object> { { "username", userName }, { "userkey", uniqueIdentifier }, { "key", Key } };
            var communicator = new Communicator(this);
            communicator.SendPayload<ICollection<string>>(Communicator.Get, "achievements", payload, r =>
            {
                if (r.Success) { r.Data = JsonConvert.DeserializeObject<ICollection<string>>(r.Raw); }
                callback(r);
            });
        }

        public void AchievementEarned(string achievementId, string userName, string uniqueIdentifier, Action<Response<Achievement>> callback)
        {
            var payload = new Dictionary<string, object> { { "aid", achievementId }, { "username", userName }, { "userkey", uniqueIdentifier } };
            var communicator = new Communicator(this);
            communicator.SendPayload<Achievement>(Communicator.Post, "achievements", payload, r =>
            {
                if (r.Success) { r.Data = JsonConvert.DeserializeObject<Achievement>(r.Raw); }
                if (callback != null) { callback(r); }
            });
        }

        public void LogApplicationStart(string uniqueIdentifier, Action<Response> callback)
        {
            var payload = new Dictionary<string, object> { { "userkey", uniqueIdentifier } };
            var communicator = new Communicator(this);
            communicator.SendPayload<object>(Communicator.Post, "stats", payload, r =>
            {
                if (callback != null) { callback(r); }
            });
        }

        public void LogCustomStat(int index, Action<Response> callback)
        {
            var payload = new Dictionary<string, object> { { "custom", index } };
            var communicator = new Communicator(this);
            communicator.SendPayload<object>(Communicator.Post, "stats", payload, r =>
            {
                if (callback != null) { callback(r); }
            });
        }

        public void LogError(string subject, string details, Action<Response> callback)
        {
            var payload = new Dictionary<string, object> { { "subject", subject }, { "details", details } };
            var communicator = new Communicator(this);
            communicator.SendPayload<object>(Communicator.Post, "errors", payload, r =>
            {
                if (callback != null) { callback(r); }
            });
        }

        public void GetAssets(Action<Response<IList<Asset>>> callback)
        {
            var payload = new Dictionary<string, object> { { "key", Key } };
            var communicator = new Communicator(this);
            communicator.SendPayload<IList<Asset>>(Communicator.Get, "assets", payload, r =>
            {
                if (r.Success) { r.Data = JsonConvert.DeserializeObject<IList<Asset>>(r.Raw); }
                if (callback != null) { callback(r); }
            });
        }

        public void Rename(string uniqueIdentifier, string oldUserName, string newUserName, Action<Response<bool>> callback)
        {
            var payload = new Dictionary<string, object> { { "username", oldUserName }, { "userkey", uniqueIdentifier }, { "newname", newUserName } };
            var communicator = new Communicator(this);
            communicator.SendPayload<bool>(Communicator.Post, "users/rename", payload, r =>
            {
                if (r.Success) { r.Data = JsonConvert.DeserializeObject<bool>(r.Raw); }
                if (callback != null) { callback(r); }
            });
        }

#endif

        #region Async and Await




        public async Task<Response<SavedScore>> SaveScoreAsync(string leaderboardId, Score score, string uniqueIdentifier)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "username", score.UserName }, { "userkey", uniqueIdentifier }, { "points", score.Points }, { "data", score.Data } };
            var communicator = new Communicator(this);
            var r = await communicator.SendPayloadAsync<SavedScore>(Communicator.Post, "scores", payload);

            if (r.Success)
            {
                r.Data = JsonConvert.DeserializeObject<SavedScore>(r.Raw);
            }
            return r;

        }

        public Task<Response<LeaderboardScores>> GetLeaderboardAsync(string leaderboardId, LeaderboardScope scope, int page, int records)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "page", page }, { "records", records }, { "scope", (int)scope } };
            return SendPayloadAsync<LeaderboardScores>("scores", payload);
        }

        public Task<Response<LeaderboardScoresWithPlayerStats>> GetLeaderboardWithPlayerStatsAsync(string leaderboardId, LeaderboardScope scope, string userName, string uniqueIdentifier, int page, int records)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "with_player", "true" }, { "username", userName }, { "userkey", uniqueIdentifier }, { "page", page }, { "records", records }, { "scope", (int)scope } };
            return SendPayloadAsync<LeaderboardScoresWithPlayerStats>("scores", payload);
        }

        public Task<Response<LeaderboardScores>> GetLeaderboardAsync(string leaderboardId, LeaderboardScope scope, string userName, string uniqueIdentifier, int records)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "username", userName }, { "userkey", uniqueIdentifier }, { "records", records }, { "scope", (int)scope } };
            return SendPayloadAsync<LeaderboardScores>("scores", payload);
        }

        public Task<Response<Score>> GetLeaderboardAsync(string leaderboardId, LeaderboardScope scope, string userName, string uniqueIdentifier)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "username", userName }, { "userkey", uniqueIdentifier }, { "records", 1 }, { "scope", (int)scope } };
            return SendPayloadAsync<Score>("scores", payload);
        }

        public Task<Response<int>> GetLeaderboardCountAsync(string leaderboardId, LeaderboardScope scope)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "scope", (int)scope } };
            return SendPayloadAsync<int>("scores/count", payload);
        }

        public async Task<Response<IList<Score>>> GetRivalsAsync(string leaderboardId, LeaderboardScope scope, string userName, string uniqueIdentifier)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "username", userName }, { "userkey", uniqueIdentifier }, { "scope", (int)scope } };
            var r = await SendPayloadAsync<IList<Score>>("scores/rivals", payload);
            if (r.Success)
            {
                if (r.Data == null)
                {
                    r.Data = new List<Score>(0);
                }
            }
            return r;
        }



        public Task<Response<int>> GetRankAsync(string leaderboardId, string userName, string uniqueIdentifier, LeaderboardScope scope)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "username", userName }, { "userkey", uniqueIdentifier }, { "scopes", (int)scope } };
            return SendPayloadAsync<int>("ranks", payload);
        }

        public Task<Response<Ranks>> GetRanksAsync(string leaderboardId, string userName, string uniqueIdentifier)
        {
            var allScopes = new[] { LeaderboardScope.Daily, LeaderboardScope.Weekly, LeaderboardScope.Overall, LeaderboardScope.Yesterday };
            return (this).GetRanksAsync(leaderboardId, userName, uniqueIdentifier, allScopes);
        }

        public Task<Response<Ranks>> GetRanksAsync(string leaderboardId, string userName, string uniqueIdentifier, LeaderboardScope[] scopes)
        {
            var realScopes = new int[scopes.Length];
            for (var i = 0; i < scopes.Length; ++i)
            {
                realScopes[i] = (int)scopes[i];
            }
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "username", userName }, { "userkey", uniqueIdentifier }, { "scopes", realScopes } };
            return SendPayloadAsync<Ranks>("ranks", payload);
        }

        public Task<Response<Ranks>> GetRanksAsync(string leaderboardId, int score)
        {
            var allScopes = new[] { LeaderboardScope.Daily, LeaderboardScope.Weekly, LeaderboardScope.Overall, LeaderboardScope.Yesterday };
            return (this).GetRanksAsync(leaderboardId, score, allScopes);
        }

        public Task<Response<int>> GetRankAsync(string leaderboardId, int score, LeaderboardScope scope)
        {
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "score", score }, { "scopes", (int)scope } };
            return SendPayloadAsync<int>("ranks", payload);
        }

        public Task<Response<Ranks>> GetRanksAsync(string leaderboardId, int score, LeaderboardScope[] scopes)
        {
            var realScopes = new int[scopes.Length];
            for (var i = 0; i < scopes.Length; ++i)
            {
                realScopes[i] = (int)scopes[i];
            }
            var payload = new Dictionary<string, object> { { "lid", leaderboardId }, { "score", score }, { "scopes", realScopes } };
            return SendPayloadAsync<Ranks>("ranks", payload);
        }


        public Task<Response<ICollection<Achievement>>> GetAchievementsAsync()
        {
            var payload = new Dictionary<string, object>() { { "key", Key } };
            return SendPayloadAsync<ICollection<Achievement>>("achievements", payload);
        }

        public Task<Response<ICollection<string>>> GetEarnedAchievementsAsync(string userName, string uniqueIdentifier)
        {
            //unlike most GET operation, this actually requies the game's key
            //though it still doesn't require signing
            var payload = new Dictionary<string, object> { { "username", userName }, { "userkey", uniqueIdentifier }, { "key", Key } };
            return SendPayloadAsync<ICollection<string>>("achievements", payload);
        }

        public async Task<Response<Achievement>> AchievementEarnedAsync(string achievementId, string userName, string uniqueIdentifier)
        {
            var payload = new Dictionary<string, object> { { "aid", achievementId }, { "username", userName }, { "userkey", uniqueIdentifier } };
            var communicator = new Communicator(this);
            var r = await communicator.SendPayloadAsync<Achievement>(Communicator.Post, "achievements", payload);

            if (r.Success)
            {
                r.Data = JsonConvert.DeserializeObject<Achievement>(r.Raw);
            }

            return r;
        }

        public async Task<Response> LogApplicationStartAsync(string uniqueIdentifier)
        {
            var payload = new Dictionary<string, object> { { "userkey", uniqueIdentifier } };
            var communicator = new Communicator(this);
            var r = await communicator.SendPayloadAsync<object>(Communicator.Post, "stats", payload);
            return r;
        }

        public async Task<Response> LogCustomStatAsync(int index)
        {
            var payload = new Dictionary<string, object> { { "custom", index } };
            var communicator = new Communicator(this);
            var r = await communicator.SendPayloadAsync<object>(Communicator.Post, "stats", payload);
            return r;
        }

        public async Task<Response> LogErrorAsync(string subject, string details)
        {
            var payload = new Dictionary<string, object> { { "subject", subject }, { "details", details } };

            var communicator = new Communicator(this);
            var r = await communicator.SendPayloadAsync<object>(Communicator.Post, "errors", payload);

            return r;
        }

        public Task<Response<IList<Asset>>> GetAssetsAsync()
        {
            var payload = new Dictionary<string, object> { { "key", Key } };
            return SendPayloadAsync<IList<Asset>>("assets", payload);
        }

        public async Task<Response<bool>> RenameAsync(string uniqueIdentifier, string oldUserName, string newUserName)
        {
            var payload = new Dictionary<string, object> { { "username", oldUserName }, { "userkey", uniqueIdentifier }, { "newname", newUserName } };
            var communicator = new Communicator(this);
            var r = await communicator.SendPayloadAsync<bool>(Communicator.Post, "users/rename", payload);

            if (r.Success)
            {
                r.Data = JsonConvert.DeserializeObject<bool>(r.Raw);
            }

            return r;
        }


        private async Task<Response<T>> SendPayloadAsync<T>(string endpoint, IDictionary<string, object> payload)
        {
            var communicator = new Communicator(this);
            var r = await communicator.SendPayloadAsync<T>(Communicator.Get, endpoint, payload);
            if (r.Success)
            {
                r.Data = JsonConvert.DeserializeObject<T>(r.Raw);
            };

            return r;
        }




        #endregion


    }
}