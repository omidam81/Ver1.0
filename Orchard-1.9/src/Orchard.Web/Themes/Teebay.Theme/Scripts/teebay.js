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
