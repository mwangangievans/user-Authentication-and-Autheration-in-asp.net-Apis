using Microsoft.AspNetCore.Identity;

namespace school_web_api.Data.Model
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string  lastName { get; set; }
        public string  Custom { get; set; }
    }
}
