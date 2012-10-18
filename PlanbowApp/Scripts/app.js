function UserViewModel() {
    var self = this;

    var id = '';
    var foursquareId = 'FOURSQUARE_ID';
    var twitterId = 'TWITTER_ID';
    var facebookId = 'FACEBOOK_ID';
    var firstName = ko.observable();
    var lastName = ko.observable();
    var userImage = 'https://irs2.4sqi.net/img/user/32x32/KE2IOZYCCSRNB0FA.jpg';
}

function ActivityViewModel(name) {
    var self = this;

    self.id = '';
}

function LocationViewModel() {
    var self = this;

    self.id = '';
}

function PlanViewModel(name) {
    var self = this;

    self.id = '';
    self.name = ko.observable(name);
}

function PlanbowViewModel() {
    // Data
    var self = this;
    self.views = ['Discover', 'Plans', 'Me'];
    self.selectedViewId = ko.observable();
    self.selectedViewData = ko.observable();
    self.selectedPlanData = ko.observable();

    // Behaviours    
    self.goToView = function (folder) { };
    self.goToPlan = function (mail) { };


};

ko.applyBindings(new PlanbowViewModel());