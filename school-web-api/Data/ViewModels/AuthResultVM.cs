using System;

namespace school_web_api.Data.ViewModels
{
    public class AuthResultVM
    {
        public string Token { get; set; }
        public string Refreshtoken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
