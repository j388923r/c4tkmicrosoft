﻿(function () {
    'use strict';

    var app = angular.module('app', [
        // Angular modules 
        'ngAnimate',
        'ngRoute',
        'ngCookies',

        // Custom modules 
        

        // 3rd Party Modules
        'autocomplete'
    ]);

    app.config(function ($routeProvider, $locationProvider) {
        $routeProvider
         .when('/', {
             templateUrl: '/Content/AngularViews/index.html',
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