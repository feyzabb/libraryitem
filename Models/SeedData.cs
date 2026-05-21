using Microsoft.EntityFrameworkCore;

namespace Library_Item.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                // Eğer veri tabanında zaten ürün varsa hiçbir şey yapma
                if (context.Items.Any())
                {
                    return;
                }

                // Eğer veri tabanı boşsa, bu örnek ilanları ekle
                context.Items.AddRange(
                    // ================= SANAT KATEGORİSİ =================
                    new Item { Title = "Keman", Description = "Profesyonel kullanıma uygun kaliteli keman. Konser, eğitim ve günlük kullanım için idealdir.", Price = 350, Category = "Sanat", ImageUrl = "https://picsum.photos/id/1062/600/400" },
                    new Item { Title = "Akustik Gitar", Description = "Yumuşak basımlı, harika tınıya sahip kiralık akustik gitar ve taşıma kılıfı.", Price = 200, Category = "Sanat", ImageUrl = "https://picsum.photos/id/145/600/400" },
                    new Item { Title = "Ahşap Ressam Şövalesi", Description = "Tuval boyama standı, yüksekliği ayarlanabilir dayanıklı ahşap gövde.", Price = 120, Category = "Sanat", ImageUrl = "https://images.unsplash.com/photo-1513364776144-60967b0f800f?w=600&auto=format&fit=crop&q=80" },
                    new Item { Title = "Profesyonel Grafik Tablet", Description = "Dijital çizim, illüstrasyon ve tasarım işleri için geniş hassas yüzey tablet.", Price = 250, Category = "Sanat", ImageUrl = "https://picsum.photos/id/0/600/400" },
                    new Item { Title = "Işıklı Kaligrafi Masası", Description = "Çizim ve hat sanatı kopya işleri için altı LED aydınlatmalı ergonomik masa.", Price = 90, Category = "Sanat", ImageUrl = "https://picsum.photos/id/250/600/400" },
                    new Item { Title = "Kil Heykel Şekillendirme Seti", Description = "Seramik ve heykel hobisi için gerekli tüm profesyonel el aletleri çantası.", Price = 70, Category = "Sanat", ImageUrl = "https://images.unsplash.com/photo-1565192647048-f997ee879457?w=600&auto=format&fit=crop&q=80" },

                    // ================= ELEKTRONİK KATEGORİSİ =================
                    new Item { Title = "Matkap", Description = "Güçlü darbeli matkap, şarjlı vidalama uç seti ve taşıma çantası.", Price = 250, Category = "Elektronik", ImageUrl = "https://images.unsplash.com/photo-1504148455328-c376907d081c?w=600&auto=format&fit=crop&q=80" },
                    new Item { Title = "4K Aksiyon Kamerası", Description = "Su altı kılıfı, kafa bandı ve bisiklet aparatı ile kiralık aksiyon kamerası.", Price = 450, Category = "Elektronik", ImageUrl = "https://picsum.photos/id/250/600/400" },
                    new Item { Title = "Bluetooth Kulaklık Üstü", Description = "Aktif gürültü engelleyici (ANC) özelliği olan yüksek stüdyo kalitesinde kulaklık.", Price = 150, Category = "Elektronik", ImageUrl = "https://picsum.photos/id/48/600/400" },
                    new Item { Title = "HD Ev Projeksiyon Cihazı", Description = "Evde sinema ve maç keyfi için yüksek lümenli, HDMI girişli projeksiyon.", Price = 500, Category = "Elektronik", ImageUrl = "https://images.unsplash.com/photo-1535016120720-40c646be5580?w=600&auto=format&fit=crop&q=80" },
                    new Item { Title = "Kameralı Drone Seti", Description = "Yedek bataryalı, stabil uçuş destekli 1080p çekim özellikli kiralık drone.", Price = 600, Category = "Elektronik", ImageUrl = "https://picsum.photos/id/26/600/400" },
                    new Item { Title = "PlayStation 5 Oyun Konsolu", Description = "Çift DualSense kol ve en popüler güncel 3 oyun yüklü kiralık konsol.", Price = 400, Category = "Elektronik", ImageUrl = "https://images.unsplash.com/photo-1606144042614-b2417e99c4e3?w=600&auto=format&fit=crop&q=80" },

                    // ================= EV EŞYASI KATEGORİSİ =================
                    new Item { Title = "Dikey Şarjlı Süpürge", Description = "Yüksek emiş güçlü, her yüzeye uygun pratik ve hafif dikey şarjlı süpürge.", Price = 300, Category = "Ev", ImageUrl = "https://images.unsplash.com/photo-1558317374-067fb5f30001?w=600&auto=format&fit=crop&q=80" },
                    new Item { Title = "Buharlı Temizleyici Paspas", Description = "Kimyasal kullanmadan sadece yüksek buhar gücüyle derinlemesine temizlik sağlar.", Price = 220, Category = "Ev", ImageUrl = "https://images.unsplash.com/photo-1581578731548-c64695cc6952?w=600&auto=format&fit=crop&q=80" },
                    new Item { Title = "Kahve Makinesi", Description = "Sabahları hızlı, pratik ve lezzetli kahve hazırlayan makine.", Price = 130, Category = "Ev", ImageUrl = "https://picsum.photos/id/425/600/400" },
                    new Item { Title = "Kazanlı Buharlı Ütü", Description = "Zorlu kumaşlar ve yoğun kırışıklıklar için yüksek basınçlı profesyonel ütü.", Price = 180, Category = "Ev", ImageUrl = "https://images.unsplash.com/photo-1517242131238-608a0d7f1d5d?w=600&auto=format&fit=crop&q=80" },
                    new Item { Title = "Hava Temizleme Cihazı", Description = "Odadaki tüm toz, polen og kötü kokuları süzen, sessiz gece modlu temizleyici.", Price = 200, Category = "Ev", ImageUrl = "https://images.unsplash.com/photo-1614713570381-8d2bf41fe425?w=600&auto=format&fit=crop&q=80" },
                    new Item { Title = "Halı ve Koltuk Yıkama Makinesi", Description = "Evde koltuk ve halılarınızı profesyonelce yıkamak için yüksek vakumlu makine.", Price = 350, Category = "Ev", ImageUrl = "https://images.unsplash.com/photo-1527515637462-cff94eecc1ac?w=600&auto=format&fit=crop&q=80" },

                    // ================= BAHÇE KATEGORİSİ =================
                    new Item { Title = "Çim Biçme Makinesi", Description = "Geniş toplama hazneli ve güçlü motorlu, bahçeniz için ideal çim biçme makinesi.", Price = 700, Category = "Bahce", ImageUrl = "https://images.unsplash.com/photo-1584473457406-6240486418e9?w=600&auto=format&fit=crop&q=80" },
                    new Item { Title = "Yüksek Basınçlı Oto Yıkama Makinesi", Description = "Bahçe taşları, duvar ve araç temizliği için ideal yüksek tazyikli su pompası.", Price = 280, Category = "Bahce", ImageUrl = "https://images.unsplash.com/photo-1520340356584-f9917d1eea6f?w=600&auto=format&fit=crop&q=80" },
                    new Item { Title = "Elektrikli Çit Budama Makinesi", Description = "Bahçe peyzaj düzenlemeleri için keskin ve emniyet kilitli çit düzeltme motoru.", Price = 190, Category = "Bahce", ImageUrl = "https://images.unsplash.com/photo-1592417817098-8f3d6eb18865?w=600&auto=format&fit=crop&q=80" },
                    new Item { Title = "Motorlu Ağaç Testeresi", Description = "Odun kesme ve kalın ağaç dallarını budama için güçlü, emniyetli benzinli testere.", Price = 320, Category = "Bahce", ImageUrl = "https://picsum.photos/id/111/600/400" },
                    new Item { Title = "Otomatik Bahçe Hortum Seti", Description = "Uzun metrajlı, farklı fıskiye modlarına sahip makaralı kolay toplama sistemi.", Price = 80, Category = "Bahce", ImageUrl = "https://images.unsplash.com/photo-1585320806297-9794b3e4eeae?w=600&auto=format&fit=crop&q=80" },
                    new Item { Title = "Büyük Boy Barbekü Izgara", Description = "Tekerlekli, ısı göstergeli kapaklı bahçe tipi mangal ve komple barbekü seti.", Price = 150, Category = "Bahce", ImageUrl = "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=600&auto=format&fit=crop&q=80" }
                );
                context.SaveChanges();
            }
        }
    }
}