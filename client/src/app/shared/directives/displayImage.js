import { DEFAULT_URL_BACKEND } from "../../shared/consts/index.js";

(function (app) {
  "use strict";

  app.directive("displayImage", displayImage);

  function displayImage() {
    return {
      restrict: "A",
      link: function (scope, element, attrs) {
        attrs.$observe("displayImage", function (value) {
          if (value) {
            // Tách chuỗi đường dẫn trả về từ Web API C# để lấy đường dẫn tương đối
            var relativePath =
              DEFAULT_URL_BACKEND +
              value.substring(
                value.indexOf("\\wwwroot\\") + "\\wwwroot\\".length
              );
            // Hiển thị đường dẫn ảnh
            element.attr("src", relativePath);
          }
        });
      },
    };
  }
})(angular.module("shop.common"));
