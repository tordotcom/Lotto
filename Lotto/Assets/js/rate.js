//------------------------get rate and discount------------------------------------------------//
$("#btnRate").click(function () {
    $.ajax({
        url: getRate,
        type: "POST",
        dataType: "json",
        success: function (data) {
            console.log(data);
            $("#ThreeUp").val(data[0].ThreeUP);
            $("#ThreeDown").val(data[0].ThreeDown);
            $("#FirstThree").val(data[0].FirstThree);
            $("#FirstThreeOod").val(data[0].FirstThreeOod);
            $("#ThreeOod").val(data[0].ThreeOod);
            $("#TwoUp").val(data[0].TwoUp);
            $("#TwoOod").val(data[0].TwoOod);
            $("#TwoDown").val(data[0].TwoDown);
            $("#Up").val(data[0].Up);
            $("#Down").val(data[0].Down);
            $("#ThreeUp_discount").val(data[0].ThreeUP_discount);
            $("#ThreeDown_discount").val(data[0].ThreeDown_discount);
            $("#FirstThree_discount").val(data[0].FirstThree_discount);
            $("#FirstThreeOod_discount").val(data[0].FirstThreeOod_discount);
            $("#ThreeOod_discount").val(data[0].ThreeOod_discount);
            $("#TwoUp_discount").val(data[0].TwoUp_discount);
            $("#TwoOod_discount").val(data[0].TwoOod_discount);
            $("#TwoDown_discount").val(data[0].TwoDown_discount);
            $("#Up_discount").val(data[0].Up_discount);
            $("#Down_discount").val(data[0].Down_discount);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });
});
//------------------------------------------------------------------------------------------//
//----------------------------------update rate and discount--------------------------------//
$("#btnUpdateRate").click(function () {
    var RateDiscount = {
        ThreeUp: $("#ThreeUp").val(),
        ThreeDown: $("#ThreeDown").val(),
        FirstThree: $("#FirstThree").val(),
        FirstThreeOod: $("#FirstThreeOod").val(),
        ThreeOod: $("#ThreeOod").val(),
        TwoUp: $("#TwoUp").val(),
        TwoOod: $("#TwoOod").val(),
        TwoDown: $("#TwoDown").val(),
        Up: $("#Up").val(),
        Down: $("#Down").val(),
        ThreeUp_discount: $("#ThreeUp_discount").val(),
        ThreeDown_discount: $("#ThreeDown_discount").val(),
        FirstThree_discount: $("#FirstThree_discount").val(),
        FirstThreeOod_discount: $("#FirstThreeOod_discount").val(),
        ThreeOod_discount: $("#ThreeOod_discount").val(),
        TwoUp_discount: $("#TwoUp_discount").val(),
        TwoOod_discount: $("#TwoOod_discount").val(),
        TwoDown_discount: $("#TwoDown_discount").val(),
        Up_discount: $("#Up_discount").val(),
        Down_discount: $("#Down_discount").val()
    };
    $.ajax({
        url: UpdateRateDiscount,
        data: { RateDiscountArr: RateDiscount },
        type: "POST",
        dataType: "json",
        success: function (data) {
            console.log(data);
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
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });
});