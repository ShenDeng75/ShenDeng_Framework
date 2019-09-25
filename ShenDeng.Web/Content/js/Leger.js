// 全选
function choose_all() {
    if ($("#ch").is(":checked"))
        $("input[type='checkbox']").prop("checked", true);
    else
        $("input[type='checkbox']").prop("checked", false);
}
toRed();
// 变色
function toRed() {
    var trs = $("tbody").children("tr[id]");
    $.each(trs, function (index, item) {
        var time = trs.eq(index).children("td").eq(15).text(); // 如果超期
        if (time == "超期")
            trs.eq(index).attr("class", "toRed");
    });
}
// 查找
function find() {
    var inputs = $("table[class='my-table'] thead tr td[class!='firsttd'] input");
    var datas = {};
    $.each(inputs, function (index, item) {
        var name = inputs.eq(index).attr("name");
        var value = inputs.eq(index).val();
        datas[name] = value;
    });
    var params = Object.keys(datas).map(k => `${k}=${datas[k]}`).join('&');
    var url = "/Ledger/Find?" + params;
    window.location.href = url;
}
// 导出
function Output() {
    var trs = $("tbody tr[id] td input[type='checkbox']");
    var datas = [];
    var ok = true;
    $.each(trs, function (index, item) {
        var inp = trs.eq(index);
        var ischeck = inp.is(':checked');
        if (ischeck) {  // 如果被选中了
            var no = inp.val();
            datas.push(no);
        }
    });
    if (ok)
        $.ajax({
            url: "/Ledger/Output2Excel",
            type: "post",
            data: JSON.stringify(datas),
            dataType: "json",
            contentType: "application/json",
            success: function (result) {
                if (result.Result == "成功") {
                    window.location.href = result.filepath;
                }
                else {
                    $('.alert').removeClass("alert-success")
                    $('.alert').html(result.Result).addClass('alert-danger').show().delay(3000).fadeOut();
                }
            }
        })
}