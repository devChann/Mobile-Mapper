using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webmapper.ViewModels
{
    public class androidViewModel
    {
        public Guid Id { get; set; }
        public string Purpose { get; set; }
        public double Acreage { get; set; }
        public IGeometry Polygon { get; set; }
    }
}
