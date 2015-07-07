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
    state: {}
};

jQuery(function() {
	app.ini(baseURL, dataUrls);
});