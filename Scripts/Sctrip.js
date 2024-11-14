function ok(element) {
    const parent = element.parentElement;
    parent.remove();
    
}
function geturrl() {
    var urlChinh = window.location.origin;
    var aTag = document.getElementById("trang-chu");
    aTag.href = urlChinh;
}