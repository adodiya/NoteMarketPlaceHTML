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
            $("#home .navbar-brand img").attr("src", "images/Homepage/logo.png");
            
        } else {

            $("#home").removeClass("white-nav-top");
            $("#home .navbar-brand img").attr("src", "images/pre-login/top-logo.png");
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

$(".toggle-password").click(function() {

  $(this).toggleClass("fa-eye fa-eye-slash");
  var input = $($(this).attr("toggle"));
  if (input.attr("type") == "password") {
    input.attr("type", "text");
  } else {
    input.attr("type", "password");
  }
});


$(".navbar-login-button").click(function(){
   window.location.href="login.html"; 
});

$("#home-learn-more-btn").click(function(){
   window.location.href="FAQ.html"; 
});

