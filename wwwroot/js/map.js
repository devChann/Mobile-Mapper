$(document).ready(function () {
    const openlayersTiles = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    });
    const Satelite = L.tileLayer('http://{s}.google.com/vt/lyrs=s&x={x}&y={y}&z={z}', {
        maxZoom: 20,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
    });
    const Hybrid = L.tileLayer('http://{s}.google.com/vt/lyrs=s,h&x={x}&y={y}&z={z}', {
        maxZoom: 20,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
    });
    const Terrain = L.tileLayer('http://{s}.google.com/vt/lyrs=p&x={x}&y={y}&z={z}', {
        maxZoom: 20,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
    });

    

    const map = L.map('map')
        .setView([-0.69896091950849093, 37.321405899334337], 18)
        .addLayer(openlayersTiles);

    let mylayer = L.layerGroup().addTo(map);
    let layerControl = {
        "Cadastral Layer": mylayer // an option to show or hide the layer you created from geojson
    };

    let baseLayers = {
        "Base Maps": openlayersTiles, Satelite, Hybrid, Terrain
    };
    L.control.layers(baseLayers, layerControl, {position:"bottomleft"}).addTo(map);
    var legend = L.control({ position: 'bottomleft' });
    legend.onAdd = function (map) {

        var div = L.DomUtil.create('div', 'info legend'),
            grades = ["Maize", "Rice", "Cassava", "Tea"],
            labels = ["Maize", "Rice", "Cassava", "Tea"];

        // loop through our density intervals and generate a label with a colored square for each interval
        for (var i = 0; i < grades.length; i++) {
            div.innerHTML +=
                '<i style="background:' + getColor(grades[i]) + '"></i> ' + labels[i]+ '<br>';
                //grades[i] + labels[i] + '<br>';
            //+
            //    grades[i] + (grades[i + 1] ? '&ndash;' + grades[i + 1] + 
        }

        return div;
    };
    legend.addTo(map);
    map.addControl(L.control.locate({
        locateOptions: {
            enableHighAccuracy: true
        },
        position: 'topleft',
        strings: {
            title: "Show my Location"
        }
    }));

    map.pm.addControls({
        position: 'topright',
        drawPolygon: true,
        editPolygon: true,
        deleteLayer: true,
        drawMarker: false,
    });

    map.on('pm.globaleditmodetoggled', function (e) {
        console.log(e);

    });
    map.pm.enableDraw('Polygon', {
        snappable: true,
        snapDistance: 20,
        pathOptions: {
            color: 'red',
            fillColor: 'orange',
            fillOpacity: 0.7,
        },
        finishOnDoubleclick: true,
    });
    map.on('pm:create', function (e) {
        var layer = e.layer;
        shape = layer.toGeoJSON();

        //var geometry = JSON.stringify(shape.geometry);

        var shape_to_db = Terraformer.WKT.convert(shape.geometry);
        SavePolygon(shape_to_db);
        //console.log(shape_to_db);
    });
    map.on('pm:cut', function (e) {
        wlayer = e.layer;
        //var layer = e.layer;
        //shape = layer.toGeoJSON();
        ////var geometry = JSON.stringify(shape.geometry);
        //var shape_to_db = Terraformer.WKT.convert(shape.geometry);
        //console.log(shape_to_db);
        ////SavePolygon(shape_to_db);
        var coords = wlayer.feature.geometry.coordinates;
        getshapeData(coords);
        //console.log(coords);

    });
    function getshapeData(c) {
        alert(JSON.stringify(c));
        //shape = wlayer.toGeoJSON();
        //var shape_to_db = Terraformer.WKT.convert(c.geometry);
        //SavePolygon(shape_to_db);

    }
    function SavePolygon(shape_to_db) {
        //console.log(shape_to_db);
        $.ajax({
            url: "/Home/SavePolygon",
            type: "POST",
            //dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(shape_to_db),
            success: function (data) {
                console.log(data);
                var purpose = prompt("Enter Purpose");
                alert("data saved succesfully");
            },
            error: function (error) {
                alert('failed');
                alert(error);
            }
        });

    }

    //this ajax call gets polygons from the database
    $.ajax({
        url: "/Home/GetPolygons",
        type: "GET",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            addPolygonsToMap(data, map);
            console.log(data);
            alert("Data loaded successfully");
        },
        error: function (error) {
            alert('failed retreiving polygons');
            alert(error.status + ": " + error.responseText);
        }
    });
    $.ajax({
        url: "/Home/getParcelsUnderCaltivation",
        type: "GET",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            addParcelsUnderCultivation(data, map);
            console.log(data);
            alert("Data loaded successfully");
        },
        error: function (error) {
            alert('failed retreiving polygons');
            alert(error.status + ": " + error.responseText);
        }
    });

    //L.control.layers(overlayMaps).addTo(map);
    //this function takes in the polygons from db and adds them to the map
    function addPolygonsToMap(polygons, map) {
       
        for (var i in polygons) {

            var geojson = Terraformer.WKT.parse(polygons[i]);

            baseLayer = L.geoJSON(geojson, {
            });
            mylayer.addLayer(baseLayer);
        }
        
        
    }
    function addParcelsUnderCultivation(geojsondata, map) {
        //for (var sa in parcels) {
        //    var geojsondata = Terraformer.WKT.parse(parcels[sa]);
        //    console.log(geojsondata);
        //    L.geoJSON(geojsondata, {

        //    }).addTo(map);
        //}
        console.log(geojsondata);
        L.geoJSON(geojsondata, {
            style: style,
            onEachFeature: onEachFeature
        }).addTo(map);
        
    }
    function getColor(d) {
        return d > 'Tea' ? '#20630e' :
                d > 'Maize' ? '#E31A1C' :
                    d > 'Rice' ? '#FC4E2A' :
                        d > 'Cassava' ? '#FD8D3C' :
                                    '#FFEDA0';
    }

    function style(feature) {
        return {
            fillColor: getColor(feature.properties.Purpose),
            weight: 2,
            opacity: 1,
            color: 'white',
            dashArray: '3',
            fillOpacity: 0.7
        };
    }
    function selectedFearture(e) {
        var layer = e.target;
        layer.setStyle({
            weight: 5,
            color: '#666',
            dashArray: '',
            fillOpacity: 0.7
        });
        //div.innerHTML +='<i style="background:' + getColor(grades[i]) + '"></i> ' + labels[i] + '<br>';
        //        //grades[i] + labels[i] + '<br>';
        layer.bindPopup("Crop: " + layer.feature.properties.Purpose + '<br>' + "Acreage:" + round(layer.feature.properties.Acreage,2)+ " Ha");
        if (!L.Browser.ie && !L.Browser.opera && !L.Browser.edge) {
            layer.bringToFront();
        }
    }
    function zoomToFeature(e) {
        map.fitBounds(e.target.getBounds());
    }
    function onEachFeature(feature, layer) {
        layer.on({
            mouseover: selectedFearture,
            click: zoomToFeature
        });
    }
    function round(value, decimals) {
        return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
    }

});

