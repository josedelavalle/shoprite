var app = angular.module('shopriteapp', ['ngMap', 'angular-loading-bar']);

app.factory('shopriteFactory', ['$http', function ($http) {
    return {
        getWeeklyCircular: function (circularUrl, pageNum) {
            return $http.get('/Home/GetWeeklyCircular?circularUrl=' + circularUrl + '&pageNum=' + pageNum);
        },
        getNearbyStores: function () {
            return $http.get('/Home/GetNearbyStores');
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

shopriteController.$inject = ['$scope', '$window', 'shopriteFactory', 'NgMap'];

function shopriteController($scope, $window, shopriteFactory, NgMap) {
    console.log('shop rite controller', Date());
    $scope.stores = [];
    $scope.circular = [];
    $scope.pageNum = 1;

    var getCircular = function (circularUrl, pageNum, ndx) {
        shopriteFactory.getWeeklyCircular(circularUrl, pageNum).then(function (res) {
            console.log('got weekly circular', res);
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

    $scope.goGetStores = function () {
        getStores();
    }

    $scope.filterStores = function (fs) {
        if (fs) {
            return $scope.stores.filter(x => x.name.indexOf(fs) >= 0 || x.address1.indexOf(fs) >= 0 || x.address2.indexOf(fs) >= 0);
        } else {
            return $scope.stores;
        }
        
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
        scrollToTop();
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