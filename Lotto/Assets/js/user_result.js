$(document).ready(function () {
    $("#datepicker").datepicker({
        isRTL: false,
        language: 'th',
        format: 'yyyy-mm-dd',
        orientation: "bottom"
    });
});
$("#getResult").click(function () {
    console.log($('#datepicker').datepicker('getFormattedDate'));
    $.ajax({
        url: getResult,
        data: { date: $('#datepicker').datepicker('getFormattedDate') },
        type: "POST",
        dataType: "json",
        success: function (data) {
            console.log(data);
            $("#t_result").html(
                '<tr><td>' + data.Lotto_day + '</td><td>หวยรัฐบาล</td><td>' + data.last_three + '</td><td>' + data.three_down_1 + ' &nbsp;' + data.three_down_2 + ' &nbsp;' + data.three_down_3 + ' &nbsp;' + data.three_down_4 + '</td><td>' + data.first_three+'</td><td>'+data.two_down+'</td></tr>'
            );
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert("ไม่พบผลสลากวันที่เลือก");
            Swal.fire({
                type: 'error',
                title: 'ไม่พบผลสลาก',
            });
            $("#t_result").html('');
        }
    });
});
//$('#datepicker').on('changeDate', function () {
//    console.log($('#datepicker').datepicker('getFormattedDate'));
//});