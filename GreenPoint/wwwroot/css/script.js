let map;
let currentMarkers = [];

function initializeMap(token) {
    mapboxgl.accessToken = token;

    map = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/streets-v11',
        center: [-2.9429086, 43.2633976], // Longitude, Latitude
        zoom: 12
    });

    map.on('style.load', function () {
        map.on('click', function (e) {
            var coordinates = e.lngLat;

            document.getElementById('inputLatitud').value = coordinates.lat
            document.getElementById('inputLongitud').value = coordinates.lng;

            var bounds = new mapboxgl.LngLatBounds();
        });
        map.on('move', function () {
            var bounds = map.getBounds();
            console.log(bounds);
            document.getElementById('lat0').value = bounds.getNorthWest().lat;
            document.getElementById('lng0').value = bounds.getNorthWest().lng;
            document.getElementById('lat1').value = bounds.getSouthEast().lat;
            document.getElementById('lng1').value = bounds.getSouthEast().lng;
        });
        map.on('zoom', function () {
            var bounds = map.getBounds();
            console.log(bounds);
            document.getElementById('lat0').value = bounds.getNorthWest().lat;
            document.getElementById('lng0').value = bounds.getNorthWest().lng;
            document.getElementById('lat1').value = bounds.getSouthEast().lat;
            document.getElementById('lng1').value = bounds.getSouthEast().lng;
        });
    });
}

function addMarker(lat, lng) {

    var element = document.createElement('div');
    element.className = 'marker';
    //element.addEventListener('click', () => { window.alert('diste click') });

    var marker = new mapboxgl.Marker(element)
        .setLngLat({ lng, lat })
        .addTo(map);
    currentMarkers.push(marker);
}

function removeAllMarkers() {

    if (currentMarkers !== null) {
        for (var i = currentMarkers.length - 1; i >= 0; i--) {
            currentMarkers[i].remove();
        }
    }
}
