
function $AjaxPost(_url, _parsm, _callback, _json, _async) {
    var asyncNp = _async;
    if (asyncNp == null) {
        asyncNp = false;
    }
    var jsons = _json;
    if (jsons == null) {
        jsons = "json";
    }
    $.ajax({
        url: _url,
        type: "post",
        data: _parsm,
        dataType: jsons,
        async: asyncNp,
        cache: false,
        success: function (_data) {
            _callback(_data);
        }
    })
}
function $AjaxGet(_url, _parsm, _callback, _json, _async) {
    var asyncNp = _async;
    if (asyncNp == null) {
        asyncNp = true;
    }
    var jsons = _json;
    if (jsons == null) {
        jsons = "json";
    }
    $.ajax({
        url: _url,
        type: "get",
        data: _parsm,
        dataType: jsons,
        async: asyncNp,
        cache: false,
        success: function (_data) {
            _callback(_data);
        }
    })
}
