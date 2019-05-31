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
            $("#ThreeOod").val(data[0].ThreeOod);
            $("#TwoUp").val(data[0].TwoUp);
            $("#TwoOod").val(data[0].TwoOod);
            $("#TwoDown").val(data[0].TwoDown);
            $("#Up").val(data[0].Up);
            $("#Down").val(data[0].Down);
            $("#ThreeUp_discount").val(data[0].ThreeUP_discount);
            $("#ThreeDown_discount").val(data[0].ThreeDown_discount);
            $("#FirstThree_discount").val(data[0].FirstThree_discount);
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