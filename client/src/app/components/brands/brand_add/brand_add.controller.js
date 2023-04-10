import * as consts from "../../../shared/consts/index.js";

(function (app) {
  app.controller("brandAddController", brandAddController);

  brandAddController.$inject = [
    "$scope",
    "apiService",
    "$state",
    "notificationService",
    "fileUploadService",
    "$q",
    "authData",
  ];

  function brandAddController(
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
    $scope.brand = {
      status: true,
    };

    $scope.AddBrand = AddBrand;
    $scope.UploadImage = UploadImage;
    $scope.DeleteImage = DeleteImage;
    // #endregion

    //#region function
    function AddBrand() {
      apiService.post(
        "api/brands",
        $scope.brand,
        function (res) {
          if (res?.data?.code == 200) {
            $state.go("brands");
            notificationService.displaySuccess("Add brand successfully.");
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function () {
          notificationService.displayError("Add brand failed");
        }
      );
    }

    function UploadImage(file) {
      var uploadUrl = consts.DEFAULT_URL_BACKEND + "api/files/upload",
        promise = fileUploadService.uploadFileToUrl(file[0], uploadUrl);

      promise.then(
        function (res) {
          if (res?.data?.code == 200) {
            $scope.brand.logoURL = res?.data?.data?.filePath;
          }
        },
        function () {
          notificationService.displayError("Thêm hình ảnh thất bại.");
        }
      );
    }

    function DeleteImage() {
      $scope.brand.logoURL = undefined;
      document.querySelector('input[type="file"]').value = null;
    }
    //#endregion
  }
})(angular.module("shop.brands"));
