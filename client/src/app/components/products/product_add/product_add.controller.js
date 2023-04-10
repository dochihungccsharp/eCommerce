import * as consts from "../../../shared/consts/index.js";

(function (app) {
  app.controller("productAddController", productAddController);

  productAddController.$inject = [
    "$scope",
    "apiService",
    "$state",
    "notificationService",
    "fileUploadService",
    "$q",
    "authData",
  ];

  function productAddController(
    $scope,
    apiService,
    $state,
    notificationService,
    fileUploadService,
    $q,
    authData
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

    $scope.AddProduct = AddProduct;
    $scope.UploadImage = UploadImage;
    $scope.DeleteImage = DeleteImage;
    // #endregion

    //#region load data
    GetBrands();
    GetCategories();
    GetSuppliers();
    //#endregion

    //#region function
    function AddProduct() {
      apiService.post(
        "api/products",
        $scope.product,
        function (res) {
          if (res?.data?.code == 200) {
            $state.go("products");
            notificationService.displaySuccess("Add product successfully.");
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function () {
          notificationService.displayError("Add product failed");
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
          notificationService.displayError("Thêm hình ảnh thất bại.");
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
