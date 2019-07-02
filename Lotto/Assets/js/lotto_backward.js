$(".lottoDetail").click(function () {
    var pid = $(this).attr("data-pid");
    $.ajax({
        url: getDetail,
        data: { PID: pid },
        type: "POST",
        dataType: "json",
        success: function (data) {
            //console.log(data);
            //var date = new Date(parseInt(data.all[0]..substr(6)));
            $("#total").html(data.all[0].Amount);
            $("#receive").html(data.all[0].CountReceive);
            $("#countuser").html(data.all[0].CountUser);
            $("#betstatus").html(data.all[0].BetStatus);
            $("#open").html(data.all[0].CreateDate);
            $("#close").html(data.all[0].CloseDate);
            $("#closeby").html(data.all[0].CloseBy);
            var tdis = 0;
            var twin = 0;
            for (var i = 0; i < Object.keys(data.TAR).length; i++) {
                tdis = tdis + parseInt(data.TAR[i].Amount_Discount);
                twin = twin + parseInt(data.TAR[i].Amount_Win);
                if (data.TAR[i].Type == "Up") {
                    $("#udis").html(data.TAR[i].Amount_Discount);
                    $("#uwin").html(data.TAR[i].Amount_Win);
                    $("#utotal").html(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win));
                }
                else if (data.TAR[i].Type == "Down") {
                    $("#ddis").html(data.TAR[i].Amount_Discount);
                    $("#dwin").html(data.TAR[i].Amount_Win);
                    $("#dtotal").html(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win));
                }
                else if (data.TAR[i].Type == "FirstThree") {
                    $("#f3dis").html(data.TAR[i].Amount_Discount);
                    $("#f3win").html(data.TAR[i].Amount_Win);
                    $("#f3total").html(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win));
                }
                else if (data.TAR[i].Type == "FirstThreeOod") {
                    $("#f3odis").html(data.TAR[i].Amount_Discount);
                    $("#f3owin").html(data.TAR[i].Amount_Win);
                    $("#f3ototal").html(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win));
                }
                else if (data.TAR[i].Type == "ThreeUp") {
                    $("#u3dis").html(data.TAR[i].Amount_Discount);
                    $("#u3win").html(data.TAR[i].Amount_Win);
                    $("#u3total").html(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win));
                }
                else if (data.TAR[i].Type == "ThreeUPOod") {
                    $("#u3odis").html(data.TAR[i].Amount_Discount);
                    $("#u3owin").html(data.TAR[i].Amount_Win);
                    $("#u3ototal").html(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win));
                }
                else if (data.TAR[i].Type == "ThreeDown") {
                    $("#d3dis").html(data.TAR[i].Amount_Discount);
                    $("#d3win").html(data.TAR[i].Amount_Win);
                    $("#d3total").html(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win));
                }
                else if (data.TAR[i].Type == "TwoUp") {
                    $("#u2dis").html(data.TAR[i].Amount_Discount);
                    $("#u2win").html(data.TAR[i].Amount_Win);
                    $("#u2total").html(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win));
                }
                else if (data.TAR[i].Type == "TwoDown") {
                    $("#d2dis").html(data.TAR[i].Amount_Discount);
                    $("#d2win").html(data.TAR[i].Amount_Win);
                    $("#d2total").html(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win));
                }
                else if (data.TAR[i].Type == "TwoOod") {
                    $("#o2dis").html(data.TAR[i].Amount_Discount);
                    $("#o2win").html(data.TAR[i].Amount_Win);
                    $("#o2total").html(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win));
                }
                else {}
            }
            $("#tdis").html(tdis);
            $("#twin").html(twin);
            $("#ttotal").html(tdis - twin);
            var str = "";
            var win = 0;
            var dis = 0;
            var count = 0;
            for (var i = 0; i < Object.keys(data.UBET).length; i++) {
                count += 1;
                dis += parseInt(data.UBET[i].Discount);
                win += parseInt(data.UBET[i].Win);
                str += "<tr><td>" + data.UBET[i].Discount + "</td><td>" + data.UBET[i].Win + "</td ><td>" + data.UBET[i].Name +"</td></tr>";
            }
            $("#amount").html(dis);
            $("#count").html(count);
            $("#detail").html(str);
            $("#usertotal").html(dis);
            $("#totalwin").html(win);
            $("#sum").html(dis - win);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });
});