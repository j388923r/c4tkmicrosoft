(function () {
    'use strict';

    angular
        .module('app')
        .factory('breadCrumbFactory', breadCrumbFactory);

    breadCrumbFactory.$inject = ['$http'];

    function breadCrumbFactory($http) {
        var crumbs = [];

        var service = {
            getData: getData,
            leaveCrumb: leaveCrumb,
            pickupCrumb: pickupCrumb
        };

        return service;

        function leaveCrumb(book, chapter) {
            console.log(book, chapter);
            crumbs.splice(0, 0, {book: book, chapter: chapter});
        }

        function pickupCrumb() {
            if (crumbs.length > 0)
                return crumbs.splice(0, 1);
            return [];
        }

        function getData() { }
    }
})();