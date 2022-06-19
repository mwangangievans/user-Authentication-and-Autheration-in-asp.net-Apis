using System.ComponentModel.DataAnnotations;

namespace school_web_api.Data.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string   FirstName  { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
      
        public string EmailAddress { get; set; }
        [Required]
        public string UserName  { get; set; }
        [Required]
        
        public string Passsword { get; set; }
    }
}
