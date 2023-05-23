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
        });
        map.on('move', function () {
            var bounds = map.getBounds();
            AddAllPoints(bounds.getNorthWest().lat, bounds.getNorthWest().lng, bounds.getSouthEast().lat, bounds.getSouthEast().lng);
        });
        map.on('zoom', function () {
            var bounds = map.getBounds();
            AddAllPoints(bounds.getNorthWest().lat, bounds.getNorthWest().lng, bounds.getSouthEast().lat, bounds.getSouthEast().lng);
        });
        map.on('load', function () {
            var bounds = map.getBounds();
            AddAllPoints(bounds.getNorthWest().lat, bounds.getNorthWest().lng, bounds.getSouthEast().lat, bounds.getSouthEast().lng);
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

function AddAllPoints(lat1, lng1, lat2, lng2) {
    const event = new CustomEvent('mapchange', {
        bubbles: true,
        detail: {
            Lat1: lat1,
            Lng1: lng1,
            Lat2: lat2,
            Lng2: lng2
        }
    });
    document.getElementById('AddAllPoints').dispatchEvent(event);
}
