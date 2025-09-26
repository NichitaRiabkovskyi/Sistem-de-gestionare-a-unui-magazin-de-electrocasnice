using MyApi.Models;
using System.Collections.Generic;

namespace MyApi.Data
{
    public static class InMemoryData
    {
        public static List<Product> Products { get; } = new List<Product>
        {
            new Product{ Id=1, Name="Telefon X", Price=699.99m, Description="Telefon performant" },
            new Product{ Id=2, Name="Laptop A", Price=999.00m, Description="Laptop pentru munca" },
            new Product{ Id=3, Name="Casti Pro", Price=149.50m, Description="Casti wireless" },
            new Product{ Id=4, Name="Mouse Gamer", Price=49.99m, Description="Mouse cu DPI reglabil" },
            new Product{ Id=5, Name="Tastatura RGB", Price=89.99m, Description="Tastatura mecanica" },
            new Product{ Id=6, Name="Monitor 27\"", Price=279.99m, Description="Monitor 2K" },
            new Product{ Id=7, Name="SSD 1TB", Price=119.99m, Description="SSD NVMe" },
            new Product{ Id=8, Name="Router AX", Price=159.99m, Description="Router Wi-Fi 6" },
            new Product{ Id=9, Name="Camera HD", Price=79.99m, Description="Camera de supraveghere" },
            new Product{ Id=10, Name="Boxe 2.1", Price=69.99m, Description="Sistem audio" }
        };
    }
}
