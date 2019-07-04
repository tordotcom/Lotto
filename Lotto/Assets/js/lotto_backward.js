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
            $("#total").html(nwc(data.all[0].Amount));
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
                    $("#udis").html(nwc(data.TAR[i].Amount_Discount));
                    $("#uwin").html(nwc(data.TAR[i].Amount_Win));
                    $("#utotal").html(nwc(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win)));
                }
                else if (data.TAR[i].Type == "Down") {
                    $("#ddis").html(nwc(data.TAR[i].Amount_Discount));
                    $("#dwin").html(nwc(data.TAR[i].Amount_Win));
                    $("#dtotal").html(nwc(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win)));
                }
                else if (data.TAR[i].Type == "FirstThree") {
                    $("#f3dis").html(nwc(data.TAR[i].Amount_Discount));
                    $("#f3win").html(nwc(data.TAR[i].Amount_Win));
                    $("#f3total").html(nwc(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win)));
                }
                else if (data.TAR[i].Type == "FirstThreeOod") {
                    $("#f3odis").html(nwc(data.TAR[i].Amount_Discount));
                    $("#f3owin").html(nwc(data.TAR[i].Amount_Win));
                    $("#f3ototal").html(nwc(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win)));
                }
                else if (data.TAR[i].Type == "ThreeUp") {
                    $("#u3dis").html(nwc(data.TAR[i].Amount_Discount));
                    $("#u3win").html(nwc(data.TAR[i].Amount_Win));
                    $("#u3total").html(nwc(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win)));
                }
                else if (data.TAR[i].Type == "ThreeUPOod") {
                    $("#u3odis").html(nwc(data.TAR[i].Amount_Discount));
                    $("#u3owin").html(nwc(data.TAR[i].Amount_Win));
                    $("#u3ototal").html(nwc(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win)));
                }
                else if (data.TAR[i].Type == "ThreeDown") {
                    $("#d3dis").html(nwc(data.TAR[i].Amount_Discount));
                    $("#d3win").html(nwc(data.TAR[i].Amount_Win));
                    $("#d3total").html(nwc(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win)));
                }
                else if (data.TAR[i].Type == "TwoUp") {
                    $("#u2dis").html(nwc(data.TAR[i].Amount_Discount));
                    $("#u2win").html(nwc(data.TAR[i].Amount_Win));
                    $("#u2total").html(nwc(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win)));
                }
                else if (data.TAR[i].Type == "TwoDown") {
                    $("#d2dis").html(nwc(data.TAR[i].Amount_Discount));
                    $("#d2win").html(nwc(data.TAR[i].Amount_Win));
                    $("#d2total").html(nwc(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win)));
                }
                else if (data.TAR[i].Type == "TwoOod") {
                    $("#o2dis").html(nwc(data.TAR[i].Amount_Discount));
                    $("#o2win").html(nwc(data.TAR[i].Amount_Win));
                    $("#o2total").html(nwc(parseInt(data.TAR[i].Amount_Discount) - parseInt(data.TAR[i].Amount_Win)));
                }
                else {}
            }
            $("#tdis").html(nwc(tdis));
            $("#twin").html(nwc(twin));
            $("#ttotal").html(nwc(tdis - twin));
            var str = "";
            var win = 0;
            var dis = 0;
            var count = 0;
            for (var i = 0; i < Object.keys(data.UBET).length; i++) {
                count += 1;
                dis += parseInt(data.UBET[i].Discount);
                win += parseInt(data.UBET[i].Win);
                str += "<tr><td>" + nwc(data.UBET[i].Discount) + "</td><td>" + nwc(data.UBET[i].Win) + "</td ><td>" + data.UBET[i].Name +"</td></tr>";
            }
            $("#amount").html(nwc(dis));
            $("#count").html(nwc(count));
            $("#detail").html(str);
            $("#usertotal").html(nwc(dis));
            $("#totalwin").html(nwc(win));
            $("#sum").html(nwc(dis - win));
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });
});