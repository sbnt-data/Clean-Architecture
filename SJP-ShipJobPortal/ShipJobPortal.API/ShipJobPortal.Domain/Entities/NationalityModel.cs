using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Entities;


    public class NationalityModel
    {
        public int NationalityId { get; set; }
        public string Nationality { get; set; }
        public string CountryCode { get; set; }
    }

