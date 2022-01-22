$('.navbar-nav li a').on("click", (function () {
    $('.navbar-nav li').removeClass("active");
    $(this).parent().addClass("active");
}));

//úplně to nefunguje, mizí po přechodu na jinou stránku