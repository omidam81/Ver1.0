window.onload = function initWizard() {
    var w = $(window).width();
    var h = $(window).height();
    var slides = $('.Slides > div');
    $('.SlideContainer').css({ height: (h - 60) + 'px' });
    $('.Slides').css({ width: slides.length + '00%' });
    slides.css({ width: w + 'px' });

    var pos = 0;
    slide();
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
