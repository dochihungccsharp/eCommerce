import * as consts from "../../../shared/consts/index.js";

(function (app) {
  app.controller("brandEditController", brandEditController);

  brandEditController.$inject = [
    "apiService",
    "$scope",
    "notificationService",
    "$state",
    "$stateParams",
    "fileUploadService",
    "authData",
  ];

  function brandEditController(
    apiService,
    $scope,
    notificationService,
    $state,
    $stateParams,
    fileUploadService,
    authData
  ) {
    //#region check auth
    if (!authData?.authenticationData?.IsAuthenticated) {
      $state.go("login");
      return;
    }
    //#endregion

    //#region scope
    $scope.brand = {
      status: true,
    };

    $scope.UpdateBrand = UpdateBrand;
    $scope.DeleteImage = DeleteImage;
    $scope.UploadImage = UploadImage;

    //#endregion

    //#region load data

    loadBrandDetail();

    //#endregion

    //#region function
    function loadBrandDetail() {
      apiService.get(
        "api/brands/" + $stateParams.id,
        null,
        function (res) {
          if (res?.data?.code == 200) {
            $scope.brand = res?.data?.data;
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function (error) {
          notificationService.displayError(error.data);
        }
      );
    }

    function UploadImage(file) {
      var uploadUrl = consts.DEFAULT_URL_BACKEND + "api/files/upload",
        promise = fileUploadService.uploadFileToUrl(file[0], uploadUrl);

      promise.then(
        function (res) {
          if (res?.data?.code == 200) {
            $scope.brand.logoURL = res.data.data.filePath;
            $scope.image = consts.DEFAULT_URL_BACKEND + res.data.data.filePath;
          }
        },
        function () {
          notificationService.displayError("Thêm hình ảnh thất bại.");
        }
      );
    }

    function UpdateBrand() {
      apiService.put(
        "api/brands/" + $stateParams.id,
        $scope.brand,
        function (res) {
          if (res?.data?.code == 200) {
            notificationService.displaySuccess("Update brand successfully.");
            $state.go("brands");
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function (error) {
          notificationService.displayError("Brand update failed.");
        }
      );
    }

    function DeleteImage() {
      $scope.image = undefined;
      $scope.brand.logoURL = undefined;
      document.querySelector('input[type="file"]').value = null;
    }
    //#endregion
  }
})(angular.module("shop.brands"));
