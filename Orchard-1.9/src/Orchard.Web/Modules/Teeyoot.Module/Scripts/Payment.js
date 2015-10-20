var Payment = function() {

    // ReSharper disable InconsistentNaming
    var _sellerCountry = $("#seller_country_id").val();
    var _country = $("#hidden_country").val();
    var _delivery = new BigNumber(0);
    var _total = new BigNumber($("#ordTotal").html());
    // ReSharper restore InconsistentNaming

    var setDelivery = function(delivery) {
        if (!delivery) {
            _delivery = new BigNumber(0);
        } else {
            _delivery = new BigNumber(delivery);
        }
    };

    var getExchangeRate = function(country) {
        var exchangeRateVal = $("#Country option[value=\"" + country + "\"]").data("exchange-rate");
        var exchangeRate = new BigNumber(exchangeRateVal);

        return exchangeRate;
    };

    var convertPrice = function(price, fromExchangeRate, toExchangeRate) {
        var convertedPrice = price.dividedBy(fromExchangeRate).times(toExchangeRate);

        return convertedPrice;
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

    var refreshPrices = function(country) {

        $("#deliveryCost").html(_delivery.toFixed(2));
        $("#ordTotal").html(_total.toFixed(2));

        var currencyCode = $("#Country option[value=\"" + country + "\"]").data("currency-code");
        $(".currency-code-placeholder").html(currencyCode);
    };

    var convertPrices = function(fromCountry, toCountry) {

        var fromExchangeRate = getExchangeRate(fromCountry);
        var toExchangeRate = getExchangeRate(toCountry);

        $(".item-price-value-placeholder").each(function(index, itemPriceElement) {
            var priceVal = $(itemPriceElement).html();
            var price = new BigNumber(priceVal);
            var convertedPrice = convertPrice(price, fromExchangeRate, toExchangeRate);

            $(itemPriceElement).html(convertedPrice.toFixed(2));
        });

        _total = convertPrice(_total, fromExchangeRate, toExchangeRate);
    };

    var changeCountryTo = function(country) {

        var cashOnDelivery = false;

        var selectedPaymentMethod = $("#paymentMethod").val();
        if (selectedPaymentMethod === "4") {
            cashOnDelivery = true;
        }

        getDeliverySettings(_sellerCountry, country, cashOnDelivery).done(function(data) {
            fillStatesControl(data);

            _total = _total.minus(_delivery);
            _delivery = new BigNumber(0);

            convertPrices(_country, country);
            refreshPrices(country);

            _country = country;
            $("#parentDelivery").hide();
        });
    };

    var initPaymentPage = function() {

        document.title = "Payment | Teeyoot";

        $(window).on("unload", function() {
            $("button").prop("disabled", false);
        });

        $(".payment-method:first")
            .css("text-decoration", "none")
            .css("color", "#ff4f00")
            .css("border-color", "#ff4f00");

        var defaultPaymentMethod = $(".payment-method:first").data("payment-method");

        if (defaultPaymentMethod === "creditcard") {
            $("#message_err").css({ "display": "none" });
            $("#payment_method_credit_card").show();
            $("#paymentMethod").val("1");
        } else if (defaultPaymentMethod === "paypal") {
            $("#message_err").css({ "display": "none" });
            $("#payment_method_paypal").show();
            $("#paymentMethod").val("2");
        } else if (defaultPaymentMethod === "mol") {
            $("#message_err").css({ "display": "none" });
            $("#payment_method_mol").show();
            $("#paymentMethod").val("3");
        } else if (defaultPaymentMethod === "cash") {
            $("#payment_method_cash").show();
            $("#message_err").css({ "display": "" });
            $("#paymentMethod").val("4");
        }

        $(".payment-method").click(function() {
            $(".payment-method")
                .css("text-decoration", "")
                .css("color", "")
                .css("border-color", "");

            $(this)
                .css("text-decoration", "none")
                .css("color", "#ff4f00")
                .css("border-color", "#ff4f00");

            var country = $("#Country option:selected").val();

            $(".payment-method-container").hide();
            var paymentMethod = $(this).data("payment-method");

            var note;

            if (paymentMethod === "creditcard") {
                $("#payment_method_credit_card").show();

                note = $("#credit_card_note").val();
                $("#message_Mol_span").text(note);

                if (note === "") {
                    $(".message_Mol").hide();
                } else {
                    $(".message_Mol").show();
                }

                $("#message_err").css({ "display": "none" });
                $("#paymentMethod").val("1");
            } else if (paymentMethod === "paypal") {
                $("#payment_method_paypal").show();

                note = $("#paypal_note").val();
                $("#message_Mol_span").text(note);

                if (note === "") {
                    $(".message_Mol").hide();
                } else {
                    $(".message_Mol").show();
                }

                $("#message_err").css({ "display": "none" });
                $("#paymentMethod").val("2");
            } else if (paymentMethod === "mol") {
                $("#payment_method_mol").show();

                note = $("#mol_note").val();
                $("#message_Mol_span").text(note);

                if (note === "") {
                    $(".message_Mol").hide();
                } else {
                    $(".message_Mol").show();
                }

                $("#message_err").css({ "display": "none" });
                $("#paymentMethod").val("3");
            } else if (paymentMethod === "cash") {
                $("#payment_method_cash").show();

                note = $("#cash_deliv_note").val();
                $("#message_Mol_span").text(note);

                if (note === "") {
                    $(".message_Mol").hide();
                } else {
                    $(".message_Mol").show();
                }

                $("#message_err").css({ "display": "" });
                $("#paymentMethod").val("4");
            }

            changeCountryTo(country);
        });

        var selectedCountry = $("#Country option:selected").val();
        changeCountryTo(selectedCountry);

        $("#Country").change(function() {
            var country = $(this).val();
            changeCountryTo(country);
        });

        $("#State").change(function() {
            _total = _total.minus(_delivery);

            var delivery = $("#State option:selected").data("delivery-cost");
            setDelivery(delivery);

            _total = _total.plus(_delivery);

            var state = $(this).val();
            if (state === "") {
                $("#parentDelivery").hide();
            } else {
                $("#parentDelivery").show();
            }

            refreshPrices(_country);
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




























/*
var deliverFromCountryId = $("#seller_country_id").val();
var deliverToCountryId = $(this).val();
var cashOnDelivery = false;
var selectedPaymentMethod = $("#paymentMethod").val();
if (selectedPaymentMethod === "4") {
    cashOnDelivery = true;
}
*/
/*
getDeliverySettings(deliverFromCountryId, deliverToCountryId, cashOnDelivery).done(function() {

});
*/
/*
getDeliverySettings(sellerCountryId, toCountryId, cashOnDelivery).done(function (data) {
    fillStatesControl(data);
    convertPrices(fromCountryId, toCountryId);
});
*/

/*
var fromCountryId = $("#hidden_country").val();
var toCountryId = $("#Country option:selected").val();
$("#hidden_country").val(toCountryId);

$("#parentDelivery").hide();

var deliveryNumber = getDelivery();
subtractDeliveryFromTotal(deliveryNumber);

var sellerCountryId = $("#seller_country_id").val();
var cashOnDelivery = false;
var selectedPaymentMethod = $("#paymentMethod").val();
if (selectedPaymentMethod === "4") {
    cashOnDelivery = true;
}

getDeliverySettings(sellerCountryId, toCountryId, cashOnDelivery).done(function(data) {
    fillStatesControl(data);
    convertPrices(fromCountryId, toCountryId);
});
*/


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


/*
            countryElement.options[countryElement.length] = new Option(settingsArr[i].State, settingsArr[i].State);

            if (settingsArr[i].State === $("#paymentState").val() ||
                settingsArr[i].State === state ||
                settingsArr[i].State === "Selangor") {

                settingIndex = i;
                selectedIndex = i + 1;
            }
            */

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

/*
        var fromCountryId = $("#seller_country_id").val();
        var toCountryId = $("#Country option:selected").val();
        var cashOnDelivery = false;

        var selectedPaymentMethod = $("#paymentMethod").val();
        if (selectedPaymentMethod === "4") {
            cashOnDelivery = true;
        }

        getDeliverySettings(fromCountryId, toCountryId, cashOnDelivery).done(function(data) {
            fillStatesControl(data);

            var deliveryNumber = getDelivery();
            subtractDeliveryFromTotal(deliveryNumber);

            var convertFromCountryId = $("#hidden_country").val();

            convertPrices(convertFromCountryId, toCountryId);

            $("#hidden_country").val(toCountryId);
        });
        */

/*
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
});
*/

/*
var state = $(this).val();
if (state === "") {
    $("#parentDelivery").hide();
} else {
    deliveryNumber = setDelivery(delivery);
    addDeliveryToTotal(deliveryNumber);
    $("#parentDelivery").show();

    $("#paymentState").val(state);
}
*/