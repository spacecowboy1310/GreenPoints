function initializeMap(token) {
    mapboxgl.accessToken = token;

    const map = new mapboxgl.Map({
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

