$("#btnEdit").click(function () {
    var id = $(this).attr("data-id");
    var UserData = {
        ID: id,
        Name: $("#txtName").val(),
        Password: $("#txtPassword").val(),
        Status: $("#select").val(),
        Description: $("#txtDescription").val(),
    };
    $.ajax({
        url: updateUser,
        data: { User: UserData },
        type: "POST",
        dataType: "json",
        success: function (data) {
            if (data=="ss") {
                Swal.fire({
                    type: 'success',
                    title: 'บันทึกเรียบร้อย',
                    showConfirmButton: false,
                    timer: 1500
                });
            }    
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });
});