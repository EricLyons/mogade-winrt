using System;
using System.Collections.Generic;
using Windows.Storage;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
//using Microsoft.Phone.Info;

namespace Mogade.WindowsMetro
{
   public class Storage : IStorage
   {
      private const string _mogadeDataFile = "mogade.dat";
      private const string _userNamesDataFile = "usernames.dat";
      private static StorageFolder _preferredFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;

      private Configuration _configuration;
      private List<string> _userNames;

      public Storage(StorageFolder storageFolder)
      {
          _preferredFolder = storageFolder;
          Initialise();
      }

      public async void Initialise()
       {
            StorageFile file  = await _preferredFolder.CreateFileAsync(_userNamesDataFile, CreationCollisionOption.OpenIfExists);

            _userNames = await Read<List<string>>(_userNamesDataFile, _preferredFolder);

            if (_userNames == null)
                _userNames = new List<string>();

            _configuration = await Read<Configuration>(_mogadeDataFile, _preferredFolder);
            
           if (_configuration == null)
            {
                _configuration = new Configuration { UniqueIdentifier = Guid.NewGuid().ToString() };
                WriteToFile(_configuration, _mogadeDataFile, _preferredFolder);
            }
        }

      public string GetUniqueIdentifier()
      {
         //if (MogadeConfiguration.Data.UniqueIdStrategy == UniqueIdStrategy.DeviceId)
         //{
         //   object raw;  
         //   if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out raw) && raw != null)
         //   {
         //      var bytes = (byte[]) raw;
         //      var sb = new StringBuilder(bytes.Length * 2);
         //      for(var i = 0; i < bytes.Length; ++i)
         //      {
         //         sb.Append(bytes[i].ToString("X2"));
         //      }
         //      return sb.ToString();
         //   }
         //}
         //else if (MogadeConfiguration.Data.UniqueIdStrategy == UniqueIdStrategy.LegacyUserId)
         //{
         //   object anid;
         //   if (UserExtendedProperties.TryGetValue("ANID", out anid) && anid != null)
         //   {
         //      return anid.ToString();
         //   }
         //}
         //else if (MogadeConfiguration.Data.UniqueIdStrategy == UniqueIdStrategy.UserId)
         //{
         //   object anid;
         //   if (UserExtendedProperties.TryGetValue("ANID", out anid) && anid != null)
         //   {
         //      return anid.ToString().Substring(2, 32);
         //   } 
         //}
          if(_configuration == null)
          {
              _configuration = new Configuration { UniqueIdentifier = Guid.NewGuid().ToString() };
              WriteToFile(_configuration, _mogadeDataFile, _preferredFolder);
          }

         return _configuration.UniqueIdentifier;
      }

      public ICollection<string> GetUserNames()
      {
         return _userNames;
      }

      public void SaveUserName(string userName)
      {
         if (string.IsNullOrEmpty(userName)) { return; }
         if ( _userNames.Contains(userName)) { return; }
         _userNames.Add(userName);
         WriteToFile(_userNames, _userNamesDataFile, _preferredFolder);
      }

      public void RemoveUserName(string userName)
      {
         if (string.IsNullOrEmpty(userName) || !_userNames.Remove(userName)) { return; }
         WriteToFile(_userNames, _userNamesDataFile, _preferredFolder);
      }

      private static async Task<T> Read<T>(string dataFile, StorageFolder storageFolder)
      {
          try
          {
              //StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

              StorageFile storageFile = await storageFolder.CreateFileAsync(dataFile, CreationCollisionOption.OpenIfExists);

              using(Stream fs = await storageFile.OpenStreamForReadAsync())
              {
                  if (fs.Length > 0)
                  {
                      using (StreamReader sr = new StreamReader(fs))
                      {
                          return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                      }
                  }
                  else
                      return default(T);
              }
          }
          catch 
          {
              return default(T);
          }
      }
      
      private static async void WriteToFile(object objectToWrite, string dataFile, StorageFolder storageFolder)
      {
          try
          {
              using (Stream fs = await storageFolder.OpenStreamForWriteAsync(dataFile, CreationCollisionOption.ReplaceExisting))
              {
                  using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                  {
                      sw.Write(JsonConvert.SerializeObject(objectToWrite));
                  }
              }
          }
          catch { }
      }
   }
}