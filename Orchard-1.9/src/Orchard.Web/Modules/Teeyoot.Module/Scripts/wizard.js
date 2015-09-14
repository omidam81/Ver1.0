window.onresize = function () {
    app.state.w = $(window).width();
    var slides = $('.Slides > div');
    slides.css({ width: app.state.w + 'px' });
};
var globalPrdc = '';
window.onload = function initWizard() {

    app.state.selectProduct = true;
    app.state.cancelSelectProduct = false;
    app.state.currentDropDownOption = 1;
    app.state.prevDropDownOption = 1;
    app.state.products = [];
    app.state.isNegativeProfit = [];
    app.state.w = $(window).width();
    app.state.h = $(window).height();
    document.getElementById('trackbar').value = 250;
    var slides = $('.Slides > div');
    $('.Slides').css({ width: slides.length + '00%' });
    slides.css({ width: app.state.w + 'px' });
    app.state.pos = 0;
    slide();

    //if (document.querySelector(".user-email") == null) {

    //$("#openTags").click(function () {
    //    document.getElementById("tags").style.display = "inline"; 
    //    document.getElementById("openTags").style.display = "none";
    //});

    $("#butAdd").click(function addElement() {

        var message = document.getElementById("error-add-message");
        $(message).addClass("hidden");
        var productId = parseInt(document.getElementById("product").value);

        var coeff = parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_front_height) / parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_front_width);
        var coeffBack = parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_back_height) / parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_back_width);

        var front = (design.products.images[productId].printable_front_height / design.products.images[productId].printable_front_width).toFixed(1);
        var back = (design.products.images[productId].printable_back_height / design.products.images[productId].printable_back_width).toFixed(1);
        if ((front != coeff.toFixed(1)) && (back != coeffBack.toFixed(1))) {
            $(message).removeClass("hidden");
            return;
        };


        var slider = document.getElementById('trackbar');
        window.count = parseInt(slider.noUiSlider.get());

        if (document.querySelectorAll(".ssp_block").length >= 7) {
            document.getElementById("ui").style.display = "none";
        }

        var prdc = {
            ProductId: 95,
            BaseCost: 7.14,
            ColorId: 2260,
            SecondColorId: 0,
            ThirdColorId: 0,
            FourthColorId: 0,
            FifthColorId: 0,
            Price: 15,
            CurrencyId: 12
        };


        var div = document.createElement("div");
        var divThumb = document.createElement("div");
        var divMeta = document.createElement("div");
        var divVcent = document.createElement("div");
        var divDelete = document.createElement("div");
        var image = document.createElement("img");
        var imageDel = document.createElement("img");
        var text = document.createElement("h4");
        var inpHidd = document.createElement("input");
        inpHidd.type = "hidden";
        inpHidd.id = productId;
        var $div = $(div);

        //var $salePriceTextDiv = $(salePriceTextDiv);
        //var $salePriceTextH4 = $(salePriceTextH4);
        //$salePriceTextDiv.append($salePriceTextH4);

        image.src = assetsUrls.products + 'product_type_' + document.getElementById("product").value + '_front_small.png';
        image.classList.add("sell");
        image.style.height = "73px";
        prdc.ProductId = parseInt(document.getElementById("product").value);

        var product = design.products.productsData[prdc.ProductId];
        prdc.ColorId = app.state.currentProduct.ColorId;//product.colors_available[0];
        var prices = product.prices;
        var changeBaseCost = false;
        for (var i = 0; i < prices.length; i++) {
            if (prices[i].color_id == prdc.ColorId) {
                prdc.BaseCost = prices[i].price;
                changeBaseCost = true;
            }
        }
        if (!changeBaseCost) {
            prdc.BaseCost = prices[0].price;
            prdc.ColorId = prices[0].color_id;
        }
        var calc = calculatePriceForNewProduct(window.frontColor, window.backColor, prdc.BaseCost);
        prdc.BaseCost = calc[0];
        prdc.Price = calc[1];

        $div.click(function () {
            if (app.state.products.indexOf(prdc) >= 0) {
                var colorId = prdc.ColorId;
                if (prdc.FifthColorId > 0) {
                    colorId = prdc.FifthColorId;
                } else if (prdc.FourthColorId > 0) {
                    colorId = prdc.FourthColorId;
                } else if (prdc.ThirdColorId > 0) {
                    colorId = prdc.ThirdColorId;
                } else if (prdc.SecondColorId > 0) {
                    colorId = prdc.SecondColorId;
                } else {
                    colorId = prdc.ColorId;
                }

                var newColor = design.products.colors[colorId];
                var src = assetsUrls.products + 'product_type_' + product.id + '_front_small.png';
                design.save(function (data) {
                    var srcFront = assetsUrls.products + 'product_type_' + product.id + '_front.png';
                    var srcBack = assetsUrls.products + 'product_type_' + product.id + '_back.png';
                    var imageData = app.state.getImage();
                    var front = {
                        top: imageData['printable_front_top'],
                        left: imageData['printable_front_left']
                    };
                    var back = {
                        top: imageData['printable_front_top'],
                        left: imageData['printable_front_left']
                    };

                    $('#prodFront').attr('src', srcFront).css('background-color', newColor.value);
                    $('#prodBack').attr('src', srcBack).css('background-color', newColor.value);
                    design.item.unselect();
                    while (document.querySelector('#stp2frArea') != null) {
                        document.querySelector('#step-2-front').removeChild(document.querySelector('#stp2frArea'));
                    }
                    $('#view-front-design-area').clone().attr("id", "stp2frArea").css({ "border": "none" }, { "margin-left": "2px" }, { "overflow": "hidden !important" }).appendTo('#step-2-front');
                    document.querySelector('#stp2frArea').removeChild(document.querySelector('#stp2frArea').childNodes[5]);
                    $('#stp2frArea').css({ "overflow": "hidden" });

                    while (document.querySelector('#stp2BackArea') != null) {
                        document.querySelector('#step-2-back').removeChild(document.querySelector('#stp2BackArea'));
                    }
                    $('#view-back-design-area').clone().attr("id", "stp2BackArea").css({ "border": "none" }, { "margin-left": "2px" }, { "overflow": "hidden !important" }).appendTo('#step-2-back');
                    document.querySelector('#stp2BackArea').removeChild(document.querySelector('#stp2BackArea').childNodes[5]);
                    $('#stp2BackArea').css({ "overflow": "hidden" });
                });

                var el = $('#primary')[0];
                var newSize = $(el).find('.block.ssp_block');

                if (newSize.hasClass('design-active')) {
                    newSize.removeClass('design-active');
                }

                div.classList.add('design-active');
            }
        });

        imageDel.classList.add("ssp_delete");
        imageDel.src = "https://d1b2zzpxewkr9z.cloudfront.net/compiled_assets/designer/ssp_del-4d7ed20752fe1fbe0917e4e4d605aa16.png";
        imageDel.style.cursor = "pointer";

        var $image = $(image);
        $image.css("background-color", design.products.colors[prdc.ColorId].value);

        //----------- profit/sale ----------------------------------


        var divAllPriceCalcul = document.createElement("div");
        divAllPriceCalcul.classList.add("all-price-calculator");
        var divPriceCalcul = document.createElement("div");
        var divCostCalcul = document.createElement("div");
        var divProfitCalcul = document.createElement("div");
        divPriceCalcul.classList.add("price-calculator");
        divCostCalcul.classList.add("cost-calculator");
        divProfitCalcul.classList.add("profit-calculator");



        var divPricing = document.createElement("div");
        var divProfit = document.createElement("div");
        var spanPrice = document.createElement("span");
        var inpPrice = document.createElement("input");
        var h4Profit = document.createElement("h4");


        var h4Price = document.createElement("h4");
        h4Price.classList.add("h4ProfSale");
        h4Price.classList.add("sales_price_text_area");
        h4Price.innerHTML = "Sale Price";
        var h6Price = document.createElement("h6");
        h6Price.classList.add("h6Sale");
        h6Price.innerHTML = "(per shirt)";

        var divCostProf = document.createElement("div");
        var h4CostProfRm = document.createElement("h4");
        var h4CostProfFloat = document.createElement("h4");
        var h4CostProfText = document.createElement("h4");
        var h6Cost = document.createElement("h6");
        divCostProf.classList.add("profitSale");
        h4CostProfRm.classList.add("h4ProfSale");
        h4CostProfRm.classList.add("costH4Rm");
        h4CostProfText.classList.add("h4ProfSale");
        h4CostProfText.classList.add("costH4Text");
        h4CostProfFloat.classList.add("h4ProfSale");
        h4CostProfFloat.classList.add("costH4Float");
        h6Cost.classList.add("h6Sale");
        h6Cost.innerHTML = "(per shirt)";
        h4CostProfRm.innerHTML = window.currency;
        h4CostProfText.innerHTML = "Cost Price";
        h4CostProfFloat.innerHTML = prdc.BaseCost.toFixed(2);
        divCostProf.appendChild(h4CostProfRm);
        divCostProf.appendChild(h4CostProfFloat);
        divCostProf.appendChild(h4CostProfText);
        divCostProf.appendChild(h6Cost);
        divCostCalcul.appendChild(divCostProf);

        var divProf = document.createElement("div");
        var h4ProfRm = document.createElement("h4");
        var h4ProfText = document.createElement("h4");
        var h6 = document.createElement("h6");
        divProf.classList.add("profitSale");
        h4ProfRm.classList.add("h4ProfSale");
        h4ProfRm.classList.add("costH4Rm");
        h4ProfRm.classList.add("profit");
        h4ProfText.classList.add("h4ProfSale");
        h4ProfText.classList.add("costH4Text");
        h4ProfText.classList.add("profit");
        h6.classList.add("h6Sale");
        h6.classList.add("profit");
        h6.innerHTML = "(per shirt)";
        h6.style.paddingTop = "2px";
        h4ProfRm.innerHTML = window.currency;
        h4ProfText.innerHTML = "Your Profit";
        divProf.appendChild(h4ProfRm);
        divProf.appendChild(h4Profit);
        divProf.appendChild(h4ProfText);
        divProf.appendChild(h6);
        divProfitCalcul.appendChild(divProf);


        divPricing.classList.add("ssp_pricing");
        divPricing.classList.add("ssp_pricing_new");
        //divPricing.style.marginLeft = "-8%";
        //divPricing.style.height = "40px";
        //divPricing.style.display = "-webkit-inline-box";
        ////divPricing.style.display = "-moz-inline-box";
        //divPricing.style.display = "-ms-inline-flexbox";
        //divPricing.style.display = "inline-flex";
        divProfit.classList.add("profitSale");
        spanPrice.classList.add("rm-for-price");
        spanPrice.style.fontWeight = "normal";
        spanPrice.innerHTML = window.currency;
        inpPrice.classList.add("ssp_input");
        inpPrice.classList.add("price_per");
        inpPrice.classList.add("form__textfield");
        inpPrice.classList.add("profSale");

        inpPrice.value = prdc.Price.toFixed(2);

        h4Profit.classList.add("h4ProfSale");
        h4Profit.classList.add("costH4Float");
        h4Profit.classList.add("profit");

        var index = app.state.products.length + 1;
        h4Profit.id = "h4ProfSale_" + parseInt(index);
        h4CostProfFloat.id = "h4CostSale_" + parseInt(index);
        h4CostProfRm.id = "h4CostProfRm_" + parseInt(index);
        divProfitCalcul.id = "divProfitCalcul_" + parseInt(index);
        h4CostProfText.id = "h4CostProfText_" + parseInt(index);
        h6Cost.id = "h6Cost_" + parseInt(index);
        h4Price.id = "h4Price_" + parseInt(index);
        h6Price.id = "h6Price_" + parseInt(index);
        inpPrice.id = "Input_" + parseInt(index);

        var chenges = prdc.Price - prdc.BaseCost.toFixed(2);
        h4Profit.innerHTML = chenges.toFixed(2);

        $inp = $(inpPrice);
        $span = $(spanPrice);


        // Ивент на остаток прибыли от суммы одной футболки -------------------
        $inp.change(function () {
            var price = parseFloat(parseFloat(String(inpPrice.value).match(/-?\d+(?:\.\d+)?/g, '') || 0, 10) - prdc.BaseCost);
            prdc.Price = parseFloat(String(inpPrice.value).match(/-?\d+(?:\.\d+)?/g, '') || 0, 10).toFixed(2);
            h4Profit.innerHTML = price.toFixed(2);
            h4CostProfFloat.innerHTML = prdc.BaseCost.toFixed(2);
            estimatedProfitChangeForManuProducts();
        });

        $inp.on({
            change: profSaleProd,
            keydown: function (e) {
                if (e.which == 13) {
                    profSaleProd();
                    document.querySelector('#' + inpPrice.id).value = Number($('#' + inpPrice.id).val()).toFixed(2);
                }
            }
        });

        function profSaleProd() {
            var asd = inpPrice.id.split("Input_");
            var dsadasd = app.state.products[asd[1] - 1];

            var price = parseFloat(parseFloat(String(inpPrice.value).match(/-?\d+(?:\.\d+)?/g, '') || 0, 10) - prdc.BaseCost);
            prdc.Price = parseFloat(String(inpPrice.value).match(/-?\d+(?:\.\d+)?/g, '') || 0, 10).toFixed(2);
            h4Profit.innerHTML = price.toFixed(2);
            h4CostProfFloat.innerHTML = prdc.BaseCost.toFixed(2);
            dsadasd.Price = inpPrice.value;
            estimatedProfitChangeForManuProducts();
        }

        var $divPricing = $(divPricing);
        $divPricing.append($span);
        $divPricing.append($inp);
        //divProfit.appendChild(h4Profit); // TODO 
        //var $divProfit = $(divProfit);

        divProfit.appendChild(h4Price);
        divProfit.appendChild(h6Price);
        divPriceCalcul.appendChild(divPricing);
        divPriceCalcul.appendChild(divProfit);
        divAllPriceCalcul.appendChild(divPriceCalcul);
        divAllPriceCalcul.appendChild(divCostCalcul);
        divAllPriceCalcul.appendChild(divProfitCalcul);


        //----------- profit/sale ----------------------------------


        //-----------------Color Picker------------------------------
        //-----Создаем тимплейт для пикера
        var divCol = document.createElement("div");
        var divColPick = document.createElement("div");
        var divSwatch = document.createElement("div");
        var divColors = document.createElement("div");
        var divClear = document.createElement("div");
        var ulAllColors = document.createElement("ul");


        var divForColors = document.createElement("div");
        var divColor1Active = document.createElement("div");
        var divColor2 = document.createElement("div");
        var divColor3 = document.createElement("div");
        var divColor4 = document.createElement("div");
        var divColor5 = document.createElement("div");
        var divColorDelete1 = document.createElement("div");
        var divColorDelete2 = document.createElement("div");
        var divColorDelete3 = document.createElement("div");
        var divColorDelete4 = document.createElement("div");
        var divColorDelete5 = document.createElement("div");
        divForColors.classList.add("div-for-colors");
        //divForColors.id = "div-for-colors-products-" + index;
        divColor1Active.classList.add("div-color-active");
        //divColor1Active.id = "div-color-" + index + "_1";
        var col = design.products.colors[prdc.ColorId];
        divColor1Active.style.backgroundColor = col.value;
        divColor2.classList.add("div-color");
        //divColor2.id = "div-color-" + index + "_2";
        divColor3.classList.add("div-color");
        //divColor3.id = "div-color-" + index + "_3";
        divColor4.classList.add("div-color");
        //divColor4.id = "div-color-" + index + "_4";
        divColor5.classList.add("div-color");
        //divColor5.id = "div-color-" + index + "_5";
        divColorDelete1.classList.add("div-color-delete");
        //divColorDelete1.id = "div-color-delete-" + index + "_1";
        divColorDelete1.innerHTML = "X";
        divColorDelete1.style.visibility = "collapse";
        divColor1Active.appendChild(divColorDelete1);
        divForColors.appendChild(divColor1Active);
        divForColors.appendChild(divColor2);
        divForColors.appendChild(divColor3);
        divForColors.appendChild(divColor4);
        divForColors.appendChild(divColor5);
        divColorDelete2.innerHTML = "X";
        divColorDelete2.classList.add("div-color-delete");
        divColorDelete3.innerHTML = "X";
        divColorDelete3.classList.add("div-color-delete");
        divColorDelete4.innerHTML = "X";
        divColorDelete4.classList.add("div-color-delete");
        divColorDelete5.innerHTML = "X";
        divColorDelete5.classList.add("div-color-delete");

        $(divColor1Active).click(function (event) {
            //changesColor("#div-color-3", 3);          
            if ($(divColor1Active).hasClass('div-color-active')) {
                $div.click();
                event.preventDefault();
                event.stopPropagation();
                var color = design.products.colors[prdc.ColorId];
                $("#prodFront").css("background-color", color.value);
                $("#prodBack").css("background-color", color.value);
                $image.css("background-color", color.value);
            }
        }).hover(function () {
            $(divColorDelete1).css("visibility", "visible");
        }).mouseleave(function () {
            $(divColorDelete1).css("visibility", "collapse");
        });
        $(divColor2).click(function (event) {
            //changesColor("#div-color-2", 2);
            if ($(divColor2).hasClass('div-color-active')) {
                $div.click();
                event.preventDefault();
                event.stopPropagation();
                var color = design.products.colors[prdc.SecondColorId];
                $("#prodFront").css("background-color", color.value);
                $("#prodBack").css("background-color", color.value);
                $image.css("background-color", color.value);
            }
        }).hover(function () {
            $(divColorDelete2).css("visibility", "visible");
        }).mouseleave(function () {
            $(divColorDelete2).css("visibility", "collapse");
        });
        $(divColor3).click(function (event) {
            //changesColor("#div-color-2", 2);
            if ($(divColor3).hasClass('div-color-active')) {
                $div.click();
                event.preventDefault();
                event.stopPropagation();
                var color = design.products.colors[prdc.ThirdColorId];
                $("#prodFront").css("background-color", color.value);
                $("#prodBack").css("background-color", color.value);
                $image.css("background-color", color.value);
            }
        }).hover(function () {
            $(divColorDelete3).css("visibility", "visible");
        }).mouseleave(function () {
            $(divColorDelete3).css("visibility", "collapse");
        });
        $(divColor4).click(function (event) {
            //changesColor("#div-color-2", 2);
            if ($(divColor4).hasClass('div-color-active')) {
                $div.click();
                event.preventDefault();
                event.stopPropagation();
                var color = design.products.colors[prdc.FourthColorId];
                $("#prodFront").css("background-color", color.value);
                $("#prodBack").css("background-color", color.value);
                $image.css("background-color", color.value);
            }
        }).hover(function () {
            $(divColorDelete4).css("visibility", "visible");
        }).mouseleave(function () {
            $(divColorDelete4).css("visibility", "collapse");
        });
        $(divColor5).click(function (event) {
            //changesColor("#div-color-2", 2);
            if ($(divColor5).hasClass('div-color-active')) {
                $div.click();
                event.preventDefault();
                event.stopPropagation();
                var color = design.products.colors[prdc.FifthColorId];
                $("#prodFront").css("background-color", color.value);
                $("#prodBack").css("background-color", color.value);
                $image.css("background-color", color.value);
            }
        }).hover(function () {
            $(divColorDelete5).css("visibility", "visible");
        }).mouseleave(function () {
            $(divColorDelete5).css("visibility", "collapse");
        });
        $(divColorDelete1).click(function () {
            if ($(divColor2).hasClass('div-color-active')) {
                $("#li_" + prdc.ProductId + "_" + prdc.ColorId).children("span").remove();

                prdc.ColorId = prdc.SecondColorId;
                prdc.SecondColorId = prdc.ThirdColorId;
                prdc.ThirdColorId = prdc.FourthColorId;
                prdc.FourthColorId = prdc.FifthColorId;
                prdc.FifthColorId = 0;

                $(divColor1Active).css("background-color", $(divColor2).css("background-color"));
                $(divColor1Active).removeClass().addClass($(divColor2).attr('class'));
                $(divColor2).css("background-color", $(divColor3).css("background-color"));
                $(divColor2).removeClass().addClass($(divColor3).attr('class'));
                if ($(divColor2).hasClass("div-color")) $(divColor2).children("div").remove();
                $(divColor3).css("background-color", $(divColor4).css("background-color"));
                $(divColor3).removeClass().addClass($(divColor4).attr('class'));
                if ($(divColor3).hasClass("div-color")) $(divColor3).children("div").remove();
                $(divColor4).css("background-color", $(divColor5).css("background-color"));
                $(divColor4).removeClass().addClass($(divColor5).attr('class'));
                if ($(divColor4).hasClass("div-color")) $(divColor4).children("div").remove();
                $(divColor5).css("background-color", "rgb(219, 219, 219)");
                $(divColor5).removeClass('div-color-active').addClass('div-color');
                if ($(divColor5).hasClass("div-color")) $(divColor5).children("div").remove();
            }
        });


        divCol.classList.add("clearfix");
        divCol.classList.add("control-group");
        divCol.classList.add("font-color-selection");
        divCol.style.width = "100px";
        divCol.style.marginBottom = "0px";
        divCol.style.marginTop = "5px";


        divColPick.classList.add("fake-input");
        divColPick.classList.add("color-picker");
        divColPick.classList.add("standard");
        divColPick.classList.add("designer-dropdown");
        divColPick.classList.add("designDrop");

        divSwatch.classList.add("swatch2");
        var $divSwatch = $(divSwatch);

        divColors.classList.add("colors");
        divColors.classList.add("shirt-colors");
        divColors.classList.add("containertip");
        divColors.style.top = "20px";
        divColors.style.left = "125px";

        ulAllColors.classList.add("all-colorsTwo");
        ulAllColors.classList.add("colors");

        var $ulAllColors = $(ulAllColors);

        divClear.classList.add("clearfix");

        var $divColPick = $(divColPick);

        divColors.appendChild(ulAllColors);
        divColPick.appendChild(divSwatch);
        divColPick.appendChild(divColors);
        divCol.appendChild(divColPick);
        divCol.appendChild(divClear);

        var $divColors = $(divColors);

        // хендлер на нажатие непосредственно на сам пикер для отображения выпадалки
        $divColPick.on('click', function (event) {
            //$div.click();
            if ($div.hasClass('design-active')) { } else {
                $div.click();
            }

            event.preventDefault();
            event.stopPropagation();

            $('.containertip--open').removeClass('containertip--open');
            $(this).parents(':first').find('.shirt-colors').addClass('containertip--open');
        });


        // Из общего списка цветов по айдишникам выбираем только те цвета которые соответствуют данному продукту
        var masColors = [];
        $.each(design.products.productsData, function (i, elemProd) {
            if (elemProd.id == document.getElementById("product").value) {
                $.each(design.products.colors, function (i, elem) {
                    if (elemProd.colors_available.indexOf(elem.id) >= 0) {
                        masColors.push(elem);
                    }
                });

            }
        });


        // Перебор всех существующих цветов
        $.each(masColors, function (i, color) {

            //var colorHtml = '<li data-value="' + color.id + ')" class="shirt-color-sample" title="' +
            //                  color.name + '" style="background-color:' + color.value + ';"></li>';
            var colorHtml;
            if (color.id == prdc.ColorId ||
                color.id == prdc.SecondColorId ||
                color.id == prdc.ThirdColorId ||
                color.id == prdc.FourthColorId ||
                color.id == prdc.FifthColorId) {
                colorHtml = '<li data-value="' + color.id + ')" class="shirt-color-sample" id="li_' + prdc.ProductId + '_' + color.id + '" title="' +
                                    color.name + '" style="background-color:' + color.value + ';"><span>✓</span></li>';
            } else {
                colorHtml = '<li data-value="' + color.id + ')" class="shirt-color-sample" id="li_' + prdc.ProductId + '_' + color.id + '" title="' +
                                    color.name + '" style="background-color:' + color.value + ';"></li>';
            }

            var $colorHtml = $(colorHtml);
            $colorHtml.click(function (event) {
                //$("#prodFront").css("background-color", color.value);
                //$("#prodBack").css("background-color", color.value);
                //event.preventDefault();
                //event.stopPropagation();
                //$image.css("background-color", color.value);

                var span = $colorHtml.find("span");
                if (prdc.ColorId > 0 && prdc.SecondColorId > 0 && prdc.ThirdColorId > 0 && prdc.FourthColorId > 0 && prdc.FifthColorId > 0) {
                    $('#max-color-for-product').modal('show');
                } else {
                    if (span.length == 0) {
                        var addSpan = document.createElement("span");
                        addSpan.innerHTML = '✓';

                        R = parseInt((cutHex(color.value)).substring(0, 2), 16);
                        G = parseInt((cutHex(color.value)).substring(2, 4), 16);
                        B = parseInt((cutHex(color.value)).substring(4, 6), 16);

                        function cutHex(h) { return (h.charAt(0) == "#") ? h.substring(1, 7) : h }
                        var spanColor = "rgb(" + (255 - R) + ", " + (255 - G) + ", " + (255 - B) + ")";

                        var $addSpan = $(addSpan);
                        addSpan.style.color = spanColor;
                        $colorHtml.append($addSpan);

                        $("#prodFront").css("background-color", color.value);
                        $("#prodBack").css("background-color", color.value);
                        event.preventDefault();
                        event.stopPropagation();
                        $image.css("background-color", color.value);
                        //$("#swatch2").css("background-color", color.value);
                        if ($(divColor2).hasClass('div-color')) {
                            $(divColor2).removeClass('div-color').addClass('div-color-active');
                            $(divColor2).css("background-color", color.value);
                            //divDelete.id = "div-color-delete-2";
                            $(divColorDelete2).click(function () {
                                $("#li_" + prdc.ProductId + "_" + prdc.SecondColorId).children("span").remove();
                                var color = design.products.colors[prdc.ColorId];

                                prdc.SecondColorId = prdc.ThirdColorId;
                                prdc.ThirdColorId = prdc.FourthColorId;
                                prdc.FourthColorId = prdc.FifthColorId;
                                prdc.FifthColorId = 0;

                                $(divColor2).css("background-color", $(divColor3).css("background-color"));
                                $(divColor2).removeClass().addClass($(divColor3).attr('class'));
                                if ($(divColor2).hasClass("div-color")) $(divColor2).children("div").remove();
                                $(divColor3).css("background-color", $(divColor4).css("background-color"));
                                $(divColor3).removeClass().addClass($(divColor4).attr('class'));
                                if ($(divColor3).hasClass("div-color")) $(divColor3).children("div").remove();
                                $(divColor4).css("background-color", $(divColor5).css("background-color"));
                                $(divColor4).removeClass().addClass($(divColor5).attr('class'));
                                if ($(divColor4).hasClass("div-color")) $(divColor4).children("div").remove();
                                $(divColor5).css("background-color", "rgb(219, 219, 219)");
                                $(divColor5).removeClass('div-color-active').addClass('div-color');
                                if ($(divColor5).hasClass("div-color")) $(divColor5).children("div").remove();

                                if ($(divColor3).hasClass("div-color")) {
                                    $("#prodFront").css("background-color", color.value);
                                    $("#prodBack").css("background-color", color.value);
                                    event.preventDefault();
                                    event.stopPropagation();
                                    $image.css("background-color", color.value);
                                }
                            });
                            $(divColorDelete2).css("visibility", "collapse");
                            $(divColor2).append($(divColorDelete2));
                            prdc.SecondColorId = parseInt(color.id);
                        } else
                            if ($(divColor3).hasClass('div-color')) {
                                $(divColor3).removeClass('div-color').addClass('div-color-active');
                                $(divColor3).css("background-color", color.value);
                                //divDelete.id = "div-color-delete-3";
                                //var $divDelete = $(divDelete);
                                $(divColorDelete3).click(function () {
                                    $("#li_" + prdc.ProductId + "_" + prdc.ThirdColorId).children("span").remove();
                                    var color = design.products.colors[prdc.SecondColorId];

                                    prdc.ThirdColorId = prdc.FourthColorId;
                                    prdc.FourthColorId = prdc.FifthColorId;
                                    prdc.FifthColorId = 0;

                                    $(divColor3).css("background-color", $(divColor4).css("background-color"));
                                    $(divColor3).removeClass().addClass($(divColor4).attr('class'));
                                    if ($(divColor3).hasClass("div-color")) $(divColor3).children("div").remove();
                                    $(divColor4).css("background-color", $(divColor5).css("background-color"));
                                    $(divColor4).removeClass().addClass($(divColor5).attr('class'));
                                    if ($(divColor4).hasClass("div-color")) $(divColor4).children("div").remove();
                                    $(divColor5).css("background-color", "rgb(219, 219, 219)");
                                    $(divColor5).removeClass('div-color-active').addClass('div-color');
                                    if ($(divColor5).hasClass("div-color")) $(divColor5).children("div").remove();

                                    if ($(divColor4).hasClass("div-color")) {
                                        $("#prodFront").css("background-color", color.value);
                                        $("#prodBack").css("background-color", color.value);
                                        event.preventDefault();
                                        event.stopPropagation();
                                        $image.css("background-color", color.value);
                                    }
                                });
                                $(divColorDelete3).css("visibility", "collapse");
                                $(divColor3).append($(divColorDelete3));
                                prdc.ThirdColorId = parseInt(color.id);
                            } else
                                if ($(divColor4).hasClass('div-color')) {
                                    $(divColor4).removeClass('div-color').addClass('div-color-active');
                                    $(divColor4).css("background-color", color.value);
                                    //divDelete.id = "div-color-delete-4";
                                    //var $divDelete = $(divDelete);
                                    $(divColorDelete4).click(function () {
                                        $("#li_" + prdc.ProductId + "_" + prdc.FourthColorId).children("span").remove();
                                        var color = design.products.colors[prdc.ThirdColorId];

                                        prdc.FourthColorId = prdc.FifthColorId;
                                        prdc.FifthColorId = 0;

                                        $(divColor4).css("background-color", $(divColor5).css("background-color"));
                                        $(divColor4).removeClass().addClass($(divColor5).attr('class'));
                                        if ($(divColor4).hasClass("div-color")) $(divColor4).children("div").remove();
                                        $(divColor5).css("background-color", "rgb(219, 219, 219)");
                                        $(divColor5).removeClass('div-color-active').addClass('div-color');
                                        if ($(divColor5).hasClass("div-color")) $(divColor5).children("div").remove();

                                        if ($(divColor5).hasClass("div-color")) {
                                            $("#prodFront").css("background-color", color.value);
                                            $("#prodBack").css("background-color", color.value);
                                            event.preventDefault();
                                            event.stopPropagation();
                                            $image.css("background-color", color.value);
                                        }
                                    });
                                    $(divColorDelete4).css("visibility", "collapse");
                                    $(divColor4).append($(divColorDelete4));
                                    prdc.FourthColorId = parseInt(color.id);
                                } else
                                    if ($(divColor5).hasClass('div-color')) {
                                        $(divColor5).removeClass('div-color').addClass('div-color-active');
                                        $(divColor5).css("background-color", color.value);
                                        //divDelete.id = "div-color-delete-5";
                                        //var $divDelete = $(divDelete);
                                        $(divColorDelete5).click(function () {
                                            $("#li_" + prdc.ProductId + "_" + prdc.FifthColorId).children("span").remove();
                                            var color = design.products.colors[prdc.FourthColorId];

                                            prdc.FifthColorId = 0;

                                            $(divColor5).css("background-color", "rgb(219, 219, 219)");
                                            $(divColor5).removeClass('div-color-active').addClass('div-color');
                                            if ($(divColor5).hasClass("div-color")) $(divColor5).children("div").remove();

                                            $("#prodFront").css("background-color", color.value);
                                            $("#prodBack").css("background-color", color.value);
                                            event.preventDefault();
                                            event.stopPropagation();
                                            $image.css("background-color", color.value);
                                        });
                                        $(divColorDelete5).css("visibility", "collapse");
                                        $(divColor5).append($(divColorDelete5));
                                        prdc.FifthColorId = parseInt(color.id);
                                    }
                    }
                }

                //$divSwatch.css("background-color", color.value);
                //prdc.ColorId = parseInt(color.id);

                //var product = design.products.productsData[prdc.ProductId];
                //var prices = product.prices;
                //for (var i = 0; i < prices.length; i++) {
                //    if (prices[i].color_id == prdc.ColorId) {
                //        prdc.BaseCost = prices[i].price;
                //    }
                //}
                //var calc = calculatePriceForNewProduct(window.frontColor, window.backColor, prdc.BaseCost);
                //prdc.BaseCost = calc[0];
                //var changes = prdc.Price - prdc.BaseCost.toFixed(2);
                //estimatedProfitChangeForManuProducts();
                //h4CostProfFloat.innerHTML = prdc.BaseCost.toFixed(2);
                //h4Profit.innerHTML = changes.toFixed(2);
                //$divColors.remove();
                $divColors.removeClass('containertip--open');
            }).hover(function () {
                //$image.css("background-color", color.value);
                //$divSwatch.css("background-color", color.value);
                var span = $colorHtml.find("span");
                if ($(divColor2).hasClass('div-color') && span.length == 0) {
                    $(divColor2).css("background-color", color.value);
                    $image.css("background-color", color.value);
                } else
                    if ($(divColor3).hasClass('div-color') && span.length == 0) {
                        $(divColor3).css("background-color", color.value);
                        $image.css("background-color", color.value);
                    } else
                        if ($(divColor4).hasClass('div-color') && span.length == 0) {
                            $(divColor4).css("background-color", color.value);
                            $image.css("background-color", color.value);
                        } else
                            if ($(divColor5).hasClass('div-color') && span.length == 0) {
                                $(divColor5).css("background-color", color.value);
                                $image.css("background-color", color.value);
                            }
            }).mouseleave(function () {
                if ($(divColor2).hasClass('div-color')) {
                    $(divColor2).css("background-color", "#dbdbdb");
                    $image.css("background-color", design.products.colors[prdc.ColorId].value);
                } else
                    if ($(divColor3).hasClass('div-color')) {
                        $(divColor3).css("background-color", "#dbdbdb");
                        $image.css("background-color", design.products.colors[prdc.SecondColorId].value);
                    } else
                        if ($(divColor4).hasClass('div-color')) {
                            $(divColor4).css("background-color", "#dbdbdb");
                            $image.css("background-color", design.products.colors[prdc.ThirdColorId].value);
                        } else
                            if ($(divColor5).hasClass('div-color')) {
                                $(divColor5).css("background-color", "#dbdbdb");
                                $image.css("background-color", design.products.colors[prdc.FourthColorId].value);
                            }
            });
            //Аппендим хтмли цветов с всеми евентами
            $ulAllColors.append($colorHtml);
        });

        //-----------------Color Picker------------------------------



        div.classList.add("block");
        div.classList.add("ssp_block");

        divThumb.classList.add("thumbnail_wrapper");

        var divColorPicAndMeta = document.createElement("div");
        divColorPicAndMeta.classList.add("ssp-ssp-metadata-and-col-pick");
        divColorPicAndMeta.appendChild(divMeta);


        divMeta.classList.add("ssp_metadata");

        divVcent.classList.add("ssp_vcent");
        divVcent.style.width = "100%"
        divVcent.style.marginLeft = "10px";

        text.classList.add("ssp_heading");
        text.style.color = "#44474d";
        text.style.fontWeight = "800";
        text.textContent = product.name;

        divDelete.classList.add("ssp_delete");

        divDelete.appendChild(imageDel);

        div.appendChild(inpHidd);

        divVcent.appendChild(text);
        divMeta.appendChild(divVcent);
        divThumb.appendChild(image);
        divColorPicAndMeta.appendChild(divCol);
        divColorPicAndMeta.appendChild(divForColors);
        div.appendChild(divThumb);
        div.appendChild(divColorPicAndMeta);
        div.appendChild(divAllPriceCalcul);
        //div.appendChild(divPricing);
        //div.appendChild(divProfit);
        div.appendChild(divDelete);
        //div.appendChild();
        div.style.height = "115px";


        var primDiv = document.getElementById("primary");
        primDiv.appendChild(div);


        $(imageDel).click(function () {
            var list = document.getElementById("item-options-dropdown-tees");
            var selectedOptionInList = list.selectedOptions[0];
            if (document.querySelectorAll(".ssp_block").length == 8) {
                document.getElementById("ui").style.display = "inline";
            }
            div.parentNode.removeChild(div);

            var products = app.state.products;
            var leng = products.length;
            var id = h4Price.id;
            var splitIndex = id.split('h4Price_');
            var count = parseInt(splitIndex[1]);
            var spliceIndex = count - 1;
            app.state.products.splice(spliceIndex, 1);
            app.state.isNegativeProfit[count - 1] = false;
            //app.state.products.pop(prdc);
            if (count == 0) {
                count = 1;
            }
            for (var k = count; k < leng; k++) {
                var input = document.getElementById("Input_" + (k + 1));
                var h4PriceOld = document.getElementById("h4Price_" + (k + 1));
                var h6PriceOld = document.getElementById("h6Price_" + (k + 1));
                var h4CostProfRmOld = document.getElementById("h4CostProfRm_" + (k + 1));
                var h4CostSaleOld = document.getElementById("h4CostSale_" + (k + 1));
                var h4CostProfTextOld = document.getElementById("h4CostProfText_" + (k + 1));
                var h6CostOld = document.getElementById("h6Cost_" + (k + 1));
                var divProfitCalculOld = document.getElementById("divProfitCalcul_" + (k + 1));
                var h4ProfSaleOld = document.getElementById("h4ProfSale_" + (k + 1));

                input.id = "Input_" + k;
                h4PriceOld.id = "h4Price_" + k;
                h6PriceOld.id = "h6Price_" + k;
                h4CostProfRmOld.id = "h4CostProfRm_" + k;
                h4CostSaleOld.id = "h4CostSale_" + k;
                h4CostProfTextOld.id = "h4CostProfText_" + k;
                h6CostOld.id = "h6Cost_" + k;
                divProfitCalculOld.id = "divProfitCalcul_" + k;
                h4ProfSaleOld.id = "h4ProfSale_" + k;
            }


            if (app.state.products.length > 1) {
                estimatedProfitChangeForManuProducts();
            } else {
                var slider = document.getElementById('trackbar');
                window.count = parseInt(slider.noUiSlider.get());
                calculatePrice(window.frontColor, window.backColor)
                estimatedProfitChange();
            }
            $("#first-product").click();

            var list = document.getElementById("item-options-dropdown-tees");
            var listProd = document.getElementById("product");

            listProd.innerHTML = "";

            //var deletedProdCatIdx = -1;
            //$.each(design.products.categoriesList, function (i, category) {
            //    $.each(category.products, function (z, element) {
            //        if (element == prdc.ProductId) {
            //            index = i;
            //        }
            //    })
            //})
            //var isInList = false;
            //$.each(list.childNodes, function (i, node) {
            //    if ((node.id == index) && (i != 0)) {
            //        isInList = true;
            //    }
            //})

            //if (design.products.categoriesList[index].products.length == 1 && !(isInList)) {
            //    var option = document.createElement("option");
            //    option.value = index;
            //    option.id = index;
            //    option.innerHTML = design.products.categoriesList[index].name;
            //    list.appendChild(option);
            //}
            //$.each(design.products.categoriesList[list[list.selectedIndex].id].products, function (i, element) {
            //    var cnt = 0;
            //    $.each(app.state.products, function (j, el) {
            //        if (el.ProductId == element) { cnt++; }
            //    });

            //    if (cnt == 0) {
            //        var option = document.createElement("option");
            //        option.value = element;
            //        option.id = element;
            //        option.innerHTML = design.products.productsData[element].name;
            //        listProd.appendChild(option);
            //    }
            //});
            var mas = [];
            var resultMas = [];
            var firstCatElemsInStateProdIds = [];
            while (listProd.childNodes.length > 0) {
                listProd.removeChild(listProd.childNodes[0]);
            };
            while (list.childNodes.length > 0) {
                list.removeChild(list.childNodes[0]);
            };

            $.each(design.products.categoriesList, function (i, category) {
                if ((this.products.length == 1) && (this.products[0] == app.state.currentProduct.ProductId)) {

                } else {
                    mas.push(category);
                }

                $.each(design.products.categoriesList[i].products, function (z, element) {
                    if (element == app.state.currentProduct.ProductId) {
                        index = i;
                    }
                })
            });
            $.each(mas, function (i, element) {
                var theOnlyInAdditionalProducts = false;
                if (element.products.length == 1) {
                    $.each(app.state.products, function (i, stateProd) {
                        if (stateProd.id == element.products[0]) {
                            theOnlyInAdditionalProducts = true;
                        }
                    });
                }
                if (element.products.length == 1 && theOnlyInAdditionalProducts) {


                } else {
                    var koeff = 0;
                    $.each(element.products, function (k, prod) {
                        $.each(app.state.products, function (ii, stateProd) {
                            if (stateProd.ProductId == prod) {
                                koeff++;

                            }
                        });
                    });
                    if (!(koeff == element.products.length)) {
                        resultMas.push(element);
                    }


                }

            });
            $.each(resultMas[0].products, function (k, prod) {
                $.each(app.state.products, function (ii, stateProd) {
                    if (prod == stateProd.ProductId) {
                        firstCatElemsInStateProdIds.push(k);
                    }
                });
            });
            $.each(resultMas, function (i, element) {
                var option = document.createElement("option");
                option.value = design.products.categoriesList.indexOf(element);
                option.id = design.products.categoriesList.indexOf(element);
                option.innerHTML = resultMas[i].name;
                list.appendChild(option);

            })
            $.each(resultMas[0].products, function (i, element) {
                if ((element != app.state.currentProduct.ProductId) && (firstCatElemsInStateProdIds.indexOf(i) < 0)) {
                    var option = document.createElement("option");
                    option.value = element;
                    option.id = element;
                    option.innerHTML = design.products.productsData[element].name;
                    listProd.appendChild(option);
                }

            });
            $.each(list.childNodes, function (i, element) {
                if (element.value == selectedOptionInList.value) {
                    list.selectedIndex = i;
                }

            })
            $('#item-options-dropdown-tees').trigger('change');
        });

        globalPrdc = prdc;
        app.state.products.push(prdc);
        $div.click();
        estimatedProfitChangeForManuProducts();

        var list = document.getElementById("item-options-dropdown-tees");
        var listProd = document.getElementById("product");

        listProd.innerHTML = "";

        //if (listProd.childNodes.length == 0) {
        //    list.removeChild(list.childNodes[list[list.selectedIndex].id]);
        //    $.each(design.products.categoriesList[list[list.selectedIndex].id].products, function (i, element) {
        //        var cnt = 0;
        //        $.each(app.state.products, function (j, el) {
        //            if (el.ProductId == element) { cnt++; }
        //        });

        //        if (cnt == 0) {
        //            var option = document.createElement("option");
        //            option.value = element;
        //            option.id = element;
        //            option.innerHTML = design.products.productsData[element].name;
        //            listProd.appendChild(option);
        //        }
        //    });

        //} else {
            $.each(design.products.categoriesList[list[list.selectedIndex].id].products, function (i, element) {
                var cnt = 0;
                $.each(app.state.products, function (j, el) {
                    if (el.ProductId == element) { cnt++; }
                });

                if (cnt == 0) {
                    var option = document.createElement("option");
                    option.value = element;
                    option.id = element;
                    option.innerHTML = design.products.productsData[element].name;
                    listProd.appendChild(option);
                }
            });
            if (listProd.childNodes.length == 0) {
                list.removeChild(list.childNodes[list.selectedIndex]);
                $.each(design.products.categoriesList[list[list.selectedIndex].id].products, function (i, element) {
                    var cnt = 0;
                    $.each(app.state.products, function (j, el) {
                        if (el.ProductId == element) { cnt++; }
                    });

                    if (cnt == 0) {
                        var option = document.createElement("option");
                        option.value = element;
                        option.id = element;
                        option.innerHTML = design.products.productsData[element].name;
                        listProd.appendChild(option);
                    }
                });
                }
        //}
    });




    // Изменение количества в инпуте #trackBarValue

    $("#trackBarValue").on({
        change: function () {
            var slider = document.getElementById('trackbar');
            slider.noUiSlider.set(parseInt(document.getElementById('trackBarValue').value));
            onChangeValueForTrackBar();
        },
        keydown: function (e) {
            if (e.which == 13) {
                var slider = document.getElementById('trackbar');
                slider.noUiSlider.set(parseInt(document.getElementById('trackBarValue').value));
                onChangeValueForTrackBar();
            }
        }
    });

    $("#swatch2").click(function () {
        if (app.state.currentProduct.id != document.querySelector('.design-active').childNodes[0].id) {
            $("#first-product").click();
        }
    });


    //  ---- Add colors for products
    $("#div-color-1").click(function (event) {
        changesColor("#div-color-1", 1);
        event.preventDefault();
        event.stopPropagation();
    }).hover(function () {
        $("#div-color-delete-1").css("visibility", "visible");
    }).mouseleave(function () {
        $("#div-color-delete-1").css("visibility", "collapse");
    });

    $("#div-color-2").click(function (event) {
        changesColor("#div-color-2", 2);
        event.preventDefault();
        event.stopPropagation();
    }).hover(function () {
        $("#div-color-delete-2").css("visibility", "visible");
    }).mouseleave(function () {
        $("#div-color-delete-2").css("visibility", "collapse");
    });

    $("#div-color-3").click(function (event) {
        changesColor("#div-color-3", 3);
        event.preventDefault();
        event.stopPropagation();
    }).hover(function () {
        $("#div-color-delete-3").css("visibility", "visible");
    }).mouseleave(function () {
        $("#div-color-delete-3").css("visibility", "collapse");
    });

    $("#div-color-4").click(function (event) {
        changesColor("#div-color-4", 4);
        event.preventDefault();
        event.stopPropagation();
    }).hover(function () {
        $("#div-color-delete-4").css("visibility", "visible");
    }).mouseleave(function () {
        $("#div-color-delete-4").css("visibility", "collapse");
    });

    $("#div-color-5").click(function (event) {
        changesColor("#div-color-5", 5);
        event.preventDefault();
        event.stopPropagation();
    }).hover(function () {
        $("#div-color-delete-5").css("visibility", "visible");
    }).mouseleave(function () {
        $("#div-color-delete-5").css("visibility", "collapse");
    });

    $("#div-color-delete-1").click(function (event) {
        if ($("#div-color-2").hasClass('div-color-active')) {
            deleteColor(1);
            event.preventDefault();
            event.stopPropagation();
        }
    });

    // --------------------------------------------------------

    $("#first-product").click(function (event) {
        var newColor = app.state.color;
        var product = app.state.currentProduct;
        var src = assetsUrls.products + 'product_type_' + product.ProductId + '_front_small.png';
        design.save(function (data) {
            var srcFront = assetsUrls.products + 'product_type_' + product.ProductId + '_front.png';
            var srcBack = assetsUrls.products + 'product_type_' + product.ProductId + '_back.png';
            var imageData = app.state.getImage();
            var front = {
                top: imageData['printable_front_top'],
                left: imageData['printable_front_left']
            };
            var back = {
                top: imageData['printable_front_top'],
                left: imageData['printable_front_left']
            };

            $('#prodFront').attr('src', srcFront).css('background-color', newColor.value);
            $('#prodBack').attr('src', srcBack).css('background-color', newColor.value);
            design.item.unselect();
            while (document.querySelector('#stp2frArea') != null) {
                document.querySelector('#step-2-front').removeChild(document.querySelector('#stp2frArea'));
            }
            $('#view-front-design-area').clone().attr("id", "stp2frArea").css({ "border": "none" }, { "margin-left": "2px" }, { "overflow": "hidden !important" }).appendTo('#step-2-front');
            document.querySelector('#stp2frArea').removeChild(document.querySelector('#stp2frArea').childNodes[5]);
            $('#stp2frArea').css({ "overflow": "hidden" });

            while (document.querySelector('#stp2BackArea') != null) {
                document.querySelector('#step-2-back').removeChild(document.querySelector('#stp2BackArea'));
            }
            $('#view-back-design-area').clone().attr("id", "stp2BackArea").css({ "border": "none" }, { "margin-left": "2px" }, { "overflow": "hidden !important" }).appendTo('#step-2-back');
            document.querySelector('#stp2BackArea').removeChild(document.querySelector('#stp2BackArea').childNodes[5]);
            $('#stp2BackArea').css({ "overflow": "hidden" });
        });

        var el = $('#primary')[0];
        var newSize = $(el).find('.block.ssp_block');

        if (newSize.hasClass('design-active')) {
            newSize.removeClass('design-active');
        }

        $("#first-product").addClass('design-active');
        event.preventDefault();
        event.stopPropagation();
    });

    $("#profSale").on({
        change: profitSale,
        keyup: function () {
            var price = $("#profSale").val();
            if (isNaN(price)) {
                price = price.substring(0, price.length - 1);
                $("#profSale").val(price);
            }
        },
        keydown: function (e) {
            if (e.which == 13) {
                profitSale();
                var price = $("#profSale").val();

                document.querySelector('#profSale').value = Number($("#profSale").val()).toFixed(2);

            }
        }
    });

    document.addEventListener('keydown', function (e) {
        if (e.which == 46) {
            if (design.item.get() != null) {
                item = design.item.get();
                design.item.remove(item[0].childNodes[1]);
            }
        }
    }, false);

    $('#preloader').animate({ opacity: 1, top: '200%' }, 100,
				function () { // пoсле aнимaции
				    // $(this).css('display', 'none'); 
				}
			);

    var elem = document.querySelector(".noUi-pips")
    elem.addEventListener("click", function (event) {
        if (event.target.className == "noUi-value noUi-value-horizontal noUi-value-large") {
            var slider = document.getElementById('trackbar');
            var val = parseInt(event.target.innerHTML);
            slider.noUiSlider.set(val);
            onChangeValueForTrackBar();
        }
    });
}

function setDesign() {
    
        var src = assetsUrls.products + 'product_type_' + app.state.product.id + '_front_small.png';
        $('#first-product .thumbnail_wrapper img').attr('src', src).css('background-color', app.state.color.value);
        $('#first-product #div-color-1').css('background-color', app.state.color.value);
        design.save(function (data) {
            var srcFront = assetsUrls.products + 'product_type_' + app.state.product.id + '_front.png';
            var srcBack = assetsUrls.products + 'product_type_' + app.state.product.id + '_back.png';
            var imageData = app.state.getImage();
            var front = {
                top: imageData['printable_front_top'],
                left: imageData['printable_front_left']
            };
            var back = {
                top: imageData['printable_front_top'],
                left: imageData['printable_front_left']
            };

            $('#prodFront, #prodFront3').attr('src', srcFront).css('background-color', app.state.color.value);
            $('#prodBack, #prodBack3').attr('src', srcBack).css('background-color', app.state.color.value);
            design.item.unselect();
            while (document.querySelector('#stp2frArea') != null) {
                document.querySelector('#step-2-front').removeChild(document.querySelector('#stp2frArea'));
            }
            $('#view-front-design-area').clone().attr("id", "stp2frArea").css({ "border": "none" }, { "margin-left": "2px" }, { "overflow": "hidden !important" }).appendTo('#step-2-front');
            document.querySelector('#stp2frArea').removeChild(document.querySelector('#stp2frArea').childNodes[5]);
            $('#stp2frArea').css({ "overflow": "hidden" });

            while (document.querySelector('#stp2BackArea') != null) {
                document.querySelector('#step-2-back').removeChild(document.querySelector('#stp2BackArea'));
            }
            $('#view-back-design-area').clone().attr("id", "stp2BackArea").css({ "border": "none" }, { "margin-left": "2px" }, { "overflow": "hidden !important" }).appendTo('#step-2-back');
            document.querySelector('#stp2BackArea').removeChild(document.querySelector('#stp2BackArea').childNodes[5]);
            $('#stp2BackArea').css({ "overflow": "hidden" });
        });
    
        while (document.querySelector('#stp3frArea') != null) {
            document.querySelector('#step-3-front').removeChild(document.querySelector('#stp3frArea'));
        }
        $('#view-front-design-area').clone().attr("id", "stp3frArea").css({ "border": "none" }, { "margin-left": "2px" }, { "overflow": "hidden !important" }).appendTo('#step-3-front');
        document.querySelector('#stp3frArea').removeChild(document.querySelector('#stp3frArea').childNodes[5]);
        $('#stp3frArea').css({ "overflow": "hidden" });

        while (document.querySelector('#stp3BackArea') != null) {
            document.querySelector('#step-3-back').removeChild(document.querySelector('#stp3BackArea'));
        }
        $('#view-back-design-area').clone().attr("id", "stp3BackArea").css({ "border": "none" }, { "margin-left": "2px" }, { "overflow": "hidden !important" }).appendTo('#step-3-back');
        document.querySelector('#stp3BackArea').removeChild(document.querySelector('#stp3BackArea').childNodes[5]);
        $('#stp3BackArea').css({ "overflow": "hidden" });
    
}

function initProducts() {
    var list = document.getElementById("item-options-dropdown-tees");
    var listProd = document.getElementById("product");
    var mas = [];
    if (app.state.products.length < 2) {
        //Если лист категорий пустой то мы его инициализируем
        if (list.value == "") {
            var index = 0;
            $.each(design.products.categoriesList, function (i, category) {
                if ((this.products.length == 1) && (this.products[0] == app.state.currentProduct.ProductId)) {

                } else {
                    mas.push(category);
                }

                $.each(design.products.categoriesList[i].products, function (z, element) {
                    if (element == app.state.currentProduct.ProductId) {
                        index = i;
                    }
                })
            });
            $.each(mas, function (i, element) {
                
                    var option = document.createElement("option");
                    option.value = design.products.categoriesList.indexOf(element);
                    option.id = design.products.categoriesList.indexOf(element);
                    option.innerHTML = mas[i].name;
                    list.appendChild(option);
                
            })
            //Если лист продуктов пустой то мы его инициализируем
            //if (listProd.value == "") {
            //    if (index > 0) {
            //        index = 0;
            //    } else {
            //        index = 1;
            //    }
            if(mas.length != 0){
                $.each(mas[0].products, function (i, element) {
                    if (element != app.state.currentProduct.ProductId) {
                        var option = document.createElement("option");
                        option.value = element;
                        option.id = element;
                        option.innerHTML = design.products.productsData[element].name;
                        listProd.appendChild(option);
                    }

                });
            }
            //$.each(design.products.productsData, function (i, el) {
            //    if (app.state.currentProduct.ProductId != el.id) {
            //        //Если список продуктов по первой категории содержит айдишники из общего списка продуктов то мы вытягиваем их в наш лист
            //        if (index > 0) {
            //            index = 0;
            //        } else {
            //            index = 1;
            //        }
            //        if (design.products.categoriesList[index].products.indexOf(el.id) != -1) {
            //            var option = document.createElement("option");
            //            option.value = i;
            //            option.id = i;
            //            option.innerHTML = el.name;
            //            listProd.appendChild(option);
            //        }
            //    }
            //});
        
    
            $(list).change(function () {
                listProd.innerHTML = "";

                $.each(design.products.categoriesList[list[list.selectedIndex].id].products, function (i, element) {
                    var cnt = 0;
                    $.each(app.state.products, function (j, el) {
                        if (el.ProductId == element) { cnt++; }
                    });

                    if (cnt == 0) {
                        var option = document.createElement("option");
                        option.value = element;
                        option.id = element;
                        option.innerHTML = design.products.productsData[element].name;
                        listProd.appendChild(option);
                    }
                });
                //if (listProd.childNodes.length == 0) {
                //    list.removeChild(list.childNodes[list[list.selectedIndex].id]);
                //    $.each(design.products.categoriesList[list[list.selectedIndex].id].products, function (i, element) {
                //        var cnt = 0;
                //        $.each(app.state.products, function (j, el) {
                //            if (el.ProductId == element) { cnt++; }
                //        });

                //        if (cnt == 0) {
                //            var option = document.createElement("option");
                //            option.value = element;
                //            option.id = element;
                //            option.innerHTML = design.products.productsData[element].name;
                //            listProd.appendChild(option);
                //        }
                //    });
                //}

            });
        } else {
            $.each(design.products.categoriesList, function (i) {
                if (((this.products.length == 1) && (this.products[0] == app.state.currentProduct.ProductId))) {
                    index = i;
                }

            });
            while (listProd.childNodes.length > 0) {
                listProd.removeChild(listProd.childNodes[0]);
            };
            while (list.childNodes.length > 0) {
                list.removeChild(list.childNodes[0]);
            };

            $.each(design.products.categoriesList, function (i, category) {
                if ((this.products.length == 1) && (this.products[0] == app.state.currentProduct.ProductId)) {

                } else {
                    mas.push(category);
                }

                $.each(design.products.categoriesList[i].products, function (z, element) {
                    if (element == app.state.currentProduct.ProductId) {
                        index = i;
                    }
                })
            });
            $.each(mas, function (i, element) {
                var option = document.createElement("option");
                option.value = design.products.categoriesList.indexOf(element);
                option.id = design.products.categoriesList.indexOf(element);
                option.innerHTML = mas[i].name;
                list.appendChild(option);

            })
            $.each(mas[0].products, function (i, element) {
                if (element != app.state.currentProduct.ProductId) {
                    var option = document.createElement("option");
                    option.value = element;
                    option.id = element;
                    option.innerHTML = design.products.productsData[element].name;
                    listProd.appendChild(option);
                }

            });
        }
    } else {
        //когда есть доп продукты
        var resultMas = [];
        var firstCatElemsInStateProdIds = [];
        while (listProd.childNodes.length > 0) {
            listProd.removeChild(listProd.childNodes[0]);
        };
        while (list.childNodes.length > 0) {
            list.removeChild(list.childNodes[0]);
        };

        $.each(design.products.categoriesList, function (i, category) {
            if ((this.products.length == 1) && (this.products[0] == app.state.currentProduct.ProductId)) {

            } else {
                mas.push(category);
            }

            $.each(design.products.categoriesList[i].products, function (z, element) {
                if (element == app.state.currentProduct.ProductId) {
                    index = i;
                }
            })
        });
        $.each(mas, function (i, element) {
            var theOnlyInAdditionalProducts = false;
            if (element.products.length == 1) {
                $.each(app.state.products, function (i, stateProd) {
                    if (stateProd.id == element.products[0]) {
                        theOnlyInAdditionalProducts = true;
                    }
                });
            }
            if (element.products.length == 1 && theOnlyInAdditionalProducts) {
                
                
            } else {
                var koeff = 0;
                $.each(element.products, function (k, prod) {
                    $.each(app.state.products, function (ii, stateProd) {
                        if (stateProd.ProductId == prod) {
                            koeff++;
                            
                        }
                    });
                });
                if (!(koeff == element.products.length)) {
                    resultMas.push(element);
                }


            }

        });
        $.each(resultMas[0].products, function (k, prod) {
            $.each(app.state.products, function (ii, stateProd) {
                if (prod == stateProd.ProductId) {
                    firstCatElemsInStateProdIds.push(k);
                }
            });
        });
        $.each(resultMas, function (i, element) {
            var option = document.createElement("option");
            option.value = design.products.categoriesList.indexOf(element);
            option.id = design.products.categoriesList.indexOf(element);
            option.innerHTML = resultMas[i].name;
            list.appendChild(option);

        })
        $.each(resultMas[0].products, function (i, element) {
            if ((element != app.state.currentProduct.ProductId) && (firstCatElemsInStateProdIds.indexOf(i)<0)) {
                var option = document.createElement("option");
                option.value = element;
                option.id = element;
                option.innerHTML = design.products.productsData[element].name;
                listProd.appendChild(option);
            }

        });

    }
}


function profitSale() {
    //window.count = parseInt(document.getElementById('trackBarValue').value);
    var slider = document.getElementById('trackbar');
    window.count = parseInt(slider.noUiSlider.get());
    var $val = document.getElementById("profSale").value.replace(',', '.');
    var selPrice = parseFloat(String($val).match(/-?\d+(?:\.\d+)?/g, '') || 0, 10).toFixed(2);
    var price = (selPrice - window.nowPrice).toFixed(2);


    if (selPrice < window.nowPrice) {
        //$("#mainH4").html("RM " + window.nowPrice + " minimum");
        //$("#mainH4").css('color', '#ff0000');
        updateMinimum(price);
        app.state.currentProduct.Price = selPrice;
        window.sellingPrice = app.state.currentProduct.Price;
        $("#total_profit").html(window.currency + " 0+");
        minimumGoal();
    } else {
        //$("#mainH4").html($price);
        //$("#mainH4").css('color', '#ff4f00');
        updateMinimum(price);
        app.state.currentProduct.Price = selPrice;
        window.sellingPrice = app.state.currentProduct.Price;
        if (app.state.products.length > 1) {
            estimatedProfitChangeForManuProducts()
        } else {
            estimatedProfitChange();
        }
    }
}

function colorInit() {

    var arrColors = [];
    $.each(design.products.colors, function (i, color) {
        if (app.state.product.colors_available.indexOf(color.id) >= 0) {
            arrColors.push(color);
        }
    });
    app.state.product.colors_available_obj = arrColors;


    var elem = $("#allColorsTwo");
    if (elem.html() === "") {
        $.each(arrColors, function (i, color) {
            //<span>✓</span>

            var colorHtml;
            if (color.id == app.state.currentProduct.ColorId ||
                color.id == app.state.currentProduct.SecondColorId ||
                color.id == app.state.currentProduct.ThirdColorId ||
                color.id == app.state.currentProduct.FourthColorId ||
                color.id == app.state.currentProduct.FifthColorId) {
                colorHtml = '<li data-value="' + color.id + ')" class="shirt-color-sample" id="li_' + app.state.currentProduct.ProductId + '_' + color.id + '" title="' +
                                    color.name + '" style="background-color:' + color.value + ';"><span>✓</span></li>';
            } else {
                colorHtml = '<li data-value="' + color.id + ')" class="shirt-color-sample" id="li_' + app.state.currentProduct.ProductId + '_' + color.id + '" title="' +
                                    color.name + '" style="background-color:' + color.value + ';"></li>';
            }
            var $colorHtml = $(colorHtml);
            $colorHtml.click(function () {
                var span = $colorHtml.find("span");
                if (app.state.currentProduct.ColorId > 0 && app.state.currentProduct.SecondColorId > 0 && app.state.currentProduct.ThirdColorId > 0 && app.state.currentProduct.FourthColorId > 0 && app.state.currentProduct.FifthColorId > 0) {
                    $('#max-color-for-product').modal('show');
                } else {
                    if (span.length == 0) {
                        var addSpan = document.createElement("span");
                        addSpan.innerHTML = '✓';

                        R = parseInt((cutHex(color.value)).substring(0, 2), 16);
                        G = parseInt((cutHex(color.value)).substring(2, 4), 16);
                        B = parseInt((cutHex(color.value)).substring(4, 6), 16);

                        var spanColor = "rgb(" + (255 - R) + ", " + (255 - G) + ", " + (255 - B) + ")";

                        var $addSpan = $(addSpan);
                        addSpan.style.color = spanColor;
                        $colorHtml.append($addSpan);

                        var divDelete = document.createElement("div");
                        divDelete.innerHTML = "X";
                        divDelete.classList.add("div-color-delete");

                        $("#minImg").css("background-color", color.value);
                        //$("#swatch2").css("background-color", color.value);
                        if ($("#div-color-2").hasClass('div-color')) {
                            $("#div-color-2").removeClass('div-color').addClass('div-color-active');
                            $("#div-color-2").css("background-color", color.value);
                            divDelete.id = "div-color-delete-2";
                            var $divDelete = $(divDelete);
                            $divDelete.click(function () {
                                deleteColor(2);
                            });
                            $divDelete.css("visibility", "collapse");
                            $("#div-color-2").append($divDelete);
                            app.state.currentProduct.SecondColorId = parseInt(color.id);
                        } else
                            if ($("#div-color-3").hasClass('div-color')) {
                                $("#div-color-3").removeClass('div-color').addClass('div-color-active');
                                $("#div-color-3").css("background-color", color.value);
                                divDelete.id = "div-color-delete-3";
                                var $divDelete = $(divDelete);
                                $divDelete.click(function () {
                                    deleteColor(3);
                                });
                                $divDelete.css("visibility", "collapse");
                                $("#div-color-3").append($divDelete);
                                app.state.currentProduct.ThirdColorId = parseInt(color.id);
                            } else
                                if ($("#div-color-4").hasClass('div-color')) {
                                    $("#div-color-4").removeClass('div-color').addClass('div-color-active');
                                    $("#div-color-4").css("background-color", color.value);
                                    divDelete.id = "div-color-delete-4";
                                    var $divDelete = $(divDelete);
                                    $divDelete.click(function () {
                                        deleteColor(4);
                                    });
                                    $divDelete.css("visibility", "collapse");
                                    $("#div-color-4").append($divDelete);
                                    app.state.currentProduct.FourthColorId = parseInt(color.id);
                                } else
                                    if ($("#div-color-5").hasClass('div-color')) {
                                        $("#div-color-5").removeClass('div-color').addClass('div-color-active');
                                        $("#div-color-5").css("background-color", color.value);
                                        divDelete.id = "div-color-delete-5";
                                        var $divDelete = $(divDelete);
                                        $divDelete.click(function () {
                                            deleteColor(5);
                                        });
                                        $divDelete.css("visibility", "collapse");
                                        $("#div-color-5").append($divDelete);
                                        app.state.currentProduct.FifthColorId = parseInt(color.id);
                                    }
                        $("#prodFront").css("background-color", color.value);
                        $("#prodBack").css("background-color", color.value);
                        //$("#prodFront3").css("background-color", color.value);
                        //$("#prodBack3").css("background-color", color.value);
                        //$(".product_images").css("background-color", color.value);
                        $('.containertip--open').removeClass('containertip--open');
                        //design.products.changeColor(color);

                        //app.state.currentProduct.ColorId = parseInt(color.id);

                        //app.state.currentProduct.ColorId = parseInt(color.id);

                        //var product = design.products.productsData[app.state.currentProduct.ProductId];
                        //var prices = product.prices;
                        //for (var i = 0; i < prices.length; i++) {
                        //    if (prices[i].color_id == app.state.currentProduct.ColorId) {
                        //        app.state.currentProduct.BaseCost = prices[i].price;
                        //    }
                        //}
                        //window.costOfMaterial = app.state.currentProduct.BaseCost;
                        //var calc = calculatePriceForNewProduct(window.frontColor, window.backColor, app.state.currentProduct.BaseCost);
                        //app.state.currentProduct.BaseCost = calc[0];
                        //var changes = app.state.currentProduct.Price - app.state.currentProduct.BaseCost;
                        ////$("#mainH4").html("RM " + parseFloat(chenges.toFixed(2)) + " Profit per sale");
                        //window.nowPrice = app.state.currentProduct.BaseCost;
                        //updateMinimum(changes.toFixed(2));
                        //if (window.nowPrice < window.sellingPrice) {
                        //    if (app.state.products.length > 1) {
                        //        estimatedProfitChangeForManuProducts()
                        //    } else {
                        //        estimatedProfitChange();
                        //    }
                        //}
                        //document.getElementById("price_preview").innerText = "RM " + window.nowPrice.toFixed(2);
                        //$(document.getElementById("price_preview")).text("RM " + window.nowPrice.toFixed(2));
                    }
                }
            }).hover(function () {
                //$("#swatch2").css("background-color", color.value);
                var span = $colorHtml.find("span");
                if ($("#div-color-2").hasClass('div-color') && span.length == 0) {
                    $("#div-color-2").css("background-color", color.value);
                    $("#minImg").css("background-color", color.value);
                } else
                    if ($("#div-color-3").hasClass('div-color') && span.length == 0) {
                        $("#div-color-3").css("background-color", color.value);
                        $("#minImg").css("background-color", color.value);
                    } else
                        if ($("#div-color-4").hasClass('div-color') && span.length == 0) {
                            $("#div-color-4").css("background-color", color.value);
                            $("#minImg").css("background-color", color.value);
                        } else
                            if ($("#div-color-5").hasClass('div-color') && span.length == 0) {
                                $("#div-color-5").css("background-color", color.value);
                                $("#minImg").css("background-color", color.value);
                            }
            }).mouseleave(function () {
                var oldColorForProducts;
                if ($("#div-color-2").hasClass('div-color')) {
                    $("#div-color-2").css("background-color", "#dbdbdb");
                    $("#minImg").css("background-color", design.products.colors[app.state.currentProduct.ColorId].value);
                } else
                    if ($("#div-color-3").hasClass('div-color')) {
                        $("#div-color-3").css("background-color", "#dbdbdb");
                        $("#minImg").css("background-color", design.products.colors[app.state.currentProduct.SecondColorId].value);
                    } else
                        if ($("#div-color-4").hasClass('div-color')) {
                            $("#div-color-4").css("background-color", "#dbdbdb");
                            $("#minImg").css("background-color", design.products.colors[app.state.currentProduct.ThirdColorId].value);
                        } else
                            if ($("#div-color-5").hasClass('div-color')) {
                                $("#div-color-5").css("background-color", "#dbdbdb");
                                $("#minImg").css("background-color", design.products.colors[app.state.currentProduct.FourthColorId].value);
                            }
            });
            elem.append($colorHtml);
        })
    }
}

function NextPage() {
    if (app.state.pos === -1) {
        Design();
    }
    else if (app.state.pos === -2) {
        Goal();
    }
    else
        window.location.assign("/");
}

function Design() {


    slideTo(1);
}

function Goal() {
    colorInit();
    initProducts();
    if (app.state.products.length < 1) {
        app.state.products.push(app.state.currentProduct);
    }
    if (parseInt(app.state.getUsedColorsCountFront()) == parseInt("0") && parseInt(app.state.getUsedColorsCountBack()) == parseInt("0")) {
        $('#no-content-error').modal('show');
    } else {
        var prod = design.products.productsData[app.state.currentProduct.ProductId];
        document.getElementById("productName").innerHTML = prod.name;

        if (app.state.color.id == app.state.currentProduct.SecondColorId) {
            $("#li_" + app.state.currentProduct.ProductId + "_" + app.state.color.id).children("span").remove();
            deleteColor(2, true);
        } else if (app.state.color.id == app.state.currentProduct.ThirdColorId) {
            $("#li_" + app.state.currentProduct.ProductId + "_" + app.state.color.id).children("span").remove();
            deleteColor(3, true);
        } else if (app.state.color.id == app.state.currentProduct.FourthColorId) {
            $("#li_" + app.state.currentProduct.ProductId + "_" + app.state.color.id).children("span").remove();
            deleteColor(4, true);
        } else if (app.state.color.id == app.state.currentProduct.FifthColorId) {
            $("#li_" + app.state.currentProduct.ProductId + "_" + app.state.color.id).children("span").remove();
            deleteColor(5, true);
        }

        slideTo(2);
        setPriceInGoalFromDesign();
        profitSale();
    }

    $("#first-product").click();
}

var slideSteps = ['design', 'goal', 'description'];
var slideTimeout;

function slideTo(slideNumber) {
    if (app.state.getUsedColorsCount('front') > maxDesignColors || app.state.getUsedColorsCount('back') > maxDesignColors) {
        $('#too-many-colors-error').modal('show');
        return;
    }
    if (app.state.getUsedColorsCountFront() < 1 && app.state.getUsedColorsCountBack() < 1) {
        $('#no-content-error').modal('show');
        return;
    }

    if (slideNumber == 3) {
        for (i = 0; i < app.state.isNegativeProfit.length; i++) {
            if (app.state.isNegativeProfit[i]) {
                $('#negative-profit-error').modal('show');
                return;
            }
        }

    }
    if (slideTimeout) {
        window.clearTimeout(slideTimeout);
    }
    setDesign();

    setPriceInGoalFromDesign();
    setPriceInDesignFromGoal();

    var $slides = $('.Slides .Slide');

    if (app.state.isFront) {
        design.products.changeView(app.state.getView(true));
        $("#card").flip(false);
        $("#card3").flip(false);
    } else {
        design.products.changeView(app.state.getView(false));
        $("#card").flip(true);
        $("#card3").flip(true);
    }

    slideTimeout = window.setTimeout(function () {
        $('.flow-step.active').removeClass('active');
        $('#' + slideSteps[slideNumber - 1]).addClass('active');
        app.state.pos = 1 - slideNumber;
        $('.Slides').stop().animate({
            left: -100 * (slideNumber - 1) + '%'
        });

        //$('.Slides').stop().animate({ left: (app.state.pos * app.state.w) + 'px' });

        $slides.eq(slideNumber - 1).addClass("slide_active").siblings(".Slide.slide_active").removeClass("slide_active");
    }, 200);//delay to render design
}

function slide() {
    app.state.w = $(window).width();
    app.state.h = $(window).height();
    var slides = $('.Slides > div');
    //('.SlideContainer').css({ height: (h - 60) + 'px' });
    $('.Slides').css({ width: slides.length + '00%' });
    //slides.css({ width: app.state.w + 'px' });
    app.state.pos = 0;

    $('.Left').click(function () {
        slideTo(3);
    });

    $('#nextPage, #goal').click(Goal);

    $('#design').click(Design);
    $('#description').click(function () {
        if (app.state.pos == 0) {
            Goal();
        }
        
        slideTo(3);
    });
}

function onChangeTrackBar() {
    //document.getElementById('trackBarValue').value = document.getElementById('trackbar').value;
    //document.getElementById('total_profit').innerHTML = "RM " + (document.getElementById('trackbar').value) * 10;


    window.count = document.getElementById('trackBarValue').value;//parseInt(document.getElementById('trackbar').value);
    calculatePrice(window.frontColor, window.backColor);
    setPriceInDesignFromGoal();
    var changes = app.state.currentProduct.Price - app.state.currentProduct.BaseCost;
    //$("#mainH4").innerHTML = chenges.toFixed(2);
    document.getElementById("base-cost-for-first-product").innerHTML = app.state.currentProduct.BaseCost.toFixed(2);
    window.nowPrice = app.state.currentProduct.BaseCost;
    updateMinimum(changes.toFixed(2));
    //profitSale();

    //if (window.nowPrice < window.sellingPrice) {
    if (app.state.products != null && app.state.products.length > 1) {
        estimatedProfitChangeForManuProducts()
    } else {
        if (window.nowPrice < window.sellingPrice) {
            estimatedProfitChange();
        }
    }
    //}
}

function onChangeValueForTrackBar() {
    //var slider = document.getElementById('trackbar');
    //slider.noUiSlider.set([null, parseInt(document.getElementById('trackBarValue').value)]);
    //if (document.getElementById('trackBarValue').value < 15)
    //    document.getElementById('trackBarValue').value = "15";
    //else if (document.getElementById('trackBarValue').value > 500)
    //    document.getElementById('trackBarValue').value = "500";
    //document.getElementById('trackbar').value = document.getElementById('trackBarValue').value;
    //document.getElementById('total_profit').innerHTML = "RM " + (document.getElementById('trackbar').value) * 10;


    window.count = parseInt(document.getElementById('trackBarValue').value);
    calculatePrice(window.frontColor, window.backColor);
    setPriceInDesignFromGoal();
    //profitSale();
    var changes = app.state.currentProduct.Price - app.state.currentProduct.BaseCost;
    //$("#mainH4").html("RM " + parseFloat(chenges.toFixed(2)) + " Profit per sale");
    document.getElementById("base-cost-for-first-product").innerHTML = app.state.currentProduct.BaseCost.toFixed(2);
    window.nowPrice = app.state.currentProduct.BaseCost;
    updateMinimum(changes.toFixed(2));

    //if (window.nowPrice < window.sellingPrice) {
    if (app.state.products.length > 1) {
        estimatedProfitChangeForManuProducts()
    } else {
        if (window.nowPrice < window.sellingPrice) {
            estimatedProfitChange();
        }
    }
    //}
}

function changesColor(id, number) {
    if ($(id).hasClass('div-color-active')) {
        $("#first-product").click();
        var color;
        switch (number) {
            case 1: color = design.products.colors[app.state.currentProduct.ColorId];
                break;
            case 2: color = design.products.colors[app.state.currentProduct.SecondColorId];
                break;
            case 3: color = design.products.colors[app.state.currentProduct.ThirdColorId];
                break;
            case 4: color = design.products.colors[app.state.currentProduct.FourthColorId];
                break;
            case 5: color = design.products.colors[app.state.currentProduct.FifthColorId];
                break;
        }

        $("#minImg").css("background-color", color.value);
        $("#prodFront").css("background-color", color.value);
        $("#prodBack").css("background-color", color.value);
        //$("#prodFront3").css("background-color", color.value);
        //$("#prodBack3").css("background-color", color.value);
        //$(".product_images").css("background-color", color.value);
        $('.containertip--open').removeClass('containertip--open');
        //app.state.color = color;
        //design.products.changeColor(color);
        //app.state.currentProduct.ColorId = color.id;
    }
}

function deleteColor(number, dontChanges) {
    switch (number) {
        case 1:
            $("#li_" + app.state.currentProduct.ProductId + "_" + app.state.currentProduct.ColorId).children("span").remove();
            color = design.products.colors[app.state.currentProduct.SecondColorId];
            $("#prodFront").css("background-color", color.value);
            $("#minImg").css("background-color", color.value);
            $("#prodBack").css("background-color", color.value);
            //$("#prodFront3").css("background-color", app.state.currentProduct.SecondColorId.value);
            //$("#prodBack3").css("background-color", app.state.currentProduct.SecondColorId.value);
            //$(".product_images").css("background-color", app.state.currentProduct.SecondColorId.value);
            design.products.changeColor(design.products.colors[app.state.currentProduct.SecondColorId]);

            app.state.currentProduct.ColorId = app.state.currentProduct.SecondColorId;
            app.state.currentProduct.SecondColorId = app.state.currentProduct.ThirdColorId;
            app.state.currentProduct.ThirdColorId = app.state.currentProduct.FourthColorId;
            app.state.currentProduct.FourthColorId = app.state.currentProduct.FifthColorId;
            app.state.currentProduct.FifthColorId = 0;

            $("#div-color-1").css("background-color", $("#div-color-2").css("background-color"));
            $("#div-color-1").removeClass().addClass($("#div-color-2").attr('class'));
            $("#div-color-2").css("background-color", $("#div-color-3").css("background-color"));
            $("#div-color-2").removeClass().addClass($("#div-color-3").attr('class'));
            if ($("#div-color-2").hasClass("div-color")) $("#div-color-2").children("div").remove();
            $("#div-color-3").css("background-color", $("#div-color-4").css("background-color"));
            $("#div-color-3").removeClass().addClass($("#div-color-4").attr('class'));
            if ($("#div-color-3").hasClass("div-color")) $("#div-color-3").children("div").remove();
            $("#div-color-4").css("background-color", $("#div-color-5").css("background-color"));
            $("#div-color-4").removeClass().addClass($("#div-color-5").attr('class'));
            if ($("#div-color-4").hasClass("div-color")) $("#div-color-4").children("div").remove();
            $("#div-color-5").css("background-color", "rgb(219, 219, 219)");
            $("#div-color-5").removeClass('div-color-active').addClass('div-color');
            if ($("#div-color-5").hasClass("div-color")) $("#div-color-5").children("div").remove();


            break;
        case 2:
            $("#li_" + app.state.currentProduct.ProductId + "_" + app.state.currentProduct.SecondColorId).children("span").remove();
            var color = design.products.colors[app.state.currentProduct.ColorId];

            app.state.currentProduct.SecondColorId = app.state.currentProduct.ThirdColorId;
            app.state.currentProduct.ThirdColorId = app.state.currentProduct.FourthColorId;
            app.state.currentProduct.FourthColorId = app.state.currentProduct.FifthColorId;
            app.state.currentProduct.FifthColorId = 0;

            $("#div-color-2").css("background-color", $("#div-color-3").css("background-color"));
            $("#div-color-2").removeClass().addClass($("#div-color-3").attr('class'));
            if ($("#div-color-2").hasClass("div-color")) $("#div-color-2").children("div").remove();
            $("#div-color-3").css("background-color", $("#div-color-4").css("background-color"));
            $("#div-color-3").removeClass().addClass($("#div-color-4").attr('class'));
            if ($("#div-color-3").hasClass("div-color")) $("#div-color-3").children("div").remove();
            $("#div-color-4").css("background-color", $("#div-color-5").css("background-color"));
            $("#div-color-4").removeClass().addClass($("#div-color-5").attr('class'));
            if ($("#div-color-4").hasClass("div-color")) $("#div-color-4").children("div").remove();
            $("#div-color-5").css("background-color", "rgb(219, 219, 219)");
            $("#div-color-5").removeClass('div-color-active').addClass('div-color');
            if ($("#div-color-5").hasClass("div-color")) $("#div-color-5").children("div").remove();

            if (dontChanges) {
                break;
            }

            if ($("#div-color-3").hasClass("div-color")) {
                $("#minImg").css("background-color", color.value);
                $("#prodFront").css("background-color", color.value);
                $("#prodBack").css("background-color", color.value);
                $("#prodFront3").css("background-color", color.value);
                $("#prodBack3").css("background-color", color.value);
                $(".product_images").css("background-color", color.value);
                $('.containertip--open').removeClass('containertip--open');
                design.products.changeColor(color);
            }
            break;
        case 3:
            $("#li_" + app.state.currentProduct.ProductId + "_" + app.state.currentProduct.ThirdColorId).children("span").remove();
            var color = design.products.colors[app.state.currentProduct.SecondColorId];

            app.state.currentProduct.ThirdColorId = app.state.currentProduct.FourthColorId;
            app.state.currentProduct.FourthColorId = app.state.currentProduct.FifthColorId;
            app.state.currentProduct.FifthColorId = 0;

            $("#div-color-3").css("background-color", $("#div-color-4").css("background-color"));
            $("#div-color-3").removeClass().addClass($("#div-color-4").attr('class'));
            if ($("#div-color-3").hasClass("div-color")) $("#div-color-3").children("div").remove();
            $("#div-color-4").css("background-color", $("#div-color-5").css("background-color"));
            $("#div-color-4").removeClass().addClass($("#div-color-5").attr('class'));
            if ($("#div-color-4").hasClass("div-color")) $("#div-color-4").children("div").remove();
            $("#div-color-5").css("background-color", "rgb(219, 219, 219)");
            $("#div-color-5").removeClass('div-color-active').addClass('div-color');
            if ($("#div-color-5").hasClass("div-color")) $("#div-color-5").children("div").remove();

            if (dontChanges) {
                break;
            }

            if ($("#div-color-4").hasClass("div-color")) {
                $("#minImg").css("background-color", color.value);
                $("#prodFront").css("background-color", color.value);
                $("#prodBack").css("background-color", color.value);
                $("#prodFront3").css("background-color", color.value);
                $("#prodBack3").css("background-color", color.value);
                $(".product_images").css("background-color", color.value);
                $('.containertip--open').removeClass('containertip--open');
                design.products.changeColor(color);
            }
            break;
        case 4:
            $("#li_" + app.state.currentProduct.ProductId + "_" + app.state.currentProduct.FourthColorId).children("span").remove();
            var color = design.products.colors[app.state.currentProduct.ThirdColorId];

            app.state.currentProduct.FourthColorId = app.state.currentProduct.FifthColorId;
            app.state.currentProduct.FifthColorId = 0;

            $("#div-color-4").css("background-color", $("#div-color-5").css("background-color"));
            $("#div-color-4").removeClass().addClass($("#div-color-5").attr('class'));
            if ($("#div-color-4").hasClass("div-color")) $("#div-color-4").children("div").remove();
            $("#div-color-5").css("background-color", "rgb(219, 219, 219)");
            $("#div-color-5").removeClass('div-color-active').addClass('div-color');
            if ($("#div-color-5").hasClass("div-color")) $("#div-color-5").children("div").remove();

            if (dontChanges) {
                break;
            }

            if ($("#div-color-5").hasClass("div-color")) {
                $("#minImg").css("background-color", color.value);
                $("#prodFront").css("background-color", color.value);
                $("#prodBack").css("background-color", color.value);
                $("#prodFront3").css("background-color", color.value);
                $("#prodBack3").css("background-color", color.value);
                $(".product_images").css("background-color", color.value);
                $('.containertip--open').removeClass('containertip--open');
                design.products.changeColor(color);
            };
            break;
        case 5:
            $("#li_" + app.state.currentProduct.ProductId + "_" + app.state.currentProduct.FifthColorId).children("span").remove();
            var color = design.products.colors[app.state.currentProduct.FourthColorId];

            app.state.currentProduct.FifthColorId = 0;

            $("#div-color-5").css("background-color", "rgb(219, 219, 219)");
            $("#div-color-5").removeClass('div-color-active').addClass('div-color');
            if ($("#div-color-5").hasClass("div-color")) $("#div-color-5").children("div").remove();

            if (dontChanges) {
                break;
            }

            $("#minImg").css("background-color", color.value);
            $("#prodFront").css("background-color", color.value);
            $("#prodBack").css("background-color", color.value);
            $("#prodFront3").css("background-color", color.value);
            $("#prodBack3").css("background-color", color.value);
            $(".product_images").css("background-color", color.value);
            $('.containertip--open').removeClass('containertip--open');
            design.products.changeColor(color);
            break;
    }
}

function cutHex(h) {
    return (h.charAt(0) == "#") ? h.substring(1, 7) : h
}
//document.addEventListener("keypress", function (e) {
//    if (e.keyCode == 40) {
//        document.querySelector("body").scrollTop = 0;
//    }
//});
document.addEventListener("keydown", function keyBut(e) {
    //alert("qwe");
    //alert("q");
    var tspans = document.querySelectorAll('svg');
    if (tspans.length != 0) {
        for (var i = 0; i < tspans.length; i++) {
            var elem = tspans[i].parentNode;
            if ($(elem).hasClass('drag-item-selected')) {
                if (e.keyCode == 37) {
                    var str = elem.style.left;
                    var left = str.substring(0, str.length - 2)

                    var boxes = elem.parentNode.querySelectorAll('.edit-box-sizer');
                    var box = boxes[0];
                    if (boxes.length > 1) {
                        for (var i = 0; i < boxes.length; i++) {
                            //console.log("box: " + box.style.zIndex + ";  boxes[i]: " + boxes[i].style.zIndex);
                            if (parseInt(boxes[i].style.zIndex) > parseInt(box.style.zIndex)) {
                                box = boxes[i];
                                //console.log(box);
                            }
                            //console.log("result: " + box.style.zIndex);

                        }
                    }
                    var boxStr = box.style.left;
                    var boxTopValue = boxStr.substring(0, boxStr.length - 2);
                    elem.style.left = left - 1 + "px";
                    box.style.left = boxTopValue - 1 + "px";
                    design.item.checkBorders(design.item.get());


                    //document.removeEventListener("keyup", keyBut);
                    //console.log(elem.style.left)
                } else if (e.keyCode == 38) {
                    var str = elem.style.top;
                    var left = str.substring(0, str.length - 2);

                    var boxes = elem.parentNode.querySelectorAll('.edit-box-sizer');
                    var box = boxes[0];
                    if (boxes.length > 1) {
                        for (var i = 0; i < boxes.length; i++) {
                            console.log("box: " + box.style.zIndex + ";  boxes[i]: " + boxes[i].style.zIndex);
                            if (parseInt(boxes[i].style.zIndex) > parseInt(box.style.zIndex)) {
                                box = boxes[i];
                                //console.log(box);
                            }
                            console.log("result: " + box.style.zIndex);

                        }
                    }
                    var boxStr = box.style.top;
                    var boxTopValue = boxStr.substring(0, boxStr.length - 2);
                    elem.style.top = parseFloat(left) - 1 + "px";
                    box.style.top = boxTopValue - 1 + "px";
                    design.item.checkBorders(design.item.get());
                    //console.log(elem.style.top)

                } else if (e.keyCode == 39) {
                    var str = elem.style.left;
                    var left = str.substring(0, str.length - 2)

                    var boxes = elem.parentNode.querySelectorAll('.edit-box-sizer');
                    var box = boxes[0];
                    if (boxes.length > 1) {
                        for (var i = 0; i < boxes.length; i++) {
                            console.log("box: " + box.style.zIndex + ";  boxes[i]: " + boxes[i].style.zIndex);
                            if (parseInt(boxes[i].style.zIndex) > parseInt(box.style.zIndex)) {
                                box = boxes[i];
                                //console.log(box);
                            }
                            console.log("result: " + box.style.zIndex);

                        }
                    }
                    var boxStr = box.style.left;
                    var boxTopValue = boxStr.substring(0, boxStr.length - 2);


                    elem.style.left = parseFloat(left) + 1 + "px";
                    box.style.left = parseFloat(boxTopValue) + 1 + "px";
                    design.item.checkBorders(design.item.get());

                } else if (e.keyCode == 40) {
                    var str = elem.style.top;
                    var left = str.substring(0, str.length - 2)

                    var boxes = elem.parentNode.querySelectorAll('.edit-box-sizer');
                    var box = boxes[0];
                    if (boxes.length > 1) {
                        for (var i = 0; i < boxes.length; i++) {
                            console.log("box: " + box.style.zIndex + ";  boxes[i]: " + boxes[i].style.zIndex);
                            if (parseInt(boxes[i].style.zIndex) > parseInt(box.style.zIndex)) {
                                box = boxes[i];
                                //console.log(box);
                            }
                            //console.log("result: " + box.style.zIndex);

                        }
                    }
                    var boxStr = box.style.top;
                    var boxTopValue = boxStr.substring(0, boxStr.length - 2);
                    //document.querySelector("body").scrollTop = 0;


                    elem.style.top = parseFloat(left) + 1 + "px";
                    box.style.top = parseFloat(boxTopValue) + 1 + "px";
                    design.item.checkBorders(design.item.get());
                    e.preventDefault ? e.preventDefault() : (e.returnValue = false);

                }
            }
        }
   



    }
});
