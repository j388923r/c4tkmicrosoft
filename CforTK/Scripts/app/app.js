(function () {
    'use strict';

    var app = angular.module('app', [
        // Angular modules 
        'ngAnimate',
        'ngRoute',

        // Custom modules 

        // 3rd Party Modules
        'autocomplete'
    ]);

    app.directive('autoComplete', function($timeout) {
        return function(scope, iElement, iAttrs) {
            iElement.autocomplete({
                source: scope[iAttrs.uiItems],
                select: function() {
                    $timeout(function() {
                        iElement.trigger('input');
                    }, 0);
                }
            });
        };
    });

    app.config(function ($routeProvider, $locationProvider) {
        $routeProvider
         .when('/', {
             templateUrl: '/Content/AngularViews/index1.html',
             controller: 'homeController'
         })
        .when('/Book/:bookId/Chapter/:chapter', {
            templateUrl: '/Content/AngularViews/chapter.html',
            controller: 'chapterController'
        })
        .otherwise({
            templateUrl: '/Content/AngularViews/index.html',
            controller: 'homeController'
        });
    });
})();