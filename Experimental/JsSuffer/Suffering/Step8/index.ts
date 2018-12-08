import L = require("leaflet");
// @ts-ignore
import Mapbox = require("mapbox.js");

var map = L.map('map').setView([51.505, -0.09], 13);

L.marker([51.5, -0.09]).addTo(map)
    .bindPopup('A pretty CSS3 popup.<br> Easily customizable.')
    .openPopup();

const mapboxAccessToken = "pk.eyJ1IjoiYnJla2kiLCJhIjoiNzM5Y0xKUSJ9.l2_sG-EyNgU-nWc1MiS6vw";
Mapbox.mapbox.accessToken = mapboxAccessToken;
var mapboxLayer = Mapbox.mapbox.tileLayer("mapbox.outdoors");
mapboxLayer.addTo(map);
