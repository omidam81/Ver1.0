window.onload = function initWizard() {

    app.state.w = $(window).width();
    app.state.h = $(window).height();
    document.getElementById('trackbar').value = 250;
    var slides = $('.Slides > div');
    $('.Slides').css({ width: slides.length + '00%' });
    slides.css({ width: app.state.w + 'px' });
    app.state.pos = 0;
    slide();



        if (document.querySelector(".user-email") == null) {
            document.querySelector(".design__area").style.marginLeft = "0%";
            document.getElementById("no-band1").style.marginLeft = "-8%";
            document.getElementById("Content3").style.marginLeft = "2%";
            document.getElementById("wizardSecondSlide").style.marginLeft = "1.7%";
        }
        

  

$("#openTags").click(function () {
    document.getElementById("tags").style.display = "inline"; 
    document.getElementById("openTags").style.display = "none";
});

$("#butAdd").click(function addElement() {

    if (document.querySelectorAll(".ssp_block").length >= 7)
    {
        document.getElementById("ui").style.display = "none";
    }

    var div = document.createElement("div");
    var divThumb = document.createElement("div");
    var divMeta = document.createElement("div");
    var divVcent = document.createElement("div");
    var divDelete = document.createElement("div");
    var image = document.createElement("img");
    var imageDel = document.createElement("img");
    var text = document.createElement("h4");




    image.src = assetsUrls.products + 'product_type_' + document.getElementById("product").value + '_front_small.png';
    image.classList.add("sell");
    image.style.height = "73px";

    imageDel.classList.add("ssp_delete");
    imageDel.src = "https://d1b2zzpxewkr9z.cloudfront.net/compiled_assets/designer/ssp_del-4d7ed20752fe1fbe0917e4e4d605aa16.png";
    imageDel.style.cursor = "pointer";

    var $image = $(image);

    //----------- profit/sale ----------------------------------
    var divPricing = document.createElement("div");
    var divProfit = document.createElement("div");
    var inpPrice = document.createElement("input");
    var h4Profit = document.createElement("h4");

    divPricing.classList.add("ssp_pricing");
    divPricing.style.marginLeft = "-8%";

    divProfit.classList.add("profitSale");
    inpPrice.classList.add("ssp_input");
    inpPrice.classList.add("price_per");
    inpPrice.classList.add("form__textfield");
    inpPrice.style.padding = "0.3em";
    inpPrice.value = "RM 15";

    h4Profit.classList.add("h4ProfSale");
    h4Profit.innerHTML = "RM 5.00 profit / sale";

    $inp = $(inpPrice);


    // Ивент на остаток прибыли от суммы одной футболки -------------------
    $inp.change(function () {
        h4Profit.innerHTML = "RM " + (parseFloat(String(inpPrice.value).match(/-?\d+(?:\.\d+)?/g, '') || 0, 10) / 3).toFixed(2) + " profit / sale";
    });

    var $divPricing = $(divPricing);
    $divPricing.append($inp);
    divProfit.appendChild(h4Profit);
    var $divProfit = $(divProfit);
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
        event.preventDefault();
        event.stopPropagation();

        $('.containertip--open').removeClass('containertip--open');
        $(this).parents(':first').find('.shirt-colors').addClass('containertip--open');
    });


    // Из общего списка цветов по айдишникам выбираем только те цвета которые соответствуют данному продукту
    var masColors = [];
    $.each(design.products.productsData, function (i, elemProd) {
        if (elemProd.id == document.getElementById("product").value)
        {
            $.each(design.products.colors, function (i,elem) {
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
            event.preventDefault();
            event.stopPropagation();
            $image.css("background-color", color.value);
            $divSwatch.css("background-color", color.value);
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

    divMeta.classList.add("ssp_metadata");

    divVcent.classList.add("ssp_vcent");
    divVcent.style.width = "100%"
    divVcent.style.marginLeft = "20px";

    text.classList.add("ssp_heading");
    text.style.color = "#44474d";
    text.style.fontWeight = "800";
    text.textContent = "Teespring Premium Tee";

    divDelete.classList.add("ssp_delete");

    divDelete.appendChild(imageDel);



    divVcent.appendChild(text);
    divMeta.appendChild(divVcent);
    divThumb.appendChild(image);
    div.appendChild(divThumb);
    div.appendChild(divMeta);
    div.appendChild(divCol);
    div.appendChild(divPricing);
    div.appendChild(divProfit);
    div.appendChild(divDelete);
    div.style.height = "90px";


    var primDiv = document.getElementById("primary");
    primDiv.appendChild(div);


    $(imageDel).click(function () {
        if (document.querySelectorAll(".ssp_block").length == 8) {
            document.getElementById("ui").style.display = "inline";
        }
        div.parentNode.removeChild(div);
    });


});


}

function setDesign() {
    if (!app.state.pos) {
        var src = assetsUrls.products + 'product_type_' + app.state.product.id + '_front_small.png';
        $('#primary .thumbnail_wrapper img').attr('src', src).css('background-color', app.state.color.value);
        $('#primary .swatch2').css('background-color', app.state.color.value);
        design.save(function(data) {
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
            $('#prodFront_d, #prodFront3_d').attr('src', data.front).css({ top: front.top, left:front.left });
            $('#prodBack_d, #prodBack3_d').attr('src', data.back).css({ top: back.top, left: back.left });
        });
    }
}


function initProducts() {
    var list = document.getElementById("item-options-dropdown-tees");
    var listProd = document.getElementById("product");
    var mas = [];

    //Если лист категорий пустой то мы его инициализируем
    if (list.value == "")
    {
        $.each(design.products.categoriesList, function (i) {
            var option = document.createElement("option");
            option.value = i;
            option.id = this.id;
            option.innerHTML = this.name;
            list.appendChild(option);
            //Запихиваем айдишники продуктов по обьектам в массив
            mas.push(this.products);
        });
        //Если лист продуктов пустой то мы его инициализируем
        if (listProd.value == "") {
            $.each(design.products.productsData, function (i, el) {
                //Если список продуктов по первой категории содержит айдишники из общего списка продуктов то мы вытягиваем их в наш лист
                if (design.products.categoriesList[0].products.indexOf(el.id) >= 0) {
                    var option = document.createElement("option");
                    option.value = i;
                    option.id = i;
                    option.innerHTML = el.name;
                    listProd.appendChild(option);
                }
            });
        }

        $(list).change(function () {
            listProd.innerHTML = "";
            $.each((mas[this.value]), function (i, id) {
                $.each(design.products.productsData, function (i, el) {
                    if (el.id == id) {
                        var option = document.createElement("option");
                        option.value = i;
                        option.id = i;
                        option.innerHTML = el.name;
                        listProd.appendChild(option);
                    }
                });
            });
        });
    }
}



function profitSale() {
    var $val = document.getElementById("profSale").value;
    var $price = (parseFloat(String($val).match(/-?\d+(?:\.\d+)?/g, '') || 0, 10) / 3).toFixed(2);
    $("#mainH4").html("RM " + $price + " profit / sale");
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
                $('.containertip--open').removeClass('containertip--open');
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
    slideTo(2);
}

var slideSteps = ['design', 'goal', 'description'];
var slideTimeout;

function slideTo(slideNumber) {
    if (app.state.getUsedColorsCount('front') > maxDesignColors || app.state.getUsedColorsCount('back') > maxDesignColors) {
        $('#too-many-colors-error').modal('show');
        return;
    }
    if (slideTimeout) {
        window.clearTimeout(slideTimeout);
    }
    setDesign();
    slideTimeout = window.setTimeout(function() {
        $('.flow-step.active').removeClass('active');
        $('#' + slideSteps[slideNumber - 1]).addClass('active');
        app.state.pos = 1 - slideNumber;
        $('.Slides').stop().animate({ left: (app.state.pos * app.state.w) + 'px' });
    }, 200);//delay to render design
}

function slide() {
    app.state.w = $(window).width();
    app.state.h = $(window).height();
    var slides = $('.Slides > div');
    //('.SlideContainer').css({ height: (h - 60) + 'px' });
    $('.Slides').css({ width: slides.length + '00%' });
    slides.css({ width: app.state.w + 'px' });
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
    document.getElementById('trackBarValue').value = document.getElementById('trackbar').value;
    document.getElementById('total_profit').innerHTML = "RM " + (document.getElementById('trackbar').value) * 10;
}

function onChangeValueForTrackBar() {
    if (document.getElementById('trackBarValue').value < 15)
        document.getElementById('trackBarValue').value = "15";
    else if (document.getElementById('trackBarValue').value > 500)
        document.getElementById('trackBarValue').value = "500";
    document.getElementById('trackbar').value = document.getElementById('trackBarValue').value;
    document.getElementById('total_profit').innerHTML = "RM " + (document.getElementById('trackbar').value) * 10;
}


