(function () {
  angular
    .module("shop", [
      "shop.common",
      "shop.brands",
      "shop.categories",
      "shop.suppliers",
      "shop.products",
    ])
    .config(config)
    .config(configAuth);

  config.$inject = [
    "$stateProvider",
    "$locationProvider",
    "$urlRouterProvider",
  ];

  function config($stateProvider, $locationProvider, $urlRouterProvider) {
    $stateProvider
      .state("base", {
        url: "",
        templateUrl: "app/shared/views/base.view.html",
        abstract: true,
      })
      .state("login", {
        url: "/login",
        templateUrl: "app/components/login/login.view.html",
        controller: "loginController",
      })
      .state("home", {
        url: "/home",
        parent: "base",
        templateUrl: "app/components/home/home.view.html",
        controller: "homeController",
        auth: true,
      });

    $locationProvider.html5Mode(true);

    $locationProvider.hashPrefix("");

    $urlRouterProvider.otherwise("/login");
  }

  configAuth.$inject = ["$httpProvider"];

  function configAuth($httpProvider) {
    $httpProvider.interceptors.push(function ($q, $injector, $location) {
      return {
        request: function (config) {
          return config;
        },
        requestError: function (rejection) {
          return $q.reject(rejection);
        },
        response: function (response) {
          if (response?.data?.status == "401") {
            $location.path("/login");
          }
          //the same response/modified/or a new one need to be returned.
          return response;
        },
        responseError: function (rejection) {
          if (rejection?.data?.code == "401") {
            $location.path("/status");
          }
          return $q.reject(rejection);
        },
      };
    });
  }
})();
