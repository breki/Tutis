import Leaflet = require("leaflet");
// @ts-ignore
import Mapbox = require("mapbox.js");
// @ts-ignore
import Geocod = require("leaflet-control-geocoder");

// declare module 'some-module-without-typings/*'

var map = Leaflet.map('map').setView([51.505, -0.09], 13);

Leaflet.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

Leaflet.marker([51.5, -0.09]).addTo(map)
    .bindPopup('A pretty CSS3 popup.<br> Easily customizable.')
    .openPopup();

const mapboxAccessToken = "pk.eyJ1IjoiYnJla2kiLCJhIjoiNzM5Y0xKUSJ9.l2_sG-EyNgU-nWc1MiS6vw";
Mapbox.mapbox.accessToken = mapboxAccessToken;
var mapboxLayer = Mapbox.mapbox.tileLayer("mapbox.outdoors");
mapboxLayer.addTo(map);

const geocoder = new Geocod.Geocoder.Nominatim({});

const geocoderLayer = Geocod.Control.geocoder({
    collapsed: false,
    geocoder,
    placeholder: "Search for places...",
});
    //.on("markgeocode",
    //    (e: any) => {
    //        //const bbox = e.geocode.bbox;
    //        //const poly = L.polygon([
    //        //    bbox.getSouthEast(),
    //        //    bbox.getNorthEast(),
    //        //    bbox.getNorthWest(),
    //        //    bbox.getSouthWest(),
    //        //]);
    //        //this.geocoderLayer.markGeocode(e);
    //        //this.map.fitBounds(poly.getBounds());
    //        //this.removeBox();
    //    });

geocoderLayer.addTo(map);

