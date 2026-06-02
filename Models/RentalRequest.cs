using System;

namespace Library_Item.Models
{
    public class RentalRequest
    {
        public int Id { get; set; }
        public int ItemId { get; set; } // Hangi ürün kiralanmak istendi?
        public string ItemTitle { get; set; } = ""; // Kolaylık olsun diye ürün adı
        public string CustomerPhone { get; set; } = ""; // Müşterinin telefon numarası
        public string RequesterName { get; set; } = ""; // Talep eden kişinin adı
        public int OwnerUserId { get; set; } // İlan sahibinin kullanıcı ID'si
        public DateTime RequestDate { get; set; } = DateTime.Now; // İstek tarihi
    }
}