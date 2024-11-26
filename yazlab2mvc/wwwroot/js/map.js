function initMap() {
    const map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 41.0082, lng: 28.9784 }, // İstanbul koordinatları
        zoom: 10
    });

    // Haritaya tıklayınca pin ekleme
    map.addListener("click", (e) => {
        new google.maps.Marker({
            position: e.latLng,
            map: map
        });
    });
}