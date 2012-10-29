var map = mapbox.map('map');
map.addLayer(mapbox.layer().id('lifeofmle.map-82k4zxio'));
map.ui.zoomer.add();
map.ui.zoombox.add();
// center the map on London
map.centerzoom({
    lat: 51.510,
    lon: -0.126
}, 13);

// Create an empty markers layer
var markerLayer = mapbox.markers.layer();

mapbox.markers.interaction(markerLayer);
map.addLayer(markerLayer);

function HotDinnersViewModel() {
    var self = this;
    self.venues = ko.observableArray();
    self.selectedVenue = ko.observable();    
    self.venueDetails = ko.observable();
    self.venueTips = ko.observable();
    self.venuePhotos = ko.observable();

    self.selectedVenue.subscribe(function (newVenue) {
        // TODO: Make map move into location

        // Look up Foursquare information
        if (newVenue.foursquareData.id != "") {
            var foursquareVenueUrl = "/api/foursquare/" + newVenue.foursquareData.id;

            $.ajax({
                url: foursquareVenueUrl,
                success: function (data) {
                    var foursquareData = $.parseJSON(data);
                    self.venueDetails(foursquareData.response.venue);

                    var tipArray = foursquareData['response']['venue']['tips']['groups'][0]['items'];
                    self.venueTips(tipArray);

                    var photoArray = foursquareData['response']['venue']['photos']['groups'][1]['items'];
                    self.venuePhotos(photoArray);

                    $('.carousel').carousel('next');
                }
            });
        }
        else {
            self.venueDetails('');
            self.venueTips('');
            self.venuePhotos('');
        }

    }.bind(self));
}

var hotDinnersVM = new HotDinnersViewModel();
ko.applyBindings(hotDinnersVM);

$(document).ready(function () {

    $('.carousel').carousel({
        interval: 5000
    });

    $.ajax({
        url: '/api/hotdinners',
        success: function (data) {
            $.each(data, function (i, row) {

                var lat = row.latitude;
                var lng = row.longitude;

                markerLayer.add_feature({
                    geometry: {
                        coordinates: [lat, lng]
                    },
                    properties: {
                        'marker-color': '#008baa',
                        'marker-symbol': 'restaurant',
                        title: row.name,
                        description: row.hotDinnerData.shortDescription
                    }
                });

                hotDinnersVM.venues.push(row);
            });

            hotDinnersVM.selectedVenue(hotDinnersVM.venues[0]);
        }
    });
});