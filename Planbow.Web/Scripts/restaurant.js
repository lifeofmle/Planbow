﻿var map = mapbox.map('map');
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

markerLayer.factory(function (m) {

    // Create a marker using the simplestyle factory
    var elem = mapbox.markers.simplestyle_factory(m);

    // Add function that centers marker on click
    MM.addEvent(elem, 'click', function (e) {
        map.ease.location({
            lat: m.geometry.coordinates[1],
            lon: m.geometry.coordinates[0]
        }).zoom(map.zoom()).optimal();

        viewModel.setVenue(m.id);
    });

    return elem;
});

mapbox.markers.interaction(markerLayer);
map.addLayer(markerLayer);

function HotDinnersViewModel() {
    var self = this;

    // Properties
    self.venueName = ko.observable();
    self.venues = ko.observableArray();
    self.selectedVenue = ko.observable();
    self.venueDetails = ko.observable();
    self.venueTips = ko.observable();
    self.venuePhotos = ko.observable();
    self.venueFilter = ko.observable("");

    // Set up subscribing properties

    //filter the items using the filter text
    self.filteredVenues = ko.computed(function () {
        var filter = self.venueFilter().toLowerCase();
        if (!filter) {
            return self.venues();
        } else {
            return ko.utils.arrayFilter(self.venues(), function (item) {
                return String(item.name).toLowerCase().indexOf(filter) >= 0;
            });
        }
    });

    self.selectedVenue.subscribe(function (newVenue) {

        self.venueName(newVenue.name);

        self.venueDetails('');
        self.venueTips('');
        self.venuePhotos('');

        if (newVenue.foursquareData.id != "") {
            self.loadVenue(newVenue.foursquareData.id);
        }

        self.showOnMap(newVenue);
    });

    // Methods
    self.loadData = function () {

        $.ajax({
            url: '/api/restaurants',
            success: function (data) {

                ko.utils.arrayPushAll(viewModel.venues(), data);
                viewModel.venues.valueHasMutated();

                self.mapVenues(data);

                if (self.filteredVenues[0] && self.filteredVenues[0].foursquareData)
                    self.setVenue(self.filteredVenues[0].foursquareData.id);
            }
        });
    };

    self.mapVenues = function (venues) {
        $.each(venues, function (i, venue) {

            var lat = venue.latitude;
            var lng = venue.longitude;

            markerLayer.add_feature({
                id: venue.foursquareData.id,
                geometry: {
                    coordinates: [lat, lng]
                },
                properties: {
                    'marker-color': '#008baa',
                    'marker-symbol': 'restaurant',
                    title: venue.name
                }
            });

        });
    };

    self.showOnMap = function (venue) {

        // Move map to adjusted center
        map.ease.location({ lat: venue.longitude, lon: venue.latitude }).zoom(map.zoom()).optimal();

        var markers = markerLayer.markers();

        var match = ko.utils.arrayFirst(markers, function (item) {
            return item.data.id === venue.foursquareData.id;
        });

        if (match)
            match.showTooltip();
    };

    self.setVenue = function (venueId) {
        // Get the venue from the event data
        var match = ko.utils.arrayFirst(self.venues(), function (item) {
            return item.foursquareData.id === venueId;
        });

        if (match)
            self.selectedVenue(match);
    };

    self.loadVenue = function (foursquareId) {
        var foursquareVenueUrl = "/api/foursquare/" + foursquareId;

        $.ajax({
            url: foursquareVenueUrl,
            success: function (data) {
                var foursquareData = $.parseJSON(data);

                if (typeof foursquareData.response.venue === "undefined")
                    self.venueDetails('');
                else
                    self.venueDetails(foursquareData.response.venue);

                var tipArray = foursquareData['response']['venue']['tips']['groups']; // [0]['items'];

                $.each(tipArray, function (i, tipGroup) {
                    if (tipGroup.name == 'Tips from others') {
                        if (typeof tipGroup.items === "undefined")
                            self.venueTips('');
                        else {
                            self.venueTips(tipGroup.items);
                        }                    
                    }
                });

                                    

                var photoArray = foursquareData['response']['venue']['photos']['groups'];

                if (typeof photoArray === "undefined")
                    self.venuePhotos('');
                else {
                    if (photoArray.length == 1) {
                        self.venuePhotos(photoArray[0]['items']);
                    }
                    else {
                        self.venuePhotos(photoArray[1]['items']);
                    }
                }

                $('.carousel').carousel('next');
            }
        });

    };
}

var viewModel = new HotDinnersViewModel();

ko.applyBindings(viewModel);

$(document).ready(function () {

    // set up filter routing
    //Router({ '/:filter': viewModel.showMode }).init();

    viewModel.loadData();

});