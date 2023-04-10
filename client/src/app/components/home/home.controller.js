(function (app) {
  app.controller("homeController", homeController);

  homeController.$inject = [
    "$scope",
    "apiService",
    "notificationService",
    "$q",
    "authData",
    "$state",
  ];
  function homeController(
    $scope,
    apiService,
    notificationService,
    $q,
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
    $scope.statistics = {};
    $scope.products = [];
    $scope.categories = [];
    $scope.users = [];
    $scope.orders = [];
    //#endregion

    //#region load data
    GetMonthlyStatistics();
    GetTopCategoriesOfCurrentMonth();
    GetTopUsersOfCurrentMonth();
    GetTopProductsOfCurrentMonth();
    GetTopOrderOfCurrentMonthly();
    //#endregion

    //#region function
    function GetMonthlyStatistics() {
      apiService.get(
        "api/statistics/monthly",
        null,
        function (res) {
          console.log(res);
          if (res?.data?.code == "200") {
            $scope.statistics = res?.data?.data;
          } else {
            notificationService.displayWarning(res?.data?.error);
          }
        },
        function (error) {
          // handle error
        }
      );
    }
    function GetTopCategoriesOfCurrentMonth() {
      apiService.get(
        "api/statistics/categories/top-monthly",
        null,
        function (res) {
          if (res?.data?.code == "200") {
            $scope.categories = res?.data?.data;
            console.log($scope.categories);
          } else {
            notificationService.displayWarning(res?.data?.error);
          }
        },
        function (error) {
          // handle error
        }
      );
    }
    function GetTopProductsOfCurrentMonth() {
      apiService.get(
        "api/statistics/products/top-monthly",
        null,
        function (res) {
          if (res?.data?.code == "200") {
            $scope.products = res?.data?.data;
          } else {
            notificationService.displayWarning(res?.data?.error);
          }
        },
        function (error) {
          // handle error
        }
      );
    }
    function GetTopUsersOfCurrentMonth() {
      apiService.get(
        "api/statistics/users/top-monthly",
        null,
        function (res) {
          if (res?.data?.code == "200") {
            $scope.users = res?.data?.data;
          } else {
            notificationService.displayWarning(res?.data?.error);
          }
        },
        function (error) {
          // handle error
        }
      );
    }
    function GetTopOrderOfCurrentMonthly() {
      apiService.get(
        "api/statistics/orders/top-monthly",
        null,
        function (res) {
          if (res?.data?.code == "200") {
            $scope.orders = res?.data?.data;
          } else {
            notificationService.displayWarning(res?.data?.error);
          }
        },
        function (error) {
          // handle error
        }
      );
    }
    //#endregion
  }
})(angular.module("shop"));
