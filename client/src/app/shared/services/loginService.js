import { DEFAULT_URL_BACKEND } from "../consts/index.js";

(function (app) {
  "use strict";
  app.service("loginService", [
    "$http",
    "$q",
    "authenticationService",
    "authData",
    "apiService",
    function ($http, $q, authenticationService, authData, apiService) {
      var userInfo;
      var deferred;

      this.login = function (userName, password) {
        deferred = $q.defer();
        var loginModel = {
          email: userName,
          password: password,
        };
        $http
          .post(DEFAULT_URL_BACKEND + "api/users/sign-in", loginModel, {
            headers: { "Content-Type": "application/json" },
          })
          .then(
            function (response) {
              if (response.data.status) {
                userInfo = {
                  accessToken: response.data.access_token,
                  userName: userName,
                };
                authenticationService.setTokenInfo(userInfo);
                authData.authenticationData.IsAuthenticated = true;
                authData.authenticationData.userName = userName;
                authData.authenticationData.accessToken = userInfo.accessToken;
                deferred.resolve(true);
              } else {
                authData.authenticationData.IsAuthenticated = false;
                authData.authenticationData.userName = "";

                deferred.resolve(null);
              }
            },
            function (err, status) {
              authData.authenticationData.IsAuthenticated = false;
              authData.authenticationData.userName = "";
              deferred.resolve(null);
            }
          );
        return deferred.promise;
      };

      this.logOut = function () {
        authenticationService.removeToken();
        authData.authenticationData.IsAuthenticated = false;
        authData.authenticationData.userName = "";
        authData.authenticationData.accessToken = "";
      };
    },
  ]);
})(angular.module("shop.common"));
