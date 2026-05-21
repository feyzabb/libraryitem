using System;

namespace Library_Item.Models
{
    public class RentalRequest
    {
        public int Id { get; set; }
        public int ItemId { get; set; } // Hangi ürün kiralanmak istendi?
        public string ItemTitle { get; set; } // Kolaylık olsun diye ürün adı
        public string CustomerPhone { get; set; } // Müşterinin telefon numarası
        public DateTime RequestDate { get; set; } = DateTime.Now; // İstek tarihi
    }
}