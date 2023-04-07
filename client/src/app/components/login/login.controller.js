(function (app) {
  app.controller("loginController", [
    "$scope",
    "loginService",
    "$injector",
    "notificationService",
    "authData",
    "$state",
    function ($scope, loginService, $injector, notificationService, authData, $state) {

      if (authData?.authenticationData?.IsAuthenticated) {
        $state.go("home");
        return;
      }

      $scope.loginData = {
        userName: "",
        password: "",
      };

      $scope.loginSubmit = function () {
        loginService
          .login($scope.loginData.userName, $scope.loginData.password)
          .then(function (response) {
            console.log(response);
            if (!response) {
              notificationService.displayError("Đăng nhập thất bại.");
            } else {
              var stateService = $injector.get("$state");
              stateService.go("home");
              console.log(authData.authenticationData);
            }
          });
      };
    },
  ]);
})(angular.module("shop"));
