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

var ua = navigator.userAgent.toLowerCase();
var isAndroid = ua.indexOf("android") > -1;
if (isAndroid) {
    var current = document.getElementsByClassName("widget-mainSearch")[0];
    $(current).attr("style", "padding-top:4px");
}