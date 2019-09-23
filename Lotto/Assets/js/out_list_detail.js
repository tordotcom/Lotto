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

$("#SendLotto").click(function () {
    if ($("#select").val() == "0") {
        Swal.fire({
            type: 'error',
            title: 'กรุณาเลือกเจ้ามือที่ต้องการส่งโพย',
            text: 'กรุณาเลือกเจ้ามือที่ต้องการที่จะส่งโพย ในกรณีที่ไม่มีเจ้ามือให้เลือก สามารถเพิ่มได้ในหน้า "เจ้ามือแทงออก"',
            showConfirmButton: true,
        });
    }
    else {
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
    }
});