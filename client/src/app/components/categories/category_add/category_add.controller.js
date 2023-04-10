import * as consts from "../../../shared/consts/index.js";

(function (app) {
  app.controller("categoryAddController", categoryAddController);

  categoryAddController.$inject = [
    "$scope",
    "apiService",
    "$state",
    "notificationService",
    "commonService",
    "authData",
    "fileUploadService",
  ];

  function categoryAddController(
    $scope,
    apiService,
    $state,
    notificationService,
    commonService,
    authData,
    fileUploadService
  ) {
    //#region check auth
    if (!authData?.authenticationData?.IsAuthenticated) {
      $state.go("login");
      return;
    }
    //#endregion

    $scope.category = {
      status: true,
    };

    $scope.parentCategories = [];

    $scope.AddCategory = AddCategory;
    $scope.UploadImage = UploadImage;
    $scope.DeleteImage = DeleteImage;

    function AddCategory() {
      apiService.post(
        "api/categories",
        $scope.category,
        function (res) {
          if (res?.data?.code == 200) {
            $state.go("categories");
            notificationService.displaySuccess("Add category successfully.");
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function (error) {
          notificationService.displayError("Add category failed");
        }
      );
    }

    function loadParentCategory() {
      apiService.get(
        "api/categories",
        null,
        function (res) {
          if (res?.data?.code == "200") {
            $scope.parentCategories = res?.data?.data?.items;
          } else {
            notificationService.displayWarning(res?.data?.error);
          }
        },
        function (error) {}
      );
    }

    function UploadImage(file) {
      var uploadUrl = consts.DEFAULT_URL_BACKEND + "api/files/upload",
        promise = fileUploadService.uploadFileToUrl(file[0], uploadUrl);

      promise.then(
        function (res) {
          if (res?.data?.code == 200) {
            $scope.category.imageUrl = res?.data?.data?.filePath;
          }
        },
        function () {
          notificationService.displayError("Thêm hình ảnh thất bại.");
        }
      );
    }

    function DeleteImage() {
      $scope.category.imageUrl = undefined;
      document.querySelector('input[type="file"]').value = null;
    }
    loadParentCategory();
  }
})(angular.module("shop.categories"));
