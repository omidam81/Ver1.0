var aniSpeed = 200;
app.state.currentProduct = {
    ProductId: 95,
    BaseCost: 7.14,
    ColorId: 2260,
    Price:  parseFloat(String(document.getElementById("profSale").value).match(/-?\d+(?:\.\d+)?/g, '') || 0, 10).toFixed(2),
    CurrencyId:1

};
var wasMouseUp = false;
var isResizing = false;
var design={
	zIndex: 1,
	design_id: null,
	design_file: '',
	designer_id: 0,
	design_key: 0,
	output: {},
	colors: [],
	teams: {},
	fonts: '',
	ini:function(){
		var me = this;
		
        $("#app-wrap").flip({
            trigger: 'manual'
        });
        $('.flip-button:not(.flip-button-active)').on('click', function(){
            design.products.changeView(app.state.getView(!app.state.isFront));
        });
        $('.design-area-zoom').on('click', function(){
            app.state.zoomed = !app.state.zoomed;
            if(app.state.zoomed){
                $('#design-area').css('transform', 'scale(1.4, 1.4)');
                var e = design.item.get();
                //design.item.checkBorders();
            }else{
                $('#design-area').css('transform', 'scale(1, 1)');
                var e = design.item.get();
                //design.item.checkBorders();
            }
            //design.products.setDesignAreaContrastColor(app.state.color);
            
        });
        $(document.body).on('click', function(event){
            if(!$(event.target).is('button')) {
                $('.containertip--open').removeClass('containertip--open');
            }
        });
        $('#flip-h, #art-flip-h').on('click', function () {
            design.item.flip('h');
        });
        $('#flip-v, #art-flip-v').on('click', function () {
            design.item.flip('v');
        });
        $('#item-center, #art-item-center').on('click', function(){
            var item = design.item.get();
            if(item.length){
                design.item.center();
                design.item.placeSizeBox(item);
                design.item.checkBorders(item);
            }
        });
        $('#duplicate-text, #art-duplicate').on('click', function () {
            var item = design.item.get();
            if(item.length){
                design.item.duplicate(item);
            }
        });
        $('#snap-to-center, #art-snap-to-center').on('change', function () {
            var isChecked = $(this).is(':checked');
            $('#snap-to-center, #art-snap-to-center').prop('checked', isChecked);
            app.state.snapToCenter = isChecked;
        });
        $('#push-back, #art-push-back').on('click', function () {
            design.item.pushBack();
        });
        $('#browse-artwork').on('click', function(e){
            e.preventDefault();
            e.stopPropagation();
            $('.containertip--open').removeClass('containertip--open');
            $('.design-art-search').addClass('containertip--open');
            $('#artwork-query').focus();
        });
        $('#artwork-upload').on('click', function(e){
            e.preventDefault();
            e.stopPropagation();
            $('.containertip--open').removeClass('containertip--open');
            $('#designer-art-choseType').hide();
            $('#dropbox').show();
        });
         $('#goBack').on('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            $('#dropbox').hide();
            $('#designer-art-choseType').show();
            });
	    $('.upload.select-file').click(function() {
	        $("input#file").click();
	    });
        $('#artwork-search-form').on('submit', function(e){
            e.preventDefault();
            e.stopPropagation();
            app.state.searchQuery = $('#artwork-query').val();
            design.designer.searchArt(false);
        });
        $('.art-search-container').on('scroll', function(){
            var padding = 10;
            var $this = $(this);
            if(app.state.searchQuery && $this.scrollTop()+padding>=$this[0].scrollHeight- $this.height() && !app.state.artSearching){
                design.designer.searchArt(true);
            }
        });
        $('.containertip, .size-dialog, .handle').on('click', function(event){
            if(!$(event.target).is('button')){
                event.preventDefault();
                event.stopPropagation();
            }
        });
        $('#enter-text').on('keyup', function(){
            var item = design.item.get();
            var text = $(this).val();
            if((!item.length || item.data('type') !== 'text')&& text){
                design.text.create();
            }else{
                if(text){
                    design.text.update('text');
                }else{
                    design.item.remove(design.item.get().children(':first')[0]);
                }
            }
        });
        $('.size-dialog input').on('input', function(){
            var $this = $(this);
            var item = design.item.get();
            var val = parseFloat($this.val());
            if(!isNaN(val) && item.length){
                if(val>30){
                    val = 30;
                }
                var isWidth = $this.hasClass('width');
                design.item.resizeTo(item, isWidth?val:undefined, !isWidth?val:undefined, true, true);
            }
        });
        $('#outline-select').on('change', function(){
            if(design.item.get().length){
                design.text.update('outline-width', $(this).val());
            }
        });
        $('#outline-select-art').on('change', function () {
            if (design.item.get().length) {
                design.text.update('outline-width', $(this).val());
            }
        });
        design.item.move();
		
		$jd('.dg-color-picker-active').click(function(){
			$jd(this).parent().find('ul').show(aniSpeed);
		});
		
		/* rotate */
		$jd('.rotate-refresh').click(function(){
			me.item.refresh('rotate');
		});
		$jd('.rotate-value').on("focus change", function(){
			var e = me.item.get();
			var angle = ($(this).val() * Math.PI)/180;
			e.rotatable("setValue", angle);	
		});
		
		/* lock */
		$jd('.ui-lock').click(function(){
			var e = me.item.get();
			e.resizable('destroy');
			if($(this).is(':checked')) me.item.resize(e, 'n, e, s, w, se');
			else me.item.resize(e, 'se');
		});
		
		/* menu */
		$jd('.menu-left a').click(function(){
			$jd('.menu-left a').removeClass('active');
			if($jd(this).hasClass('add_item_text')) me.text.create();
			if($jd(this).hasClass('add_item_team')) me.team.create();
			$jd(this).addClass('active');
		});
		
		/* share */
		$('.list-share span').click(function(){
			design.share.ini($(this).data('type'));
		});
		/* tools */
		$jd('a.dg-tool').click(function(){
			var f = $jd(this).data('type');
			switch(f){
				case 'preview':
					design.tools.preview(this);
					break;
				case 'undo':
					design.tools.undo(this);
					break;
				case 'redo':
					design.tools.redo(this);
					break;
				case 'zoom':
					design.tools.zoom();
					break;
				case 'reset':
					design.tools.reset(this);
					break;
			}
		});
		
		$('#product-attributes .size-number').keyup(function(){
			design.products.sizes();
		});
		$('#product-attributes .size-number').click(function(){
			design.team.changeSize();
		});
		design.products.sizes();
		
		$jd('.add_item_clipart').click(function(){
			me.designer.art.categories(true, 0);
			if( $('#dag-list-arts').html() === '')
				me.designer.art.arts('');
		});
		
		$jd('.add_item_mydesign').click(function(){
			me.ajax.mydesign();
		});
		
		$jd('#dag-art-panel a').click(function(){
			$('#dag-art-categories').children('ul').hide();
			var index = $jd('#dag-art-panel a').index(this);
			me.designer.art.categories(true, index);
			$('#dag-art-categories').children('ul').eq(index).toggle(aniSpeed);
		});
		$jd('#dag-art-detail button').click(function(){
			$('#dag-art-detail').hide(aniSpeed);
			$('#dag-list-arts').show(aniSpeed);
			$('#arts-add').hide();
			$('#arts-pagination').css('display', 'block');
		});
		
		/* mobile toolbar */
		$('.dg-options-toolbar button').click(function(){
			var check = $(this).hasClass('active');
			$('.dg-options-toolbar button').removeClass('active');
			var elm = $(this).parents('.dg-options');
			var type = $(this).data('type');
			
			if (check)
			{
				elm.children('.dg-options-content').removeClass('active');
				$('.toolbar-action-'+type).removeClass('active');
			}
			else
			{				
				$(this).addClass('active');				
				elm.children('.dg-options-content').addClass('active');
				elm.children('.dg-options-content').children('div').removeClass('active');
				$('.toolbar-action-'+type).addClass('active');
			}			
		});
		
		$('#close-product-detail').click(function(){
			$('#dg-products .products-detail').hide(aniSpeed);
			$('#dg-products .product-detail.active').removeClass('active');
		});
		
		/* text update */
		$jd('.text-update').each(function(){
			var e = $jd(this);
			e.bind(e.data('event'), function(){
				if (e.data('value') !== 'undefined')
					design.text.update(e.data('label'), e.data('value'));
				else
					design.text.update(e.data('label'));
			});
		});
		
		//design.item.designini(items);
		design.designer.loadColors();
		design.designer.loadFonts();
		design.designer.fonts = {};
		design.designer.fontActive = {};
		design.products.productCate(0);
        design.designer.loadRandomArt();
		//design.ajax.getPrice();
	},
	ajax:{
		form: function(){
			var datas = {};
			
			datas.product_id = product_id;
			
			/* get product color */
			var hex = design.exports.productColor();
			var index = $('#product-list-colors span').index($('#product-list-colors span.active'));					
			datas.colors = {};
			datas.colors[index] = hex;			
			
			/* get Design color and size*/
			var colors 				= {};
			colors.front 		= design.print.colors('front');			
			colors.back 		= design.print.colors('back');			
			colors.left 		= design.print.colors('left');			
			colors.right 		= design.print.colors('right');
			
			datas.print 		= {};			
			datas.print.sizes 	= JSON.stringify(design.print.size());
			datas.print.colors 	= JSON.stringify(colors);
			
			/* product attribute */
			var attributes = $('#tool_cart').serialize();
			if (attributes !== '')
			{
				var obj = JSON.parse('{"' + decodeURI(attributes).replace(/"/g, '\\"').replace(/&/g, '","').replace(/=/g,'":"') + '"}');
				datas = $.extend(datas, obj);
			}
			
			datas.cliparts = design.exports.cliparts();
			
			return datas;
		},
		getPrice: function(){
			var datas = this.form();
			
			var lable = $('#product-price .product-price-title');
			var div = $('#product-price .product-price-list');
			var title = lable.html();
			div.css('opacity', 0.1);
			lable.html('Updating...');
			$.ajax({
				type: "POST",
				dataType: "json",
				url: baseURL + "cart/prices",
				data: datas
			}).done(function( data ) {
				if (data !== '')
				{
					if (typeof data.sale != 'undefined')
					{
						$('.price-sale-number').html(data.sale);
						$('.price-old-number').html(data.old);
						
						if (data.sale === data.old)
							$('#product-price-old').css('display', 'none');
						else
							$('#product-price-old').css('display', 'inline');
					}
				}
			}).always(function(){
				lable.html(title);
				div.css('opacity', 1);
				design.print.colors();
			});
		},
		addJs: function(e){
			var quantity = document.getElementById('quantity').value;
				quantity = parseInt(quantity);
			if (isNaN(quantity) || quantity < 1)
			{
				alert('Please add quantity or size');
				return false;
			}
			if (quantity < min_order){
				alert('Minimum quantity: '+min_order+'. Please add quantity or size.');
				return false;
			}
			design.mask(true);
			design.ajax.active = 'back';
			design.svg.items('front', design.ajax.save); //этот метод рендерит фронт и бек. В нем где-то косячит, если решим править Blur - то это сюда же, рендерить в больших размерах
		},
		active: 'back',
		save: function(){
			if (design.ajax.active === 'back')
			{
				design.ajax.active = 'left';
				if ($('#view-back .product-design').html() !== '' && $('#view-back .product-design').find('img').length > 0)
				{
					design.svg.items('back', design.ajax.save);
				}
				else
				{
					delete design.output.back;
					design.ajax.save();
				}
			}
		},
		addToCart: function(){
			var options		= {};
			options.vectors	= JSON.stringify(design.exports.vector());
			
			options.images	= {};
			if (typeof design.output.front != 'undefined')
				options.images.front = design.output.front.toDataURL();
				
			if (typeof design.output.back != 'undefined')
				options.images.back = design.output.back.toDataURL();
				
			if (typeof design.output.left != 'undefined')
				options.images.left = design.output.left.toDataURL();
				
			if (typeof design.output.right != 'undefined')
				options.images.right = design.output.right.toDataURL();
			
			var datas = design.ajax.form();
			datas.design = options;				
			datas.teams = design.teams;				
			datas.fonts = design.fonts;				
			$.ajax({
				type: "POST",
				data: datas,
				url: baseURL + "cart/addJs"					
			}).done(function( data ){
				if (data !== '')
				{
					var content = eval ("(" + data + ")");
					if (content.error === 0)
					{
						$('.cart-added-img').html('<img src="'+content.product.image+'" class="img-responsive" alt="">');
						$('.cart-added-info').html(content.product.quantity +' x '+ content.product.name);
						$('#cart_notice').modal();
						
						var module = $('#shopping-cart .badge');
						if (module.length > 0)
						{
							var quantity = Math.round(module.text());
							quantity	= parseInt(content.product.quantity) + parseInt(quantity);
							module.html(quantity);
						}
					}
					else
					{
						alert(content.msg);
					}
				}
			}).always(function(){				
				design.mask(false);
			});			
		},		
		mydesign: function(){
			if (user_id == 0)
			{
				$('#f-login').modal();
			}
			else
			{
				$('#dg-mydesign').modal();
				var div = $('#dg-mydesign .modal-body');
				div.addClass('loading');
				$.ajax({
					url: baseURL + "user/userDesign",
					cache: true
				}).done(function( data ){
					div.removeClass('loading');
					div.html(data);
				});
			}
		},
		removeDesign: function(e){
			$(e).parents('.design-box').remove();
			var id = $(e).data('id');
			$.ajax({
				url: baseURL + "user/removeDesign/"+id
			}).done(function( data ){});
		}
	},
	tools:{
		preview: function(e)
		{
			$('#dg-mask').css('display', 'block');
			var html 	= '<a class="left carousel-control" href="#carousel-slide" role="button" data-slide="prev">'
						+	'<span class="glyphicons chevron-left"></span>'
						+ '</a>'
						+ '<a class="right carousel-control" href="#carousel-slide" role="button" data-slide="next">'
						+	'<span class="glyphicons chevron-right"></span>'
						+ '</a>';
			if (document.getElementById('carousel-slide') == null)
			{
				var div = '<div id="carousel-slide" class="carousel slide" data-ride="carousel">'
						+ 	'<div class="carousel-inner"></div>'
						+ '</div>';
				$('#dg-main-slider').append(div);
			}
			else
			{
				$('#carousel-slide').html('<div class="carousel-inner"></div>');
			}
			if ($('#view-front .product-design').html() != '')
				design.svg.items('front');
				
			if ($('#view-back .product-design').html() != '')
				design.svg.items('back');
				
			if ($('#view-left .product-design').html() != '')
				design.svg.items('left');
				
			if ($('#view-right .product-design').html() != '')
				design.svg.items('right');
			setTimeout(function(){
				if ($('#view-front .product-design').html() != ''){
					$('#carousel-slide .carousel-inner').append('<div class="item active"><div id="slide-front" class="slide-fill"></div><div class="carousel-caption">Front</div></div>');
					$('#slide-front').append(design.output.front);
				}
				
				if ($('#view-back .product-design').html() != ''){
					$('#carousel-slide .carousel-inner').append('<div class="item"><div id="slide-back" class="slide-fill"></div><div class="carousel-caption">Back</div></div>');
					$('#slide-back').append(design.output.back);
				}
				
				if ($('#view-left .product-design').html() != ''){
					$('#carousel-slide .carousel-inner').append('<div class="item"><div id="slide-left" class="slide-fill"></div><div class="carousel-caption">Left</div></div>');
					$('#slide-left').append(design.output.left);
				}
				
				if ($('#view-right .product-design').html() != ''){
					$('#carousel-slide .carousel-inner').append('<div class="item"><div id="slide-right" class="slide-fill"></div><div class="carousel-caption">Right</div></div>');
					$('#slide-right').append(design.output.right);
				}
				$('#dg-mask').css('display', 'none');
				$('#carousel-slide').append(html);
				$('#dg-preview').modal();
				$('#carousel-slide').carousel();
			}, 500);
		},
		undo: function(e)
		{			
		},
		redo: function(e)
		{
			var vector = design.exports.vector();
			var str = JSON.stringify(vector);
			design.imports.vector(str, 'front');
		},
		zoom: function()
		{
			var view = $('.labView.active .design-area'),
				width = view.width(),
				height = view.height();
			var id 		= $('.labView.active').attr('id');
			var postion = id.replace('view-', '');
			var area 	= eval ("(" + items['area'][postion] + ")");
			if (view.hasClass('zoom'))
			{
				var colorIndex = $('#product-list-colors span').index($('#product-list-colors span.active'));				
				view.removeClass('zoom');
				view.css({"width": area.width, "height": area.height, "top":area.top, "left":area.left});
				
				var images 	= eval ("(" + items['design'][colorIndex][postion] + ")");
				$.each(images, function(i, image){
					if (image.id !== 'area-design')
					{
						$('#'+postion+'-img-'+image.id).css({"width":image.width, "height":image.height, "left":image.left,"top":image.top});
					}
				});
				
				this.changeZoom(view, true);
			}
			else
			{
				view.addClass('zoom');
				if ( (500 - width) > (500 - height))
				{
					var newHeight = 500,
						newWidth = (newHeight * width) / height,
						zoomIn = (500/height);
				}
				else
				{				
					var newWidth = 500,
						newHeight = (newWidth * height) / width,
						zoomIn = (500/width);
				}
				var left 	= Math.round((500 - newWidth)/2);
				var top 	= Math.round((500 - newHeight)/2);
				var zoomT 	= (design.convert.px(area.top)*zoomIn - top);
				var zoomL 	= (design.convert.px(area.left)*zoomIn - left);
				$('.labView.active .product-design').find('img').each(function(){
					var imgW = design.convert.px(this.style.width)*zoomIn,
						imgH = design.convert.px(this.style.height)*zoomIn,
						imgT = design.convert.px(this.style.top)*zoomIn,
						imgL = design.convert.px(this.style.left)*zoomIn;
						
					$(this).css({"width":imgW, "height":imgH, "top":imgT-zoomT, "left":imgL-zoomL});
				});
				view.css({"width": Math.round(newWidth), "height": Math.round(newHeight), "top":top, "left": left});
				
				view.data('zoom', zoomIn);
				view.data('zoomL', zoomL);
				view.data('zoomT', zoomT);
				this.changeZoom(view, false);
			}
		},
		changeZoom: function(view, type){
			var zoomIn 	= view.data('zoom'),
				zoomT 	= view.data('zoomT'),
				zoomL 	= view.data('zoomL');
			$('.labView.active').find('.drag-item').each(function(){
				var css = {};
					css.top 	= design.convert.px(this.style.top);
					css.left 	= design.convert.px(this.style.left);
					css.width 	= design.convert.px(this.style.width);
					css.height 	= design.convert.px(this.style.height);
				
				var svg = $(this).find('svg');
				var img = $(svg).find('image');				
				var itemsCss	= {};
				if (type == false)
				{
					itemsCss.top 	= css.top * zoomIn - 0;
					itemsCss.left 	= css.left * zoomIn - 0;
					itemsCss.width 	= css.width * zoomIn;
					itemsCss.height = css.height * zoomIn;
					if (typeof img[0] != 'undefined')
					{
						var imgW 	= img[0].getAttributeNS(null, 'width') * zoomIn;
						var imgH 	= img[0].getAttributeNS(null, 'height') * zoomIn;
					}
				}
				else
				{
					itemsCss.top 	= (css.top + 0)/zoomIn;
					itemsCss.left 	= (css.left + 0)/zoomIn;
					itemsCss.width 	= css.width / zoomIn;
					itemsCss.height = css.height / zoomIn;
					if (typeof img[0] != 'undefined')
					{
						var imgW 	= img[0].getAttributeNS(null, 'width') / zoomIn;
						var imgH 	= img[0].getAttributeNS(null, 'height') / zoomIn;
					}
				}
				$(this).css({"width": itemsCss.width, "height": itemsCss.height, "top":itemsCss.top, "left": itemsCss.left});
				svg[0].setAttributeNS(null, 'width', itemsCss.width);
				svg[0].setAttributeNS(null, 'height', itemsCss.height);
				if (typeof img[0] != 'undefined')
				{
					img[0].setAttributeNS(null, 'width', imgW);
					img[0].setAttributeNS(null, 'height', imgH);
				}
			});
		},
		reset: function(e)
		{
			var remove = confirm('Are you sure you want to reset design?');
			if (remove == true)
			{
				var view = $('#app-wrap .labView.active');
				view.find('.drag-item').each(function(){
					var id = $(this).attr('id');
					var index = id.replace('item-', '');
					design.layers.remove(index);
				});
			}
		}
	},
	print:{
		colors:function(view){
			if ($('#view-'+view+ ' .product-design').html() == '')
			{
				return design.colors;
			}
			
			if (app.state.print_type == 'screen' || app.state.print_type == 'embroidery')
			{
				if (typeof view != 'undefined')
					view = ' #view-'+view;
				else
					view = '';
				design.colors = [];
				$('#app-wrap'+view).find('svg').each(function(){
					var o = document.getElementById($(this).parent().attr('id'));
					if(o.item.confirmColor == true && typeof o.item.colors != 'undefined')
					{
						var colors = o.item.colors;
						$.each(colors, function(i, hex){
							if ($.inArray(hex, design.colors) == -1 && hex != 'none')
							{
								design.colors.push('#'+hex);
							}
						});					
					}
					else
					{
						var colors = design.svg.getColors($(this));
						$.each(colors, function(hex, i){
							if ($.inArray(hex, design.colors) == -1 && hex != 'none')
							{
								design.colors.push(hex);
							}
						});
					}
				});
				$('.color-used').html('<div id="colors-used" class="list-colors"></div>');
				var div = $('#colors-used');
				$.each(design.colors, function(i, hex){
					div.append('<span style="background-color:'+hex+'" class="bg-colors"></span>');
				});
				return design.colors;
			}else{
				$('.color-used').html('<div id="colors-used" class="list-colors"></div>');				
				return design.colors;
			}
		},
		size:function(){
			var sizes = {};
			var positions = ['front', 'back'];
			$('.screen-size').html('<div id="sizes-used"></div>');
			
			$.each(positions, function(i, postion){
				if ($('#view-'+postion+ ' .content-inner').html() != '' && $('#view-'+postion+ ' .product-design').html() != '')
				{
					var top = 500, left = 500, right = 500, bottom = 500, area = {}, print = {};
					var div = $('#view-'+postion+ ' .design-area');
					area.width = design.convert.px(div.css('width'));
					area.height = design.convert.px(div.css('height'));
					
					$('#view-'+postion+ ' .drag-item').each(function(){
						var o = {}, e = $(this);
						o.left = design.convert.px(e.css('left'));
						o.top = design.convert.px(e.css('top'));
						o.width = design.convert.px(e.css('width'));
						o.height = design.convert.px(e.css('height'));
						o.right = area.width - o.left - o.width;
						o.bottom = area.height - o.top - o.height;
						
						if (o.left < 0) o.left = 0;
						if (o.top < 0) o.top = 0;
						if (o.right < 0) o.right = 0;
						if (o.bottom < 0) o.bottom = 0;
						
						if (o.top < top) top = o.top;
						if (o.left < left) left = o.left;
						if (o.right < right) right = o.right;
						if (o.bottom < bottom) bottom = o.bottom;
					});
					print.width 	= area.width - left - right;
					print.height 	= area.height - top - bottom;
                    var imageData = app.state.getImage();
                    var ppi = imageData.ppi;
					sizes[postion] = {};
					sizes[postion].width = Math.round( print.width * ppi * 25.4 );
					sizes[postion].height = Math.round( print.height * ppi * 25.4 );
					
					if (
						(sizes[postion].width < 21 && sizes[postion].height < 29)
					 || (sizes[postion].width < 29 && sizes[postion].height < 21)
					) 
						sizes[postion].size = 4;
					else sizes[postion].size = 3;
					$('#sizes-used').append('<div class="text-center"><strong>'+postion+'</strong><br /><span class="paper glyphicons file"><strong>A'+sizes[postion].size+'</strong></span></div>');
				}
			});			
			return sizes;
		},
		addColor: function(e){
			if ($(e).hasClass('active'))
			{
				$(e).removeClass('active');
			}
			else
			{
				$(e).addClass('active');
			}
		}
	},
	designer:{
		fonts: {},
		fontActive: {},
		loadColors: function(){
			var me = this;
			app.loadSwatches().then(function(data){
			    me.addColor(data);
			    me.swatches = data;
			});
		},
		toRgbColor: function (color) {
		    return 'rgb(' + color.rgb[0] + ',' + color.rgb[1] + ',' + color.rgb[2] + ')';
		},
		fromRgbColor: function (color) {
		    var chunks = color.replace('rgb(', '').replace(')', '').split(',');
		    chunks[0] = parseInt(chunks[0]);
		    chunks[1] = parseInt(chunks[1]);
		    chunks[2] = parseInt(chunks[2]);
		    for (var i = 0; i < this.swatches.length; i++) {
		        color = this.swatches[i];
		        if (color.rgb[0] === chunks[0] && color.rgb[2] === chunks[2] && color.rgb[2] === chunks[2]) {
		            return color;
		        }
		    }
		},
		showColor: function ($picker, color) {
            var $swatch = $('.swatch:first', $picker);
            var rgb = design.designer.toRgbColor(color);
            $swatch.css('background-color', rgb);
        },
		addColor: function(colors){
            var me = this;

            var $colorsContainers = $('.all-colors');
            app.state.colorsInUse = [];
            //design.products.colors = {};

			$.each(colors, function(i, color){
                if(!color.inStock){
                    return;
                }
                var rgb = 'rgb('+color.rgb[0]+','+color.rgb[1]+','+color.rgb[2]+')';
                var colorHtml = '<li data-value="'+color.id+')" class="shirt-color-sample" title="'+
                    color.name+'" style="background-color:'+rgb+';"></li>';
                $colorsContainers.each(function(){
                    var $colorHtml = $(colorHtml);
                    $colorHtml.on('click', function (event) {
                        event.preventDefault();
                        event.stopPropagation();
                        var $picker = $(this).parents('.color-picker:first');
                        var category = $picker.data('value');
                        app.state['color-' + category] = color;
                        me.showColor($picker, color);
                        if (design.item.get().length) {
                            switch (category) {
                                case 'text':
                                    design.text.update('color');
                                    break;
                                case 'art':
                                    design.text.update('art-color');
                                    break;
                                case 'outline':
                                    design.text.update('outline');
                                    break;
                                case 'art-outline':
                                    design.text.update('art-outline');
                                    break;
                            }
                        }
                        $('.containertip--open').removeClass('containertip--open');
                    }).hover(
                        function() {
                            var $picker = $(this).parents('.color-picker:first');
                            me.showColor($picker, color);
                        }, function() {
                            var $picker = $(this).parents('.color-picker:first');
                            var category = $picker.data('value');
                            me.showColor($picker, app.state['color-'+category]);
                        });
                    $(this).append($colorHtml);
                });
			});
            $colorsContainers.each(function(){
                $(this).find('li:first').click();
            });
            $('.color-picker').on('click', function(event){
                event.preventDefault();
                event.stopPropagation();
                $('.containertip--open').removeClass('containertip--open');
                $(this).parents(':first').find('.shirt-colors').addClass('containertip--open');
            });
        },
        getArtPreviewUrl: function(art){
            if(art.preview_url_42){
                return art.preview_url_42;
            }
            return assetsUrls.art+art.filename.substring(0, art.filename.length-4)+'.png';
        },
        getArtUrl: function(art){
            if(art.icon_url){
                return art.icon_url;
            }
            return assetsUrls.art+art.filename;
        },
        addAsRecentArt: function(art) {
            var $list = $('#clientArts ul');
            var $images = $list.find('img[src="' + art.src + '"]');
            if ($images.length) {
                return;
            }
            var $html = $('<li><div class="valign-outer"><div class="valign-middle"><div class="valign-inner">' +
                '<img class="art-search-preview" data-url-svg="' + art.svg + '" src="' + art.src + '">' +
                '</div></div></div></li>');
            $('#labArt').css('display', 'inline');
            $list.append($html);
            $html.on('click', function() {
                var url = $(this).find('img').data('url-svg');
                design.art.create({ item: { url: url, file_type: 'svg', change_color: 1 } });
            });
        },
        addArt: function(data){
            var me = this;
            var arts = data || design.products.art;
            var $container = $('.art-search-container');
            for(var i=0;i<arts.length;i++){
                var art = arts[i];
                var $div = $('<div class="art-search-tile"></div>');
                $div.append('<img class="art-search-preview" data-id="'+art.id
                    +'" data-url-svg="'+me.getArtUrl(art)
                    +'" src="'+me.getArtPreviewUrl(art)+'">');
                if(art.isNoun){
                    $div.append('<img class="art-search-preview-noun-logo" src="./assets/images/tnp.png"'+
                        ' title="The Noun Project">');
                }
                $container.append($div);
                $div.on('click', function (event) {
                    var url = $(this).find('img').data('url-svg');
                    var src = $(this).find('img').attr('src');
                    design.art.create({ item: { url: url, file_type: 'svg', change_color: 1 } });
                    me.addAsRecentArt({ svg: url, src: src});
                    $('.containertip--open').removeClass('containertip--open');
                });
            }
        },
        searchArt: function(append){
            var me = this;
            var query = app.state.searchQuery;
            if (query == "") {
                var $container = $('.art-search-container');
                app.loadRandomArt().then(function (data) {
                    $container.html('');
                    design.products.art = data;
                    me.addArt();
                });
            } else {
                var $container = $('.art-search-container');
                if (!append) {
                    $container.html('<div class="loading"><img src="./assets/images/small_loadwheel.gif"></div>');
                    app.state.currentArtSearchPage = 0;
                } else {
                    app.state.currentArtSearchPage = app.state.currentArtSearchPage || 0;

                }
                app.state.artSearching = true;
                app.searchArt(query, app.state.currentArtSearchPage).then(function (data) {
                    app.state.currentArtSearchPage++;
                    if (!append) {
                        $container.html('');
                        if (!data || !data.length) {
                            $container.html('<div class="no-results" ><div class="alert alert-block alert-warning">' +
                                '<h4 class="alert-heading">Sorry!</h4><p data-select-like-a-boss="1">No results were found ' +
                                'for your query. Please try again! We recommend keeping it simple like "dog" or "cat".</p>' +
                                '</div></div>');
                        }
                        design.products.art = data;
                    } else {
                        var current = design.products.art || [];
                        design.products.art = current.concat(data);
                    }
                    me.addArt(data);
                }).always(function () {
                    app.state.artSearching = false;
                });
            }           
        },
        loadRandomArt: function(){
            var me= this;
            var $container = $('.art-search-container');
            app.loadRandomArt().then(function(data){
                $container.html('');
                design.products.art = data;
                me.addArt();
            });
        },
		loadFonts: function(){
			var me = this;
            app.loadFonts().then(function(data){
                var fonts = {};
                var categories = {};
                for(var i=0;i<data.length;i++){
                    var font = data[i];
                    fonts[font.id] = font;
                    var tags;
                    if(font.tags && typeof font.tags === 'string'){
                        tags = JSON.parse(font.tags);
                    }else{
                        tags = font.tags;
                    }
                    for(var j=0;j<tags.length;j++){
                        var tag = tags[j];
                        if(!categories[tag]){
                            categories[tag] = [];
                        }
                        categories[tag].push(font.id);
                    }
                }
                design.products.fonts = fonts;
                design.products.fontCategories = categories;
                me.addFonts();
            });
		},
		addFonts: function(){
			var me = this;
            var categories = Object.keys(design.products.fontCategories);
            var $categories = $('#font-select-menu .category-menu');
            for(var i=0;i<categories.length;i++){
                var category = categories[i];
                var html = '<option value="'+category+'">'+category+'</option>';
                $categories.append(html);
            }
            $categories.on('change', function(event){
                var categoryName = event.target.value;
                var $fonts = $('#font-select-menu .options');
                $fonts.html('');
                var fontIds = design.products.fontCategories[categoryName];
                $.each(fontIds, function(i, id){
                    var font = design.products.fonts[id];
                    var $html = $('<li>'+font.family+'</li>');
                    var url = assetsUrls.fonts+font.filename+'.png';
                    $html.css('background-image', 'url("'+url+'")');
                    $html.on('click', function(event){
                        event.preventDefault();
                        event.stopPropagation();
                        app.state.font = font;
                        var html = '<img src="'+url+'">';
                        $('#font-select').html(html);
                        me.changeFont(font);
                        $(document.body).click();
                    });
                    $fonts.append($html);
                });
                if(!app.state.font){
                    $fonts.find('li:first').click();
                }
            });
            $('#font-select').on('click', function(event){
                event.stopPropagation();
                event.preventDefault();
                $(document.body).click();
                $('.containertip--open').removeClass('containertip--open');
                $('#font-select-menu').addClass('containertip--open');
            });
            $categories.change();
		},
		changeFont: function(font){
		    var id = font.family;
		    //design.fontsClear = design.fontsClear || '';
			if (typeof id != 'undefined')
			{
				if (typeof design.designer.fontActive[id] === 'undefined') {
                    if (font.filename) {
                        var url = assetsUrls.fonts + font.filename + '-webfont.woff';
                        var urlSvg = assetsUrls.fonts + font.filename + '-webfont.svg';
                        design.designer.fontActive[id] = true;
                        var fontInnerCss = "@font-face{font-family:\"" + id + "\";font-style: normal; font-weight: 400;src: local('" + id + "'), local('" + id +
                            "'), url(" + url + ") format('woff');}";
                        var css = "<style type='text/css'>"+fontInnerCss+"</style>";
                        design.fonts = design.fonts + ' ' + css;
                        //design.fontsClear += ' ' + fontInnerCss;
                        $('head').append(css);
                    }
                }
                var e = design.item.get();
                if(e.length){
                    design.text.update('fontfamily', id);
                    FontDetect.onFontLoaded(id, function () {
                        window.setTimeout(function () {
                            var txt = e.find('text');
                            var size1 = txt[0].getBBox();

                            var $w = parseInt(size1.width * (e[0].item.scaleX || 1));
                            var $h = parseInt(size1.height * (e[0].item.scaleY || 1));

                            design.item.updateSize($w, $h);

                            var svg = e.find('svg'),
                                view = svg[0].getAttributeNS(null, 'viewBox');
                            //var arr = view.split(' ');
                            //var y = txt[0].getAttributeNS(null, 'y');
                            //y = Math.round(y) + Math.round(size2.top) - Math.round(size1.top) - ( (Math.round(size2.top) - Math.round(size1.top)) * (($w - arr[2])/$w) );
                            //txt[0].setAttributeNS(null, 'y', y);

                            design.item.placeSizeBox(e);
                            design.item.checkBorders(e);
                        }, 200);
                    }, { msTimeout: 6000 });

                }
			}
		}
	},
	products:{
		categories: '',
		sizes: function(){
			var sizes = 0;
			$('#product-attributes .size-number').each(function(){
				var value = this.value;
				if (value == '') 
				{
					$(this).val(0);
					value = 0;
				}
				if (isNaN(value) == true || value < 0){
					$(this).val(0);
					value = 0;
				}
				sizes = parseInt(sizes) + parseInt(value);
			});
			//document.getElementById('quantity').value = sizes;
		},
		changeView: function (position) {
		    //Очищение значиения value 19/08/15
		    document.getElementById("file").value = "";
            app.state.isFront = position ==='front';
            $("#app-wrap").flip(!app.state.isFront);

            design.item.unselect();
			design.layers.setup();
			//design.team.changeView();
		},
		changeColor: function(color)
		{
            this.setDesignAreaContrastColor(color);
            app.state.color = color;
			design.item.designini();
			//design.products.changeView('front');
		},
		setDesignAreaContrastColor: function (color, isError) {
            //Очищение значиения value 19/08/15
		    document.getElementById("file").value = "";
            var newColor, isDarkBorder, textColor, isDarkText;
            if(!isError){
                var hex = color.value || color;
                var $div = $('<div style="background:'+hex+'"></div>');
                var background = $div.css('background-color');
                var rgb = background.replace(/^(rgb|rgba)\(/,'').replace(/\)$/,'').replace(/\s/g,'').split(',');
                var yiq = ((rgb[0]*299)+(rgb[1]*587)+(rgb[2]*114))/1000;
                isDarkBorder = yiq >= 128;
                newColor = isDarkBorder?'rgba(0,0,0,0.5)':'rgba(255,255,255,0.3)';
                textColor = !isDarkBorder?'rgba(0,0,0)':'rgba(255,255,255)';
                isDarkText = !isDarkBorder;
            }else{
                isDarkBorder = false;
                newColor = color;
                textColor = 'rgba(255,255,255)';
                isDarkText = false;
            }

           
            var isZoomed = !!app.state.zoomed;
            var selector = '.zoom-'+(!isDarkText?'light':'dark')+'-'+(isZoomed?'out':'in');
            $('.printable-area-zoom-image').hide();
            $(selector).stop().show();
            if (document.querySelector('.design-area').offsetLeft < 0) {
                $('.design-area').css({ 'display': 'none' });
                $('.design-area').css('border-color', newColor);
                $('.printable-area-toolbar')
               .toggleClass('dark', isDarkBorder)
               .css({ 'background-color': newColor, color: textColor });
            } else {
                if (app.state.isFront) {
                    $('#view-front-design-area').css('border-color', newColor);
                    $('#printable-area-front')
               .toggleClass('dark', isDarkBorder)
               .css({ 'background-color': newColor, color: textColor });
                }
                else {
                    $('#view-back-design-area').css('border-color', newColor);
                    $('#printable-area-back')
               .toggleClass('dark', isDarkBorder)
               .css({ 'background-color': newColor, color: textColor });
                }
            }

                      
            
        },
		changeDesign: function(product){
            app.state.product = product;
            var me = this;
            if(typeof app.state.isFront === 'undefined'){
                this.changeView('front');
            }
			$('#app-wrap .product-design').html('');//???
            var list_color = $('#lab-product-colors');

            list_color.html('');
            $.each(product.colors_available, function(i, colorId){
                /* add color */
                var color = me.colors[colorId];
                var $color = $('<li><a></a></li>')
                    .css('background', color.value)
                    .attr('title', color.name)
                    .on('click', function(){
                        design.products.changeColor(color);
                        //app.state.currentProduct.ColorId = parseInt(color.id);
                        if (parseInt(color.id) == app.state.currentProduct.SecondColorId) {
                            app.state.currentProduct.SecondColorId = app.state.currentProduct.ColorId;
                            $("#div-color-2").css("background-color", $("#div-color-1").css("background-color"));
                            $("#div-color-2").removeClass().addClass($("#div-color-1").attr('class'));
                        } else if (parseInt(color.id) == app.state.currentProduct.ThirdColorId) {
                            app.state.currentProduct.ThirdColorId = app.state.currentProduct.SecondColorId;
                            app.state.currentProduct.SecondColorId = app.state.currentProduct.ColorId;
                            $("#div-color-3").css("background-color", $("#div-color-2").css("background-color"));
                            $("#div-color-3").removeClass().addClass($("#div-color-2").attr('class'));
                            $("#div-color-2").css("background-color", $("#div-color-1").css("background-color"));
                            $("#div-color-2").removeClass().addClass($("#div-color-1").attr('class'));
                        } else if (parseInt(color.id) == app.state.currentProduct.FourthColorId) {
                            app.state.currentProduct.FourthColorId = app.state.currentProduct.ThirdColorId;
                            app.state.currentProduct.ThirdColorId = app.state.currentProduct.SecondColorId;
                            app.state.currentProduct.SecondColorId = app.state.currentProduct.ColorId;
                            $("#div-color-4").css("background-color", $("#div-color-3").css("background-color"));
                            $("#div-color-4").removeClass().addClass($("#div-color-3").attr('class'));
                            $("#div-color-3").css("background-color", $("#div-color-2").css("background-color"));
                            $("#div-color-3").removeClass().addClass($("#div-color-2").attr('class'));
                            $("#div-color-2").css("background-color", $("#div-color-1").css("background-color"));
                            $("#div-color-2").removeClass().addClass($("#div-color-1").attr('class'));
                        } else if (parseInt(color.id) == app.state.currentProduct.FifthColorId) {
                            app.state.currentProduct.FifthColorId = app.state.currentProduct.FourthColorId;
                            app.state.currentProduct.FourthColorId = app.state.currentProduct.ThirdColorId;
                            app.state.currentProduct.ThirdColorId = app.state.currentProduct.SecondColorId;
                            app.state.currentProduct.SecondColorId = app.state.currentProduct.ColorId;
                            $("#div-color-5").css("background-color", $("#div-color-4").css("background-color"));
                            $("#div-color-5").removeClass().addClass($("#div-color-4").attr('class'));
                            $("#div-color-4").css("background-color", $("#div-color-3").css("background-color"));
                            $("#div-color-4").removeClass().addClass($("#div-color-3").attr('class'));
                            $("#div-color-3").css("background-color", $("#div-color-2").css("background-color"));
                            $("#div-color-3").removeClass().addClass($("#div-color-2").attr('class'));
                            $("#div-color-2").css("background-color", $("#div-color-1").css("background-color"));
                            $("#div-color-2").removeClass().addClass($("#div-color-1").attr('class'));
                        }
                        app.state.currentProduct.ColorId = parseInt(color.id);
                        var prices = app.state.product.prices;
                        var price;
                        for (var i = 0; i < prices.length; i++) {
                            if (prices[i].color_id == color.id) {
                                price = prices[i].price;
                            }
                        }
                        app.state.currentProduct.BaseCost = parseFloat(price).toFixed(2);
                        window.costOfMaterial = parseFloat(price).toFixed(2);
                        calculatePrice(window.frontColor, window.backColor);
                    })
                    .hover(
                        function () {
                            
                            $('.product-design img').stop().animate({backgroundColor: color.value}, aniSpeed);
                            me.setDesignAreaContrastColor(color);
                            app.state.checkAll = true;
                            design.item.checkBorders(design.item.get());
                        }, function () {
                           
                            $('.product-design img').stop().animate({backgroundColor: app.state.color.value},aniSpeed);
                            me.setDesignAreaContrastColor(app.state.color);
                            app.state.checkAll = true;
                            design.item.checkBorders(design.item.get());
                        });
                list_color.append($color);
            });
            list_color.find('li:first').click();
		},
		productCate: function(){
			var me = this;
            app.loadProducts().then(function(data){
                me.productsData = {};
                me.colors = {};
                me.images = {};
                $.each(data.products, function(i, product){
                    me.productsData[product.id] = product;
                });
                $.each(data.product_colors, function(i, color){
                    me.colors[color.id] = color;
                });
                $.each(data.product_images, function(i, image){
                    me.images[image.product_id] = image;
                });
                me.addCategory(data.product_groups);
                me.categoriesList = data.product_groups;
            });
		},
		addProduct: function(productIds){
            var $list = $('#products-list');
            $list.html('');
			if (productIds.length == 0){
                return;
            }
			var me = this;
			$.each(productIds, function(i, productId){
                var product = me.productsData[productId];
                if(!product){
                    console.log('Invalid product id '+productId);
                    return;
                }
				var $item = $('<li class="item-option"></li>');
                $item.data('id', product.id);
                $item.click(function () {
                    var res = false;
                    var coeff = parseFloat(design.products.images[product.id].printable_front_height) / parseFloat(design.products.images[product.id].printable_front_width);
                    var coeffBack = parseFloat(design.products.images[product.id].printable_back_height) / parseFloat(design.products.images[product.id].printable_back_width);
                    if (app.state.products != null) {
                        for (var i = 1; i < app.state.products.length; i++) {
                            var front = (design.products.images[app.state.products[i].ProductId].printable_front_height / design.products.images[app.state.products[i].ProductId].printable_front_width).toFixed(1);
                            var back = (design.products.images[app.state.products[i].ProductId].printable_back_height / design.products.images[app.state.products[i].ProductId].printable_back_width).toFixed(1);
                            if ((front != coeff.toFixed(1)) && (back != coeffBack.toFixed(1))) {
                                res = true;
                            } else {
                                res = false;
                            };
                        }
                    }
                    if (res) {
                       
                        $('#incompatible-print-area').modal('show');
                        app.state.prevColor = app.state.color;
                        document.getElementById('select-product').addEventListener('click', function () {
                            $('#products-list li').removeClass('active');
                            $item.addClass('active');
                            me.changeDesign(product);
                            app.state.currentProduct.ProductId = parseInt(product.id);
                            $(".lab-colors-block").height($item.offset().top - $item.closest(".slide-1-right-box").offset().top + 57 + (parseInt($(".lab-colors-block").css("top")) < 1 ? 30 : 0));
                            design.item.unselect();
                            if (design.item.get().length == 0) {
                                var elem = document.getElementById('item-0');
                                if (elem != null) {
                                    design.item.checkBorders(design.item.get());
                                }
                            } else {
                                design.item.checkBorders(design.item.get());
                            };
                            var products = app.state.products;
                            var length = products.length;
                            while (products[1] != null) {
                                var elem = document.getElementById(products[1].ProductId);
                                var del = elem.parentNode.querySelector(".ssp_delete");
                                var img = del.querySelector("img");
                                $(img).trigger("click");
                            }

                        });
                        document.getElementById('cancel-select-product').addEventListener('click', function (event) {

                            $('#item-options-dropdown').val(app.state.prevDropDownOption).trigger('change');
                            app.state.prevDropDownOption = app.state.currentDropDownOption;
                            design.products.changeColor(design.products.colors[app.state.prevColor.id]);
                            app.state.color = app.state.prevColor;
                            app.state.currentProduct.ColorId = app.state.color.id;
                            design.item.checkBorders(design.item.get());
                            //event.stopPropagation();
                        });
                    } else {
                        app.state.prevDropDownOption = app.state.currentDropDownOption;
                    //if (app.state.selectProduct) {
                        $('#products-list li').removeClass('active');
                        $(this).addClass('active');
                        me.changeDesign(product);
                        app.state.currentProduct.ProductId = parseInt(product.id);
                        $(".lab-colors-block").height($(this).offset().top - $(this).closest(".slide-1-right-box").offset().top + 57 + (parseInt($(".lab-colors-block").css("top")) < 1 ? 30 : 0));
                        design.item.unselect();
                        if (design.item.get().length == 0) {
                            var elem = document.getElementById('item-0');
                            if (elem != null) {
                                design.item.checkBorders(design.item.get());
                            }
                        } else {
                            design.item.checkBorders(design.item.get());
                        };
                    }
                } );
                var html = '<p class="item-name">'+product.name+'</p><div class="item-overview">'+
                    '<div class="item-thumb-container item-thumb-loaded"><img class="item-thumb" src="' + assetsUrls.products + 'product_type_' + product.id + '_front_small.png"></div>' +
                    '<div class="sizes-label">'+product.list_of_sizes+'</div>';
				$item.html(html);

                $list.append($item);
			});
			$list.find('li:first').click();
		},
    	addCategory: function(categories){
            if(!categories || !categories.length){
                return;
            }
            var me = this;
            var firstCategory = categories[0].id;
            var options = '';
            me.categoryProducts = {};
			$.each(categories, function(i, category){
                options += '<option value="'+category.id+'">'+category.name+'</option>';
                me.categoryProducts[category.id] = category.products;
			});
    	    $('#item-options-dropdown').html(options).on('change', function (e) {
    	        app.state.prevDropDownOption = app.state.currentDropDownOption;
    	        app.state.currentDropDownOption = e.target.value;
    	        var value = e.target.value;
    	        var idList = me.categoryProducts[value];
                me.addProduct(idList);
            }).val(firstCategory).change();
		}
	},
	team:{
		updateBack: function(e){
			$('#txt-team-fontfamly').html(e.item.fontfamly);
			$('#team-name-color').data('color', e.item.color.replace('#', '')).css('background-color', e.item.color);
		},
		load: function(teams){
			var $this = this;
			if(typeof teams.name != 'undefined')
			{
				$this.tableView(teams);
				$.each(teams.name, function(i, name){
					var team = {};
					team.name = name;
					team.number = teams.number[i];
					team.size = teams.size[i];
					$this.addMember(team);
				});
			}
		},
		changeView: function(){			
			if ($('.labView.active .drag-item-name').length > 0)
				document.getElementById('team_add_name').checked = true;
			else
				document.getElementById('team_add_name').checked = false;
				
			if ($('.labView.active .drag-item-number').length > 0)
				document.getElementById('team_add_number').checked = true;
			else
				document.getElementById('team_add_number').checked = false;
		},
		create: function(){
			design.popover('add_item_team');
			$('.popover-title').children('span').html('Team Name & Number');
		},
		addName: function(e){
			if ($(e).is(':checked') == true)
			{
				$jd('.ui-lock').attr('checked', false);
				var txt = {};
				txt.text = 'NAME';
				txt.color = '#000000';
				txt.fontSize = '24px';
				txt.fontFamily = 'arial';
				txt.stroke = 'none';
				txt.strokew = '0';
				design.text.add(txt, 'team');
				var o = design.item.get();
				o.addClass('drag-item-name');
				design.popover('add_item_team');
			}
			else
			{
				var id = $('.labView.active .drag-item-name').attr('id');
				var index = id.replace('item-', '');
				design.layers.remove(index);
				$('.labView.active .drag-item-name').remove();
			}
		},
		addNumber: function(e){
			if ($(e).is(':checked') == true)
			{
				$jd('.ui-lock').attr('checked', false);
				var txt = {};
				txt.text = '00';
				txt.color = '#000000';
				txt.fontSize = '24px';
				txt.fontFamily = 'arial';
				txt.stroke = 'none';
				txt.strokew = '0';
				design.text.add(txt, 'team');
				var o = design.item.get();
				o.addClass('drag-item-number');
				design.popover('add_item_team');
			}
			else
			{
				var id = $('.labView.active .drag-item-number').attr('id');
				var index = id.replace('item-', '');
				design.layers.remove(index);
				$('.labView.active .drag-item-number').remove();
			}
		},
		addMember: function(team){
			var i = 1;
			$('#table-team-list tbody tr').each(function(){
				var td = $(this).find('td');
					td[0].innerHTML = i;
				i++;
			});
			if (typeof team == 'undefined')
			{
				team = {};
				team.name = '';
				team.number = '';
				team.size = '';
			}
			var sizes = this.sizes(team.size);
			var html = '<tr>'
					 + 	'<td>'+i+'</td>'					 
					 + 	'<td>'
					 + 		'<input type="text" class="form-control input-sm" value="'+team.name+'" placeholder="Enter Name">'
					 + 	'</td>'
					 + 	'<td>'
					 + 		'<input type="text" class="form-control input-sm" value="'+team.number+'" placeholder="Enter Number">'
					 + 	'</td>'
					 + 	'<td>'+sizes+'</td>'
					 + 	'<td>'
					 + 		'<a href="javascript:void(0)" onclick="design.team.removeMember(this)" title="remove">Remove</a>'
					 + 	'</td>'
					 + '</tr>';
			$('#table-team-list tbody').append(html);
		},
		removeMember: function(e){
			$(e).parents('tr').remove();
			var i = 1;
			$('#table-team-list tbody tr').each(function(){
				var td = $(this).find('td');
					td[0].innerHTML = i;
				i++;
			});
		},
		setup: function(){
			var sizes = this.sizes('');
			$('#table-team-list tbody tr').each(function(){
				var td = $(this).find('td');
				td[3].innerHTML = sizes;
			});
			$('#team_msg_error').html('Please choose sizes again.').css('display', 'block');
		},
		sizes: function(size){
			var options =  '';
			$('.p-color-sizes').each(function(){
				var groupName = $(this).parent().parent().children('label').text();
				options = options + '<optgroup label="'+groupName+'">';
				
				$(this).find('.size-number').each(function(){
					var value = $(this).attr('name');
					value = value.replace('][', '-');
					value = value.replace('][', '-');
					value = value.replace(']', '');
					value = value.replace('[', '');
					value = value.replace('attribute', '');
					var lable = $(this).parent().find('label').html();
					if (size == lable+'::'+value)
						options = options + '<option value="'+lable+'::'+value+'" selected="selected">'+lable+'</option>';
					else
						options = options + '<option value="'+lable+'::'+value+'">'+lable+'</option>';
				});
				
				options = options + '</optgroup>';
			});
			if (options == '')
			{
				var select = '<select class="form-control input-sm" disabled=""></select>';
			}
			else
			{
				var select = '<select class="form-control input-sm">'+options+'</select>';
			}
			return select;
		},
		changeSize: function(){
			if(typeof design.teams.name != 'undefined')
			{
				this.create();
				$('#dg-item_team_list').modal();
			}
		},
		save: function(){
			var teams 			= {};
				teams.name 		= {};
				teams.number 	= {};
				teams.size 		= {};
			var i = 1, checked = true;
			$('#table-team-list tbody tr').each(function(){
				var td = $(this).find('td');
				var name = $(td[1]).find('input').val();
				var number = $(td[2]).find('input').val();
				var size = $(td[3]).find('select').val();
				if (name == '' || number == '')
				{
					checked = false;
				}
				teams.name[i] = name;
				teams.number[i] = number;
				teams.size[i] = size;
				
				i++;
			});
			if (checked == false)
			{
				$('#team_msg_error').html('Please add name & number.').css('display', 'block');
			}
			else
			{
				$('#team_msg_error').css('display', 'none');
				$('#dg-item_team_list').modal('hide');
				this.tableView(teams);
			}
			design.teams = teams;
		},
		tableView: function(teams){
			if (typeof teams.name != 'undefined')
			{
				var sizes = {};
				var div = $('#item_team_list tbody');
				div.html('');
				$.each(teams.name, function(i, team){
					if (teams.size[i] == null)
					{
						var temp = []; temp[0] = '';
					}
					else
					{
						var temp = teams.size[i].split('::');
					}
					
					var html = '<tr>'
							+  	'<td>'+teams.name[i]+'</td>'
							+  	'<td>'+teams.number[i]+'</td>'
							+  	'<td>'+temp[0]+'</td>'
							+  '</tr>';
					div.append(html);
					if (typeof sizes[teams.size[i]] == 'undefined')
						sizes[teams.size[i]] = [];
					sizes[teams.size[i]].push(i);
				});
				
				$('.size-number').each(function(){
					var lable = $(this).parent().find('label').text();
					var value = $(this).attr('name');
						value = value.replace('][', '-');
						value = value.replace('][', '-');
						value = value.replace(']', '');
						value = value.replace('[', '');
						value = value.replace('attribute', '');
						
					if (typeof sizes[lable+'::'+value] != 'undefined')
						$(this).val(Object.keys(sizes[lable+'::'+value]).length);
					else
						$(this).val(0);
				});
			}
			design.products.sizes();
		}
	},
	text:{
		getValue: function(){		
			var o = {};
			o.txt 			= $jd('#addEdit').val();
			o.color 		= $jd('#dg-font-color').css('background-color');
			o.fontSize 		= $jd('#dg-font-size').text();
			o.fontFamily 	= $jd('#dg-font-family').text();
			if($jd('#font-style-bold').hasClass('active')) o.fontWeight 	= 'bold';
			var outline 	= $jd('#dg-change-outline-value a').css('left');
			outline 		= outline.replace('px', '');
			if(outline != 0){
				o.stroke 		= $jd('#dg-outline-color').css('background-color');
				o.strokeWidth 	= outline/10;
			}
			o.spacing 		= '0';			
			return o;
		},		
		create: function(cloneFrom){
            var text = $('#enter-text').val() || 'Hello';
			$('.ui-lock').attr('checked', false);
			var txt = {};
			txt.text = text;
			txt.color = design.designer.toRgbColor(app.state['color-text']);
			txt.fontSize = '24px';
			txt.fontFamily = app.state.font.family || 'arial';
			txt.outlineC = design.designer.toRgbColor(app.state['color-outline']);
			txt.outlineW = $('#outline-select').val();
			this.add(txt, undefined, cloneFrom, 1);
		},
		getFontByFamily: function(family) {
		    var fonts = design.products.fonts || {};
            var fontIds = Object.keys(fonts);
            for (var i = 0; i < fontIds.length; i++) {
                var font = fonts[fontIds[i]];
                if (font.family === family) {
                    return font;
                }
            }
            return null;
        },
		setValue: function (o) {
            $('#enter-text').val(o.text);

            var font = this.getFontByFamily(o.fontFamily);
            if (font) {
                app.state.font = font;
                var url = assetsUrls.fonts + font.filename + '.png';
                var html = '<img src="' + url + '">';
                $('#font-select').html(html);
            }

		    var color;
            if (o.color) {
                color = design.designer.fromRgbColor(o.color);
                app.state['color-text'] = color;
                design.designer.showColor($('.color-picker[data-value="text"]'), color);
            }

            if (o.outlineC && o.outlineC!=='none') {
                color = design.designer.fromRgbColor(o.outlineC);
                app.state['color-outline'] = color;
                design.designer.showColor($('.color-picker[data-value="outline"]'), color);
            }
			
            if (typeof o.outlineW == 'undefined') {
                o.outlineW = 0;
            }

            $('#outline-select').val(o.outlineW);

            $('#text-align-tools').show();
		},
		add: function(o, type, cloneFrom, style_color){
			var item = {};
				if (typeof type == 'undefined')
				{
					item.type 	= 'text';
					item.remove = true;
					item.rotate = true;
				}
				else
				{
					item.type	= type;
					item.remove 		= false;
					item.edit 			= false;
				}
				item.text 		= o.text;
				item.fontFamily = o.fontFamily;
				item.color 		= o.color;
				//item.outlineC = 'none';
				//item.outlineW = '0';
			if(o){
				this.setValue(o);
			}else{
				var o = this.getValue();
			}

			var div = document.createElement('div');
			var node = document.createTextNode(o.text);			
			    div.appendChild(node);		
			    div.style.fontSize = o.fontSize;
				div.style.fontFamily = o.fontFamily;
				
			var cacheText = document.getElementById('cacheText');
			cacheText.innerHTML = '';
			cacheText.appendChild(div);
			var $width = cacheText.offsetWidth,
				$height = cacheText.offsetHeight;

			var svgNS 	= "http://www.w3.org/2000/svg",
			tspan 		= document.createElementNS(svgNS, 'tspan'),
			text 		= document.createElementNS(svgNS, 'text'),
			content 	= document.createTextNode(o.text);
			
			tspan.setAttributeNS(null, 'x', '50%');
			tspan.setAttributeNS(null, 'dy', 0);
							
			item.outlineC = o.outlineC;
			item.outlineW = o.outlineW;
			text.setAttributeNS(null, 'fill', o.color);
			text.setAttributeNS(null, 'stroke', o.outlineC);
			if (o.color == o.outlineC) {
			    text.style.color = o.color;
			}
			text.setAttributeNS(null, 'stroke-width', o.outlineW);
			text.setAttributeNS(null, 'stroke-linecap', 'round');
			text.setAttributeNS(null, 'stroke-linejoin', 'round');
			text.setAttributeNS(null, 'x', parseInt($width/2));
			text.setAttributeNS(null, 'y', 20);				
			text.setAttributeNS(null, 'text-anchor', 'middle');				
			text.setAttributeNS(null, 'font-size', o.fontSize);
			text.setAttributeNS(null, 'font-family', o.fontFamily);
			text.setAttributeNS(null, 'color', 'black');
			
			if(typeof o.strokeWidth != 'undefined' && o.strokeWidth != 0){
				text.setAttributeNS(null, 'stroke', o.stroke);
				text.setAttributeNS(null, 'stroke-width', o.strokeWidth);
			}
			if(typeof o.rotate != 'undefined'){
				text.setAttributeNS(null, 'transform', o.rotate);
			}
			if(typeof o.style != 'undefined'){
			text.setAttributeNS(null, 'style', o.style);
			}
			tspan.appendChild(content);
			text.appendChild(tspan);
			
			var g = document.createElementNS(svgNS, 'g');
				g.id = Math.random();
			g.appendChild(text);
			
			var svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
			svg.setAttributeNS(null, 'width', $width);
			svg.setAttributeNS(null, 'height', $height);
			svg.setAttributeNS(null, 'viewBox', '0 0 '+$width+' '+$height);			
			svg.setAttribute('xmlns', 'http://www.w3.org/2000/svg');
			svg.setAttribute('xmlns:xlink', 'http://www.w3.org/1999/xlink');
			svg.appendChild(g);
			
			item.width = $width;
			item.height = $height;
			item.file = '';
			item.confirmColor	= false;
			item.svg = svg;
			
			design.item.create(item);
            $('#text-align-tools').show();
        },
		update: function(lable, value){
			var e = design.item.get();
            var oldWidth = parseInt(e.css('width'));
            var txt = e.find('text');
            if (!txt.length) {
                txt = e.find('svg');
            }
			if(typeof lable != 'undefined' && lable != '')
			{
			    var obj = document.getElementById(e.attr('id'));
			    var rgb;
				switch(lable){
					case 'fontfamily':
						txt[0].setAttributeNS(null, 'font-family', value);
						obj.item.fontFamily = value;
						break;
				    case 'color':
				        rgb = design.designer.toRgbColor(app.state['color-text']);
				        txt[0].setAttributeNS(null, 'fill', rgb);
				        if (obj.item.color) {
				            app.state.unuseColors(obj.item.color);
				        }
				        obj.item.color = rgb;
				        app.state.useColors(rgb);
				        break;
				    case 'art-color':
				        rgb = design.designer.toRgbColor(app.state['color-art']);
				        txt[0].setAttributeNS(null, 'fill', rgb);
				        var paths = e.find('path');
				        if (paths.length > 0) {
				            $(paths).each(function () {
				                this.setAttributeNS(null, 'fill', rgb);
				                var id = "" + this.id + "";
				                if (document.getElementById(id) != null) {
				                    if ((document.getElementById(id).style.fill != "rgb(255, 255, 255)") && (document.getElementById(id).style.fill != "none")) {
				                        document.getElementById(id).style.fill = "";
				                    }
				                    if (document.getElementById(id).style.fill == "none") {
				                        document.getElementById(id).style.stroke = rgb;
				                    }
				                }			                
				            }
                                );
				        }
				        var polygons = e.find('polygon');
				        if (polygons.length > 0) {
				            $(polygons).each(function () {
				                this.setAttributeNS(null, 'fill', rgb);
				            });

				        }
				        var rects = e.find('rect');
				        if (rects.length > 0) {
				            $(rects).each(function () {
				                this.setAttributeNS(null, 'fill', rgb);
				            });

				        }
				        var ellipses = e.find('ellipse');
				        if (ellipses.length > 0) {
				            $(ellipses).each(function () {
				                this.setAttributeNS(null, 'fill', rgb);
				            });

				        }
				        var g = e.find('g');
				        if (g.length > 0) {
				            $(g).each(function () {
				                this.setAttributeNS(null, 'stroke', rgb);
				            });

				        }
				        if (obj.item.color) {
				            app.state.unuseColors(obj.item.color);
				        }
				        obj.item.color = rgb;
				        app.state.useColors(rgb);
				        break;
				    case 'colorT':
						var color = $jd('#team-name-color').data('value');
						if (color == 'none') var hex = color;
						else var hex = '#' + color;
						txt[0].setAttributeNS(null, 'fill', hex);
						obj.item.color = hex;
						break;
					case 'text':
						var text = $jd('#enter-text').val();						
						$('.layer.active span').html(text.substring(0, 20));
						obj.item.text = text;
						var texts = text.split('\n');
						var svgNS 	= "http://www.w3.org/2000/svg";						
						txt[0].textContent = '';
						var fontSize = txt[0].getAttribute('font-size').split('px');
						for (var i = 0; i < texts.length; i++) {
							var tspan 	= document.createElementNS(svgNS, 'tspan');
							var dy = 0;
							if(i> 0) dy = fontSize[0];
								tspan.setAttributeNS(null, 'dy', dy);
								tspan.setAttributeNS(null, 'x', '50%');
							var content 	= document.createTextNode(texts[i]);	
							tspan.appendChild(content);
							txt[0].appendChild(tspan);
						}
						this.setSize(e);					
						break;						
					case 'alignL':
						obj.item.align = 'left';
						design.text.align(e, 'left');
						break;
					case 'alignC':
						obj.item.align = 'center';
						design.text.align(e, 'center');
						break;
					case 'alignR':
						obj.item.align = 'right';
						design.text.align(e, 'right');
						break;
					case 'styleI':
						var o = $jd('#text-style-i');
						if(o.hasClass('active')){
							o.removeClass('active');
							txt.css('font-style', 'normal');
							obj.item.Istyle = 'normal';
						}else{
							o.addClass('active');
							txt.css('font-style', 'italic');
							obj.item.Istyle = 'italic';
						}
						this.setSize(e);
						break;
					case 'styleB':
						var o = $jd('#text-style-b');
						if(o.hasClass('active')){
							o.removeClass('active');
							txt.css('font-weight', 'normal');
							obj.item.weight = 'normal';
						}else{
							o.addClass('active');
							txt.css('font-weight', 'bold');
							obj.item.weight = 'bold';
						}
						this.setSize(e);
						break;
					case 'styleU':
						var o = $jd('#text-style-u');
						if(o.hasClass('active')){
							o.removeClass('active');
							txt.css('text-decoration', 'none');
							obj.item.decoration = 'none';
						}else{
							o.addClass('active');
							txt.css('text-decoration', 'underline');
							obj.item.decoration = 'underline';
						}
						this.setSize(e);
						break;
				    case 'outline-width':
				        var oldVal = parseFloat(txt[0].getAttributeNS(null, 'stroke-width'));
				        if (oldVal && obj.item.outlineC) {
                            app.state.unuseColors(obj.item.outlineC);
                        }
						txt[0].setAttributeNS(null, 'stroke-width', value);
						txt[0].setAttributeNS(null, 'stroke-linecap', 'round');
						txt[0].setAttributeNS(null, 'stroke-linejoin', 'round');
						    txt[0].style.color = txt[0].getAttribute('stroke');
						obj.item.outlineW = value;
						if (parseFloat(value) && obj.item.outlineC) {
						    app.state.useColors(obj.item.outlineC);
						}
						if (!(value > 0)) {
						    txt[0].style.color = 'black';
						}
						break;
				    case 'outline':
				        var oWidth = parseFloat(txt[0].getAttributeNS(null, 'stroke-width'));
				        rgb = design.designer.toRgbColor(app.state['color-outline']);
				        txt[0].setAttributeNS(null, 'stroke', rgb);
				        txt[0].style.color = txt[0].getAttribute('stroke');
				        //txt[0].setAttributeNS(null, 'stroke-width', $jd('.outline-value').html()/50);
				        if (oWidth && obj.item.outlineC) {
				            app.state.unuseColors(obj.item.outlineC);
				        }
				        if (!(oWidth > 0)) {
				            txt[0].style.color = 'black';
				        } else {
				            obj.item.outlineC = rgb;
				        }
				        if (oWidth) {
				            app.state.useColors(rgb);
				        }
				        if (!(oWidth > 0)) {
				            txt[0].style.color = 'black';
				        }
				        break;
				    case 'art-outline':
				        oWidth = parseFloat(txt[0].getAttributeNS(null, 'stroke-width'));
				        rgb = design.designer.toRgbColor(app.state['color-art-outline']);
				        txt[0].setAttributeNS(null, 'stroke', rgb);
				        if (oWidth && obj.item.outlineC) {
				            app.state.unuseColors(obj.item.outlineC);
				        }
				        obj.item.outlineC = rgb;
				        if (oWidth) {
				            app.state.useColors(rgb);
				        }
				        break;
				    default:
						txt[0].setAttributeNS(null, lable, value);
						break;
				}
                var width = parseInt(e.css('width'));
                if(width!==oldWidth){
                    var left = parseInt(e.css('left'));
                    left -= (width - oldWidth)/2;
                    e.css('left', left);
                }
                design.item.placeSizeBox(e);
                design.item.checkBorders(e);
			}
		},
		updateBack: function(e){
			this.setValue(e.item);
		},
		reset:function(){
			document.getElementById('dg-font-family').innerHTML = 'arial';
			document.getElementById('dg-font-size').innerHTML = '12';
			$jd('#dg-font-style span').removeClass();
			$jd( "#dg-change-outline-value" ).slider();
		},
		setSize: function(e){
			var txt = e.find('text');
			var $w = parseInt(txt[0].getBBox().width * (e[0].item.scaleX || 1));
			var $h = parseInt(txt[0].getBBox().height * (e[0].item.scaleY || 1));
			e.css('width', $w + 'px');
			e.css('height', $h + 'px');						
			var svg = e.find('svg'),
				width = svg[0].getAttribute('width'),
				height = svg[0].getAttribute('height'),
				view = svg[0].getAttribute('viewBox').split(' '),
				vw = (view[2] * $w)/width,
				vh = (view[3] * $h)/height;
			svg[0].setAttributeNS(null, 'width', $w);
			svg[0].setAttributeNS(null, 'height', $h);			
			svg[0].setAttributeNS(null, 'viewBox', '0 0 '+vw +' '+ vh);		
		},		
		align: function(e, type){
			var span = $jd('#text-align-'+type);
			var txt = e.find('text');
			var tspan = e.find('tspan');
			if(span.hasClass('active')){
				span.removeClass('active');
				txt[0].setAttributeNS(null, 'text-anchor', 'middle');
				for(i=0; i<tspan.length; i++){
					tspan[i].setAttributeNS(null, 'x', '50%');
				}
			}else{
				$jd('#text-align span').removeClass('active');
				span.addClass('active');
				txt[0].setAttributeNS(null, 'text-anchor', 'middle');
				if(type == 'left')
					txt[0].setAttributeNS(null, 'text-anchor', 'start');
				else if(type == 'right')
					txt[0].setAttributeNS(null, 'text-anchor', 'end');
				else 
					txt[0].setAttributeNS(null, 'text-anchor', 'middle');
				
				for(i=0; i<tspan.length; i++){
					if(type == 'left')
						tspan[i].setAttributeNS(null, 'x', '0');
					else if(type == 'right')
						tspan[i].setAttributeNS(null, 'x', '100%');
					else
						tspan[i].setAttributeNS(null, 'x', '50%');
				}
			}
		},
		fonts: function(files, names){
			$.ajax({type: "POST", url: baseURL+'components/com_devn_vmattribute/assets/fonts/fonts.php', data: { files: files, names: names, url: baseURL },
			beforeSend: function ( xhr ){xhr.overrideMimeType("application/octet-stream");},
			success: function(data) {
			$("<style>"+data+"</style>").appendTo('head');
			var fonts = names.split(';');
			var html = '';
			for(i=0;i<fonts.length; i++){
				html = html + '<span style="font-family:\''+fonts[i]+'\'">test</span>';
			}
			$('<div style="display:none">'+html+'</div>').appendTo('body');
			}});
		},
	},
	myart:{
		create: function(e) {
			var item = e.item;
			//$jd('.ui-lock').attr('checked', false);				
			var o 			= {};
			o.type 			= 'clipart';			
			o.upload = 1;
			o.title 		= item.title;
			o.url 			= item.url;
			o.file_name 	= item.file_name;			
			o.thumb = item.thumb;
		    o.colors = item.colors;
			o.confirmColor	= true;
			o.remove 		= true;
			o.edit 			= false;
			o.rotate 		= true;	
			o.rotate 		= true;	
			
			
			if (item.file_type !== 'svg')
			{
				o.file		= {};
				o.file.type	= 'image';				
				var img = new Image();
				design.mask(true);
				img.onload = function() {
					o.width 	= this.width;
					o.height	= this.height;
					var imageData = app.state.getImage();
					var view = app.state.getView();
					var prefix = 'printable_'+view+'_';
					var width = imageData[prefix+'width'];
					var height = imageData[prefix+'height'];

					if (this.width > width) {
						o.width 	= width;						
						o.height 	= (width/this.width) * this.height;
					}
                    if (o.height > height) {
                        o.height 	= height;						
                        o.width 	= (height/this.height) * this.width;
                    }
					o.change_color = 0;

					var content = '<svg xmlns="http://www.w3.org/2000/svg" xml:space="preserve" xmlns:xlink="http://www.w3.org/1999/xlink">'
								 + '<g><image x="0" y="0" width="' + (o.width) + '" height="' + (o.height) + '" xlink:href="' + item.url + '" preserveAspectRatio="none"/></g>'
								 + '</svg>';
					o.svg 		= $.parseHTML(content);					
					var $div = design.item.create(o);
				    var svg = $div.find('svg:first')[0];
				    svg.setAttributeNS(null, 'width', o.width);
				    svg.setAttributeNS(null, 'height', o.height);
				    svg.setAttributeNS(null, 'viewBox', '0 0 ' + o.width + ' ' + o.height);
					svg.setAttributeNS(null, 'preserveAspectRatio', 'none');
					design.mask(false);
				}
				img.src = item.url;
				return true;
			}
		}
	},
	art:{
		create: function(e){
			var item = e.item;
			var img = $(e).children('img');			
			var o 			= {};
			o.type 			= 'clipart';			
			o.upload 		= 0;			
			o.clipart_id 	= $(e).data('clipart_id');
			o.title 		= item.title;
			o.url 			= item.url;
			o.file_name 	= item.file_name;
			o.change_color 	= parseInt(item.change_color);
			o.thumb			= img.attr('src');			
			o.remove 		= true;
			o.edit 			= true;
			o.rotate 		= true;
			o.confirmColor	= false;
			
			
			if (item.file_type !== 'svg') {
				o.confirmColor	= true;
				var canvas = document.createElement('canvas');
				var context = canvas.getContext('2d');
				img = new Image();
				img.onload = function() {				  
					o.width 	= 100;
					o.height	= Math.round((o.width/this.width) * this.height);
					o.change_color = 0;
					o.file		= {};
					o.file.type	= 'image';
					
					canvas.width = this.width;
					canvas.height = this.height;
					
					context.drawImage(img,0,0);
					context.stroke();
					var dataURL = canvas.toDataURL();
					var content = '<svg xmlns="http://www.w3.org/2000/svg" xml:space="preserve" xmlns:xlink="http://www.w3.org/1999/xlink">'
									 + '<g><image x="0" y="0" width="'+o.width+'" height="'+o.height+'" xlink:href="'+dataURL+'" /></g>'
									 + '</svg>';
					o.svg 		= $.parseHTML(content);					
					$('#arts-add button').button('reset');
					design.item.create(o);
					$jd('.modal').modal('hide');
				}
			    img.src = item.url;
			} else {
				$.ajax({
					type: "GET",
					url: item.url,
					success: function(data){					
							o.file			= item.url;
							o.svg = $(data).find('svg');

							
							var imgWidth = parseFloat($(data).find('svg').attr('width'));
							if(!($.isNumeric(imgWidth))){
							    imgWidth = ($(data).find('svg').prop('viewBox')).animVal.width;							   
							}
							var imgHeight = parseFloat($(data).find('svg').attr('height'));
							if (!($.isNumeric(imgHeight))) {
							    imgHeight = ($(data).find('svg').prop('viewBox')).animVal.height;
							}
							var viewBox = parseFloat($(data).find('svg').attr('viewBox'));
							if (!($.isNumeric(viewBox))) {
							    $(data).find('svg').attr("viewBox", "0 0 " + imgWidth + " " + imgHeight);
							}
							var imageData = app.state.getImage();
							var view = app.state.getView();
							var prefix = 'printable_' + view + '_';
							var width = imageData[prefix + 'width']-2;
							var height = imageData[prefix + 'height']-2;

							//if (imgWidth > width) {
							    o.width = width;
							    o.height = (width / imgWidth) * imgHeight;
							//}
							if (o.height > height) {
							    o.height = height;
							    o.width = (height / imgHeight) * imgWidth;
							}
							o.outlineW = $('#outline-select-art').val();
							design.item.create(o);
							var elm = design.item.get();			
							var svg = elm.children('svg');

							svg[0].setAttributeNS(null, 'width', o.width);
							svg[0].setAttributeNS(null, 'height', o.height);
							svg[0].setAttributeNS(null, 'preserveAspectRatio', 'none');
					        var html = $(svg[0]).innerHTML;
					        $(svg[0]).innerHTML = '<g>' + html + '</g>';
							design.text.update('outline-width', $('#outline-select-art').val());
							design.text.update('art-color');
							design.text.update('art-outline');
					},
					failure: function(errMsg) {
						alert(errMsg+ '. Please try again');
					},
					complete: function() {
						$('#arts-add button').button('reset');
					}
				});
			}
		},
		/*
		* change object e from color1 to color2
		*/
		changeColor: function(e, color){
			var o = e.data('colors');
			if(typeof o != 'undefined')
			{
				$(o).each(function(){
					if (color == 'none')
						var hex = color;
					else
						var hex = '#' + color;
					this.setAttributeNS(null, 'fill', hex);
				});
			}			
		},
		restore: function(){
			var e = design.item.get();
			//var html = e.data('content');
			//var o = e.children('svg');
		},
		update: function(e){			
			design.item.setup(e.item);
		}
	},
	item:{
	    designini: function(){
	        var state = app.state;
	        var color = state.color;
	        var positions = ['front', 'back'];
	        var image = state.getImage();
	        $.each(positions, function(i, view){
	            var $view = $('#view-'+view);
	            var $images = $view.find('.product-design');
	            $images.html('');
	            var $img = $('<img>')
                    .addClass('product_images')
                    .attr('src', assetsUrls.products + 'product_type_'+state.product.id+'_'+view+'.png')
                    .css({'background': color.value, 'width':image.width, 'height': image.height});
	            $images.append($img);
	            var $designArea = $('.design-area', $view);
	            var prefix = 'printable_' + view + '_';
                
	            $designArea.css({ 'height': image[prefix + 'height'], 'width': image[prefix + 'width'], 'left': image[prefix + 'left'], 'top': image[prefix + 'top'] });
	            $designArea.css({'display': 'block'});
	        });
	    },
	    getNodeRect: function($item){
	        var body = document.body;
	        var docElem = document.documentElement;
	        var scrollTop = window.pageYOffset || docElem.scrollTop || body.scrollTop;
	        var scrollLeft = window.pageXOffset || docElem.scrollLeft || body.scrollLeft;
	        var clientTop = docElem.clientTop || body.clientTop || 0;
	        var clientLeft = docElem.clientLeft || body.clientLeft || 0;
	        var parentBox;
	        var top;
	        var box;
	        var left;
	        var item;
	        var view = app.state.getView();
	        if ((app.state.checkAll == true) && ($item[0] != null)) {
	            $item[0] = null
	        }
	        if ($item[0] == null)
	        {
	            var i = 0;
	            var maxLeftOffset = 0;
	            var maxWidthOffset = 0;
	            var maxTopOffset = 0;
	            var maxHeightOffset = 0;
	            var objName = "item-0";
	            while (document.getElementById(objName) != null) {
	                item = document.getElementById(objName);
	                if (item.parentNode.parentNode.id == "view-" + view + "-design-area") {
	                    if (Math.abs(item.offsetLeft) > Math.abs(maxLeftOffset)) {
	                        maxLeftOffset = item.offsetLeft;
	                    } else {
	                        if (item.offsetLeft < 0) {
	                            maxLeftOffset = item.offsetLeft;
	                        }
	                    }
	                    if (Math.abs(item.offsetTop) > Math.abs(maxTopOffset)) {
	                        maxTopOffset = item.offsetTop;
	                    } else {
	                        if (item.offsetTop < 0) {
	                            maxTopOffset = item.offsetTop;
	                        }
	                    }
	                    if (item.offsetWidth > maxWidthOffset) {
	                        maxWidthOffset = item.offsetWidth;
	                    };
	                    if (item.offsetHeight > maxHeightOffset) {
	                        maxHeightOffset = item.offsetHeight;
	                    };
	                };
	                i++;
	                objName = "item-" + i;
	            }                 
                              
	            //box = item.getBoundingClientRect();
	            //top = item.offsetTop;
	            //left = item.offsetLeft;
	            //return { top: top, left: left, width: item.offsetWidth, height: item.offsetHeight };
	            return { top: maxTopOffset, left: maxLeftOffset, width: maxWidthOffset, height: maxHeightOffset };
                
	        }
	        else
	        {
	            parentBox = $item.parents(':first').offset();
	            box = $item[0].getBoundingClientRect();
	            top = box.top + scrollTop - clientTop - parentBox.top;
	            left = scrollLeft - clientLeft + box.left - parentBox.left;
	            return { top: top, left: left, width: box.width, height: box.height };
	        }

	        return {top: top, left:left, width:box.width, height:box.height};
	    },
	    placeSizeBox:function($item, $sizeBox, keep){
	        $item = $($item);
	        var ppi = app.state.getImage().ppi;
	        if(!$sizeBox){
	            var id = '#sizer-'+$item.data('id');
	            $sizeBox = $(id);
	        }
	        var iWidth = ($item.width()/ppi).toFixed(2);
	        var iHeight = ($item.height()/ppi).toFixed(2);


	        $sizeBox.html(iWidth+'" x '+iHeight+'"');
	        var rect = this.getNodeRect($item);
	        var view = '#view-'+app.state.getView();
	        $sizeBox.css({'top': rect.top + rect.height + 30, 'left':rect.left+rect.width/2});

	        var $dialog = $(view+' .size-dialog');
	        if(keep !== 'width'){
	            $dialog.find('input.width').val(iWidth);
	        }
	        if(keep!== 'height'){
	            $dialog.find('input.height').val(iHeight);
	        }
	        $dialog.css({'top': rect.top + rect.height + 30, 'left':rect.left+rect.width/2});
	    },
	    getNextId: function(){
	        var n = -1;
	        $('#app-wrap .drag-item').each(function(){
	            var index 	= $(this).attr('id').replace('item-', '');
	            if (index > n) n = parseInt(index);
	        });
	        n = n + 1;
	        return n;
	    },
	    create: function (item, x, y) {
	        var me = this;

	        var colors = item.colors || item.color;
	        if (colors) {
	            app.state.useColors(colors);
	        }
	        if (parseFloat(item.outlineW) && item.outlineC) {
	            app.state.useColors(item.outlineC);
	        }

	        this.unselect();
	        var view = '#view-'+app.state.getView();
	        var e = $(view+' .content-inner'),
				div = document.createElement('div');
	        var n = this.getNextId();
			
	        div.className = 'drag-item-selected drag-item';
	        div.id 		= 'item-'+n;
	        div.item 		= item;
	        item.id 		= n;
	        $(div).bind('click', function(){design.item.select(this)});
	        var center = this.align.center(item);
	        div.style.left = (x || center.left) + 'px';
	        div.style.top 	= (y || center.top) + 'px';
	        div.style.width 	= item.width+'px';
	        div.style.height 	= item.height+'px';
			
	        $(div).data('id', item.id);
	        $(div).data('type', item.type);
	        $(div).data('file', item.file);
	        $(div).data('width', item.width);
	        $(div).data('height', item.height);

	        div.style.zIndex = design.zIndex;
	        design.zIndex  	= design.zIndex + 5;
	        div.style.width = item.width;
	        div.style.height = item.height;
	        $(div).append(item.svg);

	        if(item.change_color == 1)
	        {
	            $('#clipart-colors').css('display', 'block');
	            $('.btn-action-colors').css('display', 'block');
	        }
	        else
	        {
	            $('#clipart-colors').css('display', 'none');
	            $('.btn-action-colors').css('display', 'none');
	        }
			
	        if(item.remove == true){
	            var remove = document.createElement('div');
	            remove.className = 'item-remove-on handle';
	            remove.setAttribute('title', 'Click to remove this item');
	            $(remove).on('click', function(e){
	                e.preventDefault();
	                e.stopPropagation();
	                design.item.remove(this);
	            });
	            $(div).append(remove);
	        }
	        var $div = $(div);
	        var $sizeBox = $('<div class="edit-box-sizer" id="sizer-'+item.id+'">11.38" x 1.55"</div>');
	        $sizeBox.css('z-index',div.style.zIndex);
	        $sizeBox.on('click', function(e){
	            e.stopPropagation();
	            e.preventDefault();
	            var $dialog = $(view+' .size-dialog');
	            me.placeSizeBox($div, $sizeBox);
	            $dialog.show();
	        });
			
	        if(item.edit == true){
	            var edit = document.createElement('div');
	            edit.className = 'item-edit-on glyphicons pencil';
	            edit.setAttribute('title', 'Click to edit this item');
	            edit.setAttribute('onclick', 'design.item.edit(this)');
	            $(div).append(edit);
	        }	
			
	        e.append(div);
	        e.append($sizeBox);
	        this.placeSizeBox($div, $sizeBox);
	        this.move($(div));
	        this.resize($(div));
	        if(item.rotate == true)
	            this.rotate($jd(div));
	        design.layers.add(item);
	        this.setup(item);
	        $('.btn-action-edit').css('display', 'none');
	        if (app.state.print_type == 'screen' || app.state.print_type == 'embroidery')
	        {
	            if (item.confirmColor == true)
	            {
	                this.setupColorprint(div);
	                $('.btn-action-edit').css('display', 'block');
	            }				
	        }
	        design.print.colors();			
	        design.print.size();
	        return $div;
	    },
	    duplicate: function(item){
	        var newItem = $.extend(true, {}, item[0].item);
	        newItem.width = item.css('width');
	        newItem.height = item.css('height');
	        newItem.svg  = $(newItem.svg).clone()[0];
	        var x = parseInt(item.css('left'))+20;
	        var y = parseInt(item.css('top'))+20;
	        var $div = this.create(newItem, x, y);
	        var data = $.extend({},item.data());
	        delete data['id'];
	        delete data['ui-draggable'];
	        delete data['ui-resizable'];
	        delete data['ui-rotatable'];
	        $div.data(data);

	        this.rotate($div);
	        $div.rotatable("setValue", item.data('angle'));
	        this.unselect();
	        this.select($div[0]);
	        this.checkBorders($div);

	    },
	    setupColorprint: function(o){
	        var item = o.item;
	        $('#screen_colors_images').html('<img class="img-thumbnail img-responsive" src="'+item.thumb+'">');
	        if (item.colors != 'undefined')
	        {
	            $('#screen_colors_list span').each(function(){
	                var color = $(this).data('color');
	                if ($.inArray(color, item.colors) == -1)
	                    $(this).removeClass('active');
	                else
	                    $(this).addClass('active');
	            });
	        }
	        $('#screen_colors_body').show();
	    },
	    setColor: function(){
	        var colors = [], i = 0;
	        $('#screen_colors_list .bg-colors').each(function(){
	            if ($(this).hasClass('active') == true)
	            {
	                colors.push($(this).data('color'));
	                i++;
	            }
	        });
	        if (i==0)
	        {
	            alert('Please select a color.');
	        }
	        else
	        {
	            var o = this.get();
	            if (o != 'undefined')
	            {
	                var e = document.getElementById(o.attr('id'));
	                e.item.colors = colors;
	                this.printColor(e);
	            }
	            $('#screen_colors_body').hide();
	        }
	        design.print.colors();
	    },
	    printColor: function(o){
	        var box = $('#item-print-colors');
	        $('.btn-action-edit').css('display', 'none');
	        if (app.state.print_type == 'screen' || app.state.print_type == 'embroidery')
	        {				
	            box.html('').css('display', 'none');
	            if(o.item.confirmColor == true)
	            {
	                if (typeof o.item.colors != 'undefined')
	                {
	                    var item = o.item;
	                    $('#item-print-colors').html('<div class="col-xs-6 col-md-6"><img class="img-thumbnail img-responsive" src="'+item.thumb+'"></div><div class="col-xs-6 col-md-6"><div id="print-color-added" class="list-colors"></div><br/><span id="print-color-edit">Edit ink colors</span></div>');
						
	                    $('#print-color-edit').click(function(){
	                        design.item.setupColorprint(o);
	                    });
	                    var div = $('#print-color-added');
	                    $.each(item.colors, function(i, color){
	                        var span = document.createElement('span');
	                        span.className = 'bg-colors';
	                        span.style.backgroundColor = '#'+color;
	                        div.append(span);
	                    });
	                    box.css('display', 'block');
	                    $('.btn-action-edit').css('display', 'block');
	                }
	                else{
	                    this.setupColorprint(o);
	                }
	            }				
	        }
	        else
	        {
	            box.html('').css('display', 'none');				
	        }
	    },
	    imports: function(item){		
	        this.unselect();
	        var view = '#view-'+app.state.getView();
	        var e = $(view+' .content-inner'),
				span = document.createElement('div');
	        var n = -1;
	        $('#app-wrap .drag-item').each(function(){
	            var index 	= $(this).attr('id').replace('item-', '');
	            if (index > n) n = parseInt(index);
	        });			
	        var n = n + 1;
	        if (item.type == 'team')
	        {
	            if (item.text == '00')
	                span.className = 'drag-item-selected drag-item drag-item-number';
	            else
	                span.className = 'drag-item-selected drag-item drag-item-name';
	        }
	        else
	        {			
	            span.className = 'drag-item-selected drag-item';
	        }
	        span.id 		= 'item-'+n;
	        span.item 		= item;
	        item.id 		= n;
	        $(span).bind('click', function(){design.item.select(this)});

	        span.style.left 	= item.left;
	        span.style.top 		= item.top;
	        span.style.width 	= item.width;
	        span.style.height 	= item.height;
			
	        $(span).data('id', item.id);
	        $(span).data('type', item.type);
	        if (typeof item.file != 'undefined')
	        {
	            $(span).data('file', item.file);
	        }
	        else
	        {
	            item.file = {};
	            $(span).data('file', item.file);
	        }
	        $(span).data('width', item.width);
	        $(span).data('height', item.height);

	        span.style.zIndex = item.zIndex;							
	        $(span).append(item.svg);					
			
	        if(item.change_color == 1)
	        {
	            $('#clipart-colors').css('display', 'block');
	            $('.btn-action-colors').css('display', 'block');
	        }
	        else
	        {
	            $('#clipart-colors').css('display', 'none');
	            $('.btn-action-colors').css('display', 'none');
	        }
			
	        if (item.type != 'team')
	        {
	            var remove = document.createElement('div');
	            remove.className = 'item-remove-on glyphicons bin';
	            remove.setAttribute('title', 'Click to remove this item');
	            remove.setAttribute('onclick', 'design.item.remove(this)');
	            $(span).append(remove);
	        }
			
	        e.append(span);
						
	        this.move($jd(span));
	        this.resize($jd(span));
	        if (item.type != 'team')
	            if (item.rotate != 0)
	            {				
	                this.rotate($jd(span), item.rotate * 0.0174532925);
	            }
	            else
	            {
	                this.rotate($jd(span));
	            }			
	        design.layers.add(item);
	        this.setup(item);
	        design.print.colors();
	        design.print.size();
	    },
	    align:{
	        left: function(){
	        },
	        right: function(){
	        },
	        top: function(){
	        },
	        bottom: function(){
	        },
	        center: function(item){
	            var imageData = app.state.getImage();
	            var view = app.state.getView();
	            var prefix = 'printable_'+view+'_';
	            var align 	= {};
	            align.left 	= (imageData[prefix+'width'] - item.width-2)/2;
	            align.left 	= parseInt(align.left);
	            align.top 	= (imageData[prefix+'height'] - item.height-2)/2;
	            align.top	= parseInt(align.top);
	            return align;
	        }
	    },
	    showGrid: function(){
	        if(app.state.snapToCenter){
	            var imageData = app.state.getImage();
	            var view = app.state.getView();
	            var prefix = 'printable_'+view+'_';
	            var left 	= parseInt(imageData[prefix+'width']/2)+imageData[prefix+'left'];
	            var top 	= imageData['chest_line_'+view];
	            $('#view-'+view+' .xgridline').css({opacity:1, left:left});
	            $('#view-'+view+' .ygridline').css({opacity:1, top:top});
	        }

	    },
	    hideGrid: function(){
	        $('.xgridline, .ygridline').css({opacity:0});
	    },
	    checkGrid: function(e, ui){
	        if(!app.state.snapToCenter){
	            return [0,0];
	        }
	        var left = ui.position.left;
	        var width = parseInt(e.css('width'));
	        var top = ui.position.top;
	        var height = parseInt(e.css('height'));

	        var imageData = app.state.getImage();
	        var view = app.state.getView();
	        var prefix = 'printable_'+view+'_';
	        var centerLeft 	= imageData[prefix+'width']/2;
	        var centerTop 	= imageData['chest_line_'+view] - imageData[prefix+'top'];
	        var snappedX = centerLeft - (left+width/2);
	        if(Math.abs(snappedX)>10){
	            snappedX = 0;
	        }
	        var snappedY = centerTop - (top+height/2);
	        if(Math.abs(snappedY)>10){
	            snappedY = 0;
	        }
	        return [snappedX, snappedY];

	    },
	    move: function(e){
	        var me = this;
	        if(!e) e = $jd('.drag-item-selected');
	        e.draggable({/*containment: "#dg-designer", */scroll: false,
	            start: function (event, ui) {
	                me.showGrid();
	                var left = parseInt($(this).css('left'),10);
	                left = isNaN(left) ? 0 : left;
	                var top = parseInt($(this).css('top'),10);
	                top = isNaN(top) ? 0 : top;
	                this.recoupLeft = left - ui.position.left;
	                this.recoupTop = top - ui.position.top;
	                this.first = true;
	            },
	            drag:function(event, ui){
	                var e = ui.helper;
	                if(!this.first){
	                    me.checkBorders(e, ui);
	                    me.placeSizeBox(e);
	                }
	                ui.position.left += this.recoupLeft;
	                ui.position.top += this.recoupTop;
	                var snaps = me.checkGrid(e, ui);
	                ui.position.left += snaps[0];
	                ui.position.top += snaps[1];
	                this.first = false;
	            },
	            stop: function( event, ui ) {
	                me.forceBorders($(this), ui);
	                me.hideGrid();
	                design.print.size();
	            }
	        });						
	    },
	    forceBorders: function(e, ui){
	        var imageData = app.state.getImage();
	        var view = app.state.getView();

	        var height = imageData['printable_'+view+'_height'];
	        var width = imageData['printable_'+view+'_width'];
	        var rect = this.getNodeRect(e);
	        if ((document.getElementById('design-area').style.transform != "scale(1, 1)") && ((document.getElementById('design-area').style.transform != ""))) {
	            rect.width = rect.width / 1.4;
	            rect.height = rect.height / 1.4;
	            rect.top = rect.top / 1.4;
	            rect.left = rect.left / 1.4;
	        };
	        var $width = rect.width,
                $height = rect.height,
                $top = rect.top,
                $left = rect.left;
	        //completely out of the box
	        if($left+$width < 0 || $top+$height < 0 || $left > width || $top > height){
	            var left = (width-$width)/2;
	            var top = (height-$height)/2;
	            ui.position.left = left;
	            ui.position.top = top;
	            e.css('left', left);
	            e.css('top', top);
	            this.placeSizeBox(e);
	            this.checkBorders(e, ui);
	        }
	    },
	    checkBorders: function(e){
	        var imageData = app.state.getImage();
	        var view = app.state.getView();
	        var height = imageData['printable_'+view+'_height'];
	        var width = imageData['printable_'+view+'_width'];
	        var rect = this.getNodeRect(e);
	        if ((document.getElementById('design-area').style.transform != "scale(1, 1)") && ((document.getElementById('design-area').style.transform != ""))) {
	            rect.width = rect.width / 1.4;
	            rect.height = rect.height / 1.4;
	            rect.top = rect.top / 1.4;
	            rect.left = rect.left / 1.4;
	        };
	        var $width = rect.width,
                $height = rect.height,
                $top = rect.top,
                $left = rect.left;
	        if($left < 0 || $top < 0 || ($left+$width) > width || ($top+$height) > height){
	            e.data('block', true);
	            //set error border
	            design.products.setDesignAreaContrastColor('rgba(255, 79, 0, 0.298039)', true);
	        }else{
	            e.data('block', false);
	            var block = false;
	            $('#view-'+view+' .drag-item').each(function(i, item){
	                block |= $(item).data('block');
	            });
	            if(!block){
	                design.products.setDesignAreaContrastColor(app.state.color);
	            }
	        }
	        app.state.checkAll = false;
	    },
	    resizeTo: function(e, width, height, isInches, keepRatio){
	        design.item.checkBorders(e);
	        var svg = e.find('svg');
	        var imageData = app.state.getImage();
	        var keep;
	        if(keepRatio){
	            var ratio = e.data('ratio');
	            if(!ratio){
	                var currentHeight = parseInt(e.css('height'));
	                var currentWidth = parseInt(e.css('width'));
	                currentHeight = currentHeight || 1;
	                currentWidth = currentWidth || 1;
	                ratio = currentWidth/currentHeight;
	            }
	            if(!width){
	                keep = 'height';
	                width = height*ratio;
	            }else if(!height){
	                keep = 'width';
	                height = width/ratio;
	            }
	        }

	        if(isInches){
	            var ppi = imageData.ppi;
	            width = width * ppi;
	            height = height * ppi;
	        }

	        e.css({ width: width, height: height });
	        svg.css({marginBottom : '16px'});
            svg[0].setAttributeNS(null, 'width', width);
            svg[0].setAttributeNS(null, 'height', height);
            svg[0].setAttributeNS(null, 'preserveAspectRatio', 'none');

            if(e.data('type') == 'clipart')
            {
                var file = e.data('file');
                /*if(file.type === 'image')
                {
                    var img = e.find('image');
                    img[0].setAttributeNS(null, 'width', width-2);
                    img[0].setAttributeNS(null, 'height', height-2);
                }*/
            }

            if(e.data('type') == 'text')
            {
                var txt = e.find('text');
                var $w = parseInt(txt[0].getBBox().width);
                var $h = parseInt(txt[0].getBBox().height);
                e[0].item.scaleX = width / $w;
                e[0].item.scaleY = height / $h;
                //var text = e.find('text');
                //text[0].setAttributeNS(null, 'y', 20);
            }
            if(!keepRatio){
                e.data('ratio', width/height);
            }
            this.placeSizeBox(e, null, keep);
            isResizing = true;
        },
        resize: function (e, handles, keep) {
            var $e = $(e);
            var id = $e.attr('id');
            var me = this;
            var auto = keep || false;//keep ratio
            var resizing = false;
            if ($e.hasClass('ui-resizable')) {
                $e.resizable('destroy');
            }
            $e.off('mouseenter', '.ui-resizable-e, .ui-resizable-w, .ui-resizable-s, .ui-resizable-n')
                .on('mouseenter', '.ui-resizable-e, .ui-resizable-w, .ui-resizable-s, .ui-resizable-n', function () {
                if (auto && !resizing) {
                    me.resize(e, handles, false)
                }
            });

            $e.off('mouseenter', '.ui-resizable-se').on('mouseenter', '.ui-resizable-se', function () {
                if (!auto &&!resizing) {
                    me.resize(e, handles, true)
                }
            });

			if(typeof handles == 'undefined') handles = 'n, s, w, e, se';


			if(!e) e = $jd('.drag-item-selected');
			var oldwidth = 0, oldheight = 0,oldsize = 0;
			var omp = {};
			var side = "scale";
			//var childNodes = {};
			e.resizable({minHeight: 0, minWidth: 15,				
			    aspectRatio: auto,
				handles: handles,
				start: function (event, ui) {
				    resizing = true;
				    northHandleTop = $(ui.element[0]).children('.ui-resizable-n')[0].offsetTop;
				    northHandleLeft = $(ui.element[0]).children('.ui-resizable-n')[0].offsetLeft;
				    southHandleTop = $(ui.element[0]).children('.ui-resizable-s')[0].offsetTop;
				    southHandleLeft = $(ui.element[0]).children('.ui-resizable-s')[0].offsetLeft;
				    westHandleTop = $(ui.element[0]).children('.ui-resizable-w')[0].offsetTop;
				    westHandleLeft = $(ui.element[0]).children('.ui-resizable-w')[0].offsetLeft;
				    eastHandleTop = $(ui.element[0]).children('.ui-resizable-e')[0].offsetTop;
				    eastHandleLeft = $(ui.element[0]).children('.ui-resizable-e')[0].offsetLeft;
				    omp.x = event.pageX;
				    omp.y = event.pageY;
				    var offset = ui.element.offset();
				    var tr = ui.element[0].style.transform;
				    if (ui.element[0].style.transform != "rotate(0rad)") {
				        side = "rotate";
				    } else {
				        
				        if ((omp.x > offset.left + northHandleLeft) && (omp.x < offset.left + northHandleLeft + 12)) {
				            if ((omp.y > offset.top - 6) && (omp.y < offset.top + 6)) {
				                side = "north";
				            }
				        }
				        if ((omp.x > offset.left + southHandleLeft) && (omp.x < offset.left + southHandleLeft + 12)) {
				            if ((omp.y > offset.top + southHandleTop) && (omp.y < offset.top + southHandleTop + 12)) {
				                side = "south";
				            }
				        }
				        if ((omp.y > offset.top + westHandleTop) && (omp.y < offset.top + westHandleTop + 12)) {
				            if ((omp.x > offset.left - 6) && (omp.x < offset.left + 6)) {
				                side = "west";
				            }
				        }
				        if ((omp.y > offset.top + eastHandleTop) && (omp.y < offset.top + eastHandleTop + 12)) {
				            if ((omp.x > offset.left + eastHandleLeft) && (omp.x < offset.left + eastHandleLeft + 12)) {
				                side = "east";
				            }
				        }
				    }
				    //omp.d = Math.sqrt(Math.pow(ui.size.width / 2, 2) + Math.pow(ui.size.height / 2, 2));
				    //omp.d = Math.sqrt(Math.pow(ui.size.width / 2, 2) + Math.pow(ui.size.height / 2, 2));
				    
				    oldrect = ui.element[0].getBoundingClientRect();
				    omp.centerOffset = { x: offset.left + oldrect.width / 2 - omp.x, y: offset.top + oldrect.height / 2 - omp.y };
				    omp.d = Math.sqrt(Math.pow(omp.centerOffset.x, 2) + Math.pow(omp.centerOffset.y, 2));
				    oldwidth = ui.size.width;
				    oldheight = ui.size.height;
				    oldsize = $('#dg-font-size').text();
				},
				stop: function (event, ui) {
				    resizing = false;
					design.print.size();
				},
				resizee: function (event, ui) {
				    
				    var e = ui.element;
				    var mp = { x: event.pageX, y: event.pageY };
				    var offset = ui.element.offset();
				    mp.centerOffset = { x: offset.left + oldrect.width / 2 - mp.x, y: offset.top + oldrect.height / 2 - mp.y };

				    var d = Math.sqrt(Math.pow(mp.centerOffset.x, 2) + Math.pow(mp.centerOffset.y, 2));
				    //var d = Math.sqrt(Math.pow((ui.position.left + ui.size.width) / 2 - mp.x, 2) + Math.pow((ui.position.top + ui.size.height) / 2 - mp.y, 2));
				    //var d = Math.sqrt(Math.pow(omp.x - mp.x, 2) + Math.pow(omp.y - mp.y, 2));
				    //var d = Math.max(Math.abs(omp.x - mp.x), Math.abs(omp.y - mp.y));
                    //we have at least one coord larger then
				    //direction = ui.size.width > ui.originalSize.width ? 1 : -1;//new point closer to center then starting point
				    //scale = (d) / omp.d / 2;
				    //scale = 1 + direction * scale;
				    scale = d / omp.d;
                    
				    //if (scale > 2) {
				    //    var fixed = Math.floor(scale);
				    //    scale = 1 + (scale - fixed);
				    //    //scale = 1 + Math.pow((scale - 2), 2);
				    //}
				    //if (scale > 1.1) {
				    //    var koeff = ((scale - 1) / 0.1).toFixed(0);
				    //    scale = 1 + Math.pow((scale - 1),koeff);
				    //}
				    if (scale > 1) {
				        scale = (scale - 1) / 1.4 + 1;
				    }
				    //console.log("before", ui.size, scale);
				    //console.log(mp.centerOffset, even.pageX, event.pageY, d, scale);
				    switch (side) {
				        case "north": {
				            ui.element.top = ui.originalPosition.top;
				            ui.element.left = ui.originalPosition.left;
				            ui.size.width = ui.originalSize.width;
				            ui.size.height = (ui.originalSize.height + (omp.y - mp.y)) > 0 ? ui.originalSize.height + (omp.y - mp.y) : 0 ;
				            break;}
				        case "south": {
				            ui.element.top = ui.originalPosition.top;
				            ui.element.left = ui.originalPosition.left;
				            ui.size.width = ui.originalSize.width;
				            ui.size.height = (ui.originalSize.height + (mp.y - omp.y)) > 0 ? ui.originalSize.height + (mp.y - omp.y) : 0;
				            break;
                        }
				        case "west": {
				            ui.element.top = ui.originalPosition.top;
				            ui.element.left = ui.originalPosition.left;
				            ui.size.width = (ui.originalSize.width + (omp.x - mp.x))>0 ? ui.originalSize.width + (omp.x - mp.x) : 0 ;
				            ui.size.height = ui.originalSize.height ;
				            break;
                        }
				        case "east": {
				            ui.element.top = ui.originalPosition.top;
				            ui.element.left = ui.originalPosition.left;
				            ui.size.width = (ui.originalSize.width - (omp.x - mp.x))>0 ? ui.originalSize.width - (omp.x - mp.x) : 0;
				            ui.size.height = ui.originalSize.height;
				            break;
				        }

				        case "scale": {
				            ui.size.width = ui.originalSize.width * scale;
				            ui.size.height = ui.originalSize.height * scale;
				            break;
				        }
				    }
				    //ui.size.width = ui.originalSize.width * scale;
				    //ui.size.height = ui.originalSize.height * scale;
				    //console.log("after", ui.size);
				    //$width = ui.originalSize.width * scale;
				    //$height = ui.originalSize.height * scale;
				    //ui.originalSize
					var $width = parseInt(ui.size.width),
						$height = parseInt(ui.size.height);
					oldwidth = $width;
					oldheight = $height;
				    //$(event.target).data('ui-resizable').axis === 'se'
					me.resizeTo(e, $width, $height, false, false);
					oldrect = e[0].getBoundingClientRect();

				}
			});
			
		},
		rotate: function(e, angle){
            var me = this;
			if( typeof angle == 'undefined') deg = 0;
			else deg = angle;
			if( typeof e != Object ) var o = $(e);
			else var o = e;
			o.rotatable({angle: deg, 
				rotate: function(event, angle){
					var deg = parseInt(angle.r);
					if(deg < 0) deg = 360 + deg;
					$('#' + e.data('type') + '-rotate-value').val(deg);
					o.data('rotate', deg);
					me.checkBorders(e);
					me.placeSizeBox(e);

                }
			});	
			design.print.size();
		},
        pushBack: function(){
            var e =this.get();
            if(!e.length){
                return;
            }
            var id = e.data('id');
            $('#view-'+app.state.getView()+' .drag-item').each(function(){
                this.style.zIndex = parseInt(this.style.zIndex) + 5;
            });
            e.css('z-index', 1);
            design.zIndex += 5;
        },
		select: function(e){
            var current =this.get();
            var $e = $(e);
            if(current.data('id') === $e.data('id')){
                return;
            }
            var type = $e.data('type');
			this.unselect();
			$e.addClass('drag-item-selected');
            $('#sizer-'+$e.data('id')).css('z-index', design.zIndex).show();
            e.style.zIndex = design.zIndex;
            design.zIndex  	= design.zIndex + 5;
			$(e).resizable({ disabled: false, handles: 'e' });
			$(e).draggable({ disabled: false });
			design.popover('add_item_'+$(e).data('type'));
			$('.add_item_'+$(e).data('type')).addClass('active');
			design.menu(type);
			this.update(e);
			this.printColor(e);
			design.layers.select($(e).attr('id').replace('item-', ''));			
		},
		unselect: function(e){
			$('#app-wrap .drag-item-selected').each(function(){
				$(this).removeClass('drag-item-selected');
				//$(this).css('border', 0);
				$(this).resizable({ disabled: true, handles: 'e' });
				$(this).draggable({ disabled: true });
			});
            $('.edit-box-sizer').hide();
            $('.size-dialog').hide();
            $( ".popover" ).hide();
			$('.menu-left a').removeClass('active');
            //if it was a text item
			$('#text-align-tools').hide();
            //for cliparts
			$('#designer-art-choseType').show();
			$('#dropbox').hide();
			$('#art-style').hide();

			$('#layers li').removeClass('active');
		},
		remove: function (e) {
		    this.unselect();
		    e.parentNode.parentNode.removeChild(e.parentNode);
		    var item = $(e.parentNode)[0].item;
		    var colors = item.colors || item.color;
            if (colors) {
                app.state.unuseColors(colors);
            }
            if (item.outlineC && (item.outlineW>0)) {
                app.state.unuseColors(item.outlineC);
            }
			var id = $(e.parentNode).data('id');
			if($('#layer-'+id)){
                $('#layer-'+id).remove();
            }
			$( "#dg-popover" ).hide(aniSpeed);			
			design.print.colors();
			design.print.size();
            $('#enter-text').val('');
            $('#sizer-'+id).remove();
            $('.size-dialog').hide();
            var block = false;
            $('#view-'+app.state.getView()+' .drag-item').each(function(i, item){
                block |= $(item).data('block');
            });
            if(!block){
                design.products.setDesignAreaContrastColor(app.state.color);
            }
		},
		setup: function(item){
            if(item.type === 'clipart') {
                $('#designer-art-choseType').hide();
                $('#dropbox').hide();
                $('#art-style').show();
                
				if (item.change_color) {
				    $('#art-built-in').show();
				} else {
				    $('#art-built-in').hide();
				}

				var color;
				if (item.color) {
				    color = design.designer.fromRgbColor(item.color);
				    app.state['color-art'] = color;
				    design.designer.showColor($('.color-picker[data-value="art"]'), color);
				}

				if (item.outlineC && item.outlineC !== 'none') {
				    color = design.designer.fromRgbColor(item.outlineC);
				    app.state['color-art-outline'] = color;
				    design.designer.showColor($('.color-picker[data-value="art-outline"]'), color);
				}

				if (typeof item.outlineW == 'undefined') {
				    item.outlineW = 0;
				}

				$('#outline-select-art').val(item.outlineW);
			}
			
			if(item.type === 'text'){
				$('.popover-title').children('span').html('Edit text');
			}
            //todo: block with height+width
			//document.getElementById(item.type + '-width').value = parseInt(item.width);
			//document.getElementById(item.type + '-height').value = parseInt(item.height);
			//document.getElementById(item.type + '-rotate-value').value = 0;
			
			$('.dropdown-color').popover({
				html:true,				
				placement:'bottom',
				title:'Choose a color <a class="close" href="#");">&times;</a>',
				content:function(){
					$('.dropdown-color').removeClass('active');
					var html = $('.other-colors').html();
					$(this).addClass('active');
					return '<div data-color="'+$(this).data('color')+'" class="list-colors">' + html + '</div>';
				}				
			});
			$('.dropdown-color').on('show.bs.popover', function () {
				var elm = this;
				$('.dropdown-color').each(function(){
					if (elm != this)
					{
						$(this).popover('hide');
					}
				});
			});
			$('.dropdown-color').click(function (e) {				
				e.stopPropagation();
			});
			$(document).click(function (e) {				
				$('.dropdown-color').popover('hide');				
			});			
			$('.dg-tooltip').tooltip();
			design.popover('add_item_'+item.type);
		},
		get: function(){
			var e = $('#app-wrap .drag-item-selected');
			return e;
		},
		refresh: function(name){
			var e = this.get();
			switch(name)
			{
				case 'rotate':				
					e.rotatable("setValue", 0);				
					break;
			}
		},
		flip: function(direction){
			var e = this.get(),
				svg = e.find('svg');
			var viewBox = svg[0].getAttributeNS(null, 'viewBox');
			var size = viewBox.split(' ');

			if(typeof e.data('flipX') == 'undefined') {
                e.data('flipX', true);
            }
            if(typeof e.data('flipY') == 'undefined') {
                e.data('flipY', true);
            }
            var targetX = direction==='h'?!e.data('flipX'):e.data('flipX');
            var targetY = direction==='v'?!e.data('flipY'):e.data('flipY');
            var tX = targetX?0:size[2];
            var sX = targetX?1:-1;
            var tY = targetY?0:size[3];
            var sY = targetY?1:-1;
            var transform = 'translate('+tX+', '+tY+') scale('+sX+','+sY+')';
            e.data('flipX', targetX);
            e.data('flipY', targetY);
			var g = $(svg[0]).children('g');
			if (g.length > 0)
				g[0].setAttributeNS(null, 'transform', transform);
		},
		center: function(){
			var e = this.get(),
				$width = e.width(),
				pw 		= e.parent().parent().width(),
				w = (pw - $width)/2;
			e.css('left', w+'px');
		},
		changeColor: function(e){
			
			var o 		= this.get(),
				color 	= $(e).data('color'),
				a 		= $('.dropdown-color.active');
			if (color == 'none')
			{
				$(a).addClass('bg-none');
			}
			else
			{
				$(a).removeClass('bg-none');
				$(a).css('background-color', '#'+color);
			}
			$(a).data('value', color);			
				
			if(o.data('type') == 'clipart'){
				var a = $('#list-clipart-colors .dropdown-color.active');							
				design.art.changeColor(a, color);
			}
			else if(o.data('type') == 'text'){
				design.text.update(a.data('label'), color);
			}
			else if(o.data('type') == 'team'){
				design.text.update(a.data('label'), '#'+color);
			}
			$('.dropdown-color').popover('hide');
			design.print.colors();
		},
		update: function(e){			
			var o = $jd(e),
				type = o.data('type'),
				css = e.style;
			
			/* rotate */
			if (typeof css == 'undefined')
				css = document.getElementById($(e).attr('id')).style;
			if( typeof css.transform == 'undefined'){
				var deg = 0
			}else{
				var deg = design.convert.radDeg(css.transform);
			}
			$jd('.rotate-value').val(deg);
			
			/* width and height */
			$jd('#'+type+'-width').val(design.convert.px(css.width));
			$jd('#'+type+'-height').val(design.convert.px(css.height));
		    console.log(type);
			switch(type){
				case 'clipart':
					design.art.update(e);
					break;
				case 'text':
					design.text.updateBack(e);
					break;
				case 'team':
					design.team.updateBack(e);
					break;
			}
		},
		updateSize: function(w, h){			
			var e = design.item.get(),			
				svg = e.find('svg'),
				view = svg[0].getAttributeNS(null, 'viewBox'),
				width = svg[0].getAttributeNS(null, 'width'),
				height = svg[0].getAttributeNS(null, 'height');
			view = view.split(' ');				
			svg[0].setAttributeNS(null, 'width', w);
			svg[0].setAttributeNS(null, 'height', h);
			svg[0].setAttributeNS(null, 'viewBox', '0 0 '+ (w * view[2])/width +' '+ ((h * view[3])/height));			
			$(e).css({'width':w+'px', 'height':h+'px'});
			design.print.size();
		}
	},
	layers:{
		select: function(index)
		{
			$('#layers li').removeClass('active');
			$('#layer-'+index).addClass('active');
			var o = $('#item-'+index);
			if (o.hasClass('drag-item-selected') == false)
			{
				if (document.getElementById('item-'+index) != null)
				design.item.select(document.getElementById('item-'+index));
			}
		},
		setup: function(){
			$('#layers').html('');
			$('.labView.active .drag-item').each(function(){
				design.layers.add(this.item);
			});
			design.item.unselect();
		},
		add: function(item){
			var li 				= document.createElement('li');
				li.className 	= 'layer';
				li.id 			= 'layer-' + item.id;
			$(li).bind('click', function(){
				design.layers.select(item.id);
			});
			if(item.type == 'text')
			{
				var html = '<i class="glyphicons text_bigger glyphicons-12"></i> ';
				html = html + ' <span>'+item.text+'</span>';
			}
			else if(item.type == 'team')
			{
				var html = '<i class="glyphicons soccer_ball glyphicons-small"></i> ';
				html = html + ' <span>'+item.text+'</span>';
			}
			else
			{
				var html = '<img alt="" src="'+item.thumb+'">';
				html = html + ' <span>'+item.title+'</span>';
			}
			
			
			html = html + '<div class="layer-action pull-right">'
						+ '<a class="dg-tooltip" title="" data-placement="top" data-toggle="tooltip" href="javascript:void(0)" data-original-title="Click to sorting layer">'
						+ '<i class="glyphicons move glyphicons-small"></i>'
						+ '</a>';
			if (item.type != 'team')
			{
				html = html + '<a class="dg-tooltip" title="" onclick="design.layers.remove('+item.id+')" data-placement="top" data-toggle="tooltip" href="javascript:void(0)" data-original-title="Click to delete layer">'
						+ '<i class="glyphicons bin glyphicons-small"></i></a></div>';
			}
			
			li.innerHTML = html;
			$('#layers').prepend(li);
			design.layers.select(item.id);
		},
		remove: function(id){
			var e = $jd('#item-'+id).children('.item-remove-on');
			$jd('#layer-'+id).remove();
			if (typeof e[0] != 'undefined')
			design.item.remove(e[0]);
		},
		sort: function(){
			var zIndex = $jd('#layers .layer').length;
			$jd('#layers .layer').each(function(){
				var id = $jd(this).attr('id').replace('layer-', '');
				$jd('#item-'+id).css('z-index', zIndex);
				zIndex--;
			});
		}
	},
    menu: function(type){
        $('.menu-left a').removeClass('active');
        $('.add_item_' + type ).addClass('active');
    },
	popover: function(e){
		$('.dg-options').css('display', 'none');
		$('#options-'+e).css('display', 'block');
		$('.popover').css({'top': '40px', 'display':'block'});	
		
		var index = $('.menu-left li').index($('.menu-left .'+e).parent());
		var top = (40 * index) - (index * 2 - 1) + 18;
		$('.popover .arrow').css('top', top + 'px');		
		
	},
	convert:{
		radDeg: function(rad){
			if(rad.indexOf('rotate') != -1)
			{
				var v = rad.replace('rotate(', '');
					v = v.replace('rad)', '');					
			}else{
				var v = parseFloat(rad);
			}
			
			var deg = ( v * 180 ) / Math.PI;
			
			if (deg < 0) deg = 360 + deg;
			return Math.round(deg);
		},
		px: function(value){
			if(value.indexOf('px') !== -1)
			{
				var px = value.replace('px', '');
			}
			var px = parseInt(value);
			return Math.round(px);
		}
	},
	svg:{		
		getColors: function(e){
			var color = {};
			var colors = this.find(e, 'fill', color);
			colors	= this.find(e, 'stroke', colors);
			
			return colors;
		},
		find: function(e, attribute, colors){			
			e.find('['+attribute+']').each(function(){
				var color = this.getAttributeNS(null, attribute);				
				if(typeof colors[color] != 'undefined')
				{
					var n = colors[color].length;
					colors[color][n] = this;
				}
				else{
					colors[color] = [];
					colors[color][0] = this;			
				}
			});
			return colors;
		},
		style: function(e){
			find('[style]').each(function(){
				var style = this.getAttributeNS(null, 'style');
				style = style.replace(' ', '');
				var attrs = style.split(';');
				for(i=0; i<attrs.length; i++)
				{
					var attribute = attrs[i].split(':');
					a[attribute[0]] = attribute[1];
				}
			});
		},
		items: function(postion, callback) {
		    var imageData = app.state.getImage();
		    var area = {
		        top: imageData['printable_' + postion + '_top'],
		        left: imageData['printable_' + postion + '_left'],
		        width: imageData['printable_' + postion + '_width'],
		        height: imageData['printable_' + postion + '_height']
		    };
			
			var obj 	= [], i = 0;
			var zoom = 2;
			$('#view-' + postion + ' .design-area .drag-item').each(function () {
				obj[i] 			= {};
				obj[i].top = zoom * design.convert.px($(this).css('top'));
				obj[i].left = zoom * design.convert.px($(this).css('left'));
				obj[i].width = zoom * design.convert.px($(this).css('width'));
				obj[i].height = zoom * design.convert.px($(this).css('height'));
								
				if(typeof $(this).data('rotate') != 'undefined')
					obj[i].rotate = $(this).data('rotate');
				else 
					obj[i].rotate = 0;
					
				var svg = $(this).find('svg');
				svg = $(svg).clone();
				svg[0].setAttributeNS(null, 'width', zoom * svg[0].getAttributeNS(null, 'width'));
				svg[0].setAttributeNS(null, 'height', zoom * svg[0].getAttributeNS(null, 'height'));

				obj[i].svg = new XMLSerializer().serializeToString(svg[0]);
				var image 		= $(svg).find('image');
				if (typeof image[0] == 'undefined')
				{
					obj[i].img 	= false;
				}
				else
				{
					obj[i].img 		= true;
					var src 		= $(image).attr('xlink:href');
					obj[i].src 		= src;				
				}
				obj[i].zIndex	= this.style.zIndex;
				i++;
			});
			obj.sort(function(obj1, obj2) {	
				return obj1.zIndex - obj2.zIndex;
			});
			
			var canvas = document.createElement('canvas');
			    canvas.width = zoom*area.width;
			    canvas.height = zoom * area.height;

			var $container = $('<div id="hidden-canvas-container" style="display:none"></div');
		    $('body').append($container);
		    $container.append(canvas);

			var context = canvas.getContext('2d');
			
			var radius = 0;		
			canvasLoad(obj, 0);
			function isIE() { return ((navigator.appName == 'Microsoft Internet Explorer') || ((navigator.appName == 'Netscape') && (new RegExp("Trident/.*rv:([0-9]{1,}[\.0-9]{0,})").exec(navigator.userAgent) != null))); }
			function downScaleImage(img, scale) {
			    var imgCV = document.createElement('canvas');
			    imgCV.width = img.width;
			    imgCV.height = img.height;
			    var imgCtx = imgCV.getContext('2d');
			    imgCtx.drawImage(img, 0, 0);
			    return downScaleCanvas(imgCV, scale);
			}

		    // scales the canvas by (float) scale < 1
		    // returns a new canvas containing the scaled image.
			function downScaleCanvas(cv, scale) {
			    if (!(scale < 1) || !(scale > 0)) throw ('scale must be a positive number <1 ');
			    //scale = normaliseScale(scale);
			    var sqScale = scale * scale; // square scale =  area of a source pixel within target
			    var sw = cv.width; // source image width
			    var sh = cv.height; // source image height
			    var tw = Math.floor(sw * scale); // target image width
			    var th = Math.floor(sh * scale); // target image height
			    var sx = 0, sy = 0, sIndex = 0; // source x,y, index within source array
			    var tx = 0, ty = 0, yIndex = 0, tIndex = 0; // target x,y, x,y index within target array
			    var tX = 0, tY = 0; // rounded tx, ty
			    var w = 0, nw = 0, wx = 0, nwx = 0, wy = 0, nwy = 0; // weight / next weight x / y
			    // weight is weight of current source point within target.
			    // next weight is weight of current source point within next target's point.
			    var crossX = false; // does scaled px cross its current px right border ?
			    var crossY = false; // does scaled px cross its current px bottom border ?
			    var sBuffer = cv.getContext('2d').
                getImageData(0, 0, sw, sh).data; // source buffer 8 bit rgba
			    var tBuffer = new Float32Array(3 * tw * th); // target buffer Float32 rgb
			    var tBufferA = new Float32Array(tw * th); // target buffer Float32 rgb
			    var sR = 0, sG = 0, sB = 0, sA = 0; // source's current point r,g,b

			    for (sy = 0; sy < sh; sy++) {
			        ty = sy * scale; // y src position within target
			        tY = 0 | ty;     // rounded : target pixel's y
			        yIndex = 3 * tY * tw;  // line index within target array
			        crossY = (tY !== (0 | (ty + scale)));
			        if (crossY) { // if pixel is crossing botton target pixel
			            wy = (tY + 1 - ty); // weight of point within target pixel
			            nwy = (ty + scale - tY - 1); // ... within y+1 target pixel
			        }
			        for (sx = 0; sx < sw; sx++, sIndex += 4) {
			            tx = sx * scale; // x src position within target
			            tX = 0 | tx;    // rounded : target pixel's x
			            tIndex = yIndex + tX * 3; // target pixel index within target array
			            crossX = (tX !== (0 | (tx + scale)));
			            if (crossX) { // if pixel is crossing target pixel's right
			                wx = (tX + 1 - tx); // weight of point within target pixel
			                nwx = (tx + scale - tX - 1); // ... within x+1 target pixel
			            }
			            sR = sBuffer[sIndex];   // retrieving r,g,b for curr src px.
			            sG = sBuffer[sIndex + 1];
			            sB = sBuffer[sIndex + 2];
                        sA = sBuffer[sIndex + 3]
			            if (!crossX && !crossY) { // pixel does not cross
			                // just add components weighted by squared scale.
			                tBuffer[tIndex] += sR * sqScale;
			                tBuffer[tIndex + 1] += sG * sqScale;
			                tBuffer[tIndex + 2] += sB * sqScale;
			                tBufferA[tIndex/3] += sA * sqScale;
			            } else if (crossX && !crossY) { // cross on X only
			                w = wx * scale;
			                // add weighted component for current px
			                tBuffer[tIndex] += sR * w;
			                tBuffer[tIndex + 1] += sG * w;
			                tBuffer[tIndex + 2] += sB * w;
			                tBufferA[tIndex/3] += sA * w;
			                // add weighted component for next (tX+1) px                
			                nw = nwx * scale
			                tBuffer[tIndex + 3] += sR * nw;
			                tBuffer[tIndex + 4] += sG * nw;
			                tBuffer[tIndex + 5] += sB * nw;
			                tBufferA[(tIndex + 3)/3] += sA * nw;
                        } else if (!crossX && crossY) { // cross on Y only
			                w = wy * scale;
			                // add weighted component for current px
			                tBuffer[tIndex] += sR * w;
			                tBuffer[tIndex + 1] += sG * w;
			                tBuffer[tIndex + 2] += sB * w;
			                tBufferA[tIndex/3] += sA * w;
                            // add weighted component for next (tY+1) px                
			                nw = nwy * scale
			                tBuffer[tIndex + 3 * tw] += sR * nw;
			                tBuffer[tIndex + 3 * tw + 1] += sG * nw;
			                tBuffer[tIndex + 3 * tw + 2] += sB * nw;
			                tBufferA[(tIndex + 3 * tw)/3] += sA * nw;
                        } else { // crosses both x and y : four target points involved
			                // add weighted component for current px
			                w = wx * wy;
			                tBuffer[tIndex] += sR * w;
			                tBuffer[tIndex + 1] += sG * w;
			                tBuffer[tIndex + 2] += sB * w;
			                tBufferA[tIndex/3] += sA * w;
                            // for tX + 1; tY px
			                nw = nwx * wy;
			                tBuffer[tIndex + 3] += sR * nw;
			                tBuffer[tIndex + 4] += sG * nw;
			                tBuffer[tIndex + 5] += sB * nw;
			                tBufferA[(tIndex + 3)/3] += sA * nw;
                            // for tX ; tY + 1 px
			                nw = wx * nwy;
			                tBuffer[tIndex + 3 * tw] += sR * nw;
			                tBuffer[tIndex + 3 * tw + 1] += sG * nw;
			                tBuffer[tIndex + 3 * tw + 2] += sB * nw;
			                tBufferA[(tIndex + 3 * tw)/3] += sA * nw
			                // for tX + 1 ; tY +1 px
			                nw = nwx * nwy;
			                tBuffer[tIndex + 3 * tw + 3] += sR * nw;
			                tBuffer[tIndex + 3 * tw + 4] += sG * nw;
			                tBuffer[tIndex + 3 * tw + 5] += sB * nw;
			                tBufferA[(tIndex + 3 * tw + 3)/3] += sA * nw;
			            }
			        } // end for sx 
			    } // end for sy

			    // create result canvas
			    var resCV = document.createElement('canvas');
			    resCV.width = tw;
			    resCV.height = th;
			    var resCtx = resCV.getContext('2d');
			    var imgRes = resCtx.getImageData(0, 0, tw, th);
			    var tByteBuffer = imgRes.data;
			    // convert float32 array into a UInt8Clamped Array
			    var pxIndex = 0; //  
			    for (sIndex = 0, tIndex = 0; pxIndex < tw * th; sIndex += 3, tIndex += 4, pxIndex++) {
			        tByteBuffer[tIndex] = 0 | (tBuffer[sIndex]);
			        tByteBuffer[tIndex + 1] = 0 | (tBuffer[sIndex + 1]);
			        tByteBuffer[tIndex + 2] = 0 | (tBuffer[sIndex + 2]);
			        tByteBuffer[tIndex + 3] = 0 | (tBufferA[sIndex/3]);
			    }
			    // writing result to canvas.
			    resCtx.putImageData(imgRes, 0, 0);
			    return resCV;
			}
			function log2(v) {
			    // taken from http://graphics.stanford.edu/~seander/bithacks.html
			    var b = [0x2, 0xC, 0xF0, 0xFF00, 0xFFFF0000];
			    var S = [1, 2, 4, 8, 16];
			    var i = 0, r = 0;

			    for (i = 4; i >= 0; i--) {
			        if (v & b[i]) {
			            v >>= S[i];
			            r |= S[i];
			        }
			    }
			    return r;
			}
		    // normalize a scale <1 to avoid some rounding issue with js numbers
			function normaliseScale(s) {
			    if (s > 1) throw ('s must be <1');
			    s = 0 | (1 / s);
			    var l = log2(s);
			    var mask = 1 << l;
			    var accuracy = 4;
			    while (accuracy && l) { l--; mask |= 1 << l; accuracy--; }
			    return 1 / (s & mask);
			}
			function canvasLoad(obj, i) {
				if (typeof obj[i] != 'undefined')
				{
				    var IE = isIE();
					var item = obj[i];
					i++;
					if (IE === true)
					{
						item.svg = item.svg.replace(' xmlns:NS1=""', '');
						item.svg = item.svg.replace(' NS1:xmlns:xlink="http://www.w3.org/1999/xlink"', '');
						item.svg = item.svg.replace(' xmlns="http://www.w3.org/2000/svg"', '');
					}				
					if (radius > 0) {
						context.save();
						var x = 0, 
							y = 0;
						var w = area.width;
						var h = area.height;
						var r = x + w;
						var b = y + h;
						context.beginPath();
						context.moveTo(x+radius, y);
						context.lineTo(r-radius, y);
						context.quadraticCurveTo(r, y, r, y+radius);
						context.lineTo(r, y+h-radius);
						context.quadraticCurveTo(r, b, r-radius, b);				
						context.lineTo(x+radius, b);
						context.quadraticCurveTo(x, b, x, b-radius);				
						context.lineTo(x, y+radius);
						context.quadraticCurveTo(x, y, x+radius, y);
						context.closePath();
						context.clip();
					}						
					if (item.rotate != 0)
					{
						context.save();
						context.translate(item.left, item.top);
						context.translate(item.width/2, item.height/2);
						context.rotate(item.rotate * Math.PI/180);
						item.left = (item.width/2) * -1;
						item.top = (item.height/2) * -1;
					}
					try {							
						if (item.img)
						{
							var images 	= new Image();
							images.src = item.src;
							scale = item.width / images.width;
							if (scale < 1) {
							    var scaledImage = downScaleImage(images, scale);
							    context.drawImage(scaledImage, item.left, item.top);
							} else {
							    context.drawImage(images, item.left, item.top, item.width, item.height);
							}
						}
						else
						{
						    //var fontsCss = "<defs><style type='text/css'><![CDATA["+design.fontsClear + "]]></style></defs>";
						    //context.drawSvg(item.svg.replace(/(<svg[^>]+>)/, "$1" + fontsCss), item.left, item.top);
						    context.drawSvg(item.svg, item.left, item.top);
                        }
						context.restore();
						canvasLoad(obj, i);
					}
					catch (e) {
						if (e.name == "NS_ERROR_NOT_AVAILABLE") {								
						}
					}					
				}
				else
				{
				    design.output[postion] = canvas;
				    $container.remove();
				    if (typeof callback === "function") {
				        callback();
				    }
					//design.svg.canvas(postion, canvas, callback);
				}
			}
			return canvas;
		},
		items_backup: function(postion, callback)
		{
			var area 	= eval ("(" + items['area'][postion] + ")");
			
			var obj 	= [], i = 0;
			$('#view-' +postion+ ' .design-area .drag-item').each(function(){
				obj[i] 			= {};
				obj[i].top 		= design.convert.px($(this).css('top'));
				obj[i].left 	= design.convert.px($(this).css('left'));
				obj[i].width 	= design.convert.px($(this).css('width'));
				obj[i].height 	= design.convert.px($(this).css('height'));
								
				if(typeof $(this).data('rotate') != 'undefined')
					obj[i].rotate = $(this).data('rotate');
				else 
					obj[i].rotate = 0;
					
				var svg 		= $(this).find('svg');				
				obj[i].svg 		= $('<div></div>').html($(svg).clone()).html();
				var image 		= $(svg).find('image');
				if (typeof image[0] == 'undefined')
				{
					obj[i].img 	= false;
				}
				else
				{
					obj[i].img 		= true;
					var src 		= $(image).attr('xlink:href');
					obj[i].src 		= src;				
				}
				obj[i].zIndex	= this.style.zIndex;
				i++;
			});
			obj.sort(function(obj1, obj2) {	
				return obj1.zIndex - obj2.zIndex;
			});
			
			var canvas 			= document.createElement('canvas');
				canvas.width 	= area.width;
				canvas.height 	= area.height;
			var context = canvas.getContext('2d');
			
			var count = Object.keys(obj).length;
			
			var radius = design.convert.px(area.radius);		
			canvasLoad(obj, 0);
			var IE = /msie/.test(navigator.userAgent.toLowerCase());
			function canvasLoad(obj, i)
			{
				if (typeof obj[i] != 'undefined')
				{
					var item = obj[i];
					i++;		
					if (IE == true)
					{
					item.svg = item.svg.replace(' xmlns:NS1=""', '');
					item.svg = item.svg.replace(' NS1:xmlns:xlink="http://www.w3.org/1999/xlink"', '');
					item.svg = item.svg.replace(' xmlns="http://www.w3.org/2000/svg"', '');
					}
					if (item.img == true)
					{
						var mySrc = item.src;
					}
					else
					{
						//var mySrc 	= 'data:image/svg+xml;base64,'+window.btoa(item.svg);
						var mySrc 	= 'data:image/svg+xml,'+encodeURIComponent(item.svg);
					}
					
					var images 	= new Image();									
					images.onload = function(){
						if (radius > 0)
						{
							context.save();
							var x = 0, 
								y = 0;
							var w = area.width;
							var h = area.height;
							var r = x + w;
							var b = y + h;
							context.beginPath();
							context.moveTo(x+radius, y);
							context.lineTo(r-radius, y);
							context.quadraticCurveTo(r, y, r, y+radius);
							context.lineTo(r, y+h-radius);
							context.quadraticCurveTo(r, b, r-radius, b);				
							context.lineTo(x+radius, b);
							context.quadraticCurveTo(x, b, x, b-radius);				
							context.lineTo(x, y+radius);
							context.quadraticCurveTo(x, y, x+radius, y);
							context.closePath();
							context.clip();
						}						
						if (item.rotate != 0)
						{
							context.translate(item.left, item.top);
							context.translate(item.width/2, item.height/2);
							context.rotate(item.rotate * Math.PI/180);
							item.left = (item.width/2) * -1;
							item.top = (item.height/2) * -1;
						}
						try {							
							if (item.img == true)
								context.drawImage(images, item.left, item.top, item.width, item.height);
							else
								context.drawImage(images, item.left, item.top);
							context.restore();
							canvasLoad(obj, i);
						}
						catch (e) {
							if (e.name == "NS_ERROR_NOT_AVAILABLE") {								
							}
						}
					}
					images.src 	= mySrc;
				}
				else {
					design.svg.canvas(postion, canvas, callback);
				}
			}
			return canvas;
		},
		canvas: function(postion, canvas1, callback){			
		    var imageData = app.state.getImage();
		    var area = {
		        top: imageData['printable_' + postion + '_top'],
		        left: imageData['printable_' + postion + '_left'],
		        width: imageData['printable_' + postion + '_width'],
		        height: imageData['printable_' + postion + '_height']
		    };
			
			var canvas 			= document.createElement('canvas');
			canvas.width = imageData.width;
			canvas.height = imageData.height;
			var context = canvas.getContext('2d');
			context.rect(0, 0, imageData.width, imageData.height);
			context.fillStyle = app.state.color.value;
			context.fill();

			design.output[postion] = canvas;

		    var obj = [
		        {
		            id: 'images-0',
		            width: imageData.width + 'px',
		            height: imageData.height + 'px',
		            top: '0px',
		            left: '0px',
		            zIndex: 'auto',
		            img: assetsUrls.products + 'product_type_' + app.state.product.id + '_' + postion + '.png'
		        }, 
		        { id: 'area-design' }
		    ];

			canvasLoad(obj, 0);
			function canvasLoad(obj, i) {
				if (typeof obj[i] != 'undefined')
				{
					var layer = obj[i];
					i++;
					
					if (layer.id !== 'area-design')
					{
						var imageObj = new Image();
						var left 	= design.convert.px(layer.left);
						var top 	= design.convert.px(layer.top);
						var width 	= design.convert.px(layer.width);
						var height 	= design.convert.px(layer.height);
						imageObj.onload = function(){
							context.save();
							context.drawImage(imageObj, left, top, width, height);
							context.restore();
							canvasLoad(obj, i);
						}
						imageObj.src = layer.img;				
					}
					else
					{
						context.drawImage(canvas1, area.left, area.top);
						canvasLoad(obj, i);
					}
				}
				else
				{
					if (typeof callback === "function") {
						callback();
					}
				}
			}				
		}
	},
	saveDesign: function(){
	
		var vectors	= JSON.stringify(design.exports.vector());		
		var front = design.output.front.toDataURL();
		var back = design.output.back.toDataURL();
		var data = {
		    'front': front,
            'back':back,
			'vectors':vectors, 
			'fonts': design.fonts,
			'product_id':app.state.product.id,
			'design_id':design.design_id,
			'design_file':design.design_file,
			'designer_id':design.designer_id,
			'design_key':design.design_key,
			'product_color':app.state.color
		};
		app.state.design = data;
	    return data;
	},
	save: function(callback){
	    design.svg.items('front', function () {
	        design.svg.items('back', function() {
	            var data = design.saveDesign();
                if (typeof callback === 'function') {
                    callback(data);
                }
	        });
	    });
	},
	mask: function(load){
		if (load == true){
			$('#dg-mask').css('display', 'block');
			$('#dg-designer').css('opacity', '0.3');
		}
		else{
			$('#dg-mask').css('display', 'none');
			$('#dg-designer').css('opacity', '1');
		}
	},
	exports:{
		productColor: function(){
			return $('#product-list-colors span.active').data('color');
		},
		cliparts: function(){
			var arts = {};
			$.each(['front', 'back', 'left', 'right'], function(i, view){
				var list = [];
				if ($('#view-'+view +' .product-design').html().length > 10)
				{
					if ($('#view-'+view+' .content-inner').html() != '')
					{
						$('#view-'+view+' .drag-item').each(function(){
							if (typeof this.item.clipart_id != 'undefined')
								list.push(this.item.clipart_id);
						});
					}
					arts[view] = list;
				}
			});
			return arts;
		},
		vector: function(){
			var vectors = {};
			var postions = ['front', 'back'];
			$.each(postions, function(i, postion){
				if ($('#view-'+postion +' .product-design').html().length > 10)
				{					
					vectors[postion]	= {};
					var i = 0;
					$('#view-'+ postion).find('.drag-item').each(function(){
						vectors[postion][i] = {};
						var item = {};
						item.type		= this.item.type;
						item.width		= $(this).css('width');
						item.height		= $(this).css('height');
						item.top		= $(this).css('top');
						item.left		= $(this).css('left');
						item.zIndex		= $(this).css('z-index');
						var svg 		= $(this).find('svg');				
						item.svg		= $('<div></div>').html($(svg).clone()).html();
						if ($(this).data('rotate') != 'undefined')
							item.rotate	= $(this).data('rotate');
						else
							item.rotate	= 0;
											
						if (item.type == 'text' || item.type == 'team')
						{
							item.text					= this.item.text;
							item.color					= this.item.color;
							item.fontFamily				= this.item.fontFamily;
							item.align					= this.item.align;
							item.outlineC				= this.item.outlineC;
							item.outlineW				= this.item.outlineW;
							if (typeof this.item.weight != 'undefined')
								item.weight 			= this.item.weight;
							
							if (typeof this.item.Istyle != 'undefined')
								item.Istyle 			= this.item.Istyle;
								
							if (typeof this.item.decoration != 'undefined')
								item.decoration 		= this.item.decoration;
						}
						else if(item.type === 'clipart')
						{
							item.change_color	= this.item.change_color;
							item.title			= this.item.title;
							item.file_name		= this.item.file_name;
							item.file			= this.item.file;
							item.thumb			= this.item.thumb;
							item.url			= this.item.url;						
							item.url			= this.item.url;
							if(typeof this.item.clipart_id != 'undefined'){item.clipart_id = this.item.clipart_id;}
						}
						vectors[postion][i] = item;
						i++;
					});	
				}
			});
			
			return vectors;
		}
	},
	imports:{
		vector: function(str){
			if (str == '') return false;
			
			var postions = {front:0, back:1, left:2, right:3};
			var a 		 = document.getElementById('product-thumbs').getElementsByTagName('a');
			str = str.replace('{ front":{', '{"front":{');
			var vectors = eval('('+str+')');
			
			$.each(vectors, function(postion, view){
				if ( Object.keys(view).length > 0 && $('#view-'+postion+' .product-design').html() != '' )
				{
					design.products.changeView( a[postions[postion]], postion );			
					$.each(view, function(i, item){
						design.item.imports(item);
					});
				}
			});
			design.team.changeView();
		},
		productColor: function(color){
			design.mask(true);
			var i = 0;
			$('#product-list-colors .bg-colors').each(function(){
				if($(this).data('color') == color)
				{
					design.products.changeColor(this, i);
					design.mask(false);
				}
				i++;
			});
			design.mask(false);
		},
		loadDesign: function(key){
			design.mask(true);
			var self = this;
			
			$.ajax({				
				dataType: "json",
				url: baseURL + "ajax/design/"+key		
			}).done(function( data ) {
				if (data.error == 1)
				{
					alert(data.msg);
				}
				else
				{
					design.design_id 	= data.design.id;
					design.design_file 	= data.design.image;
					design.designer_id 	= data.design.user_id;
					design.design_key 	= data.design.design_id;
					design.fonts 		= data.design.fonts;
					if (design.fonts != '')
					{
						$('head').append(design.fonts);
					}
					self.vector(data.design.vectors);
					if (data.design.teams != '')
					{
						design.teams = eval ("(" + data.design.teams + ")");
						design.team.load(design.teams);
					}					
				}
			}).always(function(){
				design.mask(false);
			});
		}
	}
};

$('#design-area').mouseup(function (e) {
    wasMouseUp = !wasMouseUp;
});

$(document).ready(function () {
    $('.max-colors-count').html(maxDesignColors);
	design.ini();
	$('#design-area').mousedown(function(e){
		var topCurso=!document.all ? e.clientY: event.clientY;
		var leftCurso=!document.all ? e.clientX: event.clientX;
        var view = app.state.getView();
        var mouseDownAt = document.elementFromPoint(leftCurso, topCurso);
        if (isResizing && wasMouseUp) {
            if (isResizing || wasMouseUp) {
                wasMouseUp = false;
                isResizing = false;
            }
        } else {
            if (mouseDownAt.parentNode.className == 'product-design'
                        || mouseDownAt.parentNode.className == 'div-design-area'
                        || mouseDownAt.parentNode.className == '#view' + view
                        || mouseDownAt.parentNode.className == 'content-inner'
                        || mouseDownAt.parentNode.className == 'back labView'
                        || mouseDownAt.parentNode.className == 'front labView') {
                design.item.unselect();
                $('#enter-text').val('');
                e.preventDefault();
                $('.drag-item').click(function () { design.item.select(this) });
            }
            
        }
        
		
	});
	$('.number').mousedown(function (e) {
	    design.item.unselect();
	});
	$('#nextPage').mousedown(function (e) {
	    design.item.unselect();
	});
    	$('.drag-item').click(function () { alert(23); });
});
