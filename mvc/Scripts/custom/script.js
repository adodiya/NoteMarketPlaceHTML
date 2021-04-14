$(function () {
    //on page loade
    showHideNav();
    $(window).scroll(function () {
        //on scroll
        showHideNav();

    });

    function showHideNav() {
        if ($(window).scrollTop() > 50) {
            $("#home").addClass("white-nav-top");
            $("#home .navbar-brand img").attr("src", "../images/logo.png");
            
        } else {

            $("#home").removeClass("white-nav-top");
            $("#home .navbar-brand img").attr("src", "../images/top-logo.png");
            //$("#back-to-top").fadeOut();
            
        }
    }
});



  
$(document).ready(function () {
    $("#mobile-nav-open-btn").click(function () {
        $("#mobile-nav").css("height", "100%");
    })

    $("#mobile-nav-close-btn , #mobile-nav a").click(function () {
        $("#mobile-nav").css("height", "0%");
    })
});







