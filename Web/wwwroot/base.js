var path = "";
var httpsPath = "";
var websocketPath="";
var ERROE_HINT = '服务器私奔了，喝口茶歇会吧。';
var index=1;
function getRootPath() {

	//获取当前网址，如： http://localhost:8083/maintain/share/meun.jsp

	var curWwwPath = window.document.location.href;

	//获取主机地址之后的目录，如： maintain/share/meun.jsp

	var pathName = window.document.location.pathname;

	var pos = curWwwPath.indexOf(pathName);

	//获取主机地址，如： http://localhost:8083

	var localhostPaht = curWwwPath.substring(0, pos);

	//获取带"/"的项目名，如：/maintain

	var projectName = pathName
			.substring(0, pathName.substr(1).indexOf('/') + 1);

	return (localhostPaht + projectName + "/");

}
$().ready(function(e) {
	path="http://dev.blibao.com/SweepPay/";
	httpsPath="https://devo.blibao.com/SweepPay/";
	var source = $("#source").val(); 
	if(source!=null && typeof(paySource) != undefined && source!="")
	  path = httpsPath;
})

function startLoading(){
    var loadObj = "";
        loadObj +='<div id="base_loading" class="fc_logo">';
        loadObj +='<div class="fc_logo_by">';
        loadObj +='<div class="fc_logo_gif">';
        loadObj +='<div class="load_inner"><img src="'+path+'assets/images/loading_inner.png"></div>';
        loadObj +='<div class="load_outer"><img src="'+path+'assets/images/loading_outer.png"></div>';
        loadObj +='</div>';
        loadObj +='</div>';
        loadObj +='</div>';
    0 == $("#base_loading").length && $("body").append(loadObj);
}
function stopLoading(){
    $("#base_loading").remove();
}
function redirectAnimate(){
    startLoading();
    setTimeout(function () {
        stopLoading()
    }, 1500);
}

String.prototype.trim = function() {
	return this.replace(/(^\s+)|(\s+$)/g,"").replace(/\s/g,"");
}

String.prototype.ltrim=function(){
      return this.replace(/(^\s*)/g,"");
}

String.prototype.rtrim=function(){
      return this.replace(/(\s*$)/g,"");
}

//数组方法
Array.prototype.contains = function ( needle ) {
	for (i in this) {
		if (this[i] === needle) return true;
	}
	return false;
}
