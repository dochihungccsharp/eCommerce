import * as consts from "../../../shared/consts/index.js";

(function (app) {
  app.controller("brandListController", brandListController);

  brandListController.$inject = [
    "$scope",
    "apiService",
    "notificationService",
    "$ngBootbox",
    "$filter",
    "authData",
  ];

  function brandListController(
    $scope,
    apiService,
    notificationService,
    $ngBootbox,
    $filter,
    authData
  ) {
    //#region check auth
    if (!authData?.authenticationData?.IsAuthenticated) {
      $state.go("login");
      return;
    }
    //#endregion

    //#region $scope
    $scope.brands = [];
    $scope.page = consts.DEFAULT_PAGE_INDEX;
    $scope.pageSize = consts.DEFAULT_PAGE_SIZE;
    $scope.pageCount = 0;
    $scope.keyword = "";

    $scope.deleteBrand = deleteBrand;
    $scope.search = search;
    $scope.getListBrand = getListBrand;
    //#endregion

    //#region load data

    $scope.getListBrand();

    //#endregion

    //#region function
    function deleteBrand(id) {
      $ngBootbox.confirm("Are you sure you want to delete?").then(function () {
        apiService.del(
          "api/brands/" + id,
          null,
          function (res) {
            if (res?.data?.code == 200) {
              notificationService.displaySuccess("Delete brand successfully.");
              $scope.search();
            } else {
              notificationService.displayError(res?.data?.error);
            }
          },
          function () {
            notificationService.displayError("Delete brand failed.");
          }
        );
      });
    }

    function search() {
      $scope.getListBrand();
    }

    function getListBrand(page) {
      page = page || consts.DEFAULT_PAGE_INDEX;

      var config = {
        params: {
          search_string: $scope.keyword,
          page_index: page,
          page_size: $scope.pageSize,
        },
      };

      apiService.get(
        "api/brands",
        config,
        function (res) {
          if (res?.data?.code == 200) {
            $scope.brands = res?.data?.data?.items;
            $scope.page = res?.data?.data?.pageIndex;
            $scope.pageCount = res?.data?.data?.totalPages;
            $scope.totalCount = res?.data?.data?.totalCount;
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function (error) {}
      );
    }

    //#endregion
  }
})(angular.module("shop.brands"));
