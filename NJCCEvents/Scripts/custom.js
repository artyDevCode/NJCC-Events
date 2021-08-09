$(document).ready(function () {
    $(function () {

        var options = {
            "appIconUrl": '../Images/njccEventPic.JPG',
            //"appIconUrl": 'https://dev-sccm/njcc/Images/njccEventPic.JPG',
            "appTitle": "NJCC Events",
            "appHelpPageUrl": "Help.html?"
                + document.URL.split("?")[1],
            "settingsLinks": [
                {
                    "linkUrl": "Account.html?"
                        + document.URL.split("?")[1],
                    "displayName": "Account settings"
                },
                {
                    "linkUrl": "Contact.html?"
                        + document.URL.split("?")[1],
                    "displayName": "Contact us"
                }
            ]
        };


        var nav = new SP.UI.Controls.Navigation("chrome_ctrl_container", options);
        nav.setVisible(true);
    });

    $(".datepicker").datepicker({
        changeMonth: true,
        changeYear: true,
        showOtherMonths: true,
        selectOtherMonths: true,
        //dateFormat: 'DD, d MM, yy',
        altField: '#date_due',
        altFormat: 'yy-mm-dd',
        firstDay: 1, // rows starts on Monday
        dateFormat: "dd-mm-yy",
        timeFormat: "hh:mm tt"       
    }).change(function () {
        if (this.id == 'EndDate') {
            if ($('#EndDate').val() <= $('#StartDate').val()) {
                var endValue1 = $('#EndDate').val();
                $('#StartDate').val(endValue1);
            }         
            checkEndTime();        
            var endValue1 = $(this).val() + ' ' + $('#EndTime').val();;
            $('#EndDateTime').val(endValue1);
        }
        else
            if (this.id == 'StartDate') {
                if ($('#EndDate').val() <= $('#StartDate').val()) {
                    var startValue1 = $('#StartDate').val();
                    $('#EndDate').val(startValue1);
            }                
                checkStartTime();
                var startValue1 = $(this).val() + ' ' + $('#StartTime').val();
                $('#StartDateTime').val(startValue1);
        }
    });


  

    function checkEndTime() {
        //   alert( $('#EndTime').val() + " -- " + $('#StartTime').val());
        if ($('#EndDate').val() == $('#StartDate').val()) {
            var from = Date.parse($('#StartDate').val() + ' ' + $('#StartTime').val());
            var to = Date.parse($('#EndDate').val() + " " + $('#EndTime').val());
            if (from > to) {    // if ($('#StartTime').val() > $('#EndTime').val()) {         
                var endValue = $('#EndTime').val();             
                $('#StartTime').val(endValue);
                $('#StartTimeDisp').text(endValue);
            }
        }        
    };

    function checkStartTime() {
        if ($('#EndDate').val() == $('#StartDate').val()) {
            var from = Date.parse($('#StartDate').val() + ' ' + $('#StartTime').val());
            var to = Date.parse($('#EndDate').val() + " " + $('#EndTime').val());
           // alert($('#StartDate').val() + ' ' + $('#StartTime').val() + " -- " + $('#EndDate').val() + " " + $('#EndTime').val());
            if (from > to) {    //if ($('#StartTime').val() > $('#EndTime').val()) {
               // alert(from + " > " + to);        
                var startValue = $('#StartTime').val() ;
                $('#EndTime').val(startValue);
                $('#EndTimeDisp').text(startValue);
            }
        }
    };


    $('#EndTime, #StartTime ').timepicker()
        .change(function () {
            if (this.id == 'StartTime') {
                checkStartTime();             
                    $('#StartTimeDisp').text($(this).val());
                    var startValue = $('#StartDate').val() + ' ' + $(this).val();
                    $('#StartDateTime').val(startValue);           
            }
            else{
                checkEndTime();           
                        $('#EndTimeDisp').text($(this).val());
                        var endValue = $('#EndDate').val() + ' ' + $(this).val();
                        $('#EndDateTime').val(endValue);               
                }
        });




    /* hide initially */
    if ($("#EventType").val() == "NJCC") {
        $("#Organisation").hide();
        $("#OrganisationTitle").hide();
    }

    $("#EventType").change(function () {
        if ($(this).val() == "Community") {
            $("#Organisation").show();
            $("#OrganisationTitle").show();
        }
        else {
            $("#Organisation").val("");
            $("#Organisation").hide();
            $("#OrganisationTitle").hide();
        }
    });

    $("#EventName").change(function () {
        $('#EventTimeDisp').text($(this).val() + " ----- " + $('#Aim').val());
    });

    $("#Aim").change(function () {
        $('#EventTimeDisp').text($('#EventName').val() + " ----- " + $(this).val());
    });


    var result = $('#Attachments').data('njcc-read');
    if (result == 'True')
        var res = true;
    else
        var res = false;
    // alert(result);
    tinymce.init(
    {
        selector: "textarea",
        readonly: res,
        theme: "modern",
        plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars code fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor"
        ],
        toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
        toolbar2: "print preview media | forecolor backcolor emoticons",
        image_advtab: true
        //templates: [
        //    { title: 'Test template 1', content: 'Test 1' },
        //    { title: 'Test template 2', content: 'Test 2' }
        //]

    });


    $('#accessgroups').dataTable
        ({
            "bLengthChange": false,
            "bPaginate": true,
            "sScrollY": 400,
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",

        })
            .rowGrouping({
                iGroupingColumnIndex: 0,
                iGroupingOrderByColumnIndex: 0,
                //iGroupingColumnIndex2: 1,
                iGroupingOrderByColumnIndex: 0,
                bExpandableGrouping: true,
                iExpandGroupOffset: 1,
                bExpandableGrouping2: true,
                iExpandGroupOffset: 2,
            });

    $("#UsersDataList").click(function () {

        $.getJSON($('#UserName').attr("data-autocompleteme"), function (data) {
            var items;
            $.each(data, function (i, alrs) {
                items += "<option>" + data[i] + "</option>";
            });
            $('#UserName').html(items);
        });

    });
    $("#StaffContactDataList").click(function () {

        $.getJSON($('#StaffContact').attr("data-autocompleteme"), function (data) {
            var items;
            $.each(data, function (i, alrs) {
                items += "<option>" + data[i] + "</option>";
            });
            $('#StaffContact').html(items);
        });

    });

});