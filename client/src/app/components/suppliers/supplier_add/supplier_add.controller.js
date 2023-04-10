import * as consts from "../../../shared/consts/index.js";

(function (app) {
  app.controller("supplierAddController", supplierAddController);

  supplierAddController.$inject = [
    "$scope",
    "apiService",
    "$state",
    "notificationService",
    "authData",
  ];

  function supplierAddController(
    $scope,
    apiService,
    $state,
    notificationService,
    authData
  ) {
    //#region check auth
    if (!authData?.authenticationData?.IsAuthenticated) {
      $state.go("login");
      return;
    }
    //#endregion

    // #region $scope
    $scope.supplier = {
      status: true,
    };

    $scope.AddSupplier = AddSupplier;
    // #endregion

    //#region function
    function AddSupplier() {
      apiService.post(
        "api/suppliers",
        $scope.supplier,
        function (res) {
          if (res?.data?.code == 200) {
            $state.go("suppliers");
            notificationService.displaySuccess("Add supplier successfully.");
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function () {
          notificationService.displayError("Add supplier failed");
        }
      );
    }
    //#endregion
  }
})(angular.module("shop.suppliers"));
