function calculatePrice(frontColor, backColor) {
    var res = parseFloat(formula(frontColor, backColor));

    window.frontColor = parseInt(frontColor);
    window.backColor = parseInt(backColor);
    window.nowPrice = parseFloat(res.toFixed(2));

    if (document.getElementById('profSale').value.length == 0) {
        window.sellingPrice = parseFloat(res.toFixed(2)) * ((parseFloat(window.percentageMarkUpRequired) / 100) * 2);
    }

    document.getElementById("price_preview").innerText = "RM " + res.toFixed(2);
}

//function calculatePriceFromGoal() {
//    var res = parseFloat(formula(window.frontColor, window.backColor));

//    window.frontColor = parseInt(frontColor);
//    window.backColor = parseInt(backColor);
//    window.nowPrice = parseFloat(res.toFixed(2));
//    window.sellingPrice = Math.round(window.nowPrice * 2);
//    document.getElementById("price_preview").innerText = "RM " + res.toFixed(2);
//}

function formula(frontColor, backColor) {
    var additionalScreenCosts = parseFloat(window.additionalScreenCosts);                       //B4
    var costOfMaterial = parseFloat(window.costOfMaterial);                                     //B10
    var dTGPrintPrice = parseFloat(window.dTGPrintPrice);                                       //B12 
    var firstScreenCost = parseFloat(window.firstScreenCost);                                   //B3
    var inkCost = parseFloat(window.inkCost);                                                   //B5
    var labourCost = parseFloat(window.labourCost);                                             //B7
    var labourTimePerColourPerPrint = parseInt(window.labourTimePerColourPerPrint);             //B8
    var labourTimePerSidePrintedPerPrint = parseInt(window.labourTimePerSidePrintedPerPrint);   //B9
    var percentageMarkUpRequired = parseFloat("0");//parseFloat(window.percentageMarkUpRequired) / 100;           //B11
    var printsPerLitre = parseInt(window.printsPerLitre);                                       //B6
    var count = parseInt(window.count);                                                         //B16

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
    document.getElementById('profSale').value = "RM " + window.sellingPrice;
    document.getElementById('trackBarValue').value = window.count;
    document.getElementById('trackbar').value = document.getElementById('trackBarValue').value;
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