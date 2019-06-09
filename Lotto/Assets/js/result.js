// Confirm Add && Update
$("#btnEdit").click(function () {
	if ($(this).val() == 1) {
		var ResultData1 = {
			ID: id,
			Name: $("#lotto_name").val(), 
			first_three: $("#first_three").val(), 
			last_three: $("#last_three").val(), 
			three_down_1: $("#three_down_1").val(), 
			three_down_2: $("#three_down_2").val(), 
			three_down_3: $("#three_down_3").val(), 
			three_down_4: $("#three_down_4").val(), 
			two_down: $("#two_down").val()
		};

		$.ajax({
			url: updateResult,
			data: { Result: ResultData1 },
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

	if ($(this).val() == 0) {
		var ResultData2 = {
			Name: $("#lotto_name").val(), 
			Lotto_day: $("#insert_datepicker").val(), 
			first_three: $("#first_three").val(), 
			last_three: $("#last_three").val(), 
			three_down_1: $("#three_down_1").val(), 
			three_down_2: $("#three_down_2").val(), 
			three_down_3: $("#three_down_3").val(), 
			three_down_4: $("#three_down_4").val(), 
			two_down: $("#two_down").val()
		};

		$.ajax({
			url: addResult,
			data: { Result: ResultData2 },
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

//Confirm Delete
$("#btnDelete").click(function () {
	var id = $(this).attr("data-id");
	var ResultData3 = {
		ID: id
	};

	$.ajax({
		url: deleteResult,
		data: { Lotto: ResultData3 },
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