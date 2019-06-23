$("#start_datepicker").datepicker(
    { dateFormat: 'yy-mm-dd' }
).datepicker("setDate", new Date());

$("#openLotto").click(function () {
    console.log($("#start_datepicker").val());
    $.ajax({
        url: addPeroid,
        data: { Date: $("#start_datepicker").val() },
        type: "POST",
        dataType: "json",
        success: function (data) {
            if (data == "ss") {
                Swal.fire({
                    type: 'success',
                    title: 'เปิดหวยรอบใหม่แล้ว',
                    showConfirmButton: false,
                    timer: 1500,
                    onAfterClose: () => {
                        location.reload();
                    }
                });
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });
});

$("#user_oc").click(function () {
    var status = $(this).attr("data-status");
    var pid = $(this).attr("data-id");
    var title = "";
    var text = "";
    var sTitle = "";
    if (status == "close") {
        title = "ต้องการปิดรับพนักงานแทง?";
        text = "กรุณายืนยันการปิดรับพนักงานแทง";
        sTitle = "ปิดรับพนักงานแทงเรียบร้อย";
    }
    else {
        title = "ต้องการเปิดรับพนักงานแทง?";
        text = "กรุณายืนยันการเปิดรับพนักงานแทง";
        sTitle = "เปิดรับพนักงานแทงเรียบร้อย";
    }
    Swal.fire({
        title: title,
        text: text,
        type: 'info',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'ตกลง',
        cancelButtonText: 'ยกเลิก'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: betStatus,
                data: { Status: status, PID: pid },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    if (data == "ss") {
                        Swal.fire({
                            type: 'success',
                            title: sTitle,
                            showConfirmButton: false,
                            timer: 1500,
                            onAfterClose: () => {
                                location.reload();
                            }
                        });
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                }
            });
        }
    })    
});

$("#closeLotto").click(function () {
    var pid = $(this).attr("data-id");
    Swal.fire({
        title: "ต้องการปิดหวยงวดนี้หรือไม่?",
        text: "เมื่อปิดแล้วจะไม่สามารถเพิ่มโพยในงวดนี้ได้อีก",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'ตกลง',
        cancelButtonText: 'ยกเลิก'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: closeLotto,
                data: { PID: pid },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    if (data == "ss") {
                        Swal.fire({
                            type: 'success',
                            title: "ปิดหวยเรียบร้อย",
                            showConfirmButton: false,
                            timer: 1500,
                            onAfterClose: () => {
                                location.reload();
                            }
                        });
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                }
            });
        }
    })
});