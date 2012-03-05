var uvOptions = {};

if (typeof console == "undefined") { //Define the console as a backup for browsers without console
    window.console = {
        log: function () { }
    };
}

$(function () { //run the page load startup code
    $('input[type=checkbox]').tzCheckbox({ labels: ['Yes', 'No'] });
    $(".dt-table, .datatable").dataTable({ "bJQueryUI": false, "sPaginationType": "full_numbers", "iDisplayLength": window.Configuration.DataTablesPageSize });
    $(".button, .text_btn, button").button();
    $('input[type="datetime"]').datepicker();
    $('input.datepicker').datepicker();

    initUservoice();
    konami(function () {
        $("#carty").show();
    });
});

function initUservoice() {
    var uv = document.createElement('script'); uv.type = 'text/javascript'; uv.async = true;
    uv.src = ('https:' == document.location.protocol ? 'https://' : 'http://') + 'widget.uservoice.com/39iNhpJPwlSkFDpX5Ajxw.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(uv, s);
}

function konami(callback) {
    var code = "38,38,40,40,37,39,37,39,66,65";
    var kkeys = [];

    $(window).on('keydown', function (e) {
        kkeys.push(e.keyCode);
        console.log(kkeys);
        while (kkeys.length > code.split(',').length) {
            kkeys.shift();
        }
        if (kkeys.toString().indexOf(code) >= 0) {
            $(this).unbind('keydown', arguments.callee);
            callback(e);
        }
    });
}