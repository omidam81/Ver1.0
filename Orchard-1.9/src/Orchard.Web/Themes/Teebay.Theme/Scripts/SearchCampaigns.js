var SearchCampaigns = function() {

    var initSearchControl = function() {

        var tbLayoutBeforeMain = $(".tb-layout-before-main");

        $("#search_campaigns").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "/SearchCampaigns?filter=" + request.term,
                    dataType: "html",
                    success: function(data) {
                        if (tbLayoutBeforeMain.length > 0) {
                            tbLayoutBeforeMain.hide();
                        }
                        $("#content .zone-content").html(data);
                    },
                    complete: function() {
                        $("#search_campaigns").removeClass("ui-autocomplete-loading");
                    }
                });
            },
            minLength: 0
        });
    };

    return {
        init: function() {
            initSearchControl();
        }
    };
}();