(function () {
    'use strict';

    angular
        .module('app')
        .controller('chapterController', chapterController);

    chapterController.$inject = ['$scope', '$http', '$routeParams', '$location', '$cookies', 'breadCrumbFactory']; 

    function chapterController($scope, $http, $routeParams, $location, $cookies, breadCrumbFactory) {
        $scope.title = 'chapterController';
        $scope.showInput = false;

        $scope.bookName = $routeParams.bookId;
        $scope.chapter = $routeParams.chapter;

        if (!$cookies.readHistory) {
            var array = new Array(1189);
            $cookies.readHistory = array.join("0");
            console.log("creatingReadHistory");
        }
        //console.log($cookies.readHistory);

        $http.get("/api/next/" + $routeParams.bookId + "q" + $routeParams.chapter).success(function (data) {
            //console.log(data);
            $('#passageText').html(data);
        }).error(function (err) {
            console.log(err);
        });

        $http.get('/api/next/').success(function (data) {
            $scope.biblebooknumbers = data;
            //console.log(data);
            for (var key in data) {
                if (insensitiveEquals(key, $scope.bookName)) {
                    $scope.currentIndex = Number(data[key]) + Number($scope.chapter);
                    console.log($scope.currentIndex);
                    $cookies.readHistory = $cookies.readHistory.substring(0, $scope.currentIndex) + '1' + $cookies.readHistory.substring($scope.currentIndex+1);
                    //console.log($cookies.readHistory);
                }
            }
        }).error(function (err) {
            $scope.biblebooknumbers = false;
        });

        $scope.previous = function () {
            var crumb = breadCrumbFactory.pickupCrumb()[0];
            console.log(crumb);
            $location.url('/Book/' + crumb['book'] + '/Chapter/' + crumb['chapter']);
        }

        $scope.next = function () {
            var nextIndex = Math.round(Math.random()*1189);
            $http.put('/api/next/' + $scope.currentIndex, { value: $cookies.readHistory }).success(function (data) {
                console.log("data", data);
                if (data > 0) {
                    nextIndex = data;
                    if ($scope.currentIndex <= nextIndex)
                        nextIndex++;
                }
                if(nextIndex === $scope.currentIndex)
                    nextIndex = Math.round(Math.random() * 1189) + 1;
            }).then(function () {
                $scope.bestmatch = ["Genesis", 0];
                for (var key in $scope.biblebooknumbers) {
                    if ($scope.biblebooknumbers[key] < nextIndex && $scope.biblebooknumbers[key] > $scope.bestmatch[1]) {
                        $scope.bestmatch = [key, $scope.biblebooknumbers[key]];
                    }
                }
                breadCrumbFactory.leaveCrumb($scope.bookName, $scope.chapter);
                $location.url('/Book/' + $scope.bestmatch[0] + '/Chapter/' + (nextIndex - $scope.bestmatch[1]));
            })
        }

        $scope.slideUp = function () {
            $('#newjourney').slideUp('slow');
            $('#search').slideDown('slow');
        }

        $scope.loadChapter = function (book, chapter) {
            console.log(book, chapter);
            $location.url('/Book/' + book + '/Chapter/' + chapter);
        }

        activate();

        function activate() { }
    }

    var insensitiveEquals = function (first, second) {
        return first.toLowerCase() === second.toLowerCase();
    }
})();
