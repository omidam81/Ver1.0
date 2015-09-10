function redirectToStartCampaign() {
    var prot = window.location.protocol;
    var host = window.location.host;
    var des = prot + "//" + host + "/GetStarted";
    window.location = des;
}

function redirectToSearchCampaign() {
    var prot = window.location.protocol;
    var host = window.location.host;
    var des = prot + "//" + host + "/TSearch?filter=";
    window.location = des;
}

var toggleMenu = function () {
    var el = $('.tb-aside-first');

    if (el.hasClass('menu-closed')) {
        el.removeClass('menu-closed');
    } else {
        el.addClass('menu-closed');
    }

    $('.fa-angle-left').toggleClass('hidden');
    $('.fa-angle-right').toggleClass('hidden');
}

var toggleMobileMenu = function () {
    if ($('.widget-SellerDashboardMenu')[0] != null) {
        var el = $('.widget-SellerDashboardMenu')[0];
    }
    else if ($('.widget-SellerDashboardMenuID')[0] != null) {
        var el = $('.widget-SellerDashboardMenuID')[0];
    }
    else {
        var el = $('.widget-SellerDashboardMenuSG')[0];
    }

    if ($(el).hasClass('shown')) {
        $(el).removeClass('shown');
        $('.menu-opener').removeClass('hidden');
        $('.menu-closer').removeClass('shown');
    } else {
        $('.menu-closer').addClass('shown');
        $('.menu-opener').addClass('hidden')
        $(el).addClass('shown');
    }
}

$(window).scroll(function (event) {
    var yOffset = window.pageYOffset;
    var windowHeight = window.innerHeight;
    var footer = $('.tb-layout-footer')[0];
    var footerOffsetTop = footer.offsetTop;

    if (windowHeight + yOffset > footerOffsetTop) {
        $(".collapse-button").removeClass("fixed");
    } else {
        $(".collapse-button").addClass("fixed");
    }
});

var ua = navigator.userAgent.toLowerCase();
var isAndroid = ua.indexOf("android") > -1;
if (isAndroid) {
    if (document.getElementsByClassName("widget-mainSearch")[0] != null) {
        var current = document.getElementsByClassName("widget-mainSearch")[0];
    }
    else if (document.getElementsByClassName("widget-mainSearchID")[0] != null) {
        var current = document.getElementsByClassName("widget-mainSearchID")[0];
    }
    else {
        var current = document.getElementsByClassName("widget-mainSearchSG")[0];
    }
    
    $(current).attr("style", "padding-top:4px");
}