window.onload = function initWizard() {
    var w = $(window).width();
    var h = $(window).height();
    var slides = $('.Slides > div');
    $('.SlideContainer').css({ height: (h - 60) + 'px' });
    $('.Slides').css({ width: slides.length + '00%' });
    document.getElementById('content').style.marginBottom = "60%";
    slides.css({ width: w + 'px' });

    var pos = 0;
    slide();




    $("#butAdd").click(function addElement() {
        var div = document.createElement("div");
        var divThumb = document.createElement("div");
        var divMeta = document.createElement("div");
        var divVcent = document.createElement("div");
        var divDelete = document.createElement("div");
        var image = document.createElement("img");
        var imageDel = document.createElement("img");
        var text = document.createElement("h4");



        image.src = "http://d1b2zzpxewkr9z.cloudfront.net/images/products/apparel/product_type_1_front_small.png";
        image.classList.add("sell");

        imageDel.classList.add("ssp_delete");
        imageDel.src = "https://d1b2zzpxewkr9z.cloudfront.net/compiled_assets/designer/ssp_del-4d7ed20752fe1fbe0917e4e4d605aa16.png";
        imageDel.style.cursor = "pointer";

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
        div.appendChild(divDelete);
       
        

        var primDiv = document.getElementById("primary");

        primDiv.appendChild(div);

        $(imageDel).click(function () {
            div.parentNode.removeChild(div);
        });


    });


}




function slide() {
    var w = $(window).width();
    var h = $(window).height();
    var slides = $('.Slides > div');
    $('.SlideContainer').css({ height: (h - 60) + 'px' });
    $('.Slides').css({ width: slides.length + '00%' });
    slides.css({ width: w + 'px' });
    var pos = 0;

    $('.Left').click(function () {
        pos--;
        $('.Slides').stop().animate({ left: (pos * w) + 'px' });
    });
    $('.Right').click(function () {
        pos++;
        $('.Slides').stop().animate({ left: (pos * w) + 'px' });
    });
    $('#design').click(function () {
        document.getElementById('goal').className = "flow-step";
        document.getElementById('design').className = "flow-step active";
        document.getElementById('description').className = "flow-step";
        pos = 0;
        $('.Slides').stop().animate({ left: (pos * w) + 'px' });
    });
    $('#goal').click(function () {

        document.getElementById('goal').className = "flow-step active";
        document.getElementById('design').className = "flow-step";
        document.getElementById('description').className = "flow-step";

        pos = -1;
        $('.Slides').stop().animate({ left: (pos * w) + 'px' });
    });
    $('#description').click(function () {
        document.getElementById('goal').className = "flow-step";
        document.getElementById('design').className = "flow-step";
        document.getElementById('description').className = "flow-step active";
        pos = -2;
        $('.Slides').stop().animate({ left: (pos * w) + 'px' });
    });
}
function onChangeTrackBar() {
    document.getElementById('trackBarValue').innerHTML = document.getElementById('trackbar').value;
    document.getElementById('total_profit').innerHTML = (document.getElementById('trackbar').value) * 10 + "$";
}


