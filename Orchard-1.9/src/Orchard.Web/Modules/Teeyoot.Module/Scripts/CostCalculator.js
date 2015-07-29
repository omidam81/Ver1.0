function calculatePrice(frontColor, backColor) {

    var price = document.getElementById("price_preview");
    price.innerText = "RM " + frontColor + backColor;
}

function formula(frontColor, backColor) {
    var additionalScreenCosts = parseFloat(window.additionalScreenCosts);                       //B4
    var costOfMaterial = parseFloat(window.costOfMaterial);                                     //B10
    var dTGPrintPrice = parseFloat(window.dTGPrintPrice);                                       //B12 
    var firstScreenCost = parseFloat(window.firstScreenCost);                                   //B3
    var inkCost = parseFloat(window.inkCost);                                                   //B5
    var labourCost = parseFloat(window.labourCost);                                             //B7
    var labourTimePerColourPerPrint = parseInt(window.labourTimePerColourPerPrint);             //B8
    var labourTimePerSidePrintedPerPrint = parseInt(window.labourTimePerSidePrintedPerPrint);   //B9
    var percentageMarkUpRequired = parseFloat(window.percentageMarkUpRequired);                 //B11
    var printsPerLitre = parseInt(window.printsPerLitre);                                       //B6
    var count = parseInt(window.count);                                                         //B16
    var defaultPrice = parseFloat(window.defaultPrice);

    // a
    var a = 1 + percentageMarkUpRequired;

    // b
    var b;
    if (backColor > 0) {
        b = parseInt("1");
    } else {
        b = parseInt("0");
    }

    // c
    var c;
    if (frontColor > 0) {
        c = parseInt("1");
    } else {
        c = parseInt("0");
    }

    // q
    var q = parseFloat(labourTimePerSidePrintedPerPrint / 3600);

    // w
    var w = frontColor + backColor;

    // e
    var e = costOfMaterial + dTGPrintPrice;

    // r
    var r;
    if (frontColor > 1) {
        r = parseInt("1");
    } else {
        r = parseInt(frontColor);
    }

    // t
    var t;
    var t1 = parseInt(frontColor - 1);
    if (t1 > 0) {
        t = parseInt(t1);
    } else {
        t = parseInt("0");
    }

    // y
    var y;
    if (backColor > 1) {
        y = parseInt("1");
    } else {
        y = parseInt(backColor);
    }

    // u
    var u;
    var u1 = parseInt(backColor - 1);
    if (u1 > 0) {
        u = parseInt(u1);
    } else {
        u = parseInt("0");
    }

    // i
    var i = costOfMaterial * count;

    // p
    var p = count * w;

    // s
    var s = labourCost * labourTimePerColourPerPrint / 3600 * p;

    // d
    var d
}