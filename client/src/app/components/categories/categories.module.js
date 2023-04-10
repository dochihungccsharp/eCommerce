(function () {
  angular.module("shop.categories", ["shop.common"]).config(config);

  config.$inject = ["$stateProvider", "$urlRouterProvider"];

  function config($stateProvider, $urlRouterProvider) {
    $stateProvider
      .state("categories", {
        url: "/categories",
        parent: "base",
        templateUrl:
          "app/components/categories/category_list/category_list.view.html",
        controller: "categoryListController",
      })
      .state("category_add", {
        url: "/category_add",
        parent: "base",
        templateUrl:
          "app/components/categories/category_add/category_add.view.html",
        controller: "categoryAddController",
      })
      .state("category_edit", {
        url: "/category_edit/:id",
        parent: "base",
        templateUrl:
          "app/components/categories/category_edit/category_edit.view.html",
        controller: "categoryEditController",
      });
  }
})();
