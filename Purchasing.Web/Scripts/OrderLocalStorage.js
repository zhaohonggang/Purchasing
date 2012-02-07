﻿///<reference path="jquery-1.6.2.js"/>
///<reference path="Order.js"/>

//Self-Executing Anonymous Function
//Adding New Functionality to Purchasing for Edit
(function (purchasing, $, undefined) {
    //Public Method
    purchasing.initLocalStorage = function () {
        if (window.Modernizr.localstorage) {
            attachUserTrackingEvents();
            attachFormSerializationEvents();
        }
    };

    function attachFormSerializationEvents() {
        purchasing.storeOrderForm = function () {
            localStorage.setItem('orderform', $("#order-form").serialize());
            
            localStorage.setItem('orderform-splittype', $("#splitType").val());

            //Store line item count and split count
            localStorage["orderform-lineitems"] = $(".line-item-row").length;
            //localStorage["orderform-linesplits"] = $(".sub-line-item-split-row").length;
            localStorage["orderform-ordersplits"] = $(".order-split-line").length;
            
            console.log("Stored", localStorage["orderform"]);
        };

        purchasing.loadOrderForm = function () {
            //Load more line items if we have > 3
            for (var j = 0; j < localStorage["orderform-lineitems"] - 3; j++) {
                $("#add-line-item").trigger('createline');
            }

            //Now we need to create the correct split, if needed
            var splitType = localStorage['orderform-splittype'];

            if (splitType === 'Order') {
                $("#split-order").trigger('click', { automate: true });

                //Load more order splits if we have > 3
                for (var i = 0; i < localStorage["orderform-ordersplits"] - 3; i++) {
                    $("#add-order-split").trigger('createsplit');
                }
            } else if (splitType === 'Line') {
                $("#split-by-line").trigger('click', { automate: true });
                
                //TODO: We need to find out splits for each line, might be a bit tricky
            }
            
            var data = localStorage["orderform"];
            $("#order-form").unserializeForm(data);
        };
    }

    function attachUserTrackingEvents() {
        checkFirstTime();

        $("#clear-user-history").live('click', function (e) {
            e.preventDefault();
            var usertoken = "user-" + $("#userid").html();
            localStorage.removeItem(usertoken);
            alert("cleared!  Refresh the page to appear like a first timer");
        });

        function checkFirstTime() {
            var usertoken = "user-" + $("#userid").html();
            var message;

            if (localStorage.getItem(usertoken) === null) {
                message = "it's your first time here";
                localStorage[usertoken] = 1;
            } else {
                message = "you've been here before " + localStorage[usertoken]++ + " times";
            }

            var statusMessage = $("<div id='status-message'>" + message + "<a id='clear-user-history' href='#' style='float:right'>Clear History</a></div>");
            $(".main > header").prepend(statusMessage);
        }
    }

} (window.purchasing = window.purchasing || {}, jQuery));