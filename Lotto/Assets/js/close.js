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