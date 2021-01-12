using System.Collections.Generic;

namespace Demo.Util.Models
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public Dictionary<string, List<string>> UserClaims { get; set; }
    }
}