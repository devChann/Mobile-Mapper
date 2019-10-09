using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GeoJSON.Net.Feature;
using webmapper.Interface;
using webmapper.Models;
using webmapper.ViewModels;
using Newtonsoft.Json;
using GeoJSON.Net.Geometry;
using NetTopologySuite.IO;
//using GeoJSON.Net.Geometry;
using Microsoft.SqlServer.Types;
using GeoJSON.Net.Contrib.MsSqlSpatial;

namespace webmapper.Services
{
    public class dataService : IWebmapper
    {
        private readonly NARIGPContext _context;

        public dataService(NARIGPContext context)
        {
            _context = context;

        }
        //public IEnumerable<Parceltbl> getCadastralLayers()
        //{
        //    var cadastralMaps = _context.Parceltbl
        //        .FromSql("EXECUTE cadastralDataSp")
        //        .ToList();
        //    return cadastralMaps;
        //}

        public FeatureCollection getCadastralLayers()
        {
            var geojsonndata = new FeatureCollection()
            {
                CRS = new GeoJSON.Net.CoordinateReferenceSystem.NamedCRS("urn:ogc:def:crs:OGC::CRS84")
            };
            var cadastralMaps = _context.AndroidViewtbl.ToList();
            var wktParcels = new WKTWriter();
            cadastralMaps.ForEach(sa =>
            {
                var wkt = wktParcels.Write(sa.Polygon);
                SqlGeometry sqlGeometry = SqlGeometry.Parse(new System.Data.SqlTypes.SqlString(wkt));
                var geojsonobject = sqlGeometry.ToGeoJSONGeometry();
                androidViewModel androidViewModel = new androidViewModel();
              
                androidViewModel.Id= sa.Id;
                //var geojson = JsonConvert.DeserializeObject<Polygon>(geojsonobject);
                var feature = new Feature(geojsonobject, androidViewModel);

                geojsonndata.Features.Add(feature);
            });
            return geojsonndata;
        }
    }
}
