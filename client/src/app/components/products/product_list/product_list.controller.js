import * as consts from "../../../shared/consts/index.js";

(function (app) {
  app.controller("productListController", productListController);

  productListController.$inject = [
    "$scope",
    "apiService",
    "notificationService",
    "authData",
    "$ngBootbox",
    "$state",
    "$timeout",
    "$q",
    "$log",
    "$filter",
  ];

  function productListController(
    $scope,
    apiService,
    notificationService,
    authData,
    $ngBootbox,
    $state,
    $timeout,
    $q,
    $log,
    $filter
  ) {
    //#region check auth
    if (!authData?.authenticationData?.IsAuthenticated) {
      $state.go("login");
      return;
    }
    //#endregion

    //#region $scope
    $scope.products = [];
    $scope.categories = [];
    $scope.brands = [];
    $scope.suppliers = [];
    $scope.page = consts.DEFAULT_PAGE_INDEX;
    $scope.pageSize = consts.DEFAULT_PAGE_SIZE;
    $scope.pageCount = 0;
    $scope.searchString = "";
    $scope.categoryId = null;
    $scope.brandId = null;
    $scope.supplierId = null;
    $scope.fromDate = null;
    $scope.toDate = null;
    $scope.fromPrice = null;
    $scope.toPrice = null;
    $scope.isBestSelling = null;
    $scope.isNew = null;
    $scope.status = null;

    //#endregion

    //#region function
    $scope.GetProducts = GetProducts;
    $scope.DeleteProduct = DeleteProduct;
    $scope.SelectAll = SelectAll;
    $scope.DeleteMultipleProducts = DeleteMultipleProducts;

    // // xử lý sự kiện checked Input Category, Supplier, Brand

    $scope.HandlerCheckedInputCategory = HandlerCheckedInputCategory;
    $scope.HandlerCheckedInputBrand = HandlerCheckedInputBrand;
    $scope.HandlerCheckedInputSupplier = HandlerCheckedInputSupplier;

    // // xử lý sự kiện
    $scope.HandlerEventChangeStatus = HandlerEventChangeStatus;

    // xử lý sự kiện click input Status của Product
    $scope.handlerEventClickInputStatusProduct =
      handlerEventClickInputStatusProduct;

    $scope.handlerEventClickInputIsBestSellingProduct =
      handlerEventClickInputIsBestSellingProduct;

    $scope.handlerEventClickInputIsNewProduct =
      handlerEventClickInputIsNewProduct;

    function DeleteProduct(id) {
      $ngBootbox
        .confirm("Are you sure you want to delete this product?") // bạn có chắc muốn xóa sản phẩm không?
        .then(function () {
          apiService.del(
            "api/products/" + id,
            null,
            function (res) {
              if (res?.data?.code == 200) {
                notificationService.displaySuccess(
                  "Delete product successfully."
                );
                GetProducts();
              } else {
                notificationService.displayError(res?.data?.error);
              }
            },
            function () {
              notificationService.displayError("Delete product failed.");
            }
          );
        });
    }

    function DeleteMultipleProducts() {
      let listProductId = [];
      $.each($scope.selected, function (idx, item) {
        listProductId.push(item.Id);
      });

      $ngBootbox
        .confirm("Are you sure you want to delete this list product?")
        .then(function () {
          apiService.del(
            "api/products/delete-multiple",
            listProductId,
            function (res) {
              if (res?.data?.code == 200) {
                notificationService.displaySuccess(
                  "Delete list product successfully."
                );
                GetProducts();
              } else {
                notificationService.displayError(res?.data?.error);
              }
            },
            function () {
              notificationService.displayError("Delete list product failed.");
            }
          );
        });
    }

    // lấy danh sách sản phẩm theo page
    function GetProducts(page) {
      page = page || consts.DEFAULT_PAGE_INDEX;

      let category = $scope.categories.find((x) => x.checked);
      $scope.categoryId = category ? category.id : null;

      let brand = $scope.brands.find((x) => x.checked);
      $scope.brandId = brand ? brand.id : null;

      let supplier = $scope.suppliers.find((x) => x.checked);
      $scope.supplierId = supplier ? supplier.id : null;

      let config = {
        params: {
          page_index: page,
          page_size: $scope.pageSize,
          search_string: $scope.searchString,
          category_id: $scope.categoryId,
          brand_id: $scope.brandId,
          supplier_id: $scope.supplierId,
          from_date: $scope.fromDate,
          to_date: $scope.toDate,
          from_price: $scope.fromPrice,
          to_price: $scope.toPrice,
          is_best_selling: $scope.isBestSelling,
          is_new: $scope.isNew,
          status: $scope.status,
        },
      };

      console.log(config);
      apiService.get(
        "api/products",
        config,
        function (res) {
          if (res?.data?.code == 200) {
            $scope.products = res?.data?.data?.items;
            $scope.page = res?.data?.data?.pageIndex;
            $scope.pageCount = res?.data?.data?.totalPages;
            $scope.totalCount = res?.data?.data?.totalCount;
          } else {
            notificationService.displayError(res?.data?.error);
          }
        },
        function (err) {}
      );
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

    function HandlerCheckedInputCategory(id) {
      // nếu productcategory == id và checked == true thì tất cả các ô input khác checked = false và ô input có id thì checked == true
      // nếu productcategory == id và checked == false thì return
      var input = $scope.categories.find((x) => x.id == id);

      console.log(id);
      console.log(input);

      if (!input.checked) {
        GetProducts();
        return;
      }

      $scope.categories = $scope.categories.map(function (pc) {
        if (pc.id === id) {
          return pc;
        } else {
          pc.checked = false;
          return pc;
        }
      });

      // call data
      GetProducts();
    }

    function HandlerCheckedInputBrand(id) {
      var input = $scope.brands.find((x) => x.id == id);
      if (!input.checked) {
        // call data
        GetProducts();
        return;
      }

      $scope.brands = $scope.brands.map(function (b) {
        if (b.id === id) {
          return b;
        } else {
          b.checked = false;
          return b;
        }
      });

      // call data
      GetProducts();
    }

    function HandlerCheckedInputSupplier(id) {
      var input = $scope.suppliers.find((x) => x.id == id);
      if (!input.checked) {
        // call data
        GetProducts();
        return;
      }

      $scope.suppliers = $scope.suppliers.map(function (b) {
        if (b.id === id) {
          return b;
        } else {
          b.checked = false;
          return b;
        }
      });

      // call data
      GetProducts();
    }

    function HandlerEventChangeStatus() {
      GetProducts();
    }

    function handlerEventClickInputStatusProduct($event, id) {
      $event.preventDefault();
      let product = { ...$scope.products.find((x) => x.Id == id) };

      if (product == null)
        notificationService.displayError("There is an error in the system!");

      product.Status = !product.Status;

      $ngBootbox.confirm("Are you sure you want to change the status?").then(
        function (result) {
          apiService.put(
            "api/products/" + id + "/status",
            null,
            function (res) {
              if (res?.data?.code == 200) {
                notificationService.displaySuccess(
                  "Status update successfully!"
                );
                GetProducts();
              } else {
                notificationService.displaySuccess(res?.data?.error);
              }
            },
            function (error) {
              notificationService.displayError("Status update failed!");
            }
          );
        },
        function () {}
      );
    }

    function handlerEventClickInputIsBestSellingProduct($event, id) {
      $event.preventDefault();
      let product = { ...$scope.products.find((x) => x.Id == id) };

      if (product == null)
        notificationService.displayError("There is an error in the system!");

      product.Status = !product.Status;

      $ngBootbox
        .confirm("Are you sure you want to change the status is best selling?")
        .then(
          function (result) {
            apiService.put(
              "api/products/" + id + "/best-selling-status",
              null,
              function (res) {
                if (res?.data?.code == 200) {
                  notificationService.displaySuccess(
                    "Status is best selling update successfully!"
                  );
                  GetProducts();
                } else {
                  notificationService.displaySuccess(res?.data?.error);
                }
              },
              function (error) {
                notificationService.displayError(
                  "Status is best sellingss update failed!"
                );
              }
            );
          },
          function () {}
        );
    }

    function handlerEventClickInputIsNewProduct($event, id) {
      $event.preventDefault();
      let product = { ...$scope.products.find((x) => x.Id == id) };

      if (product == null)
        notificationService.displayError("There is an error in the system!");

      product.Status = !product.Status;

      $ngBootbox
        .confirm("Are you sure you want to change the status is new?")
        .then(
          function (result) {
            apiService.put(
              "api/products/" + id + "/newest-status",
              null,
              function (res) {
                if (res?.data?.code == 200) {
                  notificationService.displaySuccess(
                    "Status is new update successfully!"
                  );
                  GetProducts();
                } else {
                  notificationService.displaySuccess(res?.data?.error);
                }
              },
              function (error) {
                notificationService.displayError(
                  "Status is new update failed!"
                );
              }
            );
          },
          function () {}
        );
    }

    // Thêm file js vào cuối body sau khi chạy hết logic angularjs + html/csss
    function init() {
      let body = $("body");

      body.append(
        '<script>$(function () {$("#menu1").metisMenu()});</script>;'
      );
    }

    GetProducts();
    GetCategories();
    GetBrands();
    GetSuppliers();
    $timeout(init, 0);

    handleSearchProduct($state, $scope, $q, $log, apiService);

    $scope.$watch(
      "products",
      function (newVal, oldVal) {
        let checked = $filter("filter")(newVal, { checked: true });

        if (checked && checked.length) {
          $scope.selected = checked;
          $("#btnDelete").removeAttr("disabled");
        } else {
          $("#btnDelete").attr("disabled", "disabled");
        }
      },
      true
    );

    $scope.isAll = false;
    function SelectAll() {
      // đây là func sẽ chạy nếu sảy ra sự kiện click từ 1 button nào đó
      // nếu isAll đang là true => khi click vào sẽ chuyển thành false
      if ($scope.isAll) {
        angular.forEach($scope.products, function (item) {
          item.checked = false;
        });
        $scope.isAll = false;
      } else {
        angular.forEach($scope.products, function (item) {
          item.checked = true;
        });
        $scope.isAll = true;
      }
    }
  }

  //#endregion
})(angular.module("shop.products"));

function handleSearchProduct($state, $scope, $q, $log, apiService) {
  $scope.simulateQuery = false;
  $scope.isDisabled = false;

  $scope.repos = loadAll()
    .then((res) => {
      $scope.repos = res;
    })
    .catch();
  $scope.querySearch = querySearch;
  $scope.selectedItemChange = selectedItemChange;
  $scope.searchTextChange = searchTextChange;

  function querySearch(query) {
    let result = query
        ? $scope.repos.filter(createFilterFor(query))
        : $scope.repos,
      deferred;
    return result;
  }

  function searchTextChange(text) {}

  function selectedItemChange(item) {
    let idProductSelect = item.id;
    $state.go(`product_details`, { id: idProductSelect });
  }

  function loadAll() {
    let repos = [];
    let deferred = $q.defer();
    apiService.get(
      "api/products",
      null,
      function (res) {
        if (res?.data?.code == 200) {
          repos = res?.data?.data?.items.map(function (repo) {
            repo.value = repo.name.toLowerCase();
            return repo;
          });
          deferred.resolve(repos);
        } else {
          deferred.reject(repos);
        }
      },
      function () {
        deferred.reject(repos);
      }
    );

    return deferred.promise;
  }

  function createFilterFor(query) {
    let lowercaseQuery = query.toLowerCase();

    return function filterFn(item) {
      return item.value.indexOf(lowercaseQuery) === 0;
    };
  }
}
