using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.DTO
{
    public class ClienteDTO
    {
        public string Nome { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
