var aniSpeed = 200;
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
		
		$('.dg-tooltip').tooltip();
		$( "#layers" ).sortable({stop: function( event, ui ) {
			me.layers.sort();
		}});		
		$('.popover-close').click(function(){
			$( ".popover" ).hide('show');
		});
        $("#app-wrap").flip({
            trigger: 'manual'
        });
        $('.flip-button:not(.flip-button-active)').on('click', function(event){
            design.products.changeView(app.state.isFront?'back':'front');
        });
        $('.design-area-zoom').on('click', function(){
            app.state.zoomed = !app.state.zoomed;
            if(app.state.zoomed){
                $('#design-area').css('transform','scale(1.4, 1.4)');
            }else{
                $('#design-area').css('transform','scale(1, 1)');
            }
            design.products.setDesignAreaContrastColor(app.state.color);
        });
		design.item.move();
		$jd( "#dg-outline-width" ).slider({
			animate: true,
			slide: function( event, ui ) {
				$('.outline-value').html(ui.value);
				design.text.update('outline-width', ui.value);
			}
		});
		
		$jd( "#dg-shape-width" ).slider();
		
		$jd('.dg-color-picker-active').click(function(){
			$jd(this).parent().find('ul').show(aniSpeed);
		});
		
		/* rotate */
		$jd('.rotate-refresh').click(function(){
			me.item.refresh('rotate');
		});
		$jd('.rotate-value').on("focus change", function(){
			var e = me.item.get();
			var deg = $jd(this).val();
			if(deg > 360) deg = 360;
			if(deg < 0) deg = 0;
			var angle = ($jd(this).val() * Math.PI)/180;
			e.rotatable("setValue", angle);	
		});
		
		/* lock */
		$jd('.ui-lock').click(function(){
			var e = me.item.get();
			e.resizable('destroy')			
			if($jd(this).is(':checked') == true) me.item.resize(e, 'n, e, s, w, se');
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
			if( $('#dag-list-arts').html() == '')
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
		
		/* layers-toolbar control */
		$('.layers-toolbar button').click(function(){
			var elm = $(this).parents('.div-layers');
			if (elm.hasClass('no-active') == true)
			{
				elm.removeClass('no-active');
			}
			else
			{
				elm.addClass('no-active');
			}
		});
		
		/* mobile toolbar */
		$('.dg-options-toolbar button').click(function(){
			var check = $(this).hasClass('active');
			$('.dg-options-toolbar button').removeClass('active');
			var elm = $(this).parents('.dg-options');
			var type = $(this).data('type');
			
			if (check == true)
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
				if (e.data('value') != 'undefined')
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
			colors 				= {};
			colors.front 		= design.print.colors('front');			
			colors.back 		= design.print.colors('back');			
			colors.left 		= design.print.colors('left');			
			colors.right 		= design.print.colors('right');
			
			datas.print 		= {};			
			datas.print.sizes 	= JSON.stringify(design.print.size());
			datas.print.colors 	= JSON.stringify(colors);
			
			/* product attribute */
			var attributes = $('#tool_cart').serialize();
			if (attributes != '')
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
				if (data != '')
				{
					if (typeof data.sale != 'undefined')
					{
						$('.price-sale-number').html(data.sale);
						$('.price-old-number').html(data.old);
						
						if (data.sale == data.old)
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
			if (quantity == NaN || quantity < 1)
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
			design.svg.items('front', design.ajax.save);
		},
		active: 'back',
		save: function(){
			if (design.ajax.active == 'back')
			{
				design.ajax.active = 'left';
				if ($('#view-back .product-design').html() != '' && $('#view-back .product-design').find('img').length > 0)
				{
					design.svg.items('back', design.ajax.save);
				}
				else
				{
					delete design.output.back;
					design.ajax.save();
				}
			}
			else if (design.ajax.active == 'left')
			{
				design.ajax.active = 'right';
				if ($('#view-left .product-design').html() != '' && $('#view-left .product-design').find('img').length > 0)
				{
					design.svg.items('left', design.ajax.save);
				}
				else
				{
					delete design.output.left;
					design.ajax.save();
				}	
			}
			else if (design.ajax.active == 'right')
			{
				if ($('#view-right .product-design').html() != '' && $('#view-right .product-design').find('img').length > 0)
				{
					design.svg.items('right', design.ajax.addToCart);
				}
				else
				{
					delete design.output.right;
					design.ajax.addToCart();
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
				if (data != '')
				{
					var content = eval ("(" + data + ")");
					if (content.error == 0)
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
		removeDesign: function(e)
		{
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
						+ 	'<div class="carousel-inner"></div>';
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
					if (image.id != 'area-design')
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
			
			if (print_type == 'screen' || print_type == 'embroidery')
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
			var postions = ['front', 'back', 'left', 'right'];
			$('.screen-size').html('<div id="sizes-used"></div>');
			
			$.each(postions, function(i, postion){
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
					var item = eval ("(" + items.params[postion] + ")");
					sizes[postion] = {};
					sizes[postion].width = Math.round( (print.width * item.width)/area.width );
					sizes[postion].height = Math.round( (print.height * item.height)/area.height );
					
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
		art:{
			categories: function(load, index){
				if (typeof index == 'undefined') index = 0;
				self = this;
				
				var ajax = true;
				if (typeof load != 'undefined' && load == true)
				{
					$('#dag-art-categories').children('ul').each(function(){
						if (index == $(this).data('type'))
						{
							ajax = false;
						}
					});
				}
				else
				{
					ajax = false;
				}
				
				if (ajax == true)
				{					
					$('#dag-art-categories').addClass('loading');
					$.ajax({				
						dataType: "json",
						url: baseURL + "art/categories/"+index
					}).done(function( data ) {						
						if (data != '')
						{								
							var e = document.getElementById('dag-art-categories');
							var html = self.treeCategories(data, e, index);							
						}
					}).always(function(){
						$('#dag-art-categories').removeClass('loading');
					});					
				}
			},
			arts: function(cate_id)
			{
				var self = this;
				var parent = document.getElementById('dag-list-arts');
				parent.innerHTML = '';
				$('#dag-art-detail').hide(aniSpeed);
				$('#dag-list-arts').show(aniSpeed);
				$('#arts-add').hide();
				$('#dag-list-arts').addClass('loading');

				var page = $('#art-number-page').val();
				var keyword = $('#art-keyword').val();
				$.ajax({
					type: "POST",							
					data: { page: page, keyword: keyword},
					dataType: "json",					
					url: baseURL + "art/arts/"+cate_id
				}).done(function( data ) {
					if (data == null)
					{
						$('#dag-list-arts').removeClass('loading');
						parent.innerHTML = 'Data not found!';
						var ul = $('#arts-pagination .pagination').html('');
						$('#art-number-page').val(0);
						return false;
					}
					if (data.arts.length > 0)
					{
						$.each(data.arts, function(i, art){
							var url = art.path;
							var div = document.createElement('div');
								div.className = 'col-xs-3 col-md-2 box-art';
							var a = document.createElement('a');
								a.setAttribute('title', art.title);
								a.setAttribute('class', 'thumbnail');
								a.setAttribute('href', 'javascript:void(0)');
								a.setAttribute('onclick', 'design.designer.art.artDetail(this)');
								$(a).data('id', art.clipart_id);
								$(a).data('clipart_id', art.clipart_id);
								$(a).data('medium', url + art.medium);
								art.imgThumb = url + art.thumb;
								art.imgMedium = url + art.medium;
								a.item = art;
							var img = '<img alt="" src="'+url + art.thumb+'">';
							a.innerHTML = img;
							div.appendChild(a);
							parent.appendChild(div);
						});						
						if (data.count > 1)
						{
							$('#arts-pagination').css('display', 'block');
							var ul = $('#arts-pagination .pagination');
							ul.html('');
							for(var i=1; i<= data.count; i++)
							{
								var li = document.createElement('li');
								$(li).data('id', i-1);
								if ((i- 1) == page){
									li.className = 'active';
									li.innerHTML = '<a href="javascript:void(0)">'+i+'</a>';
								}else{
									li.innerHTML = '<a href="javascript:void(0)">'+i+'</a>';
								}
								ul.append(li);
								$(li).click(function(){
									if ( $(this).hasClass('active') == false )
									{
										$('#art-number-page').val( $(this).data('id') );
										self.arts(cate_id);
									}
								});
							}
						}
					}
					$('#dag-list-arts').removeClass('loading');
				});
			},
			artDetail: function(e)
			{
				var id = $(e).data('id');
				$('.box-art-detail').css('display', 'none');
				$('#arts-pagination').css('display', 'none');
				if (document.getElementById('art-detail-'+id) == null)
				{
					var div = document.createElement('div');
						div.className = 'box-art-detail';
						div.setAttribute('id', 'art-detail-'+id);
					var html = 	'<div class="col-xs-5 col-md-5 art-detail-left">'
							+ 		'<img class="thumbnail img-responsive" src="'+$(e).data('medium')+'" alt="">'
							+ 	'</div>'
							+ 	'<div class="col-xs-7 col-md-7 art-detail-right">'							
							+ 	'</div>';
					div.innerHTML = html;
					$('#dag-art-detail').append(div);
					$('#art-detail-'+id+' .art-detail-right').addClass('loading');
					$('.art-detail-price').html('');
					$.ajax({
						dataType: "json",					
						url: baseURL + "art/detail/"+id
					}).done(function( data ) {
						if (typeof data.error != 'undefined' && data.error == 0)
						{
							var info = $('#art-detail-'+id+' .art-detail-right');
							info.html('');
							if (typeof data.info.title != 'undefined')
								info.append('<h4>'+data.info.title+'</h4>');
								info.append('<p>'+data.info.description+'</p>');
								e.item.title = data.info.title;
													
							$('.art-detail-price').html('From ' + data.price.currency_symbol + data.price.amount);
							
						}					
						$('#art-detail-'+id+' .art-detail-right').removeClass('loading');
					}).fail(function(){
						$('#art-detail-'+id+' .art-detail-right').removeClass('loading');
					});
				}
				else
				{
					$('#art-detail-'+id).css('display', 'block');
				}				
				$('#dag-list-arts').hide(aniSpeed);
				$('#dag-art-detail').show(aniSpeed);
				$('#arts-add').show();
				$('#arts-add button').unbind('click');
				$('#arts-add button').bind('click', function(event){design.art.create(e);});
				$('#arts-add button').button('reset');
			},
			treeCategories: function(categories, e, system)
			{
				self = this;
				if (categories.length == 0) return false;
				var ul = document.createElement('ul');
				$(ul).data('type', system);
				$.each(categories, function(i, cate){
					var li = document.createElement('li'),
						a = document.createElement('a');						
						if ($.isEmptyObject(cate.children) == false)
						{
							var span = document.createElement('span');
								span.innerHTML = '<i class="glyphicons plus"></i>';
							$(span).click(function(){
								var parent = this;
								$(this).parent().children('ul').toggle(aniSpeed, function(){
									var display = $(parent).parent().children('ul').css('display');
									if (display == 'none')
										$(parent).children('i').attr('class', 'glyphicons plus');
									else
										$(parent).children('i').attr('class', 'glyphicons minus');
								});
							});
							li.appendChild(span);
						}			
						a.setAttribute('href', 'javascript:void(0)');
						a.setAttribute('title', cate.title);
						$(a).data('id', cate.id);
						$(a).click(function(){
							$('#dag-art-categories a').removeClass('active');
							$(a).addClass('active');
							$('#art-number-page').val(0);
							$('#arts-pagination .pagination').html('');
							self.arts(cate.id);
						});
						a.innerHTML = cate.title;
						li.appendChild(a);
					ul.appendChild(li);					
					if ($.isEmptyObject(cate.children) == false)
						design.designer.art.treeCategories(cate.children, li);
				});
				e.appendChild(ul);
			}
		},
		fonts: {},
		fontActive: {},
		loadColors: function(){
			var self = this;
			$.ajax({				
				dataType: "json",
				url: baseURL + "ajax/colors"			
			}).done(function( data ) {
				if (data.status == 1)
				{					
					self.addColor(data.colors);					
				}
			}).always(function(){			
			});
		},
		addColor: function(colors)
		{
			var screen_colors	= $('#screen_colors_list');
			var div = $('.other-colors');
			$(div).html('<span class="bg-colors bg-none" data-color="none" title="Normal" onclick="design.item.changeColor(this)"></span>');			
			$.each(colors, function(i, color){
				var span = document.createElement('span');
					span.className = 'bg-colors';
					span.setAttribute('data-color', color.hex);
					span.setAttribute('title', color.title);							
					span.setAttribute('onclick', 'design.item.changeColor(this)');							
					span.style.backgroundColor = '#'+color.hex;						
				$(div).append(span);				
				
				screen_colors.append('<span class="bg-colors" onclick="design.print.addColor(this)" style="background-color:#'+color.hex+'" data-color="'+color.hex+'" title="'+color.title+'"></span>');
			});	
		},
		loadFonts: function(){
			var self = this;
			$.ajax({				
				dataType: "json",
				url: baseURL + "ajax/fonts"			
			}).done(function( data ) {
				if (data.status == 1)
				{
					if (typeof data.fonts.google_fonts != 'undefined')
					{
						$('head').append("<link href='http://fonts.googleapis.com/css?family="+data.fonts.google_fonts+"' rel='stylesheet' type='text/css'>");
					}
					self.fonts = data.fonts;
					self.addFonts(data.fonts);
					var div = $('.list-fonts');
					$(div).html('');
					$.each(data.fonts.fonts, function(i, font){
						var a = document.createElement('a');
							a.className = 'box-font';							
							a.setAttribute('href', 'javascript:void(0)');
							$(a).data('id', font.id);
							$(a).data('title', font.title);
							$(a).data('type', font.type);
							if (font.type == '')
							{
								font.url = baseURL + font.path.replace('\\', '/') + '/';
								$(a).data('url', font.url);
								$(a).data('filename', font.filename);
								var html = '<img src="' + font.url + font.thumb + '" alt="'+font.title+'">'+font.title;
							}
							else
							{
								var html = '<h2 class="margin-0" style="font-family:\''+font.title+'\'">abc zyz</h2>'+font.title;
							}
							$(a).bind('click', function(){self.changeFont(this)});
						a.innerHTML = html;
						$(div).append(a);
					});
				}
			}).always(function(){			
			});
		},
		addFonts: function(data)
		{
			var self = this;
			var ul = $('.font-categories');
			ul.html('');
			var li = document.createElement('li');				
			$(li).bind('click', function(){self.cateFont(this)});
			$(li).data('id', 0);
			var html = '<a href="javascript:void(0);" title="All fonts">All fonts</a>';
			li.innerHTML = html;
			$(ul).append(li);
			$.each(data.categories, function(i, cate){
				var li = document.createElement('li');				
				$(li).bind('click', function(event){ event.preventDefault(); self.cateFont(this)});
				$(li).data('id', cate.id);
				var html = '<a href="javascript:void(0);" title="'+cate.title+'">'+cate.title+'</a>';
				li.innerHTML = html;
				$(ul).append(li);
			});			
		},
		cateFont: function(e)
		{
			var self = this;
			var id = $(e).data('id');
			if (typeof id != 'undefined')
			{
				var div = $('.list-fonts');
				$(div).html('');
				if (typeof this.fonts.cateFonts[id] != 'undefined')
				{
					var fonts = this.fonts.cateFonts[id]['fonts'];
				}
				else
				{
					var fonts = this.fonts.fonts;
				}
				$.each(fonts, function(i, font){
					var a = document.createElement('a');
						a.className = 'box-font';							
						a.setAttribute('href', 'javascript:void(0)');
						$(a).data('id', font.id);
						$(a).data('title', font.title);
						$(a).data('type', font.type);
						if (font.type == '')
						{
							font.url = baseURL + font.path.replace('\\', '/') + '/';
							$(a).data('url', font.url);
							$(a).data('filename', font.filename);
							var html = '<img src="' + font.url + font.thumb + '" alt="'+font.title+'">'+font.title;
						}
						else
						{
							var html = '<h2 class="margin-0" style="font-family:\''+font.title+'\'">abc zyz</h2>'+font.title;
						}
						$(a).bind('click', function(){self.changeFont(this)});
					a.innerHTML = html;
					$(div).append(a);
				});				
			}
		},
		changeFont: function(e)
		{
			var selected = design.item.get();
			if (selected.length == 0)
			{
				$('#dg-fonts').modal('hide');
				return false;
			}
			
			$('.list-fonts a').removeClass('active');
			$(e).addClass('active');
			var id = $(e).data('id');
			$('.labView.active .content-inner').addClass('loading');
			if (typeof id != 'undefined')
			{
				var title = $(e).data('title');
				$('#txt-fontfamily').html(title);
				if (typeof design.designer.fontActive[id] != 'undefined' || $(e).data('type') == 'google')
				{					
					design.text.update('fontfamily', title);
					$('.labView.active .content-inner').removeClass('loading');
					setTimeout(function(){
						var e = design.item.get();						
						var txt = e.find('text');
						var size1 = txt[0].getBoundingClientRect();
						var size2 = e[0].getBoundingClientRect();
						
						var $w 	= parseInt(size1.width);							
						var $h 	= parseInt(size1.height);							
						
						design.item.updateSize($w, $h);	
						
						var svg = e.find('svg'),
						view = svg[0].getAttributeNS(null, 'viewBox');
						var arr = view.split(' ');						
						var y = txt[0].getAttributeNS(null, 'y');						
						y = Math.round(y) + Math.round(size2.top) - Math.round(size1.top) - ( (Math.round(size2.top) - Math.round(size1.top)) * (($w - arr[2])/$w) );						
						txt[0].setAttributeNS(null, 'y', y);
					}, 200);
				}
				else
				{
					var filename = $(e).data('filename');
					var url = $(e).data('url');
					if (filename != '')
					{
						var item = eval ("(" + filename + ")");													
						design.designer.fontActive[id] = title;
						var css = "<style type='text/css'>@font-face{font-family:'"+title+"';font-style: normal; font-weight: 400;src: local('"+title+"'), local('"+title+"'), url("+url+item.woff+") format('woff');}</style>";
						design.fonts = design.fonts + ' '+css;
						$('head').append(css);
						
						var e = design.item.get();
						var svg = e.find('svg');							
						design.text.update('fontfamily', title);
						$('.labView.active .content-inner').removeClass('loading');
						setTimeout(function(){
							var txt = e.find('text');
							var size1 = txt[0].getBoundingClientRect();
							var size2 = e[0].getBoundingClientRect();
							var $w 	= parseInt(size1.width);							
							var $h 	= parseInt(size1.height);							
							
							design.item.updateSize($w, $h);

							var svg = e.find('svg'),
							view = svg[0].getAttributeNS(null, 'viewBox');
							var arr = view.split(' ');						
							var y = txt[0].getAttributeNS(null, 'y');						
							y = Math.round(y) + Math.round(size2.top) - Math.round(size1.top) - ( (Math.round(size2.top) - Math.round(size1.top)) * (($w - arr[2])/$w) );						
							txt[0].setAttributeNS(null, 'y', y);
						}, 200);								
					}
				}
			}
			$('#dg-fonts').modal('hide');
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
		changeView: function(position){
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
        setDesignAreaContrastColor: function(color){
            var hex = color.value || color;
            var $div = $('<div style="background:'+hex+'"></div>');
            var background = $div.css('background-color');
            var rgb = background.replace(/^(rgb|rgba)\(/,'').replace(/\)$/,'').replace(/\s/g,'').split(',');
            var yiq = ((rgb[0]*299)+(rgb[1]*587)+(rgb[2]*114))/1000;
            var isDarkBorder = yiq >= 128;
            var newColor = isDarkBorder?'rgba(0,0,0,0.3)':'rgba(255,255,255,0.3)';
            var textColor = !isDarkBorder?'rgba(0,0,0)':'rgba(255,255,255)';
            $('.design-area-toolbar')
                .toggleClass('dark', isDarkBorder)
                .stop().animate({backgroundColor: newColor, color: textColor}, aniSpeed);
            var isZoomed = !!app.state.zoomed;
            var selector = '.zoom-'+(isDarkBorder?'light':'dark')+'-'+(isZoomed?'out':'in');
            $('.printable-area-zoom-image').hide();
            $(selector).stop().show();
            $('.design-area').stop().animate({borderColor: newColor}, aniSpeed);
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
                    })
                    .hover(
                        function() {
                            $('.product-design img').stop().animate({backgroundColor: color.value}, aniSpeed);
                            me.setDesignAreaContrastColor(color);
                        }, function() {
                            $('.product-design img').stop().animate({backgroundColor: app.state.color.value},aniSpeed);
                            me.setDesignAreaContrastColor(app.state.color);
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
				$item.click(function(){
                    $('#products-list li').removeClass('active');
                    $(this).addClass('active');
                    me.changeDesign(product);
                } );
                var html = '<p class="item-name">'+product.name+'</p><div class="item-overview">'+
                    '<div class="item-thumb-container item-thumb-loaded"><img class="item-thumb" src="./assets/images/product_type_'+product.id+'_front_small.png"></div>' +
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
            $('#item-options-dropdown').html(options).on('change', function(e){
                var idList = me.categoryProducts[e.target.value];
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
		create: function(){
			$jd('.ui-lock').attr('checked', false);
			var txt = {};
			
			txt.text = 'Hello';
			txt.color = '#FF0000';
			txt.fontSize = '24px';
			txt.fontFamily = 'arial';
			txt.stroke = 'none';
			txt.strokew = '0';
			this.add(txt);			
		},
		setValue: function(o){
			$jd('#enter-text').val(o.text);
			$jd('#txt-fontfamily').html(o.fontFamily);
			var color = $jd('#txt-color');
				color.data('color', o.color);
				color.css('background-color', o.color);
				
			// text-align
			if (typeof o.align == 'undefined')
				o.align = 'center';
			$('#text-align span').removeClass('active');
			$('#text-align-'+o.align).addClass('active');
			
			if (typeof o.Istyle != 'undefined' && o.Istyle == 'italic')
				$('#text-style-i').addClass('active');
			else
				$('#text-style-i').removeClass('active');
			
			if (typeof o.weight != 'undefined' && o.weight == 'bold')
				$('#text-style-b').addClass('active');
			else
				$('#text-style-b').removeClass('active');
				
			if (typeof o.decoration != 'undefined' && o.decoration == 'underline')
				$('#text-style-u').addClass('active');
			else
				$('#text-style-u').removeClass('active');
		
			if (typeof o.color != 'undefined')
			{
				var obj = $('#txt-color');
				if (o.color == 'none')
					obj.addClass('bg-none');
				else
					obj.removeClass('bg-none');
					
				obj.data('color', o.color);
				obj.data('value', o.color);
				obj.css('background-color', '#'+o.color);
			}
			
			if (typeof o.outlineC == 'undefined')
			{
				o.outlineC	= 'none';
			}
			var obj = $('.option-outline .dropdown-color');
			if (o.outlineC == 'none')
				obj.addClass('bg-none');
			else
				obj.removeClass('bg-none');
				
			obj.data('color', o.outlineC);
			obj.data('value', o.outlineC);
			obj.css('background-color', '#'+o.outlineC);					
			
			if (typeof o.outlineW == 'undefined')
			{
				o.outlineW = 0;
			}
			$('.outline-value.pull-left').html(o.outlineW);
			$('#dg-outline-width a').css('left', o.outlineW + '%');			
		},
		add: function(o, type){
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
				item.stroke		= 'none';
				item.strokew 	= '0';
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
							
			text.setAttributeNS(null, 'fill', o.color);
			text.setAttributeNS(null, 'stroke', o.stroke);
			text.setAttributeNS(null, 'stroke-width', o.strokew);
			text.setAttributeNS(null, 'stroke-linecap', 'round');
			text.setAttributeNS(null, 'stroke-linejoin', 'round');
			text.setAttributeNS(null, 'x', parseInt($width/2));
			text.setAttributeNS(null, 'y', 20);				
			text.setAttributeNS(null, 'text-anchor', 'middle');				
			text.setAttributeNS(null, 'font-size', o.fontSize);
			text.setAttributeNS(null, 'font-family', o.fontFamily);
			
			if(typeof o.fontWeight != 'undefined')
			text.setAttributeNS(null, 'font-weight', o.fontWeight);
			
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
		},
		update: function(lable, value){
			var e = design.item.get();
			var txt = e.find('text');		
			if(typeof lable != 'undefined' && lable != '')
			{
				var obj = document.getElementById(e.attr('id'));
				switch(lable){
					case 'fontfamily':
						txt[0].setAttributeNS(null, 'font-family', value);
						obj.item.fontFamily = value;
						if (obj.item.type == 'text')
							$('#txt-fontfamly').html(value);
						else
							$('#txt-team-fontfamly').html(value);
						break;
					case 'color':
						var color = $jd('#txt-color').data('value');
						if (color == 'none') var hex = color;
						else var hex = '#' + color;
						txt[0].setAttributeNS(null, 'fill', hex);
						obj.item.color = hex;
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
						txt[0].setAttributeNS(null, 'stroke-width', value/50);
						txt[0].setAttributeNS(null, 'stroke-linecap', 'round');
						txt[0].setAttributeNS(null, 'stroke-linejoin', 'round');
						obj.item.outlineW = value;
						break;
					case 'outline':
						if (value == 'none') var hex = value;
						else var hex = '#' + value;
						txt[0].setAttributeNS(null, 'stroke', hex);
						txt[0].setAttributeNS(null, 'stroke-width', $jd('.outline-value').html()/50);
						obj.item.outlineC = hex;
						break;
					default:
						txt[0].setAttributeNS(null, lable, value);
						break;
				}
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
			var $w 	= parseInt(txt[0].getBoundingClientRect().width);
			var $h 	= parseInt(txt[0].getBoundingClientRect().height);
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
		create: function(e){
		
			var item = e.item;
			$jd('.ui-lock').attr('checked', false);				
			var o 			= {};
			o.type 			= 'clipart';			
			o.upload		= 1;			
			o.title 		= item.title;
			o.url 			= item.url;
			o.file_name 	= item.file_name;			
			o.thumb			= item.thumb;
			o.confirmColor	= true;
			o.remove 		= true;
			o.edit 			= false;
			o.rotate 		= true;	
			o.rotate 		= true;	
			
			
			if (item.file_type != 'svg')
			{
				o.file		= {};
				o.file.type	= 'image';				
				var img = new Image();
				design.mask(true);
				img.onload = function() {
					o.width 	= this.width;
					o.height	= this.height;
					if (this.width > 100)
					{
						o.width 	= 100;						
						o.height 	= (100/this.width) * this.height;
					}
					o.change_color = 0;					
								
					var content = '<svg xmlns="http://www.w3.org/2000/svg" xml:space="preserve" xmlns:xlink="http://www.w3.org/1999/xlink">'
								 + '<g><image x="0" y="0" width="'+o.width+'" height="'+o.height+'" xlink:href="'+item.thumb+'" /></g>'
								 + '</svg>';
					o.svg 		= $.parseHTML(content);					
					design.item.create(o);
					$jd('#dg-myclipart').modal('hide');
					design.mask(false);
				}
				img.src = item.thumb;
				return true;
			}
		}
	},
	art:{
		create: function(e){
			$('#arts-add button').button('loading');
			var item = e.item;
			$jd('.ui-lock').attr('checked', false);
			var img = $jd(e).children('img');			
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
			
			
			if (item.file_type != 'svg')
			{
				o.confirmColor	= true;
				var canvas = document.createElement('canvas');
				var context = canvas.getContext('2d');
				var img = new Image();
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
				img.src = urlCase +'?src='+ item.imgMedium+'&w=250&h=atuto&q=90';				
			}
			else
			{
				$jd.ajax({
					type: "POST",
					data: item,
					url: baseURL + "art/getSVG",
					dataType: "json",
					success: function(data){					
							o.width 		= data.size.width;
							o.height		= data.size.height;
							o.file			= data.info;						
							o.svg 			= $.parseHTML(data.content);
							design.item.create(o);
							var elm = design.item.get();			
							var svg = elm.children('svg');
							var html = $(svg[0]).html();
							$(svg[0]).html('<g>'+html+'</g>');
							$jd('.modal').modal('hide');					
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
            var image = design.products.images[state.product.id];
            $.each(positions, function(i, view){
                var $view = $('#view-'+view);
                var $images = $view.find('.product-design');
                $images.html('');
                var $img = $('<img>')
                    .addClass('product_images')
                    .attr('src', './assets/images/product_type_'+state.product.id+'_'+view+'.png')
                    .css({'background': color.value, 'width':image.width, 'height': image.height});
                $images.append($img);

                var $designArea = $('.design-area', $view);
                var $designAreaToolbar = $('.design-area-toolbar', $view);
                var prefix = 'printable_'+view+'_';
                $designArea.css({
                    'top':image[prefix+'top'],
                    'left':image[prefix+'left'],
                    'width': image[prefix+'width'],
                    'height': image[prefix+'height']
                });
                $designAreaToolbar.css({
                    'top':image[prefix+'top']+image[prefix+'height']+2,
                    'left':image[prefix+'left'],
                    'width': image[prefix+'width']+4
                });
            });
		},
		create: function(item){		
			this.unselect();
			$('.labView.active .design-area').css('overflow', 'visible');
			var e = $jd('#app-wrap .active .content-inner'),				
				span = document.createElement('span');
			var n = -1;
			$('#app-wrap .drag-item').each(function(){
				var index 	= $(this).attr('id').replace('item-', '');
				if (index > n) n = parseInt(index);
			});			
			var n = n + 1;			
			
			span.className = 'drag-item-selected drag-item';
			span.id 		= 'item-'+n;
			span.item 		= item;
			item.id 		= n;
			$(span).bind('click', function(){design.item.select(this)});
			var center = this.align.center(item);
			span.style.left = center.left + 'px';
			span.style.top 	= center.top + 'px';
			span.style.width 	= item.width+'px';
			span.style.height 	= item.height+'px';
			
			$(span).data('id', item.id);
			$(span).data('type', item.type);
			$(span).data('file', item.file);
			$(span).data('width', item.width);
			$(span).data('height', item.height);
			
			span.style.zIndex = design.zIndex;
			design.zIndex  	= design.zIndex + 5;
			span.style.width = item.width;
			span.style.height = item.height;					
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
			
			if(item.remove == true)
			{
				var remove = document.createElement('div');
				remove.className = 'item-remove-on glyphicons bin';
				remove.setAttribute('title', 'Click to remove this item');
				remove.setAttribute('onclick', 'design.item.remove(this)');
				$(span).append(remove);				
			}
			
			if(item.edit == true)
			{
				var edit = document.createElement('div');
				edit.className = 'item-edit-on glyphicons pencil';
				edit.setAttribute('title', 'Click to edit this item');
				edit.setAttribute('onclick', 'design.item.edit(this)');
				$(span).append(edit);
			}	
			
			e.append(span);
					
			this.move($jd(span));
			this.resize($jd(span));	
			if(item.rotate == true)
				this.rotate($jd(span));
			design.layers.add(item);
			this.setup(item);
			$('.btn-action-edit').css('display', 'none');
			if (print_type == 'screen' || print_type == 'embroidery')
			{
				if (item.confirmColor == true)
				{
					this.setupColorprint(span);
					$('.btn-action-edit').css('display', 'block');
				}				
			}
			design.print.colors();			
			design.print.size();			
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
			if (print_type == 'screen' || print_type == 'embroidery')
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
			$('.labView.active .design-area').css('overflow', 'visible');
			var e = $jd('#app-wrap .active .content-inner'),				
				span = document.createElement('span');
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
				var align 	= {},
				area 		= $('.labView.active .content-inner');
				align.left 	= ($(area).width() - item.width)/2;
				align.left 	= parseInt(align.left);
				align.top 	= ($(area).height() - item.height)/2;
				align.top	= parseInt(align.top);
				return align;
			}
		},
		move: function(e){
			if(!e) e = $jd('.drag-item-selected');
			e.draggable({/*containment: "#dg-designer", */scroll: false, 
				drag:function(event, ui){
					var e = ui.helper;
					
					var o = e.parent().parent();
					var	left = o.css('left');
						left = parseInt(left.replace('px', ''));
						
					var	top = o.css('top');
						top = parseInt(top.replace('px', ''));
					var	width = o.css('width');
						width = parseInt(width.replace('px', ''));
					
					var	height = o.css('height');
						height = parseInt(height.replace('px', ''));
												
					var $left = ui.position.left,
						$top = ui.position.top,
						$width = e.width(),
						$height = e.height();
					if($left < 0 || $top < 0 || ($left+$width) > width || ($top+$height) > height){
						e.data('block', true);
						e.css('border', '1px solid #FF0000');						
					}else{
						e.data('block', false);
						e.css('border', '1px dashed #444444');
					}
				},
				stop: function( event, ui ) {
					design.print.size();
				}
			});						
		},
		resize: function(e, handles){
			if(typeof handles == 'undefined') handles = 'se';
			
			if(handles == 'se') {var auto = true; e = e;}
			else {var auto = false;}
			if(!e) e = $jd('.drag-item-selected');
						
			var oldwidth = 0, oldsize=0;		
			e.resizable({minHeight: 15, minWidth: 15,				
				aspectRatio: auto,
				handles: handles,
				start: function( event, ui ){
					oldwidth = ui.size.width;
					oldsize = $jd('#dg-font-size').text();
				},
				stop: function( event, ui ) {
					design.print.size();
				},
				resize: function(event,ui){
					var e = ui.element;
					var o = e.parent().parent();
					var	left = o.css('left');
						left = parseInt(left.replace('px', ''));
						
					var	top = o.css('top');
						top = parseInt(top.replace('px', ''));
					var	width = o.css('width');
						width = parseInt(width.replace('px', ''));
					
					var	height = o.css('height');
						height = parseInt(height.replace('px', ''));
																		
					var $left = parseInt(ui.position.left),
						$top = parseInt(ui.position.top),
						$width = parseInt(ui.size.width),
						$height = parseInt(ui.size.height);
					if(($left + $width) > width || ($top + $height)>height){
						e.data('block', true);
						e.css('border', '1px solid #FF0000');
						if(parseInt(left + $left + $width) > 490 || parseInt(top + $top + $height) > 490){
							//$jd(this).resizable('widget').trigger('mouseup');
						}
					}else{
						e.data('block', false);
						e.css('border', '1px dashed #444444');
					}
					var svg = e.find('svg');									
					
					svg[0].setAttributeNS(null, 'width', $width);
					svg[0].setAttributeNS(null, 'height', $height);		
					svg[0].setAttributeNS(null, 'preserveAspectRatio', 'none');					
					
					if(e.data('type') == 'clipart')
					{
						var file = e.data('file');
						if(file.type == 'image')
						{	
							var img = e.find('image');
							img[0].setAttributeNS(null, 'width', $width);
							img[0].setAttributeNS(null, 'height', $height);
						}
					}
					
					if(e.data('type') == 'text')
					{						
						//var text = e.find('text');
						//text[0].setAttributeNS(null, 'y', 20);						
					}
					
					$('#'+e.data('type')+'-width').val(parseInt($width));
					$('#'+e.data('type')+'-height').val(parseInt($height));
				}				
			});
		},
		rotate: function(e, angle){
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
				}
			});	
			design.print.size();
		},
		select: function(e){
			this.unselect();
			$('.labView.active .design-area').css('overflow', 'visible');
			$jd(e).addClass('drag-item-selected');
			$jd(e).css('border', '1px dashed #444444');	
			$jd(e).resizable({ disabled: false, handles: 'e' });
			$jd(e).draggable({ disabled: false });
			design.popover('add_item_'+$(e).data('type'));
			$('.add_item_'+$(e).data('type')).addClass('active');
			design.menu($(e).data('type'));
			this.update(e);
			this.printColor(e);
			design.layers.select($(e).attr('id').replace('item-', ''));			
		},
		unselect: function(e){
			$jd('#app-wrap .drag-item-selected').each(function(){
				$jd(this).removeClass('drag-item-selected');
				$jd(this).css('border', 0);				
				$jd(this).resizable({ disabled: true, handles: 'e' });
				$jd(this).draggable({ disabled: true });
			});
			$('.labView.active .design-area').css('overflow', 'hidden');
			$( ".popover" ).hide();
			$('.menu-left a').removeClass('active');
			$('#layers li').removeClass('active');
			$('#dg-popover .dg-options-toolbar button').removeClass('active');
			$('#dg-popover .dg-options-content').removeClass('active');
			$('#dg-popover .dg-options-content').children('.row').removeClass('active');
		},
		remove: function(e){
			e.parentNode.parentNode.removeChild(e.parentNode);
			var id = $(e.parentNode).data('id');
			if($jd('#layer-'+id)) $jd('#layer-'+id).remove();
			$( "#dg-popover" ).hide(aniSpeed);			
			design.print.colors();
			design.print.size();
			return;
		},
		setup: function(item){
			if(item.type == 'clipart')
			{
				$('.popover-title').children('span').html('Edit clipart');
				
				/* color of clipart */
				var e = this.get();
				if (item.change_color == 1)
				{
					var colors = design.svg.getColors(e.children('svg'));				
				}
				if(typeof colors != 'undefined' && item.change_color == 1)
				{
					$('#'+item.type+'-colors').css('display', 'block');
					$('.btn-action-colors').css('display', 'block');
					var div = $('#list-clipart-colors');
					div.html('');
					for(var color in colors)
					{
						if (color == 'none') continue;
						var a = document.createElement('a');
							a.setAttribute('class', 'dropdown-color');
							a.setAttribute('data-placement', 'top');
							a.setAttribute('data-original-title', 'Click to change color');
							a.setAttribute('href', 'javascript:void(0)');
							a.setAttribute('data-color', color);
							a.setAttribute('style', 'background-color:'+color);
							$.data(a, 'colors', colors[color]);
							a.innerHTML = '<span class="ui-accordion-header-icon ui-icon ui-icon-triangle-1-s"></span>';
							div.append(a);
					}
				}
				else{
					$('#'+item.type+'-colors').css('display', 'none');
					$('.btn-action-colors').css('display', 'none');
				}
			}
			
			if(item.type == 'text'){
				$('.popover-title').children('span').html('Edit text');
			}
			document.getElementById(item.type + '-width').value = parseInt(item.width);
			document.getElementById(item.type + '-height').value = parseInt(item.height);
			document.getElementById(item.type + '-rotate-value').value = 0;		
			
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
			var e = $jd('#app-wrap .drag-item-selected');
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
		flip: function(n){
			var e = this.get(),
				svg = e.find('svg'),
				transform = '';
			var viewBox = svg[0].getAttributeNS(null, 'viewBox');
			var size = viewBox.split(' ');
			
			if(typeof e.data('flipX') == 'undefined') e.data('flipX', true);
			if(e.data('flipX') === true){
				transform = 'translate('+size[2]+', 0) scale(-1,1)';
				e.data('flipX', false);
			}
			else{
				transform = 'translate(0, 0) scale(1,1)';
				e.data('flipX', true);
			}					
			var g = $(svg[0]).children('g');
			if (g.length > 0)
				g[0].setAttributeNS(null, 'transform', transform);
		},
		center: function(){
			var e = this.get(),
				$width = e.width(),
				pw 		= e.parent().parent().width();
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
			if(value.indexOf('px') != -1)
			{
				var px = value.replace('px', '');
			}
			var px = parseInt(value);
			return Math.round(px);
		}
	},
	upload:{
		computer: function()
		{
			if ($('#upload-copyright').is(':checked') == false)
			{
				alert('Please tick the checkbox');
				return false;
			}
			
			if ($('#files-upload').val() == '')
			{
				alert('Please choose a file upload.');
				return false;
			}
			
			return true;
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
		items: function(postion, callback)
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
			function canvasLoad(obj, i)
			{
				if (typeof obj[i] != 'undefined')
				{
					var IE = /msie/.test(navigator.userAgent.toLowerCase());
					var item = obj[i];
					i++;
					if (IE === true)
					{
						item.svg = item.svg.replace(' xmlns:NS1=""', '');
						item.svg = item.svg.replace(' NS1:xmlns:xlink="http://www.w3.org/1999/xlink"', '');
						item.svg = item.svg.replace(' xmlns="http://www.w3.org/2000/svg"', '');
					}				
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
						context.save();
						context.translate(item.left, item.top);
						context.translate(item.width/2, item.height/2);
						context.rotate(item.rotate * Math.PI/180);
						item.left = (item.width/2) * -1;
						item.top = (item.height/2) * -1;
					}
					try {							
						if (item.img == true)
						{
							var images 	= new Image();
							images.src = item.src;
							context.drawImage(images, item.left, item.top, item.width, item.height);
						}
						else
						{
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
					design.svg.canvas(postion, canvas, callback);
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
				else
				{					
					design.svg.canvas(postion, canvas, callback);
				}
			}
			return canvas;
		},
		canvas: function(postion, canvas1, callback){			
			var area 	= eval ("(" + items['area'][postion] + ")");
			var index	= $('#product-list-colors span').index($('#product-list-colors span.active'));
			
			var canvas 			= document.createElement('canvas');
				canvas.width 	= 500;
				canvas.height 	= 500;
			var context = canvas.getContext('2d');
						
			design.output[postion] = canvas;
			
			var layers 	= eval ("(" + items["design"][index][postion] + ")");			
			var count = Object.keys(layers).length;
				count = parseInt(count) - 1;
			var obj = [], j = 0;
			for (i= count; i> -1; i--)
			{
				obj[j] = layers[i];
				j++;
			}
			canvasLoad(obj, 0);
			function canvasLoad(obj, i)
			{
				if (typeof obj[i] != 'undefined')
				{
					var layer = obj[i];
					i++;
					
					if (layer.id != 'area-design')
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
						imageObj.src = baseURL +'image-tool/index.php?src='+ baseURL + layer.img +'&w='+width+'&h='+height;				
					}
					else
					{
						var left 	= design.convert.px(area.left);
						var top 	= design.convert.px(area.top);				
						context.drawImage(canvas1, left, top);
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
		var image = design.output.front.toDataURL();
		var teams = JSON.stringify(design.teams);
		var productColor = design.exports.productColor();
		var data = {
			"image":image, 
			'vectors':vectors, 
			'vectors':vectors, 
			'teams':teams,
			'fonts': design.fonts,
			'product_id':product_id,
			'design_id':design.design_id,
			'design_file':design.design_file,
			'designer_id':design.designer_id,
			'design_key':design.design_key,
			'product_color':productColor
		};
		
		$.ajax({
			url: baseURL + "user/saveDesign",
			type: "POST",
			contentType: 'application/json',
			data: JSON.stringify(data),
		}).done(function( msg ) {
			var results = eval ("(" + msg + ")");
			
			if (results.error == 1)
			{
				alert(results.msg);
			}
			else
			{
				design.design_id = results.content.design_id;
				design.design_file = results.content.design_file;
				design.designer_id = results.content.designer_id;
				design.design_key = results.content.design_key;
				design.productColor = productColor;
				design.product_id = product_id;
				var linkEdit 	= baseURL + 'design/index/'+product_id+'/'+productColor+'/'+results.content.design_key;			
				$('#link-design-saved').val(linkEdit);
				$('#dg-share').modal();				
			}
			
			$('#dg-mask').css('display', 'none');
			$('#dg-designer').css('opacity', '1');
		});		
	},
	save: function(){
		if (user_id == 0)
		{
			$('#f-login').modal();
		}
		else
		{	
			if (user_id == design.designer_id)
			{
				$( "#save-confirm" ).dialog({
					resizable: false,			  
					height: 200,
					width: 350,
					closeText: 'X',
					modal: true,
					buttons: [
						{
							text: "Save New",
							icons: {
								primary: "ui-icon-heart"
							},
							click: function() {
								$( this ).dialog( "close" );
								$('#dg-mask').css('display', 'block');
								$('#dg-designer').css('opacity', '0.3');
								
								design.design_id = 0;								
								design.design_key = '';
								design.design_file = '';								
								design.svg.items('front', design.saveDesign);
							}
						},
						{
							text: "Update",
							icons: {
								primary: "ui-icon-heart"
							},
							click: function() {
								$( this ).dialog( "close" );
								$('#dg-mask').css('display', 'block');
								$('#dg-designer').css('opacity', '0.3');
								design.svg.items('front', design.saveDesign);
							}
						}
					]
				});
			}
			else
			{
				$('#dg-mask').css('display', 'block');
				$('#dg-designer').css('opacity', '0.3');
				design.svg.items('front', design.saveDesign);
			}		
		}
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
			var postions = ['front', 'back', 'left', 'right'];
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
						else if(item.type == 'clipart')
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

$(document).ready(function(){
	design.ini();
	$('#design-area').click(function(e){
		var topCurso=!document.all ? e.clientY: event.clientY;
		var leftCurso=!document.all ? e.clientX: event.clientX;
		var mouseDownAt = document.elementFromPoint(leftCurso,topCurso);
		if( mouseDownAt.parentNode.className == 'product-design'
			|| mouseDownAt.parentNode.className == 'div-design-area'			
			|| mouseDownAt.parentNode.className == 'labView active'
			|| mouseDownAt.parentNode.className == 'content-inner' )
		{
			design.item.unselect();
			e.preventDefault();
			$('.drag-item').click(function(){design.item.select(this)});
		}
	});
	
	$('.drag-item').click(function(){alert(23); });
});
