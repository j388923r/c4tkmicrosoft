(function () {
    'use strict';

    angular
        .module('app')
        .controller('homeController', homeController);

    homeController.$inject = ['$scope', '$location']; 

    function homeController($scope, $location) {
        $scope.title = 'homeController';

        $scope.loadChapter = function (book, chapter) {
            console.log(book, chapter);
            $location.url('/Book/' + book + '/Chapter/' + chapter);
        }

        activate();

        function activate() { }
    }
})();
