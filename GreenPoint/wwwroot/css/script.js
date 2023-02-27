function initialize1(token) {
    mapboxgl.accessToken = token;

    var map1 = new mapboxgl.Map({
        container: 'map1',
        style: 'mapbox://styles/mapbox/streets-v11',
        center: [28.034088, -26.195246], // Longitude, Latitude
        zoom: 9
    });
}