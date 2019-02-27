(function () {
    "use strict";

    window.app.directive('googleMap', googleMap);

    function googleMap() {
        var bindings = {
            placeid: '@',
            latitude: '@',
            longitude: '@'
        }

        return {
            templateUrl: '/site/template/googleMap.tmpl.cshtml',
            controller: googleMapController,
            bindToController: bindings,
            scope: {},
            controllerAs: 'vm'
        }
    }

    googleMapController.$inject = ['$window', '$filter', '$rootScope', '$scope', '$uibModal','$sce','NgMap'];
    function googleMapController($window, $filter, $rootScope, $scope, $uibModal, $sce, NgMap) {
        var vm = this;
        vm.close = close;
        vm.openGoogleMapModal = openGoogleMapModal;

       

        var bindings = {
            placeid: '@',
            latitude: '@',
            longitude: '@'
        }

        function openGoogleMapModal() {
            var modalInstance = $uibModal.open({
                templateUrl: '/site/template/googleMapModal.tmpl.cshtml',
                scope: $scope,
                bindToController: true,
                controller: googleMapModalController,
                controllerAs: 'vm',      
                resolve: {
                    latitude: function () {
                        return vm.latitude;
                    },
                    longitude: function () {
                        return vm.longitude;
                    }
                    ,
                    placeid: function () {
                        return vm.placeid;
                    }
                    ,
                    NgMap: function () {
                        return NgMap;
                    }
                    ,
                    $sce: function () {
                        return $sce;
                    }
                }
            });

        }

        function close() {

        }
    }

    googleMapModalController.$inject = ['$sce', 'NgMap', 'latitude', 'longitude', 'placeid'];
    function googleMapModalController($sce, NgMap, latitude, longitude, placeid) {
        var vm = this;
        vm.latitude = latitude;
        vm.longitude = longitude;
        vm.placeid = placeid;

        vm.trustSrc = function (key) {

            var url = '';

            if (vm.placeid) {
                url = 'https://www.google.com/maps/embed/v1/place?q=place_id:' + vm.placeid + '&zoom=6&key=' + key;
            }
            else
            {
                url = 'https://www.google.com/maps/embed/v1/place?q=' + vm.latitude + ',' + vm.longitude + '&zoom=6&key=' + key;
            }
            return $sce.trustAsResourceUrl(url);
        }
     
       

    }



})();