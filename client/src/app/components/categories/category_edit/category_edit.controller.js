import * as consts from "../../../shared/consts/index.js";

(function (app) {
  app.controller("categoryEditController", categoryEditController);

  categoryEditController.$inject = [
    "apiService",
    "$scope",
    "notificationService",
    "$state",
    "$stateParams",
    "authData",
    "fileUploadService",
  ];

  function categoryEditController(
    apiService,
    $scope,
    notificationService,
    $state,
    $stateParams,
    authData,
    fileUploadService
  ) {
    $scope.category = {
      status: true,
    };

    $scope.UpdateCategory = UpdateCategory;
    $scope.UploadImage = UploadImage;
    $scope.DeleteImage = DeleteImage;

    function loadProductCategoryDetail() {
      apiService.get(
        "api/categories/" + $stateParams.id,
        null,
        function (res) {
          if (res?.data?.code == 200) {
            $scope.category = res?.data?.data;
            console.log($scope.category);
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function (error) {
          notificationService.displayError(error.data);
        }
      );
    }

    function UpdateCategory() {
      console.log("dcmmmmmmmmmmmmmmmmm");
      console.log($scope.category);
      apiService.put(
        "api/categories/" + $stateParams.id,
        $scope.category,
        function (res) {
          if (res?.data?.code == 200) {
            $state.go("categories");
            notificationService.displaySuccess("Update category successfully.");
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function () {
          notificationService.displayError("Update category failed");
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
        function () {}
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
    loadProductCategoryDetail();
  }
})(angular.module("shop.categories"));
