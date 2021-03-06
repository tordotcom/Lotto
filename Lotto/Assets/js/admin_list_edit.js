﻿$(document)
    .ajaxStart(function () {
        $('#AjaxLoader').show();
    })
    .ajaxStop(function () {
        $('#AjaxLoader').hide();
    });
$(document).on('click', '.bet-type', function () {
//$(".bet-type").click(function () {

    if($(".showPoll").data('type') != 0){
        $(this).find(".input-bet-type").fadeOut(0, function () {
            $(this).css('cursor', 'default').css('text-decoration', 'none');

            var text = $(this).text();
            if (text == 'บ') {
                $(this).html('<font color="red">ล</font>').fadeIn(10);
                document.getElementById(this.id).setAttribute('data-type', "b");
            }
            if (text == 'ล') {
                $(this).html('บ+<font color="red">ล</font>').fadeIn(10);
                document.getElementById(this.id).setAttribute('data-type', "tb");
            }
            if (text == 'บ+ล') {
                $(this).html('<font color="blue">ห</font>').fadeIn(10);
                document.getElementById(this.id).setAttribute('data-type', "f");
            }
            if (text == 'ห') {
                $(this).html('<font color="blue">ห</font>+<font color="green">ท</font>').fadeIn(10);
                document.getElementById(this.id).setAttribute('data-type', "ft");
            }
            if (text == 'ห+ท') {
                $(this).html('บ').fadeIn(10);
                document.getElementById(this.id).setAttribute('data-type', "t");
            }
            var id = (this.id).replace("t", "");
            if ($('#n' + id).val() != "" && $('#b' + id).val() != "") {
                SumAmountPoll();
            }

        })
    }
});

$("#clear").click(function () {
    Swal.fire({
        title: 'ต้องการเคลียร์เลข?',
        text: "กรุณายืนยันการเคลียร์ตัวเลข",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'เคลียร์',
        cancelButtonText: 'ยกเลิก'
    }).then((result) => {
        location.reload();
    })
});
function keyPress2(obj, id, evt) {
    evt = evt || window.event;
    var code = evt.keyCode || evt.which;
    // enter = 13, tab = 9
    // space = 32,  + = 43
    if (code == 13 || code == 9) { // press enter or tab
        nextNode(obj, id);
    } else if (code == 32 || code == 43) { // press + or space
        updateType(id);
    }

    if (code >= 48 && code <= 57) {
        $("#n" + id).removeClass("input-color");
        return true;  // Number 0-9
    }
    else if (code == 8 || code == 46 || code == 9 || code == 42 || code == 88 || code == 120) return true; // 42 = *, 8 = back, 46 = del, 9 = tab, 47 = slash, 88 = x, 120 = X
    else if (code >= 37 && code <= 40) return true;  // left, up, right, down
    else return false;
}
function keyPress(obj, id, evt) {

    evt = evt || window.event;
    var code = evt.keyCode || evt.which;
    // enter = 13, tab = 9
    // space = 32,  + = 43

    if (code == 13 || code == 9) { // press enter or tab
        nextNode(obj, id);
    } else if (code == 32 || code == 43) { // press + or space
        updateType(id);
    }

    if (code >= 48 && code <= 57) {
        $("#b" + id).removeClass("input-color");
        return true;  // Number 0-9
    }
    else if (code == 42 || code == 8 || code == 46 || code == 9 || code == 88 || code == 120 || code == 47) return true; // 42 = *, 8 = back, 46 = del, 9 = tab, 47 = slash, 88 = x, 120 = X
    else if (code >= 37 && code <= 40) return true;  // left, up, right, down
    else return false;
}
function nextNode(obj, id) {
    var currentObj = obj;
    var currentTyp = $("#t" + id).text();
    var NumberOfCell = 90;
    var MaxNodeNumber = 89;
    if (currentObj.id.indexOf('n') >= 0) {
        if (currentObj.value == null || currentObj.value == '') {
            Swal.fire({
                type: 'error',
                title: 'กรุณากรอกข้อมูล',
                showConfirmButton: true,
            });
        } else {
            $("#b" + id).focus();
        }
    } else if (currentObj.id.indexOf('b') >= 0) {
        if (currentObj.value == null || currentObj.value == '' || parseInt(currentObj.value) == 0) {
            if (id == 0) {
                Swal.fire({
                    type: 'error',
                    title: 'กรุณากรอกข้อมูล',
                    showConfirmButton: true,
                });
            }
            else {
                var newId = parseInt(id) + 1;
                if (id <= 74) {
                    currentObj.value = document.getElementById('b' + (id - 1)).value;

                    $("#n" + newId).focus();
                    setType(newId, currentTyp);
                }
            }
        }
        else {
            var newId = parseInt(id) + 1;
            if (id <= 74) {
                $("#n" + newId).focus();
                setType(newId, currentTyp);
            }

        }
    }

    //if (runSumTotal == true) {
    SumAmountPoll();
    //}
}
function setType(id, type) {
    if (type == 'ล') {
        $("#t" + id).html('<font color="red">ล</font>').fadeIn(0);
        document.getElementById('t' + id).setAttribute('data-type', "b");
    }
    if (type == 'บ+ล') {
        $("#t" + id).html('บ+<font color="red">ล</font>').fadeIn(0);
        document.getElementById('t' + id).setAttribute('data-type', "tb");
    }
    if (type == 'ห') {
        $("#t" + id).html('<font color="blue">ห</font>').fadeIn(0);
        document.getElementById('t' + id).setAttribute('data-type', "f");
    }
    if (type == 'ห+ท') {
        $("#t" + id).html('<font color="blue">ห</font>+<font color="green">ท</font>').fadeIn(0);
        document.getElementById('t' + id).setAttribute('data-type', "ft");
    }
    if (type == 'บ') {
        $("#t" + id).html('บ').fadeIn(0);
        document.getElementById('t' + id).setAttribute('data-type', "t");
    }
    //var id = (this.id).replace("t", "");
    if ($('#n' + id).val() != "" && $('#b' + id).val() != "") {
        SumAmountPoll();
    }
}
function updateType(id) {
    $("#t" + id).click();
}
function checkNumber(id) {

    var sNum = $('#n' + id).val();
    var sType = $('#t' + id).val();

    var iLen = sNum.length;

    // If it is empty, no need to check.
    if (iLen == 0) {
        $("#n" + id).addClass("input-color");
        return true;
    }

    // Change * or X to x
    sNum = sNum.replace("*", "x");
    sNum = sNum.replace("X", "x");

    $('#n' + id).val(sNum);

    // Check format
    var numFormat = /^\d{1,3}[x]?$/;
    if (!numFormat.test(sNum)) {
        Swal.fire({
            type: 'error',
            title: 'ช่องที่ ' + id + ' โปรดตรวจสอบเลขที่แทง  ต้องเป็นตัวเลข 1 - 3 ตัวเท่านั้น และใส่ * ได้ที่ตำแหน่งสุดท้ายของเลข 3 ตัวสำหรับ 3 กลับ 6 กลับเท่านั้น',
            showConfirmButton: true,
            onAfterClose: () => {
                $("#b" + id).focus();
            }
        });
        return false;
    }

    var iPosA = sNum.indexOf('x');
    if (iPosA >= 0) {
        if (iPosA != 3) {
            Swal.fire({
                type: 'error',
                title: 'ช่องที่ ' + id + ': โปรดตรวจสอบเลขที่แทง  ใส่ * ได้ที่ตำแหน่งสุดท้ายของเลข 3 ตัว สำหรับ 3 กลับ 6 กลับเท่านั้น',
                showConfirmButton: true,
                onAfterClose: () => {
                    $("#b" + id).focus();
                }
            });
            return false;
        } else {
            iLen = 3;
        }
    }
    if (iLen > 3) {
        Swal.fire({
            type: 'error',
            title: 'ช่องที่ ' + id + ' โปรดตรวจสอบเลขที่แทง  ใส่เลขได้ไม่เกิน 3 ตัวเท่านั้น',
            showConfirmButton: true,
            onAfterClose: () => {
                $("#n" + id).focus();
            }
        });
        return false;
    }
    SumAmountPoll();
    return true;
}
function checkAmount(id) {

    var sTyp = $('#t' + id).attr("data-type");
    var sNum = $('#n' + id).val();
    var sAmt = $('#b' + id).val();

    var iNumLen = sNum.length;  // Get number length
    var iAmtLen = sAmt.length;  // Get amount length
    // If it is empty, no need to check.
    if (iAmtLen == 0) {
        $("#b" + id).addClass("input-color");
        return true;
    }

    // Change * or X to x
    sNum = sNum.replace("X", "x");
    sNum = sNum.replace("*", "x");
    sAmt = sAmt.replace("X", "x");
    sAmt = sAmt.replace("*", "x");

    $('#n' + id).val(sNum);
    $('#b' + id).val(sAmt);

    // Check amount format
    var amtFormat = /^ *[0-9\/x]+ *$/;
    if (!amtFormat.test(sAmt)) {
        Swal.fire({
            type: 'error',
            title: 'ช่องที่ ' + id + ' โปรดตรวจสอบจำนวนเงินที่แทง  ต้องเป็นตัวเลข หรือ x หรือ / เท่านั้น',
            showConfirmButton: true,
            onAfterClose: () => {
                $("#b" + id).focus();
            }
        });
        return false;
    }

    var iAmtCountA = sAmt.split("x").length - 1
    if (iAmtCountA > 1) {
        Swal.fire({
            type: 'error',
            title: 'ช่องที่ ' + id + ' โปรดตรวจสอบจำนวนเงินที่แทง  ต้องมี x ได้ 1 ตัวเท่านั้น',
            showConfirmButton: true,
            onAfterClose: () => {
                $("#b" + id).focus();
            }
        });
        return false;
    }
    var iAmtCountS = sAmt.split("/").length - 1;
    if (iAmtCountS > 1) {
        Swal.fire({
            type: 'error',
            title: 'ช่องที่ ' + id + ' โปรดตรวจสอบจำนวนเงินที่แทง  ต้องมี / ได้ 1 ตัวเท่านั้น',
            showConfirmButton: true,
            onAfterClose: () => {
                $("#b" + id).focus();
            }
        });
        return false;
    }
    // Check number
    if (iNumLen == 0) {
        Swal.fire({
            type: 'error',
            title: 'ช่องที่ ' + id + ' กรุณากรอกเลขที่ต้องการแทง',
            showConfirmButton: true,
            onAfterClose: () => {
                $("#n" + id).focus();
            }
        });
        return false;
    }

    var iAmtPosA = sAmt.indexOf('x');  // Check x
    var iAmtPosS = sAmt.indexOf('/');  // Check /
    var iNumPosA = sNum.indexOf('x');  // Check x
    var bValid = false;

    // วิ่งบน
    if ((sTyp == 't') && (iNumLen == 1) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;
    }
    // 2 บน
    else if ((sTyp == 't') && (iNumLen == 2) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;

        // 2 โต๊ด
    } else if ((sTyp == 't') && (iNumLen == 2) && (iNumPosA == -1) && (iAmtPosA == 0) && (iAmtPosS == -1)) {
        bValid = true;

        // 2 บน และ ตัวกลับ
    } else if ((sTyp == 't') && (iNumLen == 2) && (iNumPosA == -1) && (iAmtPosA > 0) && (iAmtPosA < iAmtLen - 1) && (iAmtPosS == -1)) {
        bValid = true;

        // 3 บน
    } else if ((sTyp == 't') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;

        // 3 โต๊ด
    }
    else if ((sTyp == 't') && (iNumLen == 4) && (iNumPosA == 3) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;

    }
    else if ((sTyp == 't') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA == 0) && (iAmtPosS == -1)) {
        bValid = true;

        // 3 บน  3 โต๊ด
    } else if ((sTyp == 't') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA > 0) && (iAmtPosA < iAmtLen - 1) && (iAmtPosS == -1)) {
        bValid = true;
    }
    // 3 บน  3 กลับ และ 6 กลับ
    // วิ่งล่าง
    else if ((sTyp == 'b') && (iNumLen == 1) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;
    }
    // 2 ล่าง
    else if ((sTyp == 'b') && (iNumLen == 2) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;

        // 2 ล่าง และ ตัวกลับ
    } else if ((sTyp == 'b') && (iNumLen == 2) && (iNumPosA == -1) && (iAmtPosA > 0) && (iAmtPosA < iAmtLen - 1) && (iAmtPosS == -1)) {
        bValid = true;

        // 3 ล่าง
    } else if ((sTyp == 'b') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;
    }
    else if ((sTyp == 'b') && (iNumLen == 4) && (iNumPosA == 3) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;

    }
    // วิ่งบน + วิ่งล่าง
    else if ((sTyp == 'tb') && (iNumLen == 1) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;

        // 2บน + 2ล่าง
    } else if ((sTyp == 'tb') && (iNumLen == 2) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;

        // 2บน + 2ล่าง และ ตัวกลับ  (71 = 10x10)
    } else if ((sTyp == 'tb') && (iNumLen == 2) && (iNumPosA == -1) && (iAmtPosA > 0) && (iAmtPosA < iAmtLen - 1) && (iAmtPosS == -1)) {
        bValid = true;

        // 2บน + 2ล่าง (71 = 10/10)
    } else if ((sTyp == 'tb') && (iNumLen == 2) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS > 0) && (iAmtPosS < iAmtLen - 1)) {
        bValid = true;

        // 3บน + 3ล่าง (124 = 100)
    } else if ((sTyp == 'tb') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;

        // 3บน + 3ล่าง (124 = 100/50)
    } else if ((sTyp == 'tb') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS > 0) && (iAmtPosS < iAmtLen - 1)) {
        bValid = true;

        // 3บน  3โต๊ด 3ล่าง (123 = 10x20/30)
    } else if ((sTyp == 'tb') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA > 0) && (iAmtPosA < iAmtLen - 2) && (iAmtPosS > iAmtPosA) && (iAmtPosS < iAmtLen - 1)) {
        bValid = true;
    }
    else if ((sTyp == 'tb') && (iNumLen == 4) && (iNumPosA == 3) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;

    }
    // 3 หน้า
    else if ((sTyp == 'f') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;

        // 3 หน้าโต๊ด
    } else if ((sTyp == 'f') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA == 0) && (iAmtPosS == -1)) {
        bValid = true;

        // 3 หน้า  3 หน้าโต๊ด
    } else if ((sTyp == 'f') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA > 0) && (iAmtPosA < iAmtLen - 1) && (iAmtPosS == -1)) {
        bValid = true;
    }
    else if ((sTyp == 'f') && (iNumLen == 4) && (iNumPosA == 3) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;

    }
    // 3 หน้า  3 ท้าย
    else if ((sTyp == 'ft') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;
    }
    // 3หัว + 3ท้าย (124 = 100/50)
    else if ((sTyp == 'ft') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA == -1) && (iAmtPosS > 0) && (iAmtPosS < iAmtLen - 1)) {
        bValid = true;
    }
    // 3 หน้า  3 ท้าย โต๊ด
    else if ((sTyp == 'ft') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA == 0) && (iAmtPosS == -1)) {
        bValid = true;
    }
    // 3 หน้า  3 หน้าโต๊ด 3 หลัง 3 หลังโต๊ด
    else if ((sTyp == 'ft') && (iNumLen == 3) && (iNumPosA == -1) && (iAmtPosA > 0) && (iAmtPosA < iAmtLen - 1) && (iAmtPosS == -1)) {
        bValid = true;
    }
    else if ((sTyp == 'ft') && (iNumLen == 4) && (iNumPosA == 3) && (iAmtPosA == -1) && (iAmtPosS == -1)) {
        bValid = true;

    }
    else { }

    if (!bValid) {
        if (((sTyp == 'f') || (sTyp == 'ft')) && iNumLen < 3) {
            Swal.fire({
                type: 'error',
                title: 'ช่องที่ ' + id + ' รูปแบบการแทงไม่ถูกต้อง',
                text: 'เลขหน้าต้องมี 3 ตัวเลข',
                showConfirmButton: true,
                onAfterClose: () => {
                    $("#b" + id).focus();
                }
            });
        }
        else {
            Swal.fire({
                type: 'error',
                title: 'ช่องที่ ' + id + ' รูปแบบการแทงไม่ถูกต้อง',
                showConfirmButton: true,
                onAfterClose: () => {
                    $("#b" + id).focus();
                }
            });
        }
        return false;
    }
    SumAmountPoll();

    return true;
}
function SumAmountPoll() {
    sum = 0;
    var bet = {};
    var j = 1;
	for (var i = 1; i <= rowPoll; i++) {
        var n = $('#n' + i).val();
        var b = $('#b' + i).val();
        if (n.length > 0 && b.length > 0) {
            bet[j] = {};
            bet[j]["type"] = $('#t' + i).attr("data-type");
            bet[j]["Number"] = n;
            bet[j]["Amount"] = b;
            j++;
        }
    }
    var arrLength = Object.keys(bet).length;
    for (var i = 1; i <= arrLength; i++) {
        //console.log(bet[i]);
        var NumLen = bet[i]["Number"];
        //console.log(NumLen);
        NumLen = NumLen.length;
        Amt = bet[i]["Amount"].replace("X", "x");
        Amt = bet[i]["Amount"].replace("*", "x");
        var numx = bet[i]["Number"].replace("X", "x");
        numx = bet[i]["Number"].replace("*", "x");
        var AmtCountX = Amt.split("x").length - 1;
        var NumCountX = numx.split("x").length - 1;
        var AmtcountSlash = Amt.split("/").length - 1;
        if (AmtCountX > 0 && AmtcountSlash > 0) {
            var amount = Amt.split(/[x/]+/);
            sum = sum + parseInt(amount[0]) + parseInt(amount[1]) + parseInt(amount[2]);
        }
        else if (AmtcountSlash > 0) {
            var amount = Amt.split("/");
            sum = sum + parseInt(amount[0]) + parseInt(amount[1]);
        }
        else if (AmtCountX > 0) {
            var amount = Amt.split("x");
            if (amount[0] != "") {
                if (bet[i]["type"] == "tb" || bet[i]["type"] == "ft") {
                    sum = sum + (parseInt(amount[0]) * 2) + (parseInt(amount[1]) * 2);
                }
                else {
                    sum = sum + parseInt(amount[0]) + parseInt(amount[1]);
                }
            }
            else {
                if (bet[i]["type"] == "ft") {
                    sum = sum + (parseInt(amount[1]) * 2);
                }
                else {
                    sum = sum + parseInt(amount[1]);
                }
            }
        }
        else {
            if (NumCountX > 0) {
                var nnum = numx.split("x");
                var charnum = nnum[0].split("");
                if (bet[i]["type"] == "tb" || bet[i]["type"] == "ft") {
                    if (charnum[0] == charnum[1] && charnum[0] == charnum[2]) {
                        sum = sum + parseInt(bet[i]["Amount"] * 2);
                    }
                    else if (charnum[0] != charnum[1] && charnum[0] != charnum[2] && charnum[1] != charnum[2]) {
                        sum = sum + (parseInt(bet[i]["Amount"]) * 12);
                    }
                    else if (charnum[0] == charnum[1] || charnum[0] == charnum[2] || charnum[1] == charnum[2]) {
                        sum = sum + (parseInt(bet[i]["Amount"]) * 6);
                    }
                    else { }
                }
                else {
                    if (charnum[0] == charnum[1] && charnum[0] == charnum[2]) {
                        sum = sum + parseInt(bet[i]["Amount"]);
                    }
                    else if (charnum[0] != charnum[1] && charnum[0] != charnum[2] && charnum[1] != charnum[2]) {
                        sum = sum + (parseInt(bet[i]["Amount"]) * 6);
                    }
                    else if (charnum[0] == charnum[1] || charnum[0] == charnum[2] || charnum[1] == charnum[2]) {
                        sum = sum + (parseInt(bet[i]["Amount"]) * 3);
                    }
                    else { }
                }
            }
            else {
                if (bet[i]["type"] == "tb" || bet[i]["type"] == "ft") {
                    sum = sum + (parseInt(bet[i]["Amount"]) * 2);
                }
                else {
                    sum = sum + parseInt(bet[i]["Amount"]);
                }
            }
        }
    }
	$("#amount").html(sum);
}
var ipadd;
(function ($) {
    $.getJSON("http://jsonip.com?callback=?", function (data) {
        ipadd = data.ip;
    });
})(jQuery);
$("#sendPoll").click(function () {
    var state = false;
    var bet = [];
    var name = $('#poll_name').val();
	for (var i = 1; i <= rowPoll; i++) {
        var n = $('#n' + i).val();
        var b = $('#b' + i).val();
        b = b.replace("X", "x");
        b = b.replace("*", "x");
        if (n.length > 0 && b.length == 0) {
            Swal.fire({
                type: 'error',
                title: 'ช่องที่ ' + i + ' กรุณากรอกจำนวนเงิน',
                showConfirmButton: true,
                onAfterClose: () => {
                    $("#b" + i).focus();
                }
            });
            state = false;
            return false;
        }
        else if (b.length > 0 && n.length == 0) {
            Swal.fire({
                type: 'error',
                title: 'ช่องที่ ' + i + ' กรุณากรอกเลขที่จะแทง',
                showConfirmButton: true,
                onAfterClose: () => {
                    $("#n" + i).focus();
                }
            });
            state = false;
            return false;
        }
        else if (n.length > 0 && b.length > 0) {
            if (!checkNumber(i)) {
                state = false;
                return false;
            } else if (!checkAmount(i)) {
                state = false;
                return false;
            }
            else {
                bet.push({
                    bType: $('#t' + i).attr("data-type"),
                    Number: n,
                    Amount: b
                });
                state = true;
            }
        }
	}

    console.log(bet);
	var uid = $(this).attr("data-uid");
	var pid = $(this).attr("data-pid");

    if (state) {
        $.ajax({
            url: addPoll,
            data: { PollName: name, IPAddress: ipadd, poll: bet, UID: uid , PID: pid},
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data == "ss") {
                    Swal.fire({
                        type: 'success',
                        title: 'บันทึกเรียบร้อย',
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
        Swal.fire({
            type: 'danger',
            title: 'กรอกข้อมูลให้ถูกต้อง',
            showConfirmButton: true,
        });
    }
});