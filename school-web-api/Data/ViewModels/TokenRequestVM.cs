using System.ComponentModel.DataAnnotations;

namespace school_web_api.Data.ViewModels
{
    public class TokenRequestVM
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string  RefreToken { get; set; }
    }
}
