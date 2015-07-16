var app = {
    loadProducts: function(){
        return jQuery.ajax({
            url: this.urlPrefix+this.urls.products
        });
    },
    loadFonts: function(){
        return jQuery.ajax({
            url: this.urlPrefix+this.urls.fonts
        });
    },
    loadRandomArt: function(){
        return jQuery.ajax({
            url: this.urlPrefix+this.urls.randomArt
        });
    },
    searchArt: function(query, page){
        page = page||0;
        return jQuery.ajax({
            url: this.urlPrefix + this.urls.searchArt,
            data: {query: query, page: page}
        })
    },
    loadSwatches: function(){
        return jQuery.ajax({
            url: this.urlPrefix+this.urls.swatches
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
        snapToCenter: true
    }
};

jQuery(function() {
	app.ini(baseURL, dataUrls);
    app.state.print_type='screen';
});