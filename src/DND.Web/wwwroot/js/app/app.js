(function () {
    'use strict';

    window.onerror = function (msg) {
        if (window.alerts) {
            window.alerts.error("There was a problem with your last action.  Please reload the page, then try again.");
        } else {
            alert("Something serious went wrong.  Please close browser, then try again.");
        }
    };

    window.app = angular.module('DND', ['ui.bootstrap', 'ngMap']);

    window.app.run(['$http', '$rootScope', '$templateCache', 'NgMap', function ($http, $rootScope, $templateCache, NgMap) {
        //var localeMarketCurrency = $cookies.getObject('localeMarketCurrency');
        //if (!localeMarketCurrency) {
        //    localeMarketCurrency = new Object();

        //    localeMarketCurrency.locale = 'en-GB';
        //    localeMarketCurrency.market = ['AU'];
        //    localeMarketCurrency.currency = 'GBP';
        //    $cookies.putObject('localeMarketCurrency', localeMarketCurrency);
        //}

        //$rootScope.locale = localeMarketCurrency.locale;
        //$rootScope.market = localeMarketCurrency.market;
        //$rootScope.currency = localeMarketCurrency.currency;

        //$http.get('/flightsearch/template/flightSearch.tmpl.cshtml', { cache: $templateCache });
        //$http.get('/flightsearch/template/flightSearchFilters.tmpl.cshtml', { cache: $templateCache });
        //$http.get('/flightsearch/template/flightSearchSort.tmpl.cshtml', { cache: $templateCache });
        //$http.get('/site/template/localeMarketCurrency.tmpl.cshtml', { cache: $templateCache });

        NgMap.getMap().then(function (map) {
            console.log('map initialized');
            $rootScope.map = map;
        }).catch(function (map) {
            console.error('map error: ', map);
        });
    }]);

    //https://gearside.com/easily-link-to-locations-and-directions-using-the-new-google-maps/

    window.app.controller('MapCtrl', ['$compile', 'NgMap', function ($compile, NgMap) {
        var vm = this;
        vm.openGoogleMap = function (evt, placeId) {
            var url = 'https://www.google.com/maps/search/?api=1&query=' + this.position.lat() + ',' + this.position.lng();
            if (placeId)
            {
                url = url + '&query_place_id=' + placeId;
            }

            window.open(url, '_blank');
            //https://www.google.com/maps/search/?api=1&query=47.5951518,-122.3316393&query_place_id=ChIJKxjxuaNqkFQR3CK6O1HNNqY
            //window.open('https://maps.google.com?saddr=Current+Location&daddr=' + this.position.lat() + ',' + this.position.lng(), '_blank');
        };
        vm.openLocation = function (evt, slug) {
            window.location.href = '/locations/' + slug;
        };
    }]);

    window.app.factory('alerts', [function () {
        return window.alerts;
    }]);

    window.app.factory("$exceptionHandler", ['$log', 'alerts', function ($log, alerts) {
        return function (exception, cause) {
            alerts.error("There was a problem with your last action.  Please reload the page, then try again.");
            $log.error(exception, cause);
        };
    }]);

    window.app.service('LoadingInterceptor', ['$q', '$rootScope', '$log', function ($q, $rootScope, $log) {
        'use strict';

        var xhrCreations = 0;
        var xhrResolutions = 0;

        function isLoading() {
            return xhrResolutions < xhrCreations;
        }

        function updateStatus() {
            $rootScope.loading = isLoading();
        }

        return {
            request: function (config) {
                xhrCreations++;
                updateStatus();
                return config || $q.when(config);
            },
            requestError: function (rejection) {
                xhrResolutions++;
                updateStatus();
                $log.error('Request error:', rejection);
                return $q.reject(rejection);
            },
            response: function (response) {
                xhrResolutions++;
                updateStatus();
                return response || $q.when(response);
            },
            responseError: function (rejection) {
                xhrResolutions++;
                updateStatus();
                $log.error('Response error:', rejection);
                $rootScope.error = rejection.data.error;
                $rootScope.errors = rejection.data.errors;
                $('#error').modal('show');
                return $q.reject(rejection);
            }
        };
    }]);

    window.app.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.interceptors.push('LoadingInterceptor');
    }]);

    window.app.config(['$locationProvider', function ($locationProvider) {
        // In order to get the query string from the
        // $location object, it must be in HTML5 mode.
        //$locationProvider.html5Mode(true);
    }]);

    window.app.config(['$sceDelegateProvider', function ($sceDelegateProvider) {
        $sceDelegateProvider.resourceUrlWhitelist([
            // Allow same origin resource loads.
            'self',
            // Allow loading from our assets domain.  Notice the difference between * and **.
            'https://www.google.com/**'
        ]);
    }]);

    window.app.config(['$uibModalProvider', function ($uibModalProvider) {
        // Configure existing providers
        $uibModalProvider.options.windowClass = 'modal-fullscreen';
        $uibModalProvider.options.backdropClass = 'modal-backdrop-fullscreen';
        $uibModalProvider.options.backdrop = 'static';
    }]);

})();