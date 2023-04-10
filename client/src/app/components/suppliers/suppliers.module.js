(function () {
  angular.module("shop.suppliers", ["shop.common"]).config(config);

  config.$inject = ["$stateProvider", "$urlRouterProvider"];

  function config($stateProvider, $urlRouterProvider) {
    $stateProvider
      .state("suppliers", {
        url: "/suppliers",
        parent: "base",
        templateUrl:
          "app/components/suppliers/supplier_list/supplier_list.view.html",
        controller: "supplierListController",
      })
      .state("supplier_add", {
        url: "/supplier_add",
        parent: "base",
        templateUrl:
          "app/components/suppliers/supplier_add/supplier_add.view.html",
        controller: "supplierAddController",
      })
      .state("supplier_edit", {
        url: "/supplier_edit/:id",
        parent: "base",
        templateUrl:
          "app/components/suppliers/supplier_edit/supplier_edit.view.html",
        controller: "supplierEditController",
      });
  }
})();
