using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
namespace Library_Item.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; } // Veri tabanındaki benzersiz ID'si

        [Required]
        public string Title { get; set; } // İlan adı (Örn: Keman)

        public string Description { get; set; } // Açıklama

        [Required]
      
        public decimal Price { get; set; } // Fiyat

        // Üstteki eski ImageUrl satırını sildik, yerine güvenli ve tek bir tane bıraktık:
        public string? ImageUrl { get; set; } // Resim linki (Boş bırakılabilir yaptık)

        [Required]
        public string Category { get; set; } // Kategorisi (Sanat, Elektronik vb.)
      

        [NotMapped] // Veri tabanında sütun olarak açılmasını engeller
        public IFormFile? ImageFile { get; set; }
        // İlanın kime ait olduğunu bilmemiz için bu alanı ekliyoruz
        public int UserId { get; set; }
    }
}
    
