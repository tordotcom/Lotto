﻿function initialPoll(){
    $(".showPoll").click(function () {
        //alert(JSON.stringify($(this)));
        var create_by = $(this).attr("data-cBy");
        var create_date = $(this).attr("data-cDate");
        var discount = $(this).attr("data-discount");
        var amount_receive = $(this).attr("data-areceiv");
        var amount = $(this).attr("data-amount");
        var receive = $(this).attr("data-receive");
        var poll_number = $(this).attr("data-pollNumber");
        var poll_name = $(this).attr("data-pollName");
        var poll_id = $(this).attr("data-id");
		var uid = $(this).attr("data-UID");

		document.getElementById('sendPoll').setAttribute('data-uid', uid);
		document.getElementById('sendPoll').setAttribute('data-pid', poll_id);

        $("#poll_number").html(poll_number);
        if(editable == true){
            $("#poll_name").val(poll_name);
        }
        else{
            $("#poll_name").html(poll_name);
        }
        $("#receive_status").html(receive);
        $("#amount").html(nwc(amount));
        $("#receive").html(nwc(amount_receive));
        $("#discount").html(nwc(discount));
        $("#poll_date").html(create_date);
        $("#poll_by").html(create_by);
        $("#receive_date").html(create_date);
        $("#poll_id").html(poll_id);
        document.getElementById('reject').setAttribute('data-pid', poll_id);
        $.ajax({
            url: getPollHeadDetail,
            data: { UID: uid},
            type: "POST",
            dataType: "json",
            success: function (data) {
                var pollCount = 0;
                var total_amount = 0;
                var total_receive = 0;
                var reject_amount = 0;
                var total_discount = 0;
                for (var i = 0; i < Object.keys(data).length; i++) {
                    pollCount += 1;
                    if (data[i].Receive == "1") {
                        total_amount += parseInt(data[i].Amount);
                        total_receive += parseInt(data[i].Amount);
                        total_discount += parseInt(data[i].Discount);
                    }
                    else {
                        total_amount += parseInt(data[i].Amount);
                        reject_amount += parseInt(data[i].Amount);
                    }
                }
                $("#total_poll").html(pollCount);
                $("#receive_poll").html(nwc(total_receive));
                $("#receive_amount").html(nwc(total_amount));
                $("#t_discount").html(nwc(total_discount));
                $("#r_amount").html(nwc(reject_amount));
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
            }
        });

    $.ajax({
        url: getPoll,
        data: { id: poll_id },
        type: "POST",
        dataType: "json",
        success: function (data) {
            var htmlString = "";
			var type;
			rowPoll = Object.keys(data).length;
            for (var i = 0; i < Object.keys(data).length; i++) {
                var type_id = "t" + (i+1);
                var lotto_id = "n" + (i+1);
                var price_id = "b" + (i+1);
                switch (data[i].Type) {
                    case "t":
                        type = "บ";
                        break;
                    case "b":
                        type = '<font color="red">ล</font>';
                        break;
                    case "tb":
                        type = 'บ+<font color="red">ล</font>';
                        break;
                    case "f":
                        type = '<font color="blue">ห</font>';
                        break;
                    case "ft":
                        type = '<font color="blue">ห</font>+<font color="green">ท</font>';
                        break;
                    default:
                        type = "บ";
                }
                if (i == 25 || i == 50 || i == 0) {
                    htmlString += '<div class="col-md-4">';
                }
                if (parseInt(data[i].Result_Status) > 0) {
                    htmlString += '<div class="row bet-cell">' +
                        '<div class="col-xs-1 bet-no">' + (i + 1) + '&nbsp;</div>' +
                        '<div class="col-xs-3 bet-type"><div style="background-color:lightgreen" id="' + type_id + '" class="form-control form-control-sm input-bet-type" data-type="' + data[i].Type+'">' + type + '</div></div>' +
                        '<div class="col-xs-3 bet-lotto"><input style="background-color:lightgreen" id="' + lotto_id + '" type="text" maxlength="4" class="form-control form-control-sm input-lotto" placeholder="" disabled onchange="checkNumber(' + (i + 1) + ')" onkeypress="return keyPress2(this,' + (i + 1)+' , event);" value="' + data[i].Number + '"></div>' +
                        '<div class="col-xs-1 bet-equal">&nbsp;=&nbsp;</div>' +
                        '<div class="col-xs-4 bet-price"><input style="background:lightgreen" id="' + price_id + '" type="text" maxlength="12" class="form-control form-control-sm input-price" placeholder="" onchange="checkAmount(' + (i + 1) + ')" onkeypress="return keyPress(this, ' + (i + 1) +', event)" disabled value="' + data[i].Amount + '"></div>' +
                        '</div>';
                }
                else {
                    htmlString += '<div class="row bet-cell">' +
                        '<div class="col-xs-1 bet-no">' + (i + 1) + '&nbsp;</div>' +
                        '<div class="col-xs-3 bet-type"><div id="' + type_id + '" class="form-control form-control-sm input-bet-type" data-type="' + data[i].Type+'">' + type + '</div></div>' +
                        '<div class="col-xs-3 bet-lotto"><input id="' + lotto_id + '" type="text" maxlength="4" class="form-control form-control-sm input-lotto" placeholder="" disabled onchange="checkNumber(' + (i + 1) + ')" onkeypress="return keyPress2(this,' + (i + 1)+' , event);" value="' + data[i].Number + '"></div>' +
                        '<div class="col-xs-1 bet-equal">&nbsp;=&nbsp;</div>' +
                        '<div class="col-xs-4 bet-price"><input id="' + price_id + '" type="text" maxlength="12" class="form-control form-control-sm input-price" placeholder="" onchange="checkAmount(' + (i + 1) + ')" onkeypress="return keyPress(this, ' + (i + 1) +', event)" disabled value="' + data[i].Amount + '"></div>' +
                        '</div>';
                }
                if (i == 24 || i == 49 || i == 74) {
					htmlString += '</div>';
                }
            }
			$("#poll").html(htmlString);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });
});

    $("#reject").click(function () {
        var pid = $(this).attr("data-pid");
		Swal.fire({
			title: 'ต้องการคืนเลข?',
			text: "กรุณายืนยันการคืนเลข",
			type: 'warning',
			showCancelButton: true,
			confirmButtonColor: '#3085d6',
			cancelButtonColor: '#d33',
			confirmButtonText: 'คืนเลข',
			cancelButtonText: 'ยกเลิก'
		}).then((result) => {
			if (result.value) {
				$.ajax({
					url: RejectPoll,
					data: { PollID: pid },
					type: "POST",
					dataType: "json",
					success: function (data) {
						Swal.fire({
							type: 'success',
							title: 'คืนเรียบร้อย',
							showConfirmButton: false,
							timer: 1500,
							onAfterClose: () => {
								location.reload();
							}
						});
					},
					error: function (xhr, ajaxOptions, thrownError) {
						alert("error");
					}
				});
			}
		});
    });
}