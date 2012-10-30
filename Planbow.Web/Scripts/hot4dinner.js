// Make a new map in #map
var main = Map('map', {
    api: 'http://a.tiles.mapbox.com/v3/lifeofmle.map-82k4zxio.jsonp',
    center: {
        lat: 51.510,
        lon: -0.126,
        zoom: 13
    },
    zoomRange: [1, 17],
    features: [
        'zoomwheel',
        'zoombox',
        'zoompan',
        'zoom'
    ]
});

function HotDinnersViewModel() {
    var self = this;
    self.venueName = ko.observable();
    self.venues = ko.observableArray([]);
    self.selectedVenue = ko.observable();    
    self.venueDetails = ko.observable();
    self.venueTips = ko.observable();
    self.venuePhotos = ko.observable();

    self.selectedVenue.subscribe(function (newVenue) {
        // TODO: Make map move into location

        self.venueName(newVenue.name);

        // Look up Foursquare information
        if (newVenue.foursquareData.id != "") {
            var foursquareVenueUrl = "/api/foursquare/" + newVenue.foursquareData.id;

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

            // Move map to adjusted center
            MM_map.easey = easey().map(MM_map)
                .to(MM_map.locationCoordinate(locationOffset({
                    lat: newVenue.longitude,
                    lon: newVenue.latitude
                })).zoomTo(MM_map.getZoom())).run(500, function () {
                    $('#' + newVenue.foursquareData.id).addClass('active');
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

    populateRestaurants();
});

function populateRestaurants() {
    $.ajax({
        url: '/api/hotdinners',
        success: function (data) {

            ko.utils.arrayPushAll(hotDinnersVM.venues(), data);
            hotDinnersVM.venues.valueHasMutated();

            var points = {
                'type': 'FeatureCollection',
                'features': []
            };

            $.each(data, function (i, venue) {
                points.features.push({
                    id: venue.id,
                    geometry: {
                        coordinates: [venue.latitude, venue.longitude]
                    },
                    properties: {
                        'marker-color': '#008baa',
                        'marker-symbol': 'restaurant',
                        name: venue.name,
                        description: venue.hotDinnerData.shortDescription
                    }
                });
            });

            if (MM_map.venueLayer) {
                MM_map.venueLayer.geojson(points);
            }
            else {
                MM_map.venueLayer = mmg().factory(function (x) {
                    var d = document.createElement('div'),
                        overlay = document.createElement('div'),
                        anchor = document.createElement('div');

                    var template = _.template(
                        '<div class="location">' +
                            '<span class="name fn"><%= name %></span>' +
                        '</div>'
                    );
                    overlay.className = 'overlay';
                    overlay.innerHTML = template(x.properties);

                    anchor.className = 'anchor';
                    anchor.appendChild(overlay);

                    d.id = x.id;
                    d.className = 'mmg vcard';
                    d.appendChild(anchor);

                    return d;
                }).geojson(points);

                MM_map.addLayer(MM_map.venueLayer);
            }

            MM_map.setCenter({
                lat: MM_map.getCenter().lat,
                lon: MM_map.getCenter().lon
            });
        }
    });
}

// Calculate offset given #content
function locationOffset(location) {
    var offset = MM_map.locationPoint({
        lat: location.lat,
        lon: location.lon
    });
    offset = MM_map.pointLocation({
        x: offset.x - $('#list').width() / 2,
        y: offset.y
    });
    return offset;
}