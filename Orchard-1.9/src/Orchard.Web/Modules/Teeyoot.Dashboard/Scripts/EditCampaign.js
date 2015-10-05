var EditCampaign = function() {

    var initEditCampaign = function() {

        var allTagsJsonString = $("#all_tags").val();
        var allTags = JSON.parse(allTagsJsonString);

        var tagsJsonString = $("#selected_tags").val();
        var tags = JSON.parse(tagsJsonString);

        var $inputTags = $("#inputTags").selectize({
            plugins: ["remove_button"],
            delimiter: ",",
            persist: false,
            valueField: "name",
            labelField: "name",
            searchField: ["name"],
            create: function(input) {
                return {
                    name: input
                }
            },
            options: allTags,
            items: tags
        });

        var inputTags = $inputTags[0].selectize;

        $("#tags_to_save").val(inputTags.getValue());
        inputTags.on("change", function(value) {
            $("#tags_to_save").val(value);
        });

        $(".textarea").wysihtml5({
            parserRules: wysihtml5ParserRules
        });

        var isFront = true;
        $("#card").flip({
            trigger: "manual"
        }).click(function() {
            isFront = !isFront;
            console.log(isFront);
            $("#card").flip(!isFront);
        });

        $("#name").keydown(function() {
            $("#title_error_text").hide();
            $("#name").removeClass("wizard-error");
        });

        $("#edit_campaign_form").submit(function() {
            var titleValue = $("#name").val();
            var titleErrorText;

            if ($.trim(titleValue) === "") {
                titleErrorText = $("#title_cant_be_empty_error_text").val();
                $("#title_error_text").html(titleErrorText);
                $("#title_error_text").show();
                $("#name").addClass("wizard-error");
            }

            var descriptionValue = $("#campaign_description_text").val();
            var descriptionErrorText;

            if ($.trim(descriptionValue) === "") {
                descriptionErrorText = $("#description_cant_be_empty_error_text").val();
                $("#description_error-text").html(descriptionErrorText);
                $("#description_error-text").show();
                $(".wysihtml5-sandbox").addClass("wizard-error");
            }

            if (titleErrorText !== undefined || descriptionErrorText !== undefined) {
                return false;
            }

            return true;
        });
    };

    return {
        init: function() {
            initEditCampaign();
        }
    };
}();