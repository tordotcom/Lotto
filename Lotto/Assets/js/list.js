$(".showPoll").click(function () {
    var create_by = $(this).attr("data-cBy");
    var create_date = $(this).attr("data-cDate");
    var discount = $(this).attr("data-discount");
    var amount_receive = $(this).attr("data-areceiv");
    var amount = $(this).attr("data-amount");
    var receive = $(this).attr("data-receive");
    var poll_count = $(this).attr("data-pCount");
    var poll_id = $(this).attr("data-id");
    var totalPollcount = $("#totalPollcount").val();
    var total_amount = $("#total_amount").val();
    var total_receive = $("#total_receive").val();
    var total_discount = $("#total_discount").val();
    var reject_amount = $("#reject").val();

    //console.log(poll_count);
    //console.log(receive);
	$("#poll_number").html(poll_count);
	$("#receive_status").html(receive);
	$("#amount").html(nwc(amount));
	$("#receive").html(nwc(amount_receive));
	$("#discount").html(nwc(discount));
	$("#poll_date").html(create_date);
	$("#poll_by").html(create_by);
	$("#receive_date").html(create_date);
	$("#poll_id").html(poll_id);
	$("#total_poll").html(totalPollcount);
	$("#receive_poll").html(nwc(total_receive));
	$("#r_amount").html(nwc(reject_amount));
	$("#receive_amount").html(nwc(total_amount));
	$("#t_discount").html(nwc(total_discount));
    $("#poll_name").prop( "disabled", true );

    $.ajax({
        url: getPoll,
        data: { id: poll_id },
        type: "POST",
        dataType: "json",
        success: function (data) {
            var htmlString = "";
            var type;
            //console.log(data.Length);
            for (var i = 0; i < Object.keys(data).length; i++) {
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
                if (i == 30 || i == 60 || i == 0)
                {
                    htmlString += '<div class="col-md-4">';
                }
                if (parseInt(data[i].Result_Status) > 0) {
                    htmlString += '<div class="row bet-cell">' +
                        '<div class="col-xs-1 bet-no">' + (i + 1) + '&nbsp;</div>' +
                        '<div class="col-xs-3 bet-type"><div style="background-color:lightgreen" class="form-control form-control-sm input-bet-type" data-type="t">' + type + '</div></div>' +
                        '<div class="col-xs-3 bet-lotto"><input style="background-color:lightgreen" type="text" maxlength="4" class="form-control form-control-sm input-color" placeholder="" disabled value="' + data[i].Number + '"></div>' +
                        '<div class="col-xs-1 bet-equal">&nbsp;=&nbsp;</div>' +
                        '<div class="col-xs-4 bet-price"><input style="background-color:lightgreen" type="text" maxlength="12" class="form-control form-control-sm input-color" placeholder="" disabled value="' + data[i].Amount + '"></div>' +
                        '</div>';
                }
                else {
                    htmlString += '<div class="row bet-cell">' +
                        '<div class="col-xs-1 bet-no">' + (i + 1) + '&nbsp;</div>' +
                        '<div class="col-xs-3 bet-type"><div  class="form-control form-control-sm input-bet-type" data-type="t">' + type + '</div></div>' +
                        '<div class="col-xs-3 bet-lotto"><input type="text" maxlength="4" class="form-control form-control-sm input-color" placeholder="" disabled value="' + data[i].Number + '"></div>' +
                        '<div class="col-xs-1 bet-equal">&nbsp;=&nbsp;</div>' +
                        '<div class="col-xs-4 bet-price"><input type="text" maxlength="12" class="form-control form-control-sm input-color" placeholder="" disabled value="' + data[i].Amount + '"></div>' +
                        '</div>';
                }
                if (i == 29 || i == 59 || i == 89)
                {
                    htmlString +='</div>'
                }
            }
            $("#poll").html(htmlString);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });
});