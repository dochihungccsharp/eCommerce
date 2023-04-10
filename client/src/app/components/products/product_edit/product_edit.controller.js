import * as consts from "../../../shared/consts/index.js";

(function (app) {
  app.controller("productEditController", productEditController);

  productEditController.$inject = [
    "$scope",
    "apiService",
    "$state",
    "notificationService",
    "fileUploadService",
    "$q",
    "authData",
    "$stateParams",
  ];

  function productEditController(
    $scope,
    apiService,
    $state,
    notificationService,
    fileUploadService,
    $q,
    authData,
    $stateParams
  ) {
    //#region check auth
    if (!authData?.authenticationData?.IsAuthenticated) {
      $state.go("login");
      return;
    }
    //#endregion

    // #region $scope
    $scope.categories = [];
    $scope.brands = [];
    $scope.suppliers = [];

    $scope.product = {
      status: true,
    };

    $scope.UpdateProduct = UpdateProduct;
    $scope.UploadImage = UploadImage;
    $scope.DeleteImage = DeleteImage;
    // #endregion

    //#region load data
    GetBrands();
    GetCategories();
    GetSuppliers();
    GetProduct();
    //#endregion

    //#region function
    function UpdateProduct() {
      apiService.post(
        "api/products",
        $scope.product,
        function (res) {
          if (res?.data?.code == 200) {
            $state.go("products");
            notificationService.displaySuccess("Update product successfully.");
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function () {
          notificationService.displayError("Update product failed");
        }
      );
    }

    function UploadImage(file) {
      var uploadUrl = consts.DEFAULT_URL_BACKEND + "api/files/upload",
        promise = fileUploadService.uploadFileToUrl(file[0], uploadUrl);

      promise.then(
        function (res) {
          if (res?.data?.code == 200) {
            $scope.product.imageUrl = res?.data?.data?.filePath;
          }
        },
        function () {
          notificationService.displayError("Upload image failed.");
        }
      );
    }

    function DeleteImage() {
      $scope.product.imageUrl = undefined;
      document.querySelector('input[type="file"]').value = null;
    }

    function GetCategories() {
      apiService.get(
        "api/categories",
        null,
        function (res) {
          if (res?.data?.code == "200") {
            $scope.categories = res?.data?.data?.items;
          } else {
            notificationService.displayWarning(res?.data?.error);
          }
        },
        function (error) {}
      );
    }

    function GetProduct() {
      apiService.get(
        "api/products/" + $stateParams.id,
        null,
        function (res) {
          if (res?.data?.code == "200") {
            $scope.product = res?.data?.data;
            console.log($scope.product);
          } else {
            notificationService.displayWarning(res?.data?.error);
          }
        },
        function (error) {}
      );
    }

    function GetBrands() {
      apiService.get(
        "api/brands",
        null,
        function (res) {
          if (res?.data?.code == "200") {
            $scope.brands = res?.data?.data?.items;
          } else {
            notificationService.displayWarning(res?.data?.error);
          }
        },
        function (error) {}
      );
    }

    function GetSuppliers() {
      apiService.get(
        "api/suppliers",
        null,
        function (res) {
          if (res?.data?.code == "200") {
            $scope.suppliers = res?.data?.data?.items;
          } else {
            notificationService.displayWarning(res?.data?.error);
          }
        },
        function (error) {}
      );
    }
    //#endregion
  }
})(angular.module("shop.products"));
