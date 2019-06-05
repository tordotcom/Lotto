$(".bet-type").click(function () {
    $(this).find(".input-bet-type").fadeOut(function () {
        $(this).css('cursor', 'default').css('text-decoration', 'none');

        var text = $(this).text();
        if (text == 'บ') {
            $(this).text('ล').fadeIn();
        }
        if (text == 'ล') {
            $(this).text('บ+ล').fadeIn();
        }
        if (text == 'บ+ล') {
            $(this).text('ห').fadeIn();
        }
        if (text == 'ห') {
            $(this).text('ห+ท').fadeIn();
        }
        if (text == 'ห+ท') {
            $(this).text('บ').fadeIn();
        }
    })
});

$("#sendLotto").click(function () {
    var bet = new Array(3);
    for (var i = 1; i <= 90; i++) {
        var n = $('#n' + i).val();
        var b = $('#b' + i).val();
        if (n != "" && b == "") {
            Swal.fire({
                type: 'error',
                title: 'ช่องที่ ' + i + ' กรอกข้อมูลไม่ถูกต้อง',
                showConfirmButton: true
            });
            break;
        }
        else if (b != "" && n == "") {
            Swal.fire({
                type: 'error',
                title: 'ช่องที่ ' + i + ' กรอกข้อมูลไม่ถูกต้อง',
                showConfirmButton: true
            });
            break;
        }
        else { }
        bet[i] = new Array(3)
        bet[i]["t" + i] = $('#t' + i).text();
        bet[i]["n" + i] = n;
        bet[i]["b" + i] = b;
    }
    console.log(bet);
    //alert(JSON.stringify(bet));
});