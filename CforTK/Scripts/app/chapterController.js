(function () {
    'use strict';

    angular
        .module('app')
        .controller('chapterController', chapterController);

    chapterController.$inject = ['$scope', '$http', '$routeParams']; 

    function chapterController($scope, $http, $routeParams) {
        $scope.title = 'chapterController';

        $scope.bookName = $routeParams.bookId;
        $scope.chapter = $routeParams.chapter;

        $http.get("http://www.esvapi.org/v2/rest/passageQuery?key=IP&passage=" + $routeParams.bookId + '+' + $routeParams.chapter).success(function (data) {
            console.log(data);
        }).error(function (err) {
            console.log(err);
        })

        activate();

        function activate() { }
    }
})();
