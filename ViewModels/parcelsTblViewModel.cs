using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webmapper.ViewModels
{
    public class parcelsTblViewModel
    {
        public int id { get; set; }
        //public string wkt { get; set; }
        public string jsonData { get; set; }
        public IGeometry Geometry { get; set; }

    }
}
