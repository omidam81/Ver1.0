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