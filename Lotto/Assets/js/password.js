$("#confirmChange").click(function () {
    if ($("#user-password").val() == "" || $("#user-new-password").val() == "" || $("#user-comfirm-new-password").val() == "") {
        Swal.fire('กรุณากรอกข้อมูลให้ครบถ้วน');
    }
    else if ($("#user-new-password").val() != $("#user-comfirm-new-password").val()) {
        Swal.fire('กรุณากรอกรหัสผ่านใหม่ให้ถูกต้อง');
    }
    else {
        $.ajax({
            url: updatePassword,
            data: { OldPass: $("#user-password").val(), NewPass: $("#user-comfirm-new-password").val() },
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data == "ss") {
                    Swal.fire({
                        type: 'success',
                        title: 'เปลี่ยนรหัสผ่านเรียบร้อย',
                        showConfirmButton: false,
                        timer: 1500,
                        onClose: () => {
                            location.reload();
                        }
                    });
                }
                else {
                    Swal.fire('กรุณากรอกรหัสผ่านเก่าให้ถูกต้อง')
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
            }
        });
    }
});