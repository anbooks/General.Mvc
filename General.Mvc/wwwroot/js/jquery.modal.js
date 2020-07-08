

$.modal = {
	show:function(option){
		var defaults = {
			//页面层次
			zIndex: 999,
			//背景色
			backgroundColor: '#000',
			//透明度
			opacity: 0.5,
			//默认文本信息
			loadingTips:'小贱君在拼命处理中...',
			//提示文本颜色
			TipsColor:'#fff'
			//

		};
		var options = $.extend(defaults, options);
		 
		var _LoadingHtml = '<div id="loadingPage" style="position:fixed;height:100%;width:100%;z-index:' + options.zIndex + ';background:' + options.backgroundColor + ';left:0;top:0;opacity:0.2;"><div id="loadingTips" style="position:relative;top:100px;text-align:center; font-size:20px; width:500px; opacity:1;margin:0px auto;"><i class="ace-icon fa fa-spinner fa-spin white" style="font-size:50px;"></i><p style="color:' + options.TipsColor + ';">' + options.loadingTips + '</p></div></div>';
		return $(_LoadingHtml).appendTo($('body'));
	},
	hide: function () {
		$('#loadingPage').remove();
	},

	confirm: function (msg,callback) {
	    bootbox.confirm({
	        message: msg,
	        buttons: {
	            confirm: {
	                label: "确定",
	                className: "btn-primary btn-sm",
	            },
	            cancel: {
	                label: "取消",
	                className: "btn-sm",
	            }
	        },
	        callback: function (result) {
	            if (result) {
	                $.modal.show();
	                callback(fn);
	            }
	        }
	    });
	    var fn = function (result) {
	        if(result)
	            $.modal.hide();
	    }
	},
	gritter: function (data) {
	    //ajax处理提示，不需要刷新页面处理的才使用
	    $.modal.hide();
	    $.gritter.add({
            title: '提示信息',
            text: data.message == null ? '' : data.message,
            class_name: data.status ? 'gritter-success' : 'gritter-warning'
	    });
	},
	alert: function (data) {
	    //ajax处理提示，刷新页面使用
        $.modal.hide(); 
	    bootbox.alert(data.message, function () {
	        if (data.status)
	            window.location.reload();
	    });  
	}
};








































