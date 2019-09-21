// Confirm Add && Update
$("#btnEdit").click(function () {
    if ($(this).val() == 1) {
        var id = $(this).attr("data-id");
        var CoDealerData = {
            ID: id,
            Name: $("#txtDealer").val(),
            SendToUsername:$("#txtUsernameDealer").val(),
            Username: $("#txtBetUsername").val(),
            Password: $("#txtBetPassword").val()
        };

        if ($("#txtDealer").val() == "" || $("#txtUsernameDealer").val() == "" || $("#txtBetUsername").val() == "" || $("#txtBetPassword").val() == "") {
            Swal.fire({
                type: 'error',
                title: 'กรุณากรอกข้อมูลให้ครบ',
                showConfirmButton: false,
                timer: 1500
            });
        }
        else {
            $.ajax({
                url: updateCoDealer,
                data: { CoDealer: CoDealerData },
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
        }
		//}
    }

	if ($(this).val() == 0) {
        var CoDealerData = {
            UID: adminID,
            Name: $("#txtDealer").val(),
            SendToUsername:$("#txtUsernameDealer").val(),
            Username: $("#txtBetUsername").val(),
            Password: $("#txtBetPassword").val()
		};

        if ($("#txtDealer").val() == "" || $("#txtUsernameDealer").val() == "" || $("#txtBetUsername").val() == "" || $("#txtBetPassword").val() == "") {
            Swal.fire({
                type: 'warning',
                title: 'กรุณากรอกข้อมูลให้ครบ',
                showConfirmButton: true
            });
        }
        else {
            $.ajax({
                url: addCoDealer,
                data: { CoDealer: CoDealerData },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    console.log(data);
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
                    else { }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                }
            });
        }
        
    }
});

// Confirm Delete
$("#btnDelete").click(function () {
    var id = $(this).attr("data-id");
    var CoDealerData = {
        ID: id
    };

    $.ajax({
        url: deleteCoDealer,
        data: { CoDealer: CoDealerData },
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