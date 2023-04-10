$(function () {
  for (
    var e = window.location,
      o = $(".metismenu li a")
        .filter(function () {
          return this.href == e;
        })
        .addClass("")
        .parent()
        .addClass("mm-active");
    o.is("li");

  )
    o = o.parent("").addClass("mm-show").parent("").addClass("mm-active");
});

$(function () {
  $("#menu").metisMenu();
});
