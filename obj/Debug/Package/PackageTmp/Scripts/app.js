var app = angular.module('shopriteapp', ['ngMap']);

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

shopriteController.$inject = ['$scope', 'shopriteFactory', 'NgMap'];

function shopriteController($scope, shopriteFactory, NgMap) {
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

    $scope.goGetCircular = function (link, ndx) {
        $scope.link = link;
        console.log('link', link);
        getCircular(link, $scope.pageNum, ndx);
    }
    //getStores();
}
