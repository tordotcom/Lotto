$(document)
    .ajaxStart(function () {
        $('#AjaxLoader').show();
    })
    .ajaxStop(function () {
        $('#AjaxLoader').hide();
    });
$("#print").click(function () {
    //console.log(model);
    $.ajax({
        url: printOutList,
        data: { Data: model },
        type: "POST",
        dataType: "json",
        success: function (data) {
            if (data != null) {
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