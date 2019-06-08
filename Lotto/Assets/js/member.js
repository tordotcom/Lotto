// Confirm Add && Update
$("#btnEdit").click(function () {
	if ($(this).val() == 1) {
		//if ($('#editUserForm').valid()) {
			var id = $(this).attr("data-id");
			var UserData = {
				ID: id,
				Name: $("#txtName").val(),
				Password: $("#txtPassword").val(),
				Status: $("#txtStatusSelect").val(),
				Description: $("#txtDescription").val(),
			};
			$.ajax({
				url: updateUser,
				data: { User: UserData },
				type: "POST",
				dataType: "json",
				success: function (data) {
					if (data == "ss") {
						Swal.fire({
							type: 'success',
							title: 'แก้ไขข้อมูลเรียบร้อย',
							showConfirmButton: false,
							timer: 1500
						});
						$('.modal-backdrop').remove();
						$(".close").trigger('click');
						setTimeout(function () {
							location.reload(true);
						}, 2000);
					}
				},
				error: function (xhr, ajaxOptions, thrownError) {
					alert("error");
				}
			});
		//}
    }

	if ($(this).val() == 0) {
		//alert($("#txtUsername").val());
        var AddUserData = {
			Username: $("#txtUsername").val(),
            Name: $("#txtName").val(),
            Password: $("#txtPassword").val(),
            Status: $("#txtStatusSelect").val(),
            Description: $("#txtDescription").val()
		};

        $.ajax({
            url: addUser,
            data: { User: AddUserData },
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data == "ss") {
                    Swal.fire({
                        type: 'success',
                        title: 'บันทึกเรียบร้อย',
                        showConfirmButton: false,
                        timer: 1500
					});
					$('.modal-backdrop').remove();
					$(".close").trigger('click');
					setTimeout(function () {
						location.reload(true);
					}, 2000);
                }    
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
            }
        });
    }
});

// Confirm Delete
$("#btnDelete").click(function () {
    var id = $(this).attr("data-id");
    var UserData = {
        ID: id
    };

    $.ajax({
        url: deleteUser,
        data: { User: UserData },
        type: "POST",
        dataType: "json",
        success: function (data) {
            if (data == "ss") {
                Swal.fire({
                    type: 'success',
                    title: 'ลบข้อมูลเรียบร้อย',
                    showConfirmButton: false,
                    timer: 1500
				});
				$('.modal-backdrop').remove();
				$(".close").trigger('click');
				setTimeout(function () {
					location.reload(true);
				}, 2000);
            }   
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });
});