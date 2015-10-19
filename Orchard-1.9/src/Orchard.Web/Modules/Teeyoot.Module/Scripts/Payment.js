var Payment = function() {

    var setDelivery = function(delivery) {

        var deliveryNumber = new BigNumber(delivery);
        $("#hidden_delivery").val(deliveryNumber.toFixed(2));

        $("#deliveryCost").html(deliveryNumber.toFixed(2));

        return deliveryNumber;
    };

    var getDelivery = function() {

        var delivery = $("#hidden_delivery").val();
        var deliveryNumber;

        if (delivery === "") {
            deliveryNumber = new BigNumber(0);
        } else {
            deliveryNumber = new BigNumber(delivery);
        }

        return deliveryNumber;
    };

    var addDeliveryToTotal = function(deliveryNumber) {

        var orderTotal = $("#ordTotal").html();
        var orderTotalNumber = new BigNumber(orderTotal);

        orderTotalNumber = orderTotalNumber.plus(deliveryNumber);

        $("#ordTotal").html(orderTotalNumber.toFixed(2));
    };

    var subtractDeliveryFromTotal = function(deliveryNumber) {

        var orderTotal = $("#ordTotal").html();
        var orderTotalNumber = new BigNumber(orderTotal);
        /*
        var delivery = $("#deliveryCost").html();
        var deliveryNumber = new BigNumber(delivery);
        */

        orderTotalNumber = orderTotalNumber.minus(deliveryNumber);

        $("#ordTotal").html(orderTotalNumber.toFixed(2));
    };

    var convertPrices = function(fromCountryId, toCountryId) {

        var fromCountryExchangeRate = $("#Country option[value=\"" + fromCountryId + "\"]")
            .data("exchange-rate");
        var toCountryExchangeRate = $("#Country option[value=\"" + toCountryId + "\"]")
            .data("exchange-rate");

        var fromCountryExchangeRateNumber = new BigNumber(fromCountryExchangeRate);
        var toCountryExchangeRateNumber = new BigNumber(toCountryExchangeRate);

        $(".item-price-value-placeholder").each(function(index, itemPriceElement) {
            var price = $(itemPriceElement).html();

            var priceNumber = new BigNumber(price);
            priceNumber = priceNumber.dividedBy(fromCountryExchangeRateNumber).times(toCountryExchangeRateNumber);

            $(itemPriceElement).html(priceNumber.toFixed(2));
        });

        var orderTotal = $("#ordTotal").html();

        var orderTotalNumber = new BigNumber(orderTotal);
        orderTotalNumber = orderTotalNumber.dividedBy(fromCountryExchangeRateNumber).times(toCountryExchangeRate);

        $("#ordTotal").html(orderTotalNumber.toFixed(2));

        var currencyCode = $("#Country option:selected").data("currency-code");
        $(".currency-code-placeholder").html(currencyCode);
    };

    var fillStatesControl = function(settings) {

        $("#State option:not(:first)").remove();

        var stateOptionHtml;

        for (var i = 0; i < settings.length; i++) {
            if (settings[i].Enabled) {
                stateOptionHtml = "<option value=\"%STATEVALUE%\" data-delivery-cost=\"%DELIVERYCOST%\">%STATENAME%</option>"
                    .replace("%STATEVALUE%", settings[i].State)
                    .replace("%DELIVERYCOST%", settings[i].DeliveryCost)
                    .replace("%STATENAME%", settings[i].State);

                $("#State").append(stateOptionHtml);
            }


/*
            countryElement.options[countryElement.length] = new Option(settingsArr[i].State, settingsArr[i].State);

            if (settingsArr[i].State === $("#paymentState").val() ||
                settingsArr[i].State === state ||
                settingsArr[i].State === "Selangor") {

                settingIndex = i;
                selectedIndex = i + 1;
            }
            */
        }
    };

    var getDeliverySettings = function(fromCountryId, toCountryId, cashOnDelivery) {

        var url = "/GetSettings?countryFromId=" + fromCountryId + "&countryToId=" + toCountryId;
        if (cashOnDelivery === true) {
            url += "&cashOnDelivery=true";
        }

        var deferred = $.ajax({
            cache: false,
            type: "GET",
            url: url,
            error: function(data) {
                alert("Status: " + data.status + ". Error message: " + data.statusText);
            }
        }).pipe(function(data) {
            return data.settings;
        });

        return deferred.promise();
    };

    var initPaymentPage = function() {

        $("#Country").change(function() {
            var fromCountryId = $("#hidden_country").val();
            var toCountryId = $("#Country option:selected").val();
            $("#hidden_country").val(toCountryId);

            var cashOnDelivery = false;
            var selectedPaymentMethod = $("#paymentMethod").val();
            if (selectedPaymentMethod === "4") {
                cashOnDelivery = true;
            }

            getDeliverySettings(fromCountryId, toCountryId, cashOnDelivery).done(function(data) {
                $("#parentDelivery").hide();

                fillStatesControl(data);
                var deliveryNumber = getDelivery();
                subtractDeliveryFromTotal(deliveryNumber);
                convertPrices(fromCountryId, toCountryId);
            });

            //get

            /*
            var currencyCode = $("#Country option:selected").data("currency-code");
            $(".currency-code-placeholder").html(currencyCode);

            ConvertPrices(fromCountryId, toCountryId);

            if (selectedPaymentMethod === "4") {
                populateCountriesForCashOnDelivery("State");
            } else {
                populateCountries("State", toCountryId);
            }
            */
        });

        $("#State").change(function() {
            var selectedState = $(this).val();

            var deliveryNumber = getDelivery();
            subtractDeliveryFromTotal(deliveryNumber);

            var delivery = $("#State option:selected").data("delivery-cost");

            if (selectedState === "") {
                $("#parentDelivery").hide();
            } else {
                deliveryNumber = setDelivery(delivery);
                addDeliveryToTotal(deliveryNumber);
                $("#paymentState").val(selectedState);
                $("#parentDelivery").show();
            }

            /*
            var orderTotal;
            var orderTotalNumber;
            var deliveryCostNumber;
            */

            /*
            if (state === "") {
                $("#parentDelivery").hide();
                orderTotal = $("#ordTotal").html();
                orderTotalNumber = new BigNumber(orderTotal);
                orderTotalNumber = orderTotalNumber.minus(delivCost);
                $("#ordTotal").html(orderTotalNumber.toFixed(2));
                delivCost = new BigNumber(0);
            } else {
                for (var i = 0; i < Settings.length; i++) {
                    if (Settings[i].State === state) {
                        deliveryCostNumber = new BigNumber(Settings[i].DeliveryCost);
                        $("#deliveryCost").html(deliveryCostNumber.toFixed(2));
                        $("#parentDelivery").show();
                        orderTotal = $("#ordTotal").html();
                        orderTotalNumber = new BigNumber(orderTotal);
                        orderTotalNumber = orderTotalNumber.minus(delivCost).plus(deliveryCostNumber);
                        $("#ordTotal").html(orderTotalNumber.toFixed(2));
                        delivCost = deliveryCostNumber;
                        $("#paymentState").val(state);
                    }
                }
            }
            */
        });
    };

    return {
        init: function() {
            initPaymentPage();
        }
    };
}();

/*

var Settings = [];
var delivCost = new BigNumber(0);

function populateCountries(countryElementId, countryToId) {
    var countryFromId = $("#seller_country_id").val();

    $("#Country").prop("disabled", false);

    var state = "@Model.Order.State";
    var settingsArr = [];

    $.ajax({
        async: false,
        cache: false,
        type: "GET",
        url: "/GetSettings?countryFromId=" + countryFromId + "&countryToId=" + countryToId,
        success: function (data) {
            settingsArr = data.settings;
        },
        error: function (data) {
            alert("Status: " + data.status + ". Error message: " + data.statusText);
        }
    });

    Settings = settingsArr;

    var countryElement = document.getElementById(countryElementId);
    countryElement.length = 0;
    countryElement.options[0] = new Option("Select State", "");

    var settingIndex;
    var selectedIndex = 0;

    for (var i = 0; i < settingsArr.length; i++) {
        countryElement.options[countryElement.length] = new Option(settingsArr[i].State, settingsArr[i].State);

        if (settingsArr[i].State === $("#paymentState").val() ||
            settingsArr[i].State === state ||
            settingsArr[i].State === "Selangor") {

            settingIndex = i;
            selectedIndex = i + 1;
        }
    }

    var orderTotal;
    var orderTotalNumber;
    var deliveryCostNumber;

    if (settingIndex === undefined) {
        $("#parentDelivery").hide();
        orderTotal = $("#ordTotal").html();
        orderTotalNumber = new BigNumber(orderTotal);
        orderTotalNumber = orderTotalNumber.minus(delivCost);
        $("#ordTotal").html(orderTotalNumber.toFixed(2));
        delivCost = new BigNumber(0);
    } else {
        deliveryCostNumber = new BigNumber(settingsArr[settingIndex].DeliveryCost);
        $("#deliveryCost").html(deliveryCostNumber.toFixed(2));
        $("#parentDelivery").show();
        orderTotal = $("#ordTotal").html();
        orderTotalNumber = new BigNumber(orderTotal);
        orderTotalNumber = orderTotalNumber.minus(delivCost).plus(deliveryCostNumber);
        $("#ordTotal").html(orderTotalNumber.toFixed(2));
        delivCost = deliveryCostNumber;
    }

    countryElement.selectedIndex = selectedIndex;
};

function populateCountriesForCashOnDelivery(countryElementId) {
    var countryFromId = $("#seller_country_id").val();

    $('#Country option[value=" + countryFromId + "]').prop('selected', true);
    $("#Country").prop("disabled", true);

    var currencyCode = $("#Country option:selected").data("currency-code");
    $(".currency-code-placeholder").html(currencyCode);

    var countryToId = $("#Country option:selected").val();
    $("#hidden_country").val(countryToId);

    var state = "@Model.Order.State";
    var settingsArr = [];

    $.ajax({
        async: false,
        cache: false,
        type: "GET",
        url: "/GetSettings?countryFromId=" + countryFromId + "&countryToId=" + countryToId + "&cashOnDelivery=true",
        success: function (data) {
            settingsArr = data.settings;
        },
        error: function (data) {
            alert("Status: " + data.status + ". Error message: " + data.statusText);
        }
    });

    Settings = settingsArr;

    var countryElement = document.getElementById(countryElementId);
    countryElement.length = 0;
    countryElement.options[0] = new Option("Select State", "");

    var settingIndex;
    var statesCurrentCount = 0;
    var selectedIndex = 0;

    for (var i = 0; i < settingsArr.length; i++) {
        if (settingsArr[i].Enabled) {
            countryElement.options[countryElement.length] = new Option(settingsArr[i].State, settingsArr[i].State);
            statesCurrentCount++;

            if (settingsArr[i].State === $("#paymentState").val() ||
                settingsArr[i].State === state ||
                settingsArr[i].State === "Selangor") {

                settingIndex = i;
                selectedIndex = statesCurrentCount;
            }
        }
    }

    var orderTotal;
    var orderTotalNumber;
    var deliveryCostNumber;

    if (settingIndex === undefined) {
        $("#parentDelivery").hide();
        orderTotal = $("#ordTotal").html();
        orderTotalNumber = new BigNumber(orderTotal);
        orderTotalNumber = orderTotalNumber.minus(delivCost);
        $("#ordTotal").html(orderTotalNumber.toFixed(2));
        delivCost = new BigNumber(0);
    } else {
        deliveryCostNumber = new BigNumber(settingsArr[settingIndex].DeliveryCost);
        $("#deliveryCost").html(deliveryCostNumber.toFixed(2));
        $("#parentDelivery").show();
        orderTotal = $("#ordTotal").html();
        orderTotalNumber = new BigNumber(orderTotal);
        orderTotalNumber = orderTotalNumber.minus(delivCost).plus(deliveryCostNumber);
        $("#ordTotal").html(orderTotalNumber.toFixed(2));
        delivCost = deliveryCostNumber;
    }

    countryElement.selectedIndex = selectedIndex;
};

function ConvertPrices(fromCountryId, toCountryId) {
    var fromCountryExchangeRate = $('#Country option[value="' + fromCountryId + '"]').data("exchange-rate");
    var toCountryExchangeRate = $('#Country option[value="' + toCountryId + '"]').data("exchange-rate");

    var fromCountryExchangeRateNumber = new BigNumber(fromCountryExchangeRate);
    var toCountryExchangeRateNumber = new BigNumber(toCountryExchangeRate);

    $(".item-price-value-placeholder").each(function (index, element) {
        var price = $(element).html();

        var priceNumber = new BigNumber(price);
        priceNumber = priceNumber.dividedBy(fromCountryExchangeRateNumber).times(toCountryExchangeRateNumber);

        $(element).html(priceNumber.toFixed(2));
    });

    var orderTotal = $("#ordTotal").html();

    var orderTotalNumber = new BigNumber(orderTotal);
    orderTotalNumber = orderTotalNumber.dividedBy(fromCountryExchangeRateNumber).times(toCountryExchangeRate);

    $("#ordTotal").html(orderTotalNumber.toFixed(2));
};

$("#State").change(function () {
    var stateValue = $("#State option:selected").val();

    var orderTotal;
    var orderTotalNumber;
    var deliveryCostNumber;

    if (stateValue === "") {
        $("#parentDelivery").hide();
        orderTotal = $("#ordTotal").html();
        orderTotalNumber = new BigNumber(orderTotal);
        orderTotalNumber = orderTotalNumber.minus(delivCost);
        $("#ordTotal").html(orderTotalNumber.toFixed(2));
        delivCost = new BigNumber(0);
    } else {
        for (var i = 0; i < Settings.length; i++) {
            if (Settings[i].State === stateValue) {
                deliveryCostNumber = new BigNumber(Settings[i].DeliveryCost);
                $("#deliveryCost").html(deliveryCostNumber.toFixed(2));
                $("#parentDelivery").show();
                orderTotal = $("#ordTotal").html();
                orderTotalNumber = new BigNumber(orderTotal);
                orderTotalNumber = orderTotalNumber.minus(delivCost).plus(deliveryCostNumber);
                $("#ordTotal").html(orderTotalNumber.toFixed(2));
                delivCost = deliveryCostNumber;
                $("#paymentState").val(stateValue);
            }
        }
    }
});
*/