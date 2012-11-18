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
    self.showMode = ko.observable('opened');
    self.venueName = ko.observable();
    self.venueWhen = ko.observable();
    self.venueWhere = ko.observable();
    self.venueDescription = ko.observable();
    self.venues = ko.observableArray();
    self.selectedVenue = ko.observable();
    self.venueDetails = ko.observable();
    self.venueTips = ko.observable();
    self.venuePhotos = ko.observable();
    self.venueFilter = ko.observable("");

    // Set up subscribing properties
    self.showMode.subscribe(function (mode) {

        markerLayer.filter(function (f) {
            // Returning true for all markers shows everything.
            switch (mode) {
                case 'opened':
                    return f.properties['open'] === true;
                case 'soon':
                    return f.properties['open'] === false;
                default:
                    return true;
            }

        });
    });

    self.validVenues = ko.computed(function () {
        switch (self.showMode()) {
            case 'opened':
                return self.venues().filter(function (venue) {
                    return venue.hotDinnerData.isOpen;
                });
            case 'soon':
                return self.venues().filter(function (venue) {
                    return !venue.hotDinnerData.isOpen;
                });
            default:
                return self.venues().filter(function (venue) {
                    return venue.hotDinnerData.isOpen;
                });
        }
    });

    //filter the items using the filter text
    self.filteredVenues = ko.computed(function () {
        var filter = self.venueFilter().toLowerCase();
        if (!filter) {
            return self.validVenues();
        } else {
            return ko.utils.arrayFilter(self.validVenues(), function (item) {

                return String(item.name).toLowerCase().indexOf(filter) >= 0;
            });
        }
    });    

    self.selectedVenue.subscribe(function (newVenue) {

        self.venueName(newVenue.name);

        self.venueDetails('');
        self.venueTips('');
        self.venuePhotos('');
        self.loadDescription(newVenue.id);

        if (newVenue.hotDinnerData.isOpen) {
            self.venueWhen('');
            self.venueWhere('');

            if (newVenue.foursquareData.id != "") {
                self.loadVenue(newVenue.foursquareData.id);
            }        
            
        }
        else {
            self.venueWhen(newVenue.hotDinnerData.when);
            self.venueWhere(newVenue.hotDinnerData.where);
        }

        self.showOnMap(newVenue);
    });

    // Methods
    self.setMode = function () {
        // console.log(self.showMode());
    };

    self.loadData = function () {

        self.setMode();

        $.ajax({
            url: '/api/hotdinners',
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
           
            if (!venue.hotDinnerData.isOpen){
                markerLayer.add_feature({
                    id: venue.foursquareData.id,
                    geometry: {
                        coordinates: [lat, lng]
                    },
                    properties: {
                        'marker-color': '#ffae00',
                        'marker-symbol': 'restaurant',
                        title: venue.name,
                        description: venue.hotDinnerData.shortDescription,
                        open: venue.hotDinnerData.isOpen
                    }
                });
            }
            else {
                markerLayer.add_feature({
                    id: venue.foursquareData.id,
                    geometry: {
                        coordinates: [lat, lng]
                    },
                    properties: {
                        'marker-color': '#008baa',
                        'marker-symbol': 'restaurant',
                        title: venue.name,
                        description: venue.hotDinnerData.shortDescription,
                        open: venue.hotDinnerData.isOpen
                    }
                });
            }
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

    self.loadDescription = function (venueId) {
        $.ajax({
            url: '/api/hotdinners/' + venueId,
            success: function (data) {

                if (data && data.hotDinnerData)
                    self.venueDescription(data.hotDinnerData.description);
            }
        });
    }

    self.loadVenue = function (foursquareId) {
        var foursquareVenueUrl = "/api/foursquare/" + foursquareId;

        self.loadDescription();

        $.ajax({
            url: foursquareVenueUrl,
            success: function (data) {
                var foursquareData = $.parseJSON(data);

                if (typeof foursquareData.response.venue === "undefined")
                    self.venueDetails('');
                else
                    self.venueDetails(foursquareData.response.venue);

                var tipArray = foursquareData['response']['venue']['tips']['groups'][0]['items'];

                if (typeof tipArray === "undefined")
                    self.venueTips('');
                else
                    self.venueTips(tipArray);

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
    Router({ '/:filter': viewModel.showMode }).init();  

    viewModel.loadData();  

});