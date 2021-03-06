﻿function calculatePrice(frontColor, backColor) {
    var res = parseFloat(formula(frontColor, backColor));

    window.frontColor = parseInt(frontColor);
    window.backColor = parseInt(backColor);
    window.nowPrice = parseFloat(res.toFixed(2));

    if (document.getElementById('profSale').value.length == 0) {
        window.sellingPrice = Math.round(parseFloat(res.toFixed(2)) * 2).toFixed(2);
        app.state.currentProduct.Price = window.sellingPrice;
    }

    document.getElementById("price_preview").innerHTML = window.currency + " " + res.toFixed(2);
    app.state.currentProduct.BaseCost = window.nowPrice;

    var changes = parseFloat(app.state.currentProduct.Price) - window.nowPrice;
    updateMinimum(changes.toFixed(2));
}

function calculatePriceForNewProduct(frontColor, backColor, cost) {
    var res = parseFloat(formula(frontColor, backColor, cost));

    var price = Math.round(parseFloat(res.toFixed(2)) * 2);
    var prices = [parseFloat(res.toFixed(2)), price];

    return prices;
}

function formula(frontColor, backColor, cost, newCount) {
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

    if (cost) {
        costOfMaterial = parseFloat(cost);
    }

    if (newCount) {
        count = newCount;
    }

    // argument1
    var argument1 = 1 + percentageMarkUpRequired;

    // argument2
    var argument2 = backColor > 0 ? parseInt("1") : parseInt("0");

    // argument3
    var argument3 = frontColor > 0 ? parseInt("1") : parseInt("0");

    // argument4
    var argument4 = parseFloat(labourTimePerSidePrintedPerPrint / 3600);

    // argument5
    var argument5 = frontColor + backColor;

    // argument6
    var argument6 = costOfMaterial + dTGPrintPrice;

    // argument7
    var argument7 = Math.min(frontColor, parseInt("1"));

    // argument8
    var argument8 = Math.max(parseInt("0"), parseInt(frontColor - 1));

    // argument9
    var argument9 = Math.min(backColor, parseInt("1"));

    // argument10
    var argument10 = Math.max(parseInt("0"), parseInt(backColor - 1));

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
    var function7 = argument6 > function6 ? parseFloat(function6) : parseFloat(argument6);

    var result = function7 * argument1;

    return result;
}

function setPriceInGoalFromDesign() {
    document.getElementById('profSale').value = window.sellingPrice;
    var slider = document.getElementById('trackbar');
    window.count = parseInt(slider.noUiSlider.get());
    document.getElementById('trackBarValue').value = window.count;
    //document.getElementById('trackbar').value = document.getElementById('trackBarValue').value;
    document.getElementById('base-cost-for-first-product').innerHTML = app.state.currentProduct.BaseCost.toFixed(2);

    if (app.state.products.length > 1) {
        estimatedProfitChangeForManuProducts();
    } else {
        estimatedProfitChange();
    }
}

function setPriceInDesignFromGoal() {
    var slider = document.getElementById('trackbar');
    window.count = parseInt(slider.noUiSlider.get());

    calculatePrice(window.frontColor, window.backColor);

    $(document.getElementById("price_preview")).text(window.currency + " " + window.nowPrice.toFixed(2));
    document.getElementById('count_preview').innerHTML = "Base cost &#64; " + window.count + " shirts";
}

function estimatedProfitChange() {
    //var est = (parseFloat((window.sellingPrice - window.nowPrice) * window.count)).toFixed(2);
    var est = (parseFloat((window.sellingPrice - window.nowPrice) * window.count)).toFixed(0);
    if (est.length == 6) {
        var val = est.toString().substring(0, 3) + "," + est.toString().substring(3, 6)
        $("#total_profit").html(window.currency + " " + val + "+");
    } else if (est.length == 7) {
        var val = est.toString().substring(0, 1) + "," + est.toString().substring(1, 4) + "," + est.toString().substring(4, 7);
        $("#total_profit").html(window.currency + " " + val + "+");
    } else if (est.length == 8) {
        var val = est.toString().substring(0, 2) + "," + est.toString().substring(2, 5) + "," + est.toString().substring(5, 8);
        $("#total_profit").html(window.currency + " " + val + "+");
    } else if (est.length == 9) {
        var val = est.toString().substring(0, 3) + "," + est.toString().substring(3, 6) + "," + est.toString().substring(6, 9);
        $("#total_profit").html(window.currency + " " + val + "+");
    } else {
        $("#total_profit").html(window.currency + " " + est + "+");
    }

    minimumGoal();
}

function estimatedProfitChangeForManuProducts() {
    var slider = document.getElementById('trackbar');
    window.count = parseInt(slider.noUiSlider.get());
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

        var prices = calculatePriceForNewProduct(window.frontColor, window.backColor, parseFloat(cost));
        products[i].BaseCost = prices[0];
        result.push(parseFloat(products[i].Price - products[i].BaseCost) * window.count);
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
            if (profit <= 0) {

                app.state.isNegativeProfit[i] = true;
                // $(divProfitCalcul).css('display', 'none');
                // $(h6Cost).css('display', 'none');
                // $(h6Price).css('display', 'none');
                // $(h4Price).css('display', 'none');
                //$(h4CostProfText).html("minimum");
                //$(indexCost).html(products[i].BaseCost.toFixed(2));
                //$(h4CostProfText).css('color', '#ff0000');
                //$(indexCost).css('color', '#ff0000');
                //$(h4CostProfRm).css('color', '#ff0000');

                $(indexProf).html(profit);
                //$(indexProf).css('color', '#ff0000');
                //$("#total_profit").html("RM 0+");
            } else {
                app.state.isNegativeProfit[i] = false;
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
    var min = Math.min.apply(null, result).toFixed(0);
    var max = Math.max.apply(null, result).toFixed(0);
    if (min < 0) min = 0;
    if (min == max) {

        if (min.length == 6) {
            var val = min.toString().substring(0, 3) + "," + min.toString().substring(3, 6)
            $("#total_profit").html(window.currency + " " + val + "+");
        } else if (min.length == 7) {
            var val = min.toString().substring(0, 1) + "," + min.toString().substring(1, 4) + "," + min.toString().substring(4, 7);
            $("#total_profit").html(window.currency + " " + val + "+");
        } else if (min.length == 8) {
            var val = min.toString().substring(0, 2) + "," + min.toString().substring(2, 5) + "," + min.toString().substring(5, 8);
            $("#total_profit").html(window.currency + " " + val + "+");
        } else if (min.length == 9) {
            var val = min.toString().substring(0, 3) + "," + min.toString().substring(3, 6) + "," + min.toString().substring(6, 9);
            $("#total_profit").html(window.currency + " " + val + "+");
        } else {
            $("#total_profit").html(window.currency + " " + min + "+");
        }


        //$("#total_profit").html("RM " + min + "+");


    } else {
        var valMin = min;
        var valMax = max;

        if (min.length == 6) {
             valMin = min.toString().substring(0, 3) + "," + min.toString().substring(3, 6)
        } else if (min.length == 7) {
             valMin = min.toString().substring(0, 1) + "," + min.toString().substring(1, 4) + "," + min.toString().substring(4, 7);
        } else if (min.length == 8) {
             valMin = min.toString().substring(0, 2) + "," + min.toString().substring(2, 5) + "," + min.toString().substring(5, 8);
        } else if (min.length == 9) {
             valMin = min.toString().substring(0, 3) + "," + min.toString().substring(3, 6) + "," + min.toString().substring(6, 9);
        }

        if (max.length == 6) {
            valMax = max.toString().substring(0, 3) + "," + max.toString().substring(3, 6)
        } else if (max.length == 7) {
            valMax = max.toString().substring(0, 1) + "," + max.toString().substring(1, 4) + "," + max.toString().substring(4, 7);
        } else if (max.length == 8) {
            valMax = max.toString().substring(0, 2) + "," + max.toString().substring(2, 5) + "," + max.toString().substring(5, 8);
        } else if (max.length == 9) {
            valMax = max.toString().substring(0, 3) + "," + max.toString().substring(3, 6) + "," + max.toString().substring(6, 9);
        }



        $("#total_profit").html(window.currency + " " + valMin + "-" + valMax + "+");

    }
    minimumGoal();
}

function updateMinimum(changes) {
    if (changes <= 0) {
        // $("#profit-calculator").css('display', 'none');
        //$("#price-for-first-product-text").css('display', 'none');
        //$("#base-cost-for-first-product-text-smoll").css('display', 'none');
        //$("#").css('display', 'none');
        //$("#base-cost-for-first-product-text").html("minimum");
        //$("#base-cost-for-first-product").html(app.state.currentProduct.BaseCost.toFixed(2));
        //$("#base-cost-for-first-product-rm").css('color', '#ff0000');
        //$("#base-cost-for-first-product").css('color', '#ff0000');
        //$("#base-cost-for-first-product-text").css('color', '#ff0000');
        if (app.state.isNegativeProfit != null) {
            app.state.isNegativeProfit[0] = true;
        }
        $("#mainH4").html(changes);
        //$("#mainH4").css('color', '#ff0000');
        if (app.state.products.length < 2) {
            $("#total_profit").html(window.currency + " 0");
        }
    } else {
        if (app.state.isNegativeProfit != null) {
            app.state.isNegativeProfit[0] = false;
        }
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
        $("#mainH4").html(changes);
        $("#mainH4").css('color', '#ff4f00');
        //if (app.state.products != null & app.state.products.length > 1) {
        //    estimatedProfitChangeForManuProducts()
        //} else {
        //    estimatedProfitChange();
        //}
    }
}

function minimumGoal() {
    if (app.state.products != null) {
        var slider = document.getElementById('trackbar');
        window.count = parseInt(slider.noUiSlider.get());
        var products = app.state.products;
        var price = 0;
        var baseCost = 0;

        var count = parseInt("0");
        
        for (var i = 0; i < products.length; i++) {
            price = parseFloat(products[i].Price);
            var prod = design.products.productsData[products[i].ProductId];
            baseCost = parseFloat(prod.prices[0].price);

            var nowCount = window.count + 1;//Math.ceil(window.count / 2) + 1;
            var newPrice = 0;
            while (price - newPrice > 0) {
                if(nowCount == 0) break;
                nowCount--;
                newPrice = (formula(window.frontColor, window.backColor, baseCost, nowCount)).toFixed(2);
            }
            window.count = parseInt(slider.noUiSlider.get());
            nowCount++;
            //if (nowCount <= 0) {
            //    nowCount = 1;
            //}
            //if (window.count <= 100) {
            //    nowCount = nowCount - 1;
            //}

            count = count + Math.floor(nowCount);
        }

        if (count >= window.count) {
            count = window.count;
        }
        
        document.getElementById("minimmumGoal").value = count;
    }
}