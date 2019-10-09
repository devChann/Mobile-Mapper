using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GeoAPI.Geometries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using webmapper.Models;
using webmapper.ViewModels;
using GeoJSON.Net.Contrib.MsSqlSpatial;
using Microsoft.SqlServer.Types;
using GeoJSON.Net.Feature;

namespace webmapper.Controllers
{
    public class HomeController : Controller
    {
        private readonly NARIGPContext _context;
        public HomeController(NARIGPContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SavePolygon([FromBody] string shape_to_db)
        {

            IPolygon polygon = (IPolygon)new WKTReader().Read(shape_to_db);
            polygon.SRID = 3857;
            var purpose = new List<string> { "Maize", "Rice", "Cassava", "Tea" };
            var random = new Random();
            int index = random.Next(purpose.Count());
            var shapeToDb = new AndroidViewtbl()
            {
                Id = Guid.NewGuid(),//use unique
                Purpose = purpose[index],
                Polygon = polygon,
                Acreage = (polygon.Area) * 0.404686

            };

            _context.Add(shapeToDb);

            _context.SaveChanges();

            return View("Index");
        }


        [HttpGet]       
        public ActionResult<List<string>> GetPolygons()
        {
            var polygons = _context.Parceltbl.Select(a => a.Geometry).ToList();

            var wktPolygons = new List<string>();

            var wktWriter = new WKTWriter();

            foreach(var polygon in polygons)
            {
                wktPolygons.Add(wktWriter.Write(polygon));
            }

            return wktPolygons;
        }

        [HttpGet]
        public ActionResult getParcelsUnderCaltivation()
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
                SqlGeometry sqlGeometry = SqlGeometry.Parse(wkt);
                var geojsonobject = sqlGeometry.ToGeoJSONGeometry();
                androidViewModel androidViewModel = new androidViewModel();

                androidViewModel.Id = sa.Id;
                androidViewModel.Acreage = sa.Acreage * 0.404686;
                androidViewModel.Purpose = sa.Purpose;
                
                var feature = new Feature(geojsonobject, androidViewModel);

                geojsonndata.Features.Add(feature);
            });

            return Json(geojsonndata);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
