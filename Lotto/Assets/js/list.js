﻿$(".showPoll").click(function () {
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
	var reject_amount = $(this).attr("data-reject");

	$("#poll_number").html(poll_count);
	$("#receive_status").html(receive);
	$("#amount").html(amount);
	$("#receive").html(amount_receive);
	$("#discount").html(discount);
	$("#poll_date").html(create_date);
	$("#poll_by").html(create_by);
	$("#receive_date").html(create_date);
	$("#poll_id").html(poll_id);
	$("#total_poll").html(totalPollcount);
	$("#receive_poll").html(total_receive);
	$("#r_amount").html(reject_amount);
	$("#receive_amount").html(total_amount);
	$("#t_discount").html(total_discount);

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
                        type = "ล";
                        break;
                    case "tb":
                        type = "บ+ล";
                        break;
                    case "f":
                        type = "ห";
                        break;
                    case "ft":
                        type = "ห+ท";
                        break;
                    default:
                        type = "บ";
                }
                if (i == 30 || i == 60 || i == 0)
                {
                    htmlString += '<div class="col-lg-4">';
                }
                htmlString+='<div class="bet-cell">'+
                    '<div class="bet-no">' + (i+1) + '</div>' +
                    '<div class="bet-type"><div  class="form-control form-control-sm input-bet-type" data-type="t">' + type+'</div></div>'+
                    '<div class="bet-lotto"><input type="text" maxlength="3" class="form-control form-control-sm input-color" placeholder="" disabled value="'+data[i].Number+'"></div>'+
                    '<div class="bet-equal">=</div>'+
                    '<div class="bet-price"><input type="text" maxlength="12" class="form-control form-control-sm input-color" placeholder="" disabled value="'+data[i].Amount+'"></div>'+
                '</div>'
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