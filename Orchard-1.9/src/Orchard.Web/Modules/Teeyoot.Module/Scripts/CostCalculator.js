function calculatePrice(frontColor, backColor) {
    var res = parseFloat(formula(frontColor, backColor));

    window.frontColor = parseInt(frontColor);
    window.backColor = parseInt(backColor);
    window.nowPrice = parseFloat(res.toFixed(2));

    if (document.getElementById('profSale').value.length == 0) {
        window.sellingPrice = Math.round(parseFloat(res.toFixed(2)) * 2);
        app.state.currentProduct.Price = window.sellingPrice;
    }

    document.getElementById("price_preview").innerHTML = "RM " + res.toFixed(2);
    app.state.currentProduct.BaseCost = window.nowPrice;

    var changes = parseFloat(app.state.currentProduct.Price) - window.nowPrice;
    updateMinimum(changes);
}

function calculatePriceForNewProduct(frontColor, backColor, cost) {
    var res = parseFloat(formula(frontColor, backColor, cost));

    var price = Math.round(parseFloat(res.toFixed(2)) * 2);
    var prices = [parseFloat(res.toFixed(2)), price];

    return prices;
}

function formula(frontColor, backColor, cost) {
    var additionalScreenCosts = parseFloat(window.additionalScreenCosts);                       //B4
    var costOfMaterial = parseFloat(window.costOfMaterial);                                     //B10
    var dTGPrintPrice = parseFloat(window.dTGPrintPrice);                                       //B12 
    var firstScreenCost = parseFloat(window.firstScreenCost);                                   //B3
    var inkCost = parseFloat(window.inkCost);                                                   //B5
    var labourCost = parseFloat(window.labourCost);                                             //B7
    var labourTimePerColourPerPrint = parseInt(window.labourTimePerColourPerPrint);             //B8
    var labourTimePerSidePrintedPerPrint = parseInt(window.labourTimePerSidePrintedPerPrint);   //B9
    var percentageMarkUpRequired = parseFloat(window.percentageMarkUpRequired) / 100;           //B11
    var printsPerLitre = parseInt(window.printsPerLitre);                                       //B6
    var count = parseInt(window.count);                                                         //B16

    if (cost != null) {
        costOfMaterial = parseFloat(cost.toFixed(2));
    }

    // argument1
    var argument1 = 1 + percentageMarkUpRequired;

    // argument2
    var argument2;
    if (backColor > 0) {
        argument2 = parseInt("1");
    } else {
        argument2 = parseInt("0");
    }

    // argument3
    var argument3;
    if (frontColor > 0) {
        argument3 = parseInt("1");
    } else {
        argument3 = parseInt("0");
    }

    // argument4
    var argument4 = parseFloat(labourTimePerSidePrintedPerPrint / 3600);

    // argument5
    var argument5 = frontColor + backColor;

    // argument6
    var argument6 = costOfMaterial + dTGPrintPrice;

    // argument7
    var argument7;
    if (frontColor > 1) {
        argument7 = parseInt("1");
    } else {
        argument7 = parseInt(frontColor);
    }

    // argument8
    var argument8;
    var argument8_1 = parseInt(frontColor - 1);
    if (argument8_1 > 0) {
        argument8 = parseInt(argument8_1);
    } else {
        argument8 = parseInt("0");
    }

    // argument9
    var argument9;
    if (backColor > 1) {
        argument9 = parseInt("1");
    } else {
        argument9 = parseInt(backColor);
    }

    // argument10
    var argument10;
    var argument10_1 = parseInt(backColor - 1);
    if (argument10_1 > 0) {
        argument10 = parseInt(argument10_1);
    } else {
        argument10 = parseInt("0");
    }

    // argument11
    var argument11 = costOfMaterial * count;

    // argument12
    var argument12 = count * argument5;

    // argument13
    var argument13 = labourCost * labourTimePerColourPerPrint / 3600 * argument12;

    // argument14
    var argument14 = inkCost / printsPerLitre * argument12;

    // argument15
    var argument15_1 = argument3 + argument2;
    var argument15 = labourCost * argument4 * argument15_1 * count;

    // основная функция
    var function1 = argument7 + argument9;
    var function2 = firstScreenCost * function1;
    var function3 = argument8 + argument10;
    var function4 = additionalScreenCosts * function3;
    var function5 = function2 + function4 + argument13 + argument14 + argument15 + argument11;
    var function6 = function5 / count;
    var function7;
    if (argument6 > function6) {
        function7 = parseFloat(function6);
    } else {
        function7 = parseFloat(argument6);
    }

    var result = function7 * argument1;

    return result;
}

function setPriceInGoalFromDesign() {
    document.getElementById('profSale').value = window.sellingPrice;
    document.getElementById('trackBarValue').value = window.count;
    document.getElementById('trackbar').value = document.getElementById('trackBarValue').value;
    document.getElementById('base-cost-for-first-product').innerHTML = app.state.currentProduct.BaseCost.toFixed(2);

    if (app.state.products.length > 1) {
        estimatedProfitChangeForManuProducts();
    } else {
        estimatedProfitChange();
    }
}

function setPriceInDesignFromGoal() {
    window.count = document.getElementById('trackbar').value;

    calculatePrice(window.frontColor, window.backColor);

    document.getElementById("price_preview").innerText = "RM " + window.nowPrice.toFixed(2);
    document.getElementById('count_preview').innerHTML = "Base cost &#64; " + window.count + " shirts";
}

function estimatedProfitChange() {
    var est = Math.floor(parseFloat((window.sellingPrice - window.nowPrice) * window.count));
    $("#total_profit").html("RM " + est + "+");
}

function estimatedProfitChangeForManuProducts() {
    var products = app.state.products;

    var result = [];
    for (var i = 0; i < products.length; i++) {
        var cost;
        var product = design.products.productsData[products[i].ProductId];
        var prices = product.prices;
        for (var k = 0; k < prices.length; k++) {
            if (prices[k].color_id == products[i].ColorId) {
                cost = prices[k].price;
            }
        }

        var prices = calculatePriceForNewProduct(window.frontColor, window.backColor, parseFloat(cost.toFixed(2)));
        products[i].BaseCost = prices[0];
        result.push(Math.floor(parseFloat(products[i].Price - products[i].BaseCost) * window.count));
        if (i > 0) {
            var profit = parseFloat((products[i].Price - products[i].BaseCost).toFixed(2));
            var indexCost = "#h4CostSale_" + parseInt(i + 1);
            var indexProf = "#h4ProfSale_" + parseInt(i + 1);

            var h4CostProfRm = "#h4CostProfRm_" + parseInt(i + 1);
            var divProfitCalcul = "#divProfitCalcul_" + parseInt(i + 1);
            var h4CostProfText = "#h4CostProfText_" + parseInt(i + 1);
            var h6Cost = "#h6Cost_" + parseInt(i + 1);
            var h4Price = "#h4Price_" + parseInt(i + 1);
            var h6Price = "#h6Price_" + parseInt(i + 1);


            $(indexProf).html(profit.toFixed(2));
            $(indexCost).html(products[i].BaseCost.toFixed(2));
            if (profit < 0) {
                $(divProfitCalcul).css('display', 'none');
                $(h6Cost).css('display', 'none');
                $(h6Price).css('display', 'none');
                $(h4Price).css('display', 'none');
                $(h4CostProfText).html("minimum");
                $(indexCost).html(products[i].BaseCost.toFixed(2));
                $(h4CostProfText).css('color', '#ff0000');
                $(indexCost).css('color', '#ff0000');
                $(h4CostProfRm).css('color', '#ff0000');

                //$(indexProf).html("RM " + products[i].BaseCost.toFixed(2) + " minimum");
                //$(indexProf).css('color', '#ff0000');
                //$("#total_profit").html("RM 0+");
            } else {
                $(divProfitCalcul).css('display', 'block');
                $(h6Cost).css('display', 'block');
                $(h6Price).css('display', 'block');
                $(h4Price).css('display', 'block');
                $(h4CostProfText).html("Cost Price");
                $(h4CostProfText).css('color', '#ff4f00');
                $(indexCost).css('color', '#ff4f00');
                $(h4CostProfRm).css('color', '#ff4f00');

                $(indexProf).html(profit.toFixed(2));
                $(indexProf).css('color', '#ff4f00');
                $(indexCost).html(products[i].BaseCost.toFixed(2));
                //if (app.state.products != null & app.state.products.length > 1) {
                //    estimatedProfitChangeForManuProducts()
                //} else {
                //    estimatedProfitChange();
                //}
            }
        }
    }
    var min = Math.min.apply(null, result);
    var max = Math.max.apply(null, result);
    if (min < 0) min = 0;
    if (min == max) {
        $("#total_profit").html("RM " + min + "+");
    } else {
        $("#total_profit").html("RM " + min + "-" + max + "+");
    }
}

function updateMinimum(changes) {
    if (changes < 0) {
        $("#profit-calculator").css('display', 'none');
        $("#price-for-first-product-text").css('display', 'none');
        $("#base-cost-for-first-product-text-smoll").css('display', 'none');
        //$("#").css('display', 'none');
        $("#base-cost-for-first-product-text").html("minimum");
        $("#base-cost-for-first-product").html(app.state.currentProduct.BaseCost.toFixed(2));
        $("#base-cost-for-first-product-rm").css('color', '#ff0000');
        $("#base-cost-for-first-product").css('color', '#ff0000');
        $("#base-cost-for-first-product-text").css('color', '#ff0000');


        //$("#mainH4").html("RM " + window.nowPrice + " minimum");
        //$("#mainH4").css('color', '#ff0000');
        //$("#total_profit").html("RM 0+");
    } else {
        $("#profit-calculator").css('display', 'block');
        $("#price-for-first-product-text").css('display', '-webkit-inline-box');
        $("#price-for-first-product-text").css('display', '-moz-inline-box');
        $("#price-for-first-product-text").css('display', '-ms-inline-flexbox');
        $("#base-cost-for-first-product-text-smoll").css('display', 'block');
        //$("#").css('display', 'block');
        $("#base-cost-for-first-product-text").html("Cost Price");
        $("#base-cost-for-first-product-rm").css('color', '#ff4f00');
        $("#base-cost-for-first-product").css('color', '#ff4f00');
        $("#base-cost-for-first-product-text").css('color', '#ff4f00');
        $("#base-cost-for-first-product").html(app.state.currentProduct.BaseCost.toFixed(2));
        $("#mainH4").html(changes.toFixed(2));
        $("#mainH4").css('color', '#ff4f00');
        //if (app.state.products != null & app.state.products.length > 1) {
        //    estimatedProfitChangeForManuProducts()
        //} else {
        //    estimatedProfitChange();
        //}
    }
}