function initializeMap(token) {
    mapboxgl.accessToken = token;

    var map = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/streets-v11',
        center: [-2.9429086, 43.2633976], // Longitude, Latitude
        zoom: 12
    });
}