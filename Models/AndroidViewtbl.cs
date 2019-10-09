using System;
using System.Collections.Generic;
using GeoAPI.Geometries;

namespace webmapper.Models
{
    public partial class AndroidViewtbl
    {
        public Guid Id { get; set; }
        public string Purpose { get; set; }
        public double Acreage { get; set; }
        public IGeometry Polygon { get; set; }
    }
}
