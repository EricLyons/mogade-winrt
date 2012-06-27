using System.Collections.Generic;

namespace Mogade.WindowsMetro
{
   public interface IStorage
   {
      string GetUniqueIdentifier();
      ICollection<string> GetUserNames();
      void SaveUserName(string userName);
      void RemoveUserName(string userName);
   }
}