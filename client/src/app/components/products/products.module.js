(function () {
  angular.module("shop.products", ["shop.common"]).config(config);

  config.$inject = ["$stateProvider", "$urlRouterProvider"];

  function config($stateProvider, $urlRouterProvider) {
    $stateProvider
      .state("products", {
        url: "/products",
        parent: "base",
        templateUrl:
          "app/components/products/product_list/product_list.view.html",
        controller: "productListController",
      })
      .state("product_add", {
        url: "/product_add",
        parent: "base",
        templateUrl:
          "app/components/products/product_add/product_add.view.html",
        controller: "productAddController",
      })
      .state("product_edit", {
        url: "/product_edit/:id",
        parent: "base",
        templateUrl:
          "app/components/products/product_edit/product_edit.view.html",
        controller: "productEditController",
      })
      .state("product_details", {
        url: "/product_details/:id",
        parent: "base",
        templateUrl:
          "app/components/products/product_details/product_details.view.html",
        controller: "productDetailsController",
      });
  }
})();
