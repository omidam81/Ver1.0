function formula(frontColor, backColor, cost, newCount, tshirtCost) {
    var additionalScreenCosts = parseFloat(tshirtCost.AdditionalScreenCosts);                       //B4
    var costOfMaterial = parseFloat(cost);                                     //B10
    var dTGPrintPrice = parseFloat(tshirtCost.DTGPrintPrice);                                       //B12 
    var firstScreenCost = parseFloat(tshirtCost.FirstScreenCost);                                   //B3
    var inkCost = parseFloat(tshirtCost.InkCost);                                                   //B5
    var labourCost = parseFloat(tshirtCost.LabourCost);                                             //B7
    var labourTimePerColourPerPrint = parseInt(tshirtCost.LabourTimePerColourPerPrint);             //B8
    var labourTimePerSidePrintedPerPrint = parseInt(tshirtCost.LabourTimePerSidePrintedPerPrint);   //B9
    var percentageMarkUpRequired = parseFloat(tshirtCost.PercentageMarkUpRequired) / 100;           //B11
    var printsPerLitre = parseInt(tshirtCost.PrintsPerLitre);                                       //B6
    var count = parseInt("0");                                                         //B16

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

function availablePromoSize(oldBaseCost, newBaseCost, countSold, prodPrice) {

    nowProfit = countSold * (prodPrice - oldBaseCost);
    newProfit = (countSold+1) * (prodPrice - newBaseCost);
    return newProfit - nowProfit;

}

function estimatedProfitChange(products, count) {
    
    //var est = (parseFloat((window.sellingPrice - window.nowPrice) * window.count)).toFixed(2);
    //var est = (parseFloat((price - nowPrice) * count)).toFixed(0);

    var profit = [];
    for (var i = 0; i < products.length; i++) {
        var est = (parseFloat((parseFloat(products[i].Price) - products[i].BaseCost) * count)).toFixed(0);
     if (est.length == 6) {
            var val = est.toString().substring(0, 3) + "," + est.toString().substring(3, 6)
        } else if (est.length == 7) {
            var val = est.toString().substring(0, 1) + "," + est.toString().substring(1, 4) + "," + est.toString().substring(4, 7);
        } else if (est.length == 8) {
            var val = est.toString().substring(0, 2) + "," + est.toString().substring(2, 5) + "," + est.toString().substring(5, 8);
        } else if (est.length == 9) {
            var val = est.toString().substring(0, 3) + "," + est.toString().substring(3, 6) + "," + est.toString().substring(6, 9);
        } else {
            var val = est;
        }
        profit.push(val);
    }
    var minProfit = profit[0];
    var maxProfin = profit[0];
    if (products.length > 1) {
        for (var i = 0; i < profit.length; i++) {
            if (minProfit > profit[i]) {
                minProfit = profit[i];
            }
            if (maxProfin < profit[i]) {
                maxProfin = profit[i];
            }
        }
    }
    if (minProfit == maxProfin) {
        $("#total_profit").html("RM " + minProfit + "+");
    } else {
        $("#total_profit").html("RM " + minProfit + "-" + maxProfin + "+");
    }
    return minProfit;
}

function minProfitProductIndex(products, frontColor, backColor, cnt, tshirtCost) {
    if (products != null) {
        var productsChanged = [];
        for (var i = 0; i < products.length; i++) {
            price = parseFloat(products[i].Price);
            baseProductCost = parseFloat(products[i].BaseCostForProduct);
            baseCost = formula(frontColor, backColor, baseProductCost, cnt, tshirtCost);
            productsChanged.push({ id: products[i].ProductId, BaseCost: baseCost.toFixed(2), Price: products[i].Price });
        }
        var min = productsChanged[0];
        var z = 0;
        for (var i = 0; i < productsChanged.length; i++) {
            if (productsChanged[i] < min) {
                z = i;

            }
        }
        return z;
    }

}

function minimumGoal(products, frontColor, backColor, cnt, tshirtCost) {
    if (products != null) {
        
        var price = 0;
        var baseCost = 0;
        var count = parseInt("0");
        var productsChanged = [];

        for (var i = 0; i < products.length; i++) {
            price = parseFloat(products[i].Price);
            baseProductCost = parseFloat(products[i].BaseCostForProduct);
            baseCost = formula(frontColor, backColor, baseProductCost, cnt, tshirtCost);
            productsChanged.push({ id: products[i].ProductId, BaseCost: baseCost.toFixed(2), Price :  products[i].Price });


            var nowCount = cnt + 1;//Math.ceil(cnt / 2) + 1;
            var newPrice = 0;
            while (price - newPrice > 0) {
                if (nowCount == 0) break;
                nowCount--;
                newPrice = (formula(frontColor, backColor, baseProductCost, nowCount, tshirtCost)).toFixed(2);
            }
            nowCount++;

            //if (nowCount <= 0) {
            //    nowCount = 1;
            //}
            //if (cnt <= 100) {
            //    nowCount = nowCount - 1;
            //}

            count = count + Math.floor(nowCount);
        }

        if (count >= cnt) {
            count = cnt;
        }
       
        return productsChanged;
    }
}