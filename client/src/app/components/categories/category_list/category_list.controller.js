import * as consts from "../../../shared/consts/index.js";

(function (app) {
  app.controller("categoryListController", categoryListController);

  categoryListController.$inject = [
    "$scope",
    "apiService",
    "notificationService",
    "$ngBootbox",
    "authData",
    "$state",
  ];

  function categoryListController(
    $scope,
    apiService,
    notificationService,
    $ngBootbox,
    authData,
    $state
  ) {
    //#region check auth
    if (!authData?.authenticationData?.IsAuthenticated) {
      $state.go("login");
      return;
    }
    //#endregion

    //#region $scope
    $scope.categories = [];
    $scope.page = consts.DEFAULT_PAGE_INDEX;
    $scope.pageSize = consts.DEFAULT_PAGE_SIZE;
    $scope.pageCount = 0;
    $scope.keyword = "";

    //#endregion

    //#region function
    $scope.deleteCategory = function (id) {
      $ngBootbox.confirm("Are you sure you want to delete?").then(function () {
        var config = {
          params: {
            id: id,
          },
        };
        apiService.del(
          "api/categories/" + id,
          config,
          function (res) {
            if (res?.data?.code == 200) {
              notificationService.displaySuccess(
                "Delete category successfully."
              );
              $scope.search();
            } else {
              notificationService.displayError(res?.data?.error);
            }
          },
          function () {
            notificationService.displayError("Delete category failed.");
          }
        );
      });
    };

    $scope.search = function () {
      $scope.getListCategory();
    };

    $scope.getListCategory = function (page) {
      // params: thông số

      page = page || consts.DEFAULT_PAGE_INDEX;

      var config = {
        params: {
          search_string: $scope.keyword,
          page_index: page,
          page_size: $scope.pageSize,
        },
      };

      apiService.get(
        "api/categories",
        config,
        function (res) {
          if (res?.data?.data?.items == 0) {
            notificationService.displayWarning("Categories is not found");
          } else {
            $scope.categories = res?.data?.data?.items;
            $scope.page = res?.data?.data?.pageIndex;
            $scope.pageCount = res?.data?.data?.totalPages;
            $scope.totalCount = res?.data?.data?.totalCount;
          }
        },
        function (error) {}
      );
    };
    //#endregion

    //#region load data
    $scope.getListCategory();
    //#endregion
  }
})(angular.module("shop.categories"));
