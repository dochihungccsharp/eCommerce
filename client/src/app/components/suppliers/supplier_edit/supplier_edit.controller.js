import * as consts from "../../../shared/consts/index.js";

(function (app) {
  app.controller("supplierEditController", supplierEditController);

  supplierEditController.$inject = [
    "apiService",
    "$scope",
    "notificationService",
    "$state",
    "$stateParams",
    "authData",
  ];

  function supplierEditController(
    apiService,
    $scope,
    notificationService,
    $state,
    $stateParams,
    authData
  ) {
    //#region check auth
    if (!authData?.authenticationData?.IsAuthenticated) {
      $state.go("login");
      return;
    }
    //#endregion

    //#region scope
    $scope.supplier = {
      status: true,
    };

    $scope.UpdateSupplier = UpdateSupplier;

    //#endregion

    //#region load data

    GetSupplier();

    //#endregion

    //#region function
    function GetSupplier() {
      apiService.get(
        "api/suppliers/" + $stateParams.id,
        null,
        function (res) {
          if (res?.data?.code == 200) {
            $scope.supplier = res?.data?.data;
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function (error) {
          notificationService.displayError(error.data);
        }
      );
    }

    function UpdateSupplier() {
      apiService.put(
        "api/suppliers/" + $stateParams.id,
        $scope.supplier,
        function (res) {
          if (res?.data?.code == 200) {
            notificationService.displaySuccess("Update supplier successfully.");
            $state.go("suppliers");
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function () {
          notificationService.displayError("Supplier update failed.");
        }
      );
    }

    //#endregion
  }
})(angular.module("shop.suppliers"));
