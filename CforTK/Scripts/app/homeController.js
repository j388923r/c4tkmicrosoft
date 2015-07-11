(function () {
    'use strict';

    angular
        .module('app')
        .controller('homeController', homeController);

    homeController.$inject = ['$scope', '$location', '$http']; 

    function homeController($scope, $location, $http) {
        $scope.title = 'homeController';

        $http.get('/api/next/').success(function (data) {
            console.log(Object.keys(data));
            $scope.bookList = Object.keys(data);
        });

        $scope.loadChapter = function (book, chapter) {
            console.log(book, chapter);
            $location.url('/Book/' + book + '/Chapter/' + chapter);
        }

        activate();

        function activate() { }
    }
})();
