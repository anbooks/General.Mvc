


$.ajaxSetup({
    contentType: "application/x-www-form-urlencoded; charset=utf-8",
    error: function (xhr, status, errorMsg) {
        if (xhr.status == 500) {
            try{
                errorMsg = xhr.responseJSON.Message;
            }catch(e){

            }
        }
        $.gritter.add({
            title: '错误提示',
            text: '发送AJAX请求到"' + this.url + '"时出错[' + xhr.status + ']：' + errorMsg,
            class_name: 'gritter-error gritter-center'
        });
        $.modal.hide();
    }
});




















