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
            document.getElementById('lat1').value = bounds.getNorthWest().lat;
            document.getElementById('lng1').value = bounds.getNorthWest().lng;
            document.getElementById('lat2').value = bounds.getSouthEast().lat;
            document.getElementById('lng2').value = bounds.getSouthEast().lng;
            DotNet.invokeMethodAsync('GreenPoints', 'AddAllPointsVisible', bounds.getNorthWest().lat, bounds.getNorthWest().lng, bounds.getSouthEast().lat), bounds.getSouthEast().lng;
        });
        map.on('zoom', function () {
            var bounds = map.getBounds();
            document.getElementById('lat1').value = bounds.getNorthWest().lat;
            document.getElementById('lng1').value = bounds.getNorthWest().lng;
            document.getElementById('lat2').value = bounds.getSouthEast().lat;
            document.getElementById('lng2').value = bounds.getSouthEast().lng;
            DotNet.invokeMethodAsync('GreenPoints', 'GreenPoints.Pages.MapPage.razor.AddAllPointsVisible', bounds.getNorthWest().lat, bounds.getNorthWest().lng, bounds.getSouthEast().lat), bounds.getSouthEast().lng;
        });
        map.on('load', function () {
            var bounds = map.getBounds();
            document.getElementById('lat1').value = bounds.getNorthWest().lat;
            document.getElementById('lng1').value = bounds.getNorthWest().lng;
            document.getElementById('lat2').value = bounds.getSouthEast().lat;
            document.getElementById('lng2').value = bounds.getSouthEast().lng;
            DotNet.invokeMethodAsync('GreenPoints', 'GreenPoints.Pages.MapPage.AddAllPointsVisible', bounds.getNorthWest().lat, bounds.getNorthWest().lng, bounds.getSouthEast().lat), bounds.getSouthEast().lng;
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
