(function () {
    'use strict';

    angular
        .module('app')
        .controller('homeController', homeController)
        .directive('autoComplete', function ($timeout) {
            return function (scope, iElement, iAttrs) {
                console.log(scope);
                console.log(iAttrs);
                $timeout(function () {
                    $(iElement).autocomplete({
                        open: function () {
                            $('.ui-autocomplete').width('18em');
                            $('.ui-menu-item').css('border-radius','6px');
                        },
                        source: scope[iAttrs.uiItems],
                        select: function () {
                            $timeout(function () {
                                iElement.trigger('input');
                            }, 50);
                        }
                    });
                }, 1000);
            };
        });

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
