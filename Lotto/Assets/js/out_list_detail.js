$(document)
    .ajaxStart(function () {
        $('#AjaxLoader').show();
    })
    .ajaxStop(function () {
        $('#AjaxLoader').hide();
    });
    $("#print").click(function () {

        var status = $(this).data('status');

        if(status == 0){

            $.ajax({
                url: printOutList,
                data: { Data: model },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    if (data != null) {
                        location.reload();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                }
            });
        }

        var printContent = "";

        var newWin = window.open('', 'Print-Window');

        var period = '<b>หวยรัฐบาล งวดที่ </b>'+$('#todayDate').html()+'<br/>';

        var total = '<b>รวม </b>'+$('#bet-total').html()+' <b>บาท</b><br/>';

        var today = new Date();

        var year    = today.getFullYear();
        var month   = today.getMonth()+1; 
        var day     = today.getDate();
        var hour    = today.getHours();
        var minute  = today.getMinutes();
        var second  = today.getSeconds(); 
        if(month.toString().length == 1) {
             month = '0'+month;
        }
        if(day.toString().length == 1) {
             day = '0'+day;
        }   
        if(hour.toString().length == 1) {
             hour = '0'+hour;
        }
        if(minute.toString().length == 1) {
             minute = '0'+minute;
        }
        if(second.toString().length == 1) {
             second = '0'+second;
        }   
        var date = year +'-'+ month +'-'+ day +' '+ hour + ":" + minute + ":" + second;

        var created = '<b>พิมพ์เมื่อ </b>'+date +'<br/>';

        var from = '<b>ผู้ส่ง </b>'+$('#profile').html()+'<br/><br/>';

        $(".get-bet-number").each(function() { 
            var table = document.getElementById($(this).attr('id'));
            var targetTDs = table.querySelectorAll('tr > td:first-child');
            if(targetTDs.length > 0){
                printContent = printContent+'<p><b>'+$(this).data('name')+'</b></p>';
                printContent = printContent+'<div class="row">';
                for (var i = 0; i < targetTDs.length; i++) {
                    var td = targetTDs[i];
                    printContent = printContent+'<div class="col-md-2">'+td.innerHTML+'</div>';
                }
                printContent = printContent+'</div>';
                printContent = printContent+'<hr/>';
            }
        });

        var context = period+total+created+from+printContent;

        newWin.document.open();

        newWin.document.write('<html><head><link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.1.3/dist/css/bootstrap.min.css"><style>.bet-no{float: left;width: 10%;padding: 2px;padding-top: 10px}.bet-type{float: left;width: 20%;padding: 2px;text-align: center}.bt{float: left;width: 20%;padding: 2px;text-align: center}.bet-lotto{float: left;width: 20%;padding: 2px}.bet-equal{float: left;width: 5%;padding: 2px;padding-top: 10px}.bet-price{float: left;width: 40%;padding: 2px}</style></head><body onload="window.print()">' + context + '</body></html>');

        newWin.document.close();
        
    });
var ipadd;
(function ($) {
    $.getJSON("http://jsonip.com?callback=?", function (data) {
        ipadd = data.ip;
    });
})(jQuery);
$("#SendLotto").click(function () {
    if ($("#select").val() == "0")
    {
        Swal.fire({
            type: 'error',
            title: 'กรุณาเลือกเจ้ามือที่ต้องการส่งโพย',
            text: 'กรุณาเลือกเจ้ามือที่ต้องการที่จะส่งโพย ในกรณีที่ไม่มีเจ้ามือให้เลือก สามารถเพิ่มได้ในหน้า "เจ้ามือแทงออก"',
            showConfirmButton: true,
        });
    }
    else
    {        
        console.log(model);
        $.ajax({
            url: SendLotto,
            data: { Data: model, ID: $("#select").val(), IPAddress: ipadd},
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    console.log(data);
                    if (data =="login fail") {
                        Swal.fire({
                            type: 'error',
                            title: 'Username หรือ Password แทงออกไม่ถูกต้อง',
                            showConfirmButton: true,
                        });
                    }
                    else if (data == "admin not valid") {
                        Swal.fire({
                            type: 'error',
                            title: 'Username เจ้ามือปลายทางไม่ถูกต้อง',
                            showConfirmButton: true,
                        });
                    }
                    else if (data == "close period") {
                        Swal.fire({
                            type: 'error',
                            title: 'เจ้ามือยังไม่เปิดรับแทง',
                            showConfirmButton: true,
                        });
                    }
                    else if (data == "ss") {
                        Swal.fire({
                            type: 'success',
                            title: 'แทงออกเรียบร้อย',
                            showConfirmButton: true,
                            timer: 1500,
                            onAfterClose: () => {
                                window.location.href = "/Admin/Outlist";
                            }
                        });
                    }
                    else {

                    }
                    //Swal.fire({
                    //    type: 'success',
                    //    title: 'บันทึกเรียบร้อย',
                    //    showConfirmButton: false,
                    //    timer: 1500,
                    //    onAfterClose: () => {
                    //        location.reload();
                    //    }
                    //});
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
            }
        });
    }
});