(function ($) {
	var listbox_pair = function (element, options) {
		var $el = $(element);
		
		this.element = $el;
		this.options = $.extend({}, $.fn.listbox_pair.defaults, options);
		
		var $first = $el.find('select[multiple].listbox-from');
		if ( !$first.length ) $first = $('<select></select>').attr('multiple', 'multiple').addClass('listbox-from').appendTo($el);
		
		var $second = $el.find('select[multiple].listbox-to');
		if ( !$second.length ) $second = $('<select></select>').attr('multiple', 'multiple').addClass('listbox-to').insertAfter($first);
		
		var $btnAdd = $el.find('.button-add');
		var $btnAddAll = $el.find('.button-add-all');
		var $btnRemove = $el.find('.button-remove');
		var $btnRemoveAll = $el.find('.button-remove-all');
		
		var $children = $el.children();
		var place = ( $children.index($first) < $children.index($second) ? 'insertBefore' : 'insertAfter');
		
		if ( !$btnAdd.length ) $btnAdd = $('<button></button>').attr('type', 'button').addClass('.button-add')[place]($second);
		if ( !$btnAddAll.length ) $btnAddAll = $('<button></button>').attr('type', 'button').addClass('.button-add-all')[place]($second);
		if ( !$btnRemove.length ) $btnRemove = $('<button></button>').attr('type', 'button').addClass('.button-remove')[place]($second);
		if ( !$btnRemoveAll.length ) $btnRemoveAll = $('<button></button>').attr('type', 'button').addClass('.button-remove-all')[place]($second);
		
		$btnAdd.click(function() {
			$first.find('> option[selected]').appendTo($second);
		});
		$btnAddAll.click(function() {
			$first.find('> option').appendTo($second);
		});
		
		$btnRemove.click(function() {
			$second.find('> option[selected]').appendTo($first);
		});
		$btnRemoveAll.click(function() {
			$second.find('> option').appendTo($first);
		});
	};
	
	$.fn.listbox_pair = function (options) {
		return this.each(function() {
			$.data(this, 'listbox_pair', new listbox_pair(this, options));
		});
	};
	
	$.fn.listbox_pair.defaults = {
		
	};
})(jQuery);