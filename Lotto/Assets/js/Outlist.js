function OutPoll (ToUid,PeriodID){
    console.log(ToUid);
    console.log(PeriodID);
    $.ajax({
        url: getOutPoll,
        data: { SendToUID: ToUid, PeriodID: PeriodID },
        type: "POST",
        dataType: "json",
        success: function (data) {
            //console.log(data);
            var total_amount = 0;
            OutTable.clear();
            $.each(data, function (index, value) {
                //total_amount += parseInt(value.Amount);
                //value.Username = value.Username + " - " + value.Name;
                OutTable.row.add(value);
            });
            OutTable.draw();
            //$('#total_amount').html(nwc(total_amount));
            //initialPoll2();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });
}

var OutTable = $('.out-poll-data-table').DataTable({
    "lengthMenu": [[10, 20, 50, -1], [10, 20, 50, "All"]],
    "paging": true,
    "lengthChange": true,
    "searching": true,
    "ordering": true,
    "info": true,
    "autoWidth": true,
    "data": [],
    "columns": [
        {
            "title": "จำนวนโพย",
            "data": "Username"
        },
        {
            "title": "ยอดแทง",
            "data": "poll_count"
        },
        {
            "title": "ยอดถูก",
            "data": "Amount"
        },
        {
            "title": "สถานะโพย",
            "data": "Amount"
        },
        {
            "title": "วิธีส่ง",
            "data": "Amount"
        },
        {
            "title": "",
            "data": null,
            "render": function (data, type, row, meta) {
                return '<button type="button" class="btn btn-sm btn-warning showPoll2" ' +
                    'data-uid="' + row.UID + '" ' +
                    'data-number="' + row.Number + '" ' +
                    'data-type="' + row.Type + '" ' +
                    'data-nlen="' + row.nLen + '" ' +
                    'data-toggle="modal" data-target="#checkModal"  data-backdrop="static" data-keyboard="false">ดูโพย</button';
            }
        }
    ]
});