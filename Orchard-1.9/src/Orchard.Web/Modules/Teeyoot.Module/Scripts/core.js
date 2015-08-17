var app = {
    loadProducts: function(){
        return jQuery.ajax({
            //url: this.urlPrefix+this.urls.products  
            url: this.urls.products  
        });
    },
    loadFonts: function(){
        return jQuery.ajax({
            //url: this.urlPrefix+this.urls.fonts
            url: this.urls.fonts
        });
    },
    loadRandomArt: function(){
        return jQuery.ajax({
            //url: this.urlPrefix+this.urls.randomArt
            url: this.urls.randomArt
        });
    },
    searchArt: function(query, page){
        page = page||0;
        return jQuery.ajax({
            //url: this.urlPrefix + this.urls.searchArt,
			url: this.urls.searchArt,
            data: {query: query, page: page}
        })
    },
    loadSwatches: function(){
        return jQuery.ajax({
            //url: this.urlPrefix+this.urls.swatches
            url: this.urls.swatches
        });
    },
    getPrice:function(params){
        return jQuery.ajax({
            url: this.urlPrefix+this.urls.productPricing,
            data: params
        });
    },
	ini: function(urlPrefix, urls){
        this.urlPrefix = urlPrefix;
        this.urls = urls;
        this.loadProducts();
        this.loadFonts();
        this.loadSwatches();
	},
    state: {
        getView: function(value){
            if(typeof value === 'undefined'){
                value = this.isFront;
            }
            return value?'front':'back';
        },
        getImage: function(){
            return design.products.images[this.product.id];
        },
        getUsedColorsCount: function (view) {
            var usedColors = this['usedColors_' + (view || this.getView())];
            var count = Object.keys(usedColors).length;
            if (count > maxDesignColors) {
                $('#color-warning').show();
                $('#quote').hide();
            } else {
                $('#color-warning').hide();
                $('#quote').show();
            }
            return count;
        },
        getUsedColorsCountBack: function (view) {
            var usedColors = this['usedColors_back'];
            var count = Object.keys(usedColors).length;
            if (count > maxDesignColors) {
                $('#color-warning').show();
                $('#quote').hide();
            } else {
                $('#color-warning').hide();
                $('#quote').show();
            }
            return count;
        },
        getUsedColorsCountFront: function (view) {
            var usedColors = this['usedColors_front'];
            var count = Object.keys(usedColors).length;
            if (count > maxDesignColors) {
                $('#color-warning').show();
                $('#quote').hide();
            } else {
                $('#color-warning').hide();
                $('#quote').show();
            }
            return count;
        },
        useColors: function (colors) {
            if (colors === 'none') {
                return;
            }
            if (typeof colors === 'string') {
                colors = [colors];
            }
            var usedColors = this['usedColors_' + this.getView()];
            for (var i = 0; i < colors.length; i++) {
                var color = colors[i];
                if (!usedColors[color]) {
                    usedColors[color] = 0;
                }
                usedColors[color]++;
            }
            console.log('colors: ' + this.getUsedColorsCountFront());
            console.log('where: ' + this.getView());
            console.log('colors: ' + this.getUsedColorsCountBack());
            console.log('where: ' + 'back');
            //var elem1111 = document.getElementById("price_preview");
            //elem1111.innerText = "asdasd";
            calculatePrice(this.getUsedColorsCountFront(), this.getUsedColorsCountBack());
        },
        unuseColors: function (colors) {
            if (colors === 'none') {
                return;
            }
            if (typeof colors === 'string') {
                colors = [colors];
            }
            var usedColors = this['usedColors_' + this.getView()];
            for (var i = 0; i < colors.length; i++) {
                var color = colors[i];
                if (usedColors[color]) {
                    usedColors[color]--;
                    if (usedColors[color] === 0) {
                        delete usedColors[color];
                    }
                }
            }
            console.log('colors: ' + this.getUsedColorsCount());
            calculatePrice(this.getUsedColorsCountFront(), this.getUsedColorsCountBack());
        },
        snapToCenter: true,
        usedColors_front: {},
        usedColors_back: {}
}
};

jQuery(function() {
	app.ini(baseURL, dataUrls);
    app.state.print_type='screen';
});