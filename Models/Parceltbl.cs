using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using GeoAPI.Geometries;

namespace webmapper.Models
{
    public class Parceltbl
    {
        public int Id { get; set; }
        //public string wkt { get; set; }
        public string jsonData { get; set; }
     
        public IGeometry Geometry { get; set; }
       
    }
}
