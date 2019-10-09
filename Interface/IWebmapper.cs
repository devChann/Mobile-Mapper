using GeoJSON.Net.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webmapper.Models;

namespace webmapper.Interface
{
    public interface IWebmapper
    {

        //IEnumerable<Parceltbl> getCadastralLayers();
        FeatureCollection getCadastralLayers();

    }
}
