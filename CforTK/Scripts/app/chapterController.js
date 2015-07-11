(function () {
    'use strict';

    angular
        .module('app')
        .controller('chapterController', chapterController);

    chapterController.$inject = ['$scope', '$http', '$routeParams', '$location']; 

    function chapterController($scope, $http, $routeParams, $location) {
        $scope.title = 'chapterController';

        $scope.bookName = $routeParams.bookId;
        $scope.chapter = $routeParams.chapter;
        
        $http.get("http://www.esvapi.org/v2/rest/passageQuery?key=IP&passage=" + $routeParams.bookId + '+' + $routeParams.chapter).success(function (data) {
            console.log(data);
        }).error(function (err) {
            console.log(err);
        });

        $http.get('/api/next/').success(function (data) {
            $scope.biblebooknumbers = data;
            console.log(data);
            for (var key in data) {
                if (key === $scope.bookName) {
                    console.log(key);
                    $scope.currentIndex = data[key] + $scope.chapter;
                }
            }
        }).error(function (err) {
            $scope.biblebooknumbers = false;
        });

        $scope.previous = function () {

        }

        $scope.next = function () {
            var nextIndex = Math.round(Math.random()*1189);
            $http.get('/api/next/' + $scope.currentIndex, { value: "101010100101010101010101010101010101" }).success(function (data) {
                console.log(data);
                var nextIndex = data;
            }).then(function () {
                $scope.bestmatch = ["Genesis", 0];
                for (var key in $scope.biblebooknumbers) {
                    if ($scope.biblebooknumbers[key] < nextIndex && $scope.biblebooknumbers[key] > $scope.bestmatch[1]) {
                        $scope.bestmatch = [key, $scope.biblebooknumbers[key]];
                    }
                }
                console.log($scope.bestmatch[0]);
                $location.url('/Book/' + $scope.bestmatch[0] + '/Chapter/' + (nextIndex - $scope.bestmatch[1]));
            })
        }

        activate();

        function activate() { }
    }
})();
