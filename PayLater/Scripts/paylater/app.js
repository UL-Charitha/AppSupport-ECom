function findChariPost() {
    var pnr = $('#pnrId').val();
    var pnrcur = $('#pnrcur').val();
    var bankcode = $('#bankcode').val();
    var uri2 = 'api/pnr/1/1/1';
    // here to input headers
    $.ajaxSetup({
        headers: {
            //'Authorization': 'Basic faskd52352rwfsdfs',
            'Token': 'Test01_Token_0123456789ABCDXXX'
        }
    });
    $.post(uri2, { pnrID: pnr, currency: pnrcur, bankId: bankcode }, function (data, textStatus) {
        $('#chariPnr').text(formatItem3(data));
    }, "json");

    //$.post(uri2, { pnr: pnr, currency: pnrcur, bankId: bankcode })
    //    .done(function (data) {
    //        $('#chariPnr').text(formatItem3(data));
    //    })
    //    .fail(function (jqXHR, textStatus, err) {
    //        $('#product').text(formatItem(data));
    //    });
}









function test01() {
    $.ajax({
        url: 'service.svc/Request',
        type: 'GET',
        dataType: 'json',
        success: function () { alert('hello!'); },
        error: function () { alert('boo!'); },
        beforeSend: setHeader
    });
}
function setHeader(xhr) {
    xhr.setRequestHeader('securityCode', 'Foo');
    xhr.setRequestHeader('passkey', 'Bar');
}