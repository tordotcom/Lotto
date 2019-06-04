$("#btnEdit").click(function () {
    var id = $(this).attr("data-id");
    var rate_id = $(this).attr("data-rateID");
    var discount_id = $(this).attr("data-discountID");
    console.log(id);
    console.log(rate_id);
    console.log(discount_id);
    var RateDiscount = {
        ID: id,
        rateID: rate_id,
        discountID: discount_id,
        ThreeUp: $("#ThreeUp_edit").val(),
        ThreeDown: $("#ThreeDown_edit").val(),
        FirstThree: $("#FirstThree_edit").val(),
        ThreeOod: $("#ThreeOod_edit").val(),
        TwoUp: $("#TwoUp_edit").val(),
        TwoOod: $("#TwoOod_edit").val(),
        TwoDown: $("#TwoDown_edit").val(),
        Up: $("#Up_edit").val(),
        Down: $("#Down_edit").val(),
        ThreeUp_discount: $("#ThreeUp_discount_edit").val(),
        ThreeDown_discount: $("#ThreeDown_discount_edit").val(),
        FirstThree_discount: $("#FirstThree_discount_edit").val(),
        ThreeOod_discount: $("#ThreeOod_discount_edit").val(),
        TwoUp_discount: $("#TwoUp_discount_edit").val(),
        TwoOod_discount: $("#TwoOod_discount_edit").val(),
        TwoDown_discount: $("#TwoDown_discount_edit").val(),
        Up_discount: $("#Up_discount_edit").val(),
        Down_discount: $("#Down_discount_edit").val()
    };

    $.ajax({
        url: UpdateUserRateDiscount,
        data: { User_Rate_Discount: RateDiscount },
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
            }
            else {
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