using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace school_web_api.Data.Model
{
    public class RefrestToken
    {
        public int id { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }

        public bool IsRevoked { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateExpire { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User  { get; set; }
    }
}
