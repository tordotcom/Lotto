function WinPoll(uid) {
    $.ajax({
        url: getUserPoll2,
        data: { UID: uid},
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
            $('#total_amount3').html(nwc(total_amount2));
            $('#total_recieve3').html(nwc(total_recieve2));
            $('#total_discount3').html(nwc(total_discount2));
            $('#total_win3').html(nwc(total_win2));
            $('#total_sum3').html(nwc(total_discount2 - total_win2));
            initialPoll();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });
}
