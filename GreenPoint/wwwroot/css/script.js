let map

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
            //new mapboxgl.Popup()
            //    .setLngLat(coordinates)
            //    .setHTML('you clicked here: <br/>' + coordinates)
            //    .addTo(map);
        });
    });
}

function addMarker(lat, lng) {

    var element = document.createElement('div');
    element.className = 'marker';
    element.addEventListener('click', () => { window.alert('diste click') });

    var marker = new mapboxgl.Marker(element)
        .setLngLat({ lng, lat })
        .addTo(map);
}