using System.ComponentModel.DataAnnotations;

namespace Library_Item.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; } // Gerçek projelerde şifreler şifrelenerek (hash) tutulur, şimdilik düz metin yapıyoruz.
    }
}