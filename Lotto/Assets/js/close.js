$(document)
    .ajaxStart(function () {
        $('#AjaxLoader').show();
    })
    .ajaxStop(function () {
        $('#AjaxLoader').hide();
    });
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

$("#user_oc").click(function () {
    var status = $(this).attr("data-status");
    var pid = $(this).attr("data-id");
    var title = "";
    var text = "";
    var sTitle = "";
    if (status == "close") {
        title = "ต้องการปิดรับพนักงานแทง?";
        text = "กรุณายืนยันการปิดรับพนักงานแทง";
        sTitle = "ปิดรับพนักงานแทงเรียบร้อย";
    }
    else {
        title = "ต้องการเปิดรับพนักงานแทง?";
        text = "กรุณายืนยันการเปิดรับพนักงานแทง";
        sTitle = "เปิดรับพนักงานแทงเรียบร้อย";
    }
    Swal.fire({
        title: title,
        text: text,
        type: 'info',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'ตกลง',
        cancelButtonText: 'ยกเลิก'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: betStatus,
                data: { Status: status, PID: pid },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    if (data == "ss") {
                        Swal.fire({
                            type: 'success',
                            title: sTitle,
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
        }
    })    
});

$("#closeLotto").click(function () {
    var pid = $(this).attr("data-id");
    Swal.fire({
        title: "ต้องการปิดหวยงวดนี้หรือไม่?",
        text: "เมื่อปิดแล้วจะไม่สามารถเพิ่มโพยในงวดนี้ได้อีก",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'ตกลง',
        cancelButtonText: 'ยกเลิก'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: closeLotto,
                data: { PID: pid },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    if (data == "ss") {
                        Swal.fire({
                            type: 'success',
                            title: "ปิดหวยเรียบร้อย",
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
        }
    })
});
function keyPress(obj, evt) {

    evt = evt || window.event;
    var code = evt.keyCode || evt.which;
    // enter = 13, tab = 9
    // space = 32,  + = 43

    if (code >= 48 && code <= 57) {
        return true;  // Number 0-9
    }
    else if (code == 42 || code == 8 || code == 46 || code == 9 || code == 88 || code == 120) return true; // 42 = *, 8 = back, 46 = del, 9 = tab, 47 = slash, 88 = x, 120 = X
    else if (code >= 37 && code <= 40) return true;  // left, up, right, down
    else return false;
}
$("#check_result").click(function () {
    var pid = $(this).attr("data-id");
    var day = $(this).attr("data-day");
    //console.log(day);
    var first_three = $("#ft").val();
    var three_up = $("#tt").val();
    var two_down = $("#twob").val();
    var three_down1 = $("#tb1").val();
    var three_down2 = $("#tb2").val();
    var three_down3 = $("#tb3").val();
    var three_down4 = $("#tb4").val();
    if (first_three != "" && three_up != "" && two_down != "" && three_down1 != "" && three_down2 != "" && three_down3 != "" && three_down4 != "") {
        if (first_three.length == 3 && three_up.length == 3 && two_down.length == 2 && three_down1.length == 3 && three_down2.length == 3 && three_down3.length == 3 && three_down4.length == 3) {
            var ft = first_three.split('');
            var ft_ood_1 = ft[0] + ft[2] + ft[1];
            var ft_ood_2 = ft[1] + ft[0] + ft[2];
            var ft_ood_3 = ft[1] + ft[2] + ft[0];
            var ft_ood_4 = ft[2] + ft[0] + ft[1];
            var ft_ood_5 = ft[2] + ft[1] + ft[0];
            
            var tu = three_up.split('');
            var tu_ood_1 = tu[0] + tu[2] + tu[1];
            var tu_ood_2 = tu[1] + tu[0] + tu[2];
            var tu_ood_3 = tu[1] + tu[2] + tu[0];
            var tu_ood_4 = tu[2] + tu[0] + tu[1];
            var tu_ood_5 = tu[2] + tu[1] + tu[0];

            var two_up = tu[1] + tu[2];
            var two_up_ood = tu[2] + tu[1];

            var up = tu[2];

            var td = two_down.split('');
            var td_ood_1 = td[1] + td[0];

            var down = td[1];
            $.ajax({
                url: CheckResult,
                data: { PID: pid, DAY: day, FT: first_three, FTO1: ft_ood_1, FTO2: ft_ood_2, FTO3: ft_ood_3, FTO4: ft_ood_4, FTO5: ft_ood_5, TU: three_up, TUO1: tu_ood_1, TUO2: tu_ood_2, TUO3: tu_ood_3, TUO4: tu_ood_4, TUO5: tu_ood_5, TD: two_down, TDO1: td_ood_1, ThDown1: three_down1, ThDown2: three_down2, ThDown3: three_down3, ThDown4: three_down4, TwUP: two_up, Tw_up_ood: two_up_ood, UP: up, DOWN: down },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    console.log(data);
                    if (data == "ss") {
                        Swal.fire({
                            type: 'success',
                            title: "ตรวจเลขเรียบร้อย",
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
        }
        else {
            Swal.fire('กรุณากรอกผลหวยให้ถูกต้อง');
        }
    }
    else {
        Swal.fire('กรุณากรอกผลหวยให้ครบถ้วน');
    }
});