﻿window.onresize = function () {
    app.state.w = $(window).width();
    var slides = $('.Slides > div');
    slides.css({ width: app.state.w + 'px' });
};
var globalPrdc = '';
window.onload = function initWizard() {


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
        var prodId = parseInt(document.getElementById("product").value);

        var coeff = parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_front_height) / parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_front_width);
        var coeffBack = parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_back_height) / parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_back_width);

        var front = (design.products.images[prodId].printable_front_height / design.products.images[prodId].printable_front_width).toFixed(1);
        var back = (design.products.images[prodId].printable_back_height / design.products.images[prodId].printable_back_width).toFixed(1);
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
        for (var i = 0; i < prices.length; i++) {
            if (prices[i].color_id == prdc.ColorId) {
                prdc.BaseCost = prices[i].price;
            }
        }
        var calc = calculatePriceForNewProduct(window.frontColor, window.backColor, prdc.BaseCost);
        prdc.BaseCost = calc[0];
        prdc.Price = calc[1];

        $div.click(function () {
            if (app.state.products.indexOf(prdc) >= 0) {
                var newColor = design.products.colors[prdc.ColorId];
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
        $image.css("background-color", app.state.color.value);

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
        h4CostProfRm.innerHTML = "RM";
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
        h4ProfRm.innerHTML = "RM";
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
        spanPrice.innerHTML = "RM"
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


        divCol.classList.add("clearfix");
        divCol.classList.add("control-group");
        divCol.classList.add("font-color-selection");
        divCol.style.width = "100px";

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
        divColors.style.top = "28%";
        divColors.style.left = "28%";

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
            $div.click();
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

            var colorHtml = '<li data-value="' + color.id + ')" class="shirt-color-sample" title="' +
                              color.name + '" style="background-color:' + color.value + ';"></li>';
            var $colorHtml = $(colorHtml);


            $colorHtml.click(function (event) {
                $("#prodFront").css("background-color", color.value);
                $("#prodBack").css("background-color", color.value);
                event.preventDefault();
                event.stopPropagation();
                $image.css("background-color", color.value);
                $divSwatch.css("background-color", color.value);
                prdc.ColorId = parseInt(color.id);

                var product = design.products.productsData[prdc.ProductId];
                var prices = product.prices;
                for (var i = 0; i < prices.length; i++) {
                    if (prices[i].color_id == prdc.ColorId) {
                        prdc.BaseCost = prices[i].price;
                    }
                }
                var calc = calculatePriceForNewProduct(window.frontColor, window.backColor, prdc.BaseCost);
                prdc.BaseCost = calc[0];
                var changes = prdc.Price - prdc.BaseCost.toFixed(2);
                estimatedProfitChangeForManuProducts();
                h4CostProfFloat.innerHTML = prdc.BaseCost.toFixed(2);
                h4Profit.innerHTML = changes.toFixed(2);
                //$divColors.remove();
                $divColors.removeClass('containertip--open');
            }).hover(function () {
                $image.css("background-color", color.value);
                $divSwatch.css("background-color", color.value);
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
        divVcent.style.marginLeft = "20px";

        text.classList.add("ssp_heading");
        text.style.color = "#44474d";
        text.style.fontWeight = "800";
        text.textContent = "Teeyoot Premium Tee";

        divDelete.classList.add("ssp_delete");

        divDelete.appendChild(imageDel);



        divVcent.appendChild(text);
        divMeta.appendChild(divVcent);
        divThumb.appendChild(image);
        divColorPicAndMeta.appendChild(divCol);
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
        });

        globalPrdc = prdc;
        app.state.products.push(prdc);
        $div.click();
        estimatedProfitChangeForManuProducts();
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
        $("#first-product").click();
    });

    $("#first-product").click(function () {
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
    if (!app.state.pos) {
        var src = assetsUrls.products + 'product_type_' + app.state.product.id + '_front_small.png';
        $('#first-product .thumbnail_wrapper img').attr('src', src).css('background-color', app.state.color.value);
        $('#first-product .swatch2').css('background-color', app.state.color.value);
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
    } else {
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
}

function initProducts() {
    var list = document.getElementById("item-options-dropdown-tees");
    var listProd = document.getElementById("product");
    var mas = [];

    //Если лист категорий пустой то мы его инициализируем
    if (list.value == "") {


        //var coeff = parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_front_height) / parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_front_width);
        //var coeffBack = parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_back_height) / parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_back_width);
        $.each(design.products.categoriesList, function (i) {
            var z = 0;
            //for (var k = 0; k < this.products.length; k++) {
            //    var front = (design.products.images[this.products[k]].printable_front_height / design.products.images[this.products[k]].printable_front_width).toFixed(1);
            //    var back = (design.products.images[this.products[k]].printable_back_height / design.products.images[this.products[k]].printable_back_width).toFixed(1);
            //    if ((front == coeff.toFixed(1)) && (back == coeffBack.toFixed(1))) {
            //        z++;
            //    }
            //}
            //var gfgf = z;
            //if (z >= 1) {
                var option = document.createElement("option");
                option.value = i;
                option.id = this.id;
                option.innerHTML = this.name;
                list.appendChild(option);
                //Запихиваем айдишники продуктов по обьектам в массив
                mas.push(this.products);
            //}
        });
        //Если лист продуктов пустой то мы его инициализируем
        if (listProd.value == "") {
            //var coeff = parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_front_height) / parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_front_width);
            //var coeffBack = parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_back_height) / parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_back_width);
            $.each(design.products.productsData, function (i, el) {
                //var ww = (parseFloat(design.products.images[el.id].printable_front_height) / parseFloat(design.products.images[el.id].printable_front_width)).toFixed(1);
                //var back = (parseFloat(design.products.images[el.id].printable_back_height) / parseFloat(design.products.images[el.id].printable_back_width)).toFixed(1);
                //if ((ww == coeff.toFixed(1)) && (back == coeffBack.toFixed(1))) {
                    //Если список продуктов по первой категории содержит айдишники из общего списка продуктов то мы вытягиваем их в наш лист
                    if (design.products.categoriesList[0].products.indexOf(el.id) >= 0) {
                        var option = document.createElement("option");
                        option.value = i;
                        option.id = i;
                        option.innerHTML = el.name;
                        listProd.appendChild(option);
                    }
                //};
            });
        }

        $(list).change(function () {
            listProd.innerHTML = "";
            //var coeff = parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_front_height) / parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_front_width);
            //var coeffBack = parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_back_height) / parseFloat(design.products.images[app.state.currentProduct.ProductId].printable_back_width);
            $.each(design.products.categoriesList[this.value].products, function (i, element) {
                //var qq = (parseFloat(design.products.images[element].printable_front_height) / parseFloat(design.products.images[element].printable_front_width)).toFixed(1);
                //var back = (parseFloat(design.products.images[element].printable_back_height) / parseFloat(design.products.images[element].printable_back_width)).toFixed(1);
                //if ((qq == coeff.toFixed(1)) && (back == coeffBack.toFixed(1))) {
                    var option = document.createElement("option");
                    option.value = element;
                    option.id = element;
                    option.innerHTML = design.products.productsData[element].name;
                    listProd.appendChild(option);
                //};
            });

            //$.each((mas[this.value]), function (i, id) {
            //    $.each(design.products.productsData, function (i, el) {

            //            if (el.id == id) {
            //                var option = document.createElement("option");
            //                option.value = i;
            //                option.id = i;
            //                option.innerHTML = el.name;
            //                listProd.appendChild(option);
            //            }

            //    });
            //});
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
        $("#total_profit").html("RM 0+");
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
            var colorHtml = '<li data-value="' + color.id + ')" class="shirt-color-sample" title="' +
                                color.name + '" style="background-color:' + color.value + ';"></li>';
            var $colorHtml = $(colorHtml);
            $colorHtml.click(function () {
                $("#minImg").css("background-color", color.value);
                $("#swatch2").css("background-color", color.value);
                $("#prodFront").css("background-color", color.value);
                $("#prodBack").css("background-color", color.value);
                $("#prodFront3").css("background-color", color.value);
                $("#prodBack3").css("background-color", color.value);
                $(".product_images").css("background-color", color.value);
                $('.containertip--open').removeClass('containertip--open');
                design.products.changeColor(color);
                app.state.currentProduct.ColorId = parseInt(color.id);

                app.state.currentProduct.ColorId = parseInt(color.id);

                var product = design.products.productsData[app.state.currentProduct.ProductId];
                var prices = product.prices;
                for (var i = 0; i < prices.length; i++) {
                    if (prices[i].color_id == app.state.currentProduct.ColorId) {
                        app.state.currentProduct.BaseCost = prices[i].price;
                    }
                }
                window.costOfMaterial = app.state.currentProduct.BaseCost;
                var calc = calculatePriceForNewProduct(window.frontColor, window.backColor, app.state.currentProduct.BaseCost);
                app.state.currentProduct.BaseCost = calc[0];
                var changes = app.state.currentProduct.Price - app.state.currentProduct.BaseCost;
                //$("#mainH4").html("RM " + parseFloat(chenges.toFixed(2)) + " Profit per sale");
                window.nowPrice = app.state.currentProduct.BaseCost;
                updateMinimum(changes.toFixed(2));
                if (window.nowPrice < window.sellingPrice) {
                    if (app.state.products.length > 1) {
                        estimatedProfitChangeForManuProducts()
                    } else {
                        estimatedProfitChange();
                    }
                }
                //document.getElementById("price_preview").innerText = "RM " + window.nowPrice.toFixed(2);
                $(document.getElementById("price_preview")).text("RM " + window.nowPrice.toFixed(2));
            }).hover(function () {
                $("#minImg").css("background-color", color.value);
                $("#swatch2").css("background-color", color.value);
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