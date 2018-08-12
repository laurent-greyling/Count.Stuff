using System;
using System.Collections.Generic;
using System.Text;

namespace Count.Functions.Models
{
    public class ObjectModel
    {
        public bool IsVerhuurd { get; set; }

        public bool IsVerkocht { get; set; }

        public bool IsVerkochtOfVerhuurd { get; set; }

        public int MakelaarId { get; set; }

        public int MakelaarNaam { get; set; }
    }
}
