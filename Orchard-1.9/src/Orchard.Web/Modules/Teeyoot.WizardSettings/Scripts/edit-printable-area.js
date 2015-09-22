(function () {
    var isFront = true;
    var productId;
    var areaZoom;
    var loadedData;
    function fetchData(id) {
        $.get('/Admin/ProductImage/Get/' + id).always(function (data) {
            var data = $.extend({}, {
                ChestLineBack: 150,
                ChestLineFront: 180,
                Ppi: 18,
                PrintableBackHeight: 330,
                PrintableBackLeft: 155,
                PrintableBackTop: 60,
                PrintableBackWidth: 215,
                PrintableFrontHeight: 330,
                PrintableFrontLeft: 152,
                PrintableFrontTop: 100,
                PrintableFrontWidth: 220,
                ProductId: productId
            }, data);
            if (!data.Ppi || isNaN(data.Ppi)) {
                data.Ppi = 18;
            }
            if (!data.PrintableFrontWidth) {
                data.PrintableFrontWidth = 220;
            }
            if (!data.PrintableBackWidth) {
                data.PrintableBackWidth = 215;
            }
            if (!data.PrintableFrontHeight) {
                data.PrintableFrontHeight = 330;
            }
            if (!data.PrintableBackHeight) {
                data.PrintableBackHeight = 330;
            }
            setValues(data);
            showDialog(data);
        });
    }
    function save() {
        $.post('/Admin/ProductImage/Edit', getValues());
        $("#set-printable-area").dialog("close");
    }
    function getValues() {
        var $front = $('#card .front');
        var $back = $('#card .back');

        var area = $front.find('.area-design');
        loadedData.ChestLineFront = parseInt($front.find('.chestline').css('top'));
        loadedData.PrintableFrontWidth = parseInt(area.css('width'));
        loadedData.PrintableFrontHeight = parseInt(area.css('height'));
        loadedData.PrintableFrontTop = parseInt(area.css('top'));
        loadedData.PrintableFrontLeft = parseInt(area.css('left'));

        area = $back.find('.area-design');
        loadedData.ChestLineBack = parseInt($back.find('.chestline').css('top'));
        loadedData.PrintableBackWidth = parseInt(area.css('width'));
        loadedData.PrintableBackHeight = parseInt(area.css('height'));
        loadedData.PrintableBackTop = parseInt(area.css('top'));
        loadedData.PrintableBackLeft = parseInt(area.css('left'));

        loadedData.Ppi = Math.round(loadedData.PrintableFrontWidth * 2.54 / $front.find('.width').val());
        loadedData.ProductId = productId;
        loadedData.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
        return loadedData;
    }
    function setValues(data) {
        loadedData = data;
        var $front = $('#card .front');
        var $back = $('#card .back');
        $front.find('.width').val((data.PrintableFrontWidth / data.Ppi * 2.54).toFixed(2));
        $front.find('.height').val((data.PrintableFrontHeight / data.Ppi * 2.54).toFixed(2));
        //---------------
        $front.find('.left').val(data.PrintableFrontLeft);
        $front.find('.top').val(data.PrintableFrontTop);
        //---------------
        $front.find('.area-chest').val((data.ChestLineFront / data.Ppi * 2.54).toFixed(2));
        $front.find('.area-design').css({
            top: data.PrintableFrontTop,
            left: data.PrintableFrontLeft,
            width: data.PrintableFrontWidth,
            height: data.PrintableFrontHeight
        });
        $front.find('.chestline').css('top', data.ChestLineFront);

        $back.find('.width').val((data.PrintableBackWidth / data.Ppi * 2.54).toFixed(2));
        $back.find('.height').val((data.PrintableBackHeight / data.Ppi * 2.54).toFixed(2));
        //---------------
        $back.find('.left').val(data.PrintableBackLeft);
        $back.find('.top').val(data.PrintableBackTop);
        //----------------------
        $back.find('.area-chest').val((data.ChestLineBack / data.Ppi * 2.54).toFixed(2));
        $back.find('.area-design').css({
            top: data.PrintableBackTop,
            left: data.PrintableBackLeft,
            width: data.PrintableBackWidth,
            height: data.PrintableBackHeight
        });
        $back.find('.chestline').css('top', data.ChestLineBack);

    }
    function inputsKeyUp() {
        var o = $(this);
        var $plane = $(isFront ? '#card .front' : '#card .back');
        var value = o.val();
        var filter = /^[0-9.]+$/;
        if (filter.test(value)) {
            var area = $plane.find('.area-design');

            if (o.hasClass('width')) {
                var C_areaZoom = area.height() / $plane.find('.height').val();
                var width = value * C_areaZoom;
                area.width(width.toFixed(2));
            } else if (o.hasClass('height')) {
                var C_areaZoom = area.width() / $plane.find('.width').val();
                var height = value * C_areaZoom;
                area.height(height.toFixed(2));
                //---------------------------------------------
            } else if (o.hasClass('left')) {
                area.css('left', +value);

            } else if (o.hasClass('top')) {
                area.css('top', +value);
                //-------------------------------------------------
            } else {
                var C_areaZoom = area.width() / $plane.find('.width').val();
                $plane.find('.chestline').css('top', C_areaZoom * value);
            }
        }
    }
    function updateInputsOnResize(ui) {
        var $plane = $(isFront ? '#card .front' : '#card .back');
        var isLockedW = $plane.find('.lock-w').is(':checked');
        var isLockedH = $plane.find('.lock-h').is(':checked');
        var oldZoom = areaZoom;
        if (!isLockedH || !isLockedW) {
            if (isLockedW) {
                areaZoom = $plane.find('.width').val() / $plane.find('.area-design').width();
                var height = ui.size.height * areaZoom;
                $plane.find('.height').val(height.toFixed(2));
            }
            else if (isLockedH) {
                areaZoom = $plane.find('.height').val() / $plane.find('.area-design').height();
                var width = ui.size.width * areaZoom;
                $plane.find('.width').val(width.toFixed(2));


            } else {
                var width = ui.size.width * areaZoom;
                $plane.find('.width').val(width.toFixed(2));
                var height = ui.size.height * areaZoom;
                $plane.find('.height').val(height.toFixed(2));
            }
        }
        if (areaZoom !== oldZoom) {
            $plane = $(!isFront ? '#card .front' : '#card .back');
            isLockedW = $plane.find('.lock-w').is(':checked');
            isLockedH = $plane.find('.lock-h').is(':checked');
            $plane.find('.area-design').css({
                width: $plane.find('.width').val() / areaZoom,
                height: $plane.find('.height').val() / areaZoom
            });
        }

        UpdateInputsOnMove(ui);
    }

    function UpdateInputsOnMove(ui) {
        var $plane = $(isFront ? '#card .front' : '#card .back');
        $plane.find('.left').val(ui.position.left);
        $plane.find('.top').val(ui.position.top);
    }

    function setResizeable() {
        var $front = $('#card .front');
        var $back = $('#card .back');
        var aspect = $front.find('.lock-w').is(':checked') && $front.find('.lock-h').is(':checked')
        var area = $('.front .area-design')
        if (area.hasClass('ui-resizable')) {
            area.resizable('destroy');
        }

        area.resizable({
            handles: "ne, se, sw, nw", aspectRatio: aspect,
            resize: function (event, ui) { updateInputsOnResize(ui); },
            start: function (event, ui) { areaZoom = $front.find('.width').val() / $front.find('.area-design').width(); }
        }).draggable({
            containment: "parent",
            drag: function (event, ui) { UpdateInputsOnMove(ui); }
        });
        aspect = $back.find('.lock-w').is(':checked') && $back.find('.lock-h').is(':checked')
        var area = $('.back .area-design');
        if (area.hasClass('ui-resizable')) {
            area.resizable('destroy');
        }
        area.resizable({
            handles: "ne, se, sw, nw", aspectRatio: aspect,
            resize: function (event, ui) { updateInputsOnResize(ui); },
            start: function (event, ui) { areaZoom = $back.find('.width').val() / $back.find('.area-design').width(); }
        }).draggable({
            containment: "parent",
            drag: function (event, ui) { UpdateInputsOnMove(ui); }
        });
    }
    function showDialog(data) {
        isFront = true;
        $("#card").flip(!isFront);
        $('#set-printable-area').dialog({
            modal: true,
            width: 720,
            height: 720,
            dialogClass: 'dialog-printable-area'
        });
        setResizeable();
    }
    $('#edit_image').click(function () {
        var id = $('#product_id').val();
        productId = id;
        fetchData(id);
        var $front = $('#card .front');
        var $back = $('#card .back');
        $front.find('.image').css('background-image', 'url("/Modules/Teeyoot.Module/Content/images/product_type_' + productId + '_front.png")');
        $back.find('.image').css('background-image', 'url("/Modules/Teeyoot.Module/Content/images/product_type_' + productId + '_back.png")');
    });
    $("#card").flip({
        trigger: 'manual'
    });
    $('.flip-button:not(.flip-button-active)').on('click', function () {
        isFront = !isFront;
        $("#card").flip(!isFront);
    });
    $('.area-design, #card').click(function (e) {
        $('#card .selected').removeClass('selected');
        $(this).addClass('selected');
        e.stopPropagation();
    })
    $('.lock-h, .lock-w').change(function () {
        setResizeable();
    });
    $('#card input[type=text]').on('keyup', inputsKeyUp);
    $('#card .docancel').click(function () {
        $("#set-printable-area").dialog("close");
    });
    $('#card .save').click(function () {
        save();
    });
})();