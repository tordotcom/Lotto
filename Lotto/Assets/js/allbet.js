function UserPoll(number,Htype,type,nLen) {
    $("#headernumber").html(number);
    $("#headertype").html(Htype);

    $.ajax({
        url: getUserPoll2,
        data: { Number: number, Type: type, NumLen: nLen, },
        type: "POST",
        dataType: "json",
        success: function (data) {
            //console.log(data);
            var total_amount = 0;
            pollTable.clear();
            $.each(data, function (index, value) {
                total_amount += parseInt(value.Amount);
                value.Username = value.Username + " - " + value.Name;
                pollTable.row.add(value);
            });
            pollTable.draw();
            $('#total_amount').html(nwc(total_amount));
            initialPoll2();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });
}
function initialPoll2() {
    $(".showPoll2").click(function () {
        var uid = $(this).attr("data-uid");
        var number = $(this).attr("data-number");
        var type = $(this).attr("data-type");
        var nlen = $(this).attr("data-nlen");
        $.ajax({
            url: getUserPollDetail,
            data: { UID: uid, Number: number, Type: type, NumLen: nlen },
            type: "POST",
            dataType: "json",
            success: function (data) {
                //console.log(data);
                var total_amount2 = 0;
                var total_recieve2 = 0;
                var total_discount2 = 0;
                var total_win2 = 0;
                pollDetailTable.clear();
                $.each(data, function (index, value) {
                    total_amount2 += parseInt(value.Amount);
                    total_discount2 += parseInt(value.Discount);
                    total_win2 += parseInt(value.Win);
                    if (value.Receive == 1) {
                        value.Receive = "รับแล้ว";
                        total_recieve2 += parseInt(value.Amount);
                    }
                    else {
                        value.Receive = "คืนทั้งใบ";
                    }
                    pollDetailTable.row.add(value);
                });
                pollDetailTable.draw();
                $('#total_amount2').html(nwc(total_amount2));
                $('#total_recieve2').html(nwc(total_recieve2));
                $('#total_discount2').html(nwc(total_discount2));
                $('#total_win2').html(nwc(total_win2));
                $('#total_sum2').html(nwc(total_discount2 - total_win2));
                initialPoll();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
            }
        });
    });
}