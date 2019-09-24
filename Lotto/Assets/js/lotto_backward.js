$(".lottoDetail").click(function () {
    var pid = $(this).attr("data-pid");
    var date = $(this).attr("data-date");
    $('.get-balance').attr("data-date", date);
    $('.get-keep').attr("data-date", date);
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
            var up = 0.00;
            var down = 0.00;
            var fthree = 0.00;
            var fthreeood = 0.00;
            var threeup = 0.00;
            var threeupood = 0.00;
            var threedown = 0.00;
            var twoup = 0.00;
            var twodown = 0.00;
            var twoood = 0.00;

            var wup = 0;
            var wdown = 0;
            var wfthree = 0;
            var wfthreeood = 0;
            var wthreeup = 0;
            var wthreeupood = 0;
            var wthreedown = 0;
            var wtwoup = 0;
            var wtwodown = 0;
            var wtwoood = 0;

            var tup = 0;
            var tdown = 0;
            var tfthree = 0;
            var tfthreeood = 0;
            var tthreeup = 0;
            var tthreeupood = 0;
            var tthreedown = 0;
            var ttwoup = 0;
            var ttwodown = 0;
            var ttwoood = 0;

            var totaldis = 0;
            var toralwin = 0;

            var oup = 0.00;
            var odown = 0.00;
            var ofthree = 0.00;
            var ofthreeood = 0.00;
            var othreeup = 0.00;
            var othreeupood = 0.00;
            var othreedown = 0.00;
            var otwoup = 0.00;
            var otwodown = 0.00;
            var otwoood = 0.00;

            var owup = 0;
            var owdown = 0;
            var owfthree = 0;
            var owfthreeood = 0;
            var owthreeup = 0;
            var owthreeupood = 0;
            var owthreedown = 0;
            var owtwoup = 0;
            var owtwodown = 0;
            var owtwoood = 0;

            var otup = 0;
            var otdown = 0;
            var otfthree = 0;
            var otfthreeood = 0;
            var otthreeup = 0;
            var otthreeupood = 0;
            var otthreedown = 0;
            var ottwoup = 0;
            var ottwodown = 0;
            var ottwoood = 0;

            var ototaldis = 0;
            var otoralwin = 0;

            var tdis = 0;
            var twin = 0;
            for (var i = 0; i < Object.keys(data.RECEIVE).length; i++) {
                if (data.RECEIVE[i].Type == "t" && data.RECEIVE[i].NumLen == "1") {
                    up = parseFloat(data.RECEIVE[i].AmountDiscount);
                    wup = parseInt(data.RECEIVE[i].AmountWin);
                    tup = parseInt(up) - wup;
                    $("#rudis").html(nwc(parseInt(up)));
                    $("#ruwin").html(nwc(wup));
                    $("#rutotal").html(nwc(tup));
                    totaldis = totaldis + parseInt(up);
                    toralwin = toralwin + wup;
                }
                else if (data.RECEIVE[i].Type == "b" && data.RECEIVE[i].NumLen == "1") {
                    down = parseFloat(data.RECEIVE[i].AmountDiscount);
                    wdown = parseInt(data.RECEIVE[i].AmountWin);
                    tdown = parseInt(down) - wdown;
                    $("#rddis").html(nwc(parseInt(down)));
                    $("#rdwin").html(nwc(wdown));
                    $("#rdtotal").html(nwc(tdown));
                    totaldis = totaldis + parseInt(down);
                    toralwin = toralwin + wdown;
                }
                else if (data.RECEIVE[i].Type == "f" && data.RECEIVE[i].NumLen == "3") {
                    fthree = parseFloat(data.RECEIVE[i].AmountDiscount);
                    wfthree = parseInt(data.RECEIVE[i].AmountWin);
                    tfthree = parseInt(fthree) - wfthree;
                    $("#rf3dis").html(nwc(parseInt(fthree)));
                    $("#rf3win").html(nwc(wfthree));
                    $("#rf3total").html(nwc(tfthree));
                    totaldis = totaldis + parseInt(fthree);
                    toralwin = toralwin + wfthree;
                }
                else if (data.RECEIVE[i].Type == "f_" && data.RECEIVE[i].NumLen == "3") {
                    fthreeood = parseFloat(data.RECEIVE[i].AmountDiscount);
                    wfthreeood = parseInt(data.RECEIVE[i].AmountWin);
                    tfthreeood = parseInt(fthreeood) - wfthreeood;
                    $("#rf3odis").html(nwc(parseInt(fthreeood)));
                    $("#rf3owin").html(nwc(wfthreeood));
                    $("#rf3ototal").html(nwc(tfthreeood));
                    totaldis = totaldis + parseInt(fthreeood);
                    toralwin = toralwin + wfthreeood;
                }
                else if (data.RECEIVE[i].Type == "t" && data.RECEIVE[i].NumLen == "3") {
                    threeup = parseFloat(data.RECEIVE[i].AmountDiscount);
                    wthreeup = parseInt(data.RECEIVE[i].AmountWin);
                    tthreeup = parseInt(threeup) - wthreeup;
                    $("#ru3dis").html(nwc(parseInt(threeup)));
                    $("#ru3win").html(nwc(wthreeup));
                    $("#ru3total").html(nwc(tthreeup));
                    totaldis = totaldis + parseInt(threeup);
                    toralwin = toralwin + wthreeup;
                }
                else if (data.RECEIVE[i].Type == "t_" && data.RECEIVE[i].NumLen == "3") {
                    threeupood = parseFloat(data.RECEIVE[i].AmountDiscount);
                    wthreeupood = parseInt(data.RECEIVE[i].AmountWin);
                    tthreeupood = parseInt(threeupood) - wthreeupood;
                    $("#ru3odis").html(nwc(parseInt(threeupood)));
                    $("#ru3owin").html(nwc(wthreeupood));
                    $("#ru3ototal").html(nwc(tthreeupood));
                    totaldis = totaldis + parseInt(threeupood);
                    toralwin = toralwin + wthreeupood;
                }
                else if (data.RECEIVE[i].Type == "b" && data.RECEIVE[i].NumLen == "3") {
                    threedown = parseFloat(data.RECEIVE[i].AmountDiscount);
                    wthreedown = parseInt(data.RECEIVE[i].AmountWin);
                    tthreedown = parseInt(threedown) - wthreedown;
                    $("#rd3dis").html(nwc(parseInt(threedown)));
                    $("#rd3win").html(nwc(wthreedown));
                    $("#rd3total").html(nwc(tthreedown));
                    totaldis = totaldis + parseInt(threedown);
                    toralwin = toralwin + wthreedown;
                }
                else if (data.RECEIVE[i].Type == "t" && data.RECEIVE[i].NumLen == "2") {
                    twoup = parseFloat(data.RECEIVE[i].AmountDiscount);
                    wtwoup = parseInt(data.RECEIVE[i].AmountWin);
                    ttwoup = parseInt(twoup) - wtwoup;
                    $("#ru2dis").html(nwc(parseInt(twoup)));
                    $("#ru2win").html(nwc(wtwoup));
                    $("#ru2total").html(nwc(ttwoup));
                    totaldis = totaldis + parseInt(twoup);
                    toralwin = toralwin + wtwoup;
                }
                else if (data.RECEIVE[i].Type == "b" && data.RECEIVE[i].NumLen == "2") {
                    twodown = parseFloat(data.RECEIVE[i].AmountDiscount);
                    wtwodown = parseInt(data.RECEIVE[i].AmountWin);
                    ttwodown = parseInt(twodown) - wtwodown;
                    $("#rd2dis").html(nwc(parseInt(twodown)));
                    $("#rd2win").html(nwc(wtwodown));
                    $("#rd2total").html(nwc(ttwodown));
                    totaldis = totaldis + parseInt(twodown);
                    toralwin = toralwin + wtwodown;
                }
                else if (data.RECEIVE[i].Type == "t_" && data.RECEIVE[i].NumLen == "2") {
                    twoood = parseFloat(data.RECEIVE[i].AmountDiscount);
                    wtwoood = parseInt(data.RECEIVE[i].AmountWin);
                    ttwoood = parseInt(twoood) - wtwoood;
                    $("#ro2dis").html(nwc(parseInt(twoood)));
                    $("#ro2win").html(nwc(wtwoood));
                    $("#ro2total").html(nwc(ttwoood));
                    totaldis = totaldis + parseInt(twoood);
                    toralwin = toralwin + wtwoood;
                }
                else {}
            }

//-----------------------------------------------------------------------------------------------------------------------------//

            for (var i = 0; i < Object.keys(data.BetOut).length; i++) {
                if (data.BetOut[i].Type == "t" && data.BetOut[i].NumLen == "1") {
                    oup = parseFloat(data.BetOut[i].AmountDiscount);
                    owup = parseInt(data.BetOut[i].AmountWin);
                    otup = parseInt(oup) - owup;
                    $("#oudis").html(nwc(parseInt(oup)));
                    $("#ouwin").html(nwc(owup));
                    $("#outotal").html(nwc(otup));
                    ototaldis = ototaldis + parseInt(oup);
                    otoralwin = otoralwin + owup;
                }
                else if (data.BetOut[i].Type == "b" && data.BetOut[i].NumLen == "1") {
                    odown = parseFloat(data.BetOut[i].AmountDiscount);
                    owdown = parseInt(data.BetOut[i].AmountWin);
                    otdown = parseInt(odown) - owdown;
                    $("#oddis").html(nwc(parseInt(odown)));
                    $("#odwin").html(nwc(owdown));
                    $("#odtotal").html(nwc(otdown));
                    ototaldis = ototaldis + parseInt(odown);
                    otoralwin = otoralwin + owdown;
                }
                else if (data.BetOut[i].Type == "f" && data.BetOut[i].NumLen == "3") {
                    ofthree = parseFloat(data.BetOut[i].AmountDiscount);
                    owfthree = parseInt(data.BetOut[i].AmountWin);
                    otfthree = parseInt(ofthree) - owfthree;
                    $("#of3dis").html(nwc(parseInt(ofthree)));
                    $("#of3win").html(nwc(owfthree));
                    $("#of3total").html(nwc(otfthree));
                    ototaldis = ototaldis + parseInt(ofthree);
                    otoralwin = otoralwin + owfthree;
                }
                else if (data.BetOut[i].Type == "f_" && data.BetOut[i].NumLen == "3") {
                    ofthreeood = parseFloat(data.BetOut[i].AmountDiscount);
                    owfthreeood = parseInt(data.BetOut[i].AmountWin);
                    otfthreeood = parseInt(ofthreeood) - owfthreeood;
                    $("#of3odis").html(nwc(parseInt(ofthreeood)));
                    $("#of3owin").html(nwc(owfthreeood));
                    $("#of3ototal").html(nwc(otfthreeood));
                    ototaldis = ototaldis + parseInt(ofthreeood);
                    otoralwin = otoralwin + owfthreeood;
                }
                else if (data.BetOut[i].Type == "t" && data.BetOut[i].NumLen == "3") {
                    othreeup = parseFloat(data.BetOut[i].AmountDiscount);
                    owthreeup = parseInt(data.BetOut[i].AmountWin);
                    otthreeup = parseInt(othreeup) - owthreeup;
                    $("#ou3dis").html(nwc(parseInt(othreeup)));
                    $("#ou3win").html(nwc(owthreeup));
                    $("#ou3total").html(nwc(otthreeup));
                    ototaldis = ototaldis + parseInt(othreeup);
                    otoralwin = otoralwin + owthreeup;
                }
                else if (data.BetOut[i].Type == "t_" && data.BetOut[i].NumLen == "3") {
                    othreeupood = parseFloat(data.BetOut[i].AmountDiscount);
                    owthreeupood = parseInt(data.BetOut[i].AmountWin);
                    otthreeupood = parseInt(othreeupood) - owthreeupood;
                    $("#ou3odis").html(nwc(parseInt(othreeupood)));
                    $("#ou3owin").html(nwc(owthreeupood));
                    $("#ou3ototal").html(nwc(otthreeupood));
                    ototaldis = ototaldis + parseInt(othreeupood);
                    otoralwin = otoralwin + owthreeupood;
                }
                else if (data.BetOut[i].Type == "b" && data.BetOut[i].NumLen == "3") {
                    othreedown = parseFloat(data.BetOut[i].AmountDiscount);
                    owthreedown = parseInt(data.BetOut[i].AmountWin);
                    otthreedown = parseInt(othreedown) - owthreedown;
                    $("#od3dis").html(nwc(parseInt(othreedown)));
                    $("#od3win").html(nwc(owthreedown));
                    $("#od3total").html(nwc(otthreedown));
                    ototaldis = ototaldis + parseInt(othreedown);
                    otoralwin = otoralwin + owthreedown;
                }
                else if (data.BetOut[i].Type == "t" && data.BetOut[i].NumLen == "2") {
                    otwoup = parseFloat(data.BetOut[i].AmountDiscount);
                    owtwoup = parseInt(data.BetOut[i].AmountWin);
                    ottwoup = parseInt(otwoup) - owtwoup;
                    $("#ou2dis").html(nwc(parseInt(otwoup)));
                    $("#ou2win").html(nwc(owtwoup));
                    $("#ou2total").html(nwc(ottwoup));
                    ototaldis = ototaldis + parseInt(otwoup);
                    otoralwin = otoralwin + owtwoup;
                }
                else if (data.BetOut[i].Type == "b" && data.BetOut[i].NumLen == "2") {
                    otwodown = parseFloat(data.BetOut[i].AmountDiscount);
                    owtwodown = parseInt(data.BetOut[i].AmountWin);
                    ottwodown = parseInt(otwodown) - owtwodown;
                    $("#od2dis").html(nwc(parseInt(otwodown)));
                    $("#od2win").html(nwc(owtwodown));
                    $("#od2total").html(nwc(ottwodown));
                    ototaldis = ototaldis + parseInt(otwodown);
                    otoralwin = otoralwin + owtwodown;
                }
                else if (data.BetOut[i].Type == "t_" && data.BetOut[i].NumLen == "2") {
                    otwoood = parseFloat(data.BetOut[i].AmountDiscount);
                    owtwoood = parseInt(data.BetOut[i].AmountWin);
                    ottwoood = parseInt(otwoood) - owtwoood;
                    $("#oo2dis").html(nwc(parseInt(otwoood)));
                    $("#oo2win").html(nwc(owtwoood));
                    $("#oo2total").html(nwc(ottwoood));
                    ototaldis = ototaldis + parseInt(otwoood);
                    otoralwin = otoralwin + owtwoood;
                }
                else { }

                //-----------------------------------------------------------------------------------------------------------------------------//

                $("#tudis").html(nwc(parseInt(up+oup)));
                $("#tuwin").html(nwc(wup+owup));
                $("#tutotal").html(nwc(tup+otup));
                $("#tddis").html(nwc(parseInt(down+odown)));
                $("#tdwin").html(nwc(wdown+owdown));
                $("#tdtotal").html(nwc(tdown+otdown));
                $("#tf3dis").html(nwc(parseInt(fthree+ofthree)));
                $("#tf3win").html(nwc(wfthree+owfthree));
                $("#tf3total").html(nwc(tfthree+otfthree));
                $("#tf3odis").html(nwc(parseInt(fthreeood+ofthreeood)));
                $("#tf3owin").html(nwc(wfthreeood+owfthreeood));
                $("#tf3ototal").html(nwc(tfthreeood+otfthreeood));
                $("#tu3dis").html(nwc(parseInt(threeup+othreeup)));
                $("#tu3win").html(nwc(wthreeup+owthreeup));
                $("#tu3total").html(nwc(tthreeup+otthreeup));
                $("#tu3odis").html(nwc(parseInt(threeupood+othreeupood)));
                $("#tu3owin").html(nwc(wthreeupood+owthreeupood));
                $("#tu3ototal").html(nwc(tthreeupood+otthreeupood));
                $("#td3dis").html(nwc(parseInt(threedown+othreedown)));
                $("#td3win").html(nwc(wthreedown+owthreedown));
                $("#td3total").html(nwc(tthreedown+otthreedown));
                $("#tu2dis").html(nwc(parseInt(twoup+otwoup)));
                $("#tu2win").html(nwc(wtwoup+owtwoup));
                $("#tu2total").html(nwc(ttwoup+ottwoup));
                $("#td2dis").html(nwc(parseInt(twodown+otwodown)));
                $("#td2win").html(nwc(wtwodown+owtwodown));
                $("#td2total").html(nwc(ttwodown+ottwodown));
                $("#to2dis").html(nwc(parseInt(twoood+otwoood)));
                $("#to2win").html(nwc(wtwoood+owtwoood));
                $("#to2total").html(nwc(ttwoood+ottwoood));
            }
            $("#rtdis").html(nwc(totaldis));
            $("#rtwin").html(nwc(toralwin));
            $("#rttotal").html(nwc(totaldis - toralwin));
            $("#otdis").html(nwc(ototaldis));
            $("#otwin").html(nwc(otoralwin));
            $("#ottotal").html(nwc(ototaldis - otoralwin));
            $("#ttdis").html(nwc(totaldis+ototaldis));
            $("#ttwin").html(nwc(toralwin+otoralwin));
            $("#tttotal").html(nwc((totaldis - toralwin)+(ototaldis - otoralwin)));
            var str = "";
            var win = 0;
            var dis = 0;
            var count = 0;
            for (var i = 0; i < Object.keys(data.UBET).length; i++) {
                count += 1;
                dis += parseInt(data.UBET[i].Discount);
                win += parseInt(data.UBET[i].Win);
                str += "<tr><td>" + data.UBET[i].Name +"</td><td>" + nwc(data.UBET[i].Discount) + "</td><td>" + nwc(data.UBET[i].Win) + "</td ></tr>";
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