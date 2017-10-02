var app = angular.module('shopriteapp', ['ngMap', 'angular-loading-bar']);

app.factory('shopriteFactory', ['$http', function ($http) {
    return {
        getWeeklyCircular: function (circularUrl, pageNum) {
            return $http.get('/Home/GetWeeklyCircular?circularUrl=' + circularUrl + '&pageNum=' + pageNum);
        },
        getNearbyStores: function () {
            return $http.get('/Home/GetStores');
        },
        getEvents: function () {
            return $http.get('/Home/GetEvents');
        }
    };
}]);

app.directive('productCard', productCardDirective);
function productCardDirective() {
    return {
        restrict: 'E',
        templateUrl: '/templates/productcard'
    }
}

app.directive('storeCard', storeCardDirective);
function storeCardDirective() {
    return {
        restrict: 'E',
        templateUrl: '/templates/storecard'
    }
}

app.controller('shopriteController', shopriteController);

shopriteController.$inject = ['$scope', '$window', '$interval', 'shopriteFactory', 'NgMap'];

function shopriteController($scope, $window, $interval, shopriteFactory, NgMap) {
    console.log('shop rite controller', Date());
    $scope.stores = [];
    $scope.circular = [];
    $scope.pageNum = 1;

    var getCircular = function (circularUrl, pageNum, ndx) {
        shopriteFactory.getWeeklyCircular(circularUrl, pageNum).then(function (res) {
            console.log('got weekly circular', res);
            scrollToTop();
            $scope.validDates = res.data.validDates;
            $scope.circular = res.data.dataList;
            if ($scope.circular.length === 0) {
                if (ndx >= 0) $scope.stores[ndx].hidden = true;
            }
        }).catch(function (e) {
            console.log('error getting weekly circular', e);
        });
    };
    //getCircular();

    var getStores = function () {
        shopriteFactory.getNearbyStores().then(function (res) {
            $scope.stores = res.data.storeList;
            console.log('got nearbystores', $scope.stores);
        }).catch(function (e) {
            console.log('error getting nearby stores', e);
        });
    };

    $scope.eventClick = function (item) {
        var e = $scope.events.filter(x => item.name.trim().indexOf(x.store.trim()) >= 0);
        if (e.length === 0) {
            item.events = false;
        } else {
            $window.open(e[0].link, "_blank");
        }
    }

    $scope.areEvents = function (item) {
        //if ($scope.events.filter(x => item.name.trim().indexOf(x.store.trim()) >= 0).length > 0) {
        if ($scope.events.filter(x => item.name.trim() === x.store.trim()).length > 0) {
            item.events = true;
            return true;
        } else {
            item.events = false;
            return false;
        }
    }

    var getEvents = function () {
        shopriteFactory.getEvents().then(function (res) {
            $scope.events = res.data.eventList;
            console.log('got events', $scope.events);
        }).catch(function (e) {
            console.log('error getting events', e);
        });
    };
    getEvents();

    $scope.goGetStores = function () {
        getStores();
    }
    getStores();
    $scope.filterStores = function (fs) {
        var stores;
        if (fs) {
            // does entered value exist in properties of object
            stores = $scope.stores.filter(x => x.name.indexOf(fs) >= 0
                || x.address1.indexOf(fs) >= 0
                || x.address2.indexOf(fs) >= 0
                || ((x.phonenumber) && x.phonenumber.indexOf(fs) >= 0));
        } else {
            stores = $scope.stores;
        }
        // add event filter if turned on
        if ($scope.onlyStoresWithEvents) {
            stores = stores.filter(x => x.events === true);           
        }
        return stores;
    }

    $scope.showDisclaimer = false;
    $scope.showAllMaps = function (i, doShow) {
        var x = 0;
        $scope.showDisclaimer = doShow;
        if (doShow === false) {
            $interval.cancel(promise);
        }
        promise = $interval(function () {
            $scope.stores[x].showmap = doShow;
            x++;
            if (x == $scope.stores.length) $scope.showDisclaimer = false;
        }, i, $scope.stores.length);

    }
    
    $scope.nextPage = function (link) {
        $scope.pageNum++;
        getCircular($scope.link, $scope.pageNum);
    }

    $scope.prevPage = function () {
        $scope.pageNum--;
        getCircular($scope.link, $scope.pageNum);
    }

    $scope.goBack = function () {
        $scope.circular = [];
    }

    var scrollToTop = function () {
        $('#app').goTo();
    }

    $scope.goGetCircular = function (link, name, ndx) {
        $scope.link = link;
        $scope.thisStore = name;
        console.log('name', name);
        getCircular(link, $scope.pageNum, ndx);
        
    }
    //getStores();
}

$(function () {
    $.fn.goTo = function () {
        $('html, body').animate({
            scrollTop: ($(this).offset().top - 50) + 'px'
        }, 'fast');
        $(this).focus();
        return this; // for chaining...
    };
});