using System.ComponentModel.DataAnnotations;

namespace school_web_api.Data.ViewModels
{
    public class LoginVM
    {
     
        public string EmailAddress { get; set; }
        [Required]
       
        public string Passsword { get; set; }
    }
}
