function  loadImage(url, callback) {
    var img = new Image();
    img.onload = function () {
        var canvas = document.createElement('canvas');
        var context = canvas.getContext('2d');

        document.body.appendChild(canvas);

        var width = canvas.width = img.width;
        var height = canvas.height = img.height;

        context.drawImage(img, 0, 0, width, height);
        callback({
            canvas: canvas,
            context: context,
            width: width,
            height: height
        });
    }
    img.src = url;
}

function checkPoint(image, x, y, imageData) {
    //var d = [[0, -2], [-1, -1], [0, -1], [1, -1], [-2, 0], [-1, 0], [0, 0], [1, 0], [2, 0], [-1, 1], [0, 1], [1, 1], [0, 2]];
    var d = [[0, 0]];
    var r = 0, g = 0, b = 0, a = 0;
    for(var i = 0; i < d.length; i++) {
        var offset = ((y + d[i][0]) * image.width + x + d[i][1]) * 4;
        r += imageData.data[offset + 0] / d.length;
        g += imageData.data[offset + 1] / d.length;
        b += imageData.data[offset + 2] / d.length;
        a += imageData.data[offset + 3] / d.length;
    }
    var color = 'rgb(' + r + ',' + g + ',' + b + ')';
    return color;
}

function countColors(url, callback) {
    loadImage(url, function (image) {
        var imageData =image.context.getImageData(0, 0, image.width, image.height);
        var pixels = imageData.data;
        var pixelCount = image.width*image.height;

        // Store the RGB values in an array format suitable for quantize function
        var colorHash = {};
        var colors = [];
        for (var x = 2; x < image.width - 2; x+=5) {
            for (var y = 2; y < image.height - 2; y+=5) {
                var color = checkPoint(image, x, y, imageData);
                if (color) {
                    if (!colorHash[color]) {
                        colorHash[color] = true;
                        colors.push(color);
                        if (colors.length > 10) {
                            //break;
                        }
                    }
                }
            }
        }
        image.canvas.parentNode.removeChild(image.canvas);
        callback(colors);
    });
}