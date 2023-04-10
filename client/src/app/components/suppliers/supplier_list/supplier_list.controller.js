import * as consts from "../../../shared/consts/index.js";

(function (app) {
  app.controller("supplierListController", supplierListController);

  supplierListController.$inject = [
    "$scope",
    "apiService",
    "notificationService",
    "$ngBootbox",
    "authData",
  ];

  function supplierListController(
    $scope,
    apiService,
    notificationService,
    $ngBootbox,
    authData
  ) {
    //#region check auth
    if (!authData?.authenticationData?.IsAuthenticated) {
      $state.go("login");
      return;
    }
    //#endregion

    //#region $scope
    $scope.suppliers = [];
    $scope.page = consts.DEFAULT_PAGE_INDEX;
    $scope.pageSize = consts.DEFAULT_PAGE_SIZE;
    $scope.pageCount = 0;
    $scope.keyword = "";

    $scope.DeleteSupplier = DeleteSupplier;
    $scope.Search = Search;
    $scope.GetSuppliers = GetSuppliers;
    //#endregion

    //#region load data

    $scope.GetSuppliers();

    //#endregion

    //#region function
    function DeleteSupplier(id) {
      $ngBootbox.confirm("Are you sure you want to delete?").then(function () {
        apiService.del(
          "api/suppliers/" + id,
          null,
          function (res) {
            if (res?.data?.code == 200) {
              notificationService.displaySuccess(
                "Delete supplier successfully."
              );
              $scope.Search();
            } else {
              notificationService.displayError(res?.data?.error);
            }
          },
          function () {
            notificationService.displayError("Delete supplier failed.");
          }
        );
      });
    }

    function Search() {
      $scope.GetSuppliers();
    }

    function GetSuppliers(page) {
      page = page || consts.DEFAULT_PAGE_INDEX;

      var config = {
        params: {
          search_string: $scope.keyword,
          page_index: page,
          page_size: $scope.pageSize,
        },
      };

      apiService.get(
        "api/suppliers",
        config,
        function (res) {
          if (res?.data?.code == 200) {
            $scope.suppliers = res?.data?.data?.items;
            $scope.page = res?.data?.data?.pageIndex;
            $scope.pageCount = res?.data?.data?.totalPages;
            $scope.totalCount = res?.data?.data?.totalCount;
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function () {}
      );
    }

    //#endregion
  }
})(angular.module("shop.suppliers"));
