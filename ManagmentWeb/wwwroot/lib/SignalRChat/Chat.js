const connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalRChatHub")
    .build();
var userId = userId = document.getElementById("UserId").value;
var $div = ""

//This method receive the message and Append to our list
connection.on("OnConnectedAsync", (user, message) => {
    alert()
    debugger
});
connection.on("ReceiveMessage", (user, message) => {
    const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    const encodedMsg = user + " :: " + msg;
    const li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});
connection.on("GetAllUsers", (user) => {
    var allUsers = JSON.parse(user)
       for (i = 0; i < allUsers.length; i++) {
        debugger;
        AddUser(chatHub, allUsers[i].sConnectionId, allUsers[i].DisplayName, allUsers[i].eStatus, allUsers[i].lUserID, allUsers[i].UnReadMessage, allUsers[i].sImagePath);
    }
});
connection.start(function () {
    alert()
}).catch(err => console.error(err.toString()));

//Send the message

document.getElementById("sendMessage").addEventListener("click", event => {
    const user = document.getElementById("userName").value;
    const message = document.getElementById("userMessage").value;
    connection.invoke("SendPrivateMessage", userId, message).catch(err => console.error(err.toString()));
    event.preventDefault();
});

// Send Button event
document.getElementById("sendMessage").addEventListener("click", event => {
    $textBox = $div.find("#userMessage");
    debugger;
    var msg = $textBox.val();
    if (msg.length > 0 && msg != "") {
        connection.invoke("sendPrivateMessage", userId, msg).catch(err => console.error(err.toString()));
        $div.find("#userMessage").val('');
        event.preventDefault();
    }
});
function OpenPrivateChatWindow(chatHub, id, userName, userEmail, email) {
    var ctrId = 'private_' + id;
    if ($('#' + ctrId).length > 0) return;

    createPrivateChatWindow(chatHub, id, ctrId, userName, userEmail, email, false);

    connection.on("getPrivateMessage", userEmail, email, loadMesgCount,(msg) => {

        var divclas = '';
        var spanclas = '';
        debugger
        for (i = 0; i < msg.length; i++) {
            //divclas = 'class="text-left message "';
            //spanclas = 'class="bg-dark "';
            //if (msg[i].IsMe) {
            //    divclas = 'class="text-right message "';
            //    spanclas = 'class="bg-info "';
            //}
            //<span class="userName">' + msg[i].userName + '</span>:
            $('#' + ctrId).find('#divMessage').append('<div style="border: 1px solid #eee;" title=' + msg[i].Date + '><span ><b> ' + msg[i].userName + ':</b></span><br><span > ' + msg[i].message + '</span></div>');
            // set scrollbar
            scrollTop(ctrId);
        }
    });
}

function createPrivateChatWindow(chatHub, userId, ctrId, userName, userEmail, email, IsAlreadyOpen) {
    debugger
    var winclas = 'class_' + email + ''
    if (IsAlreadyOpen) {
        winclas = 'class_' + email + ''
        if ($('.' + winclas + '').length > 0) {
            $('.' + winclas + '').remove();
        }
        winclas = 'class_' + userEmail + ''
        if ($('.' + winclas + '').length > 0) {
            $('.' + winclas + '').remove();
        }
    }
    else {
        if ($('.' + winclas + '').length > 0) {
            $('.' + winclas + '').remove();
        }
    }


    $('#unread' + ctrId).html('');
    var div = '<div id="' + ctrId + '" class="ui-widget-content draggable col-md-5 ' + winclas + '" rel="0">' +
        '<div class="header">' +
        '<div  style="float:right;">' +
        '<img id="imgDelete"  style="cursor:pointer;height: 15px;padding-bottom: 2px; width: 15px;" src="../Content/images/delete.png"/>' +
        '</div>' +

        '<span class="selText" rel="0">' + userName + '</span>' +
        '<span class="selText" id="msgTypeingName" rel="0">...</span>' +
        '</div>' +
        '<div id="divMessage" class="messageArea" style="overflow-y: scroll;">' +

        '</div>' +
        '<div class="buttonBar">' +
        ' <div class="row justify-content-center">' +
        '  <div class="col-md-8">' +
        //  ' <p class="lead emoji-picker-container" >' +
        //data-emojiable="true" hasFocus="true" data-emoji-input="unicode"
        '<input type="text"   class="form-control textarea-control privatetxtbox"  id="txtPrivateMessage" ></input>' +
        //'</p>' +
        '</div>' +
        '  <div class="col-md-4">' +
        '<input id="btnSendMessage" class="submitButton button private" type="button" value="Send"   />' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div id="scrollLength"></div>' +
        '</div>';

    var $div = $(div);
    debugger;
    // ------------------------------------------------------------------ Scroll Load Data ----------------------------------------------------------------------//

    var scrollLength = 2;
    $div.find('.messageArea').scroll(function () {
        debugger
        if ($(this).scrollTop() == 0) {
            if ($('#' + ctrId).find('#scrollLength').val() != '') {
                var c = parseInt($('#' + ctrId).find('#scrollLength').val(), 10);
                scrollLength = c + 1;
            }
            $('#' + ctrId).find('#scrollLength').val(scrollLength);
            var count = $('#' + ctrId).find('#scrollLength').val();

            chatHub.server.getScrollingChatData(userEmail, email, loadMesgCount, count).done(function (msg) {
                for (i = 0; i < msg.length; i++) {
                    var firstMsg = $('#' + ctrId).find('#divMessage').find('.message:first');

                    // Where the page is currently:
                    var curOffset = firstMsg.offset().top - $('#' + ctrId).find('#divMessage').scrollTop();
                    var divclas = 'class="text-left message "';
                    var spanclas = 'class="bg-dark "';
                    if (msg[i].IsMe) {
                        divclas = 'class="text-right message "';
                        spanclas = 'class="bg-info "';
                    }
                    // Prepend
                    $('#' + ctrId).find('#divMessage').prepend('<div ><span ><b>' + msg[i].userName + ':<b></span><br><span >' + msg[i].message + '</span></div>');

                    // Offset to previous first message minus original offset/scroll
                    $('#' + ctrId).find('#divMessage').scrollTop(firstMsg.offset().top - curOffset);
                }
            });
        }
    });

    // DELETE BUTTON IMAGE
    $div.find('#imgDelete').click(function () {
        $('#' + ctrId).remove();
    });

    // Send Button event
    $div.find("#btnSendMessage").click(function () {
        $textBox = $div.find("#txtPrivateMessage");
        debugger;
        var msg = $textBox.val();
        if (msg.length > 0 && msg != "") {
            chatHub.server.sendPrivateMessage(userId, msg, 'Click');
            $div.find("#txtPrivateMessage").val('');

        }
    });


    // Text Box event
    $div.find("#txtPrivateMessage").keyup(function (e) {
        debugger;
        if (e.which == 13 && !e.shiftKey) {
            $div.find("#btnSendMessage").click();
        }

        // Typing
        $textBox = $div.find("#txtPrivateMessage");
        var msg = $textBox.val();
        if (msg.length > 0 && msg != "") {
            chatHub.server.sendPrivateMessage(userId, msg, 'Type');
        }
        else {
            chatHub.server.sendPrivateMessage(userId, msg, 'Empty');
        }

        clearInterval(refreshId);
        checkTyping(chatHub, userId, msg, $div, 5000);
    });

    AddDivToContainer($div);
    test();
}
// Add User
function AddUser(chatHub, id, name, status, email, unreadmessage, sImagePath) {
    debugger;
    var badgecss = "badge";
    if (parseInt(unreadmessage) == 0) {
        unreadmessage = '';
        badgecss = '';
    }
    var subname = ''
    subname = name;
    if (name.length > 17) {
        subname = name.substr(0, 9) + '...' + '-' + name.split('-')[1];

    }
    var userId = $('#hdId').val();
    var userEmail = $("#hdEmailID").val();
    var code = "";
    var imgpath = '';
    imgpath = "../UserImages/Images/" + sImagePath;
    if (sImagePath == '') {
        imgpath = "../UserImages/Images/noPhotoAvailable.jpg";
    }
    if (userEmail != email && $('.loginUser').length == 0) {

        var statusimg = "";
        if (status == 1) {
            statusimg = '<img id="userstatusimg_' + email + '"  src="../Content/images/onstatus.png" style="width:15px;height:15px ;float:right"/>';
        }
        else if (status == null || status == 0) {
            statusimg = '<img id="userstatusimg_' + email + '" src="../Content/images/offline-icon.png" style="width:15px;height:15px;float:right"/>';
        }
        else {
            statusimg = '<img id="userstatusimg_' + email + '" src="../Content/images/away-icon.png" style="width:15px;height:15px;float:right"/>';
        }
        var finalid = id;
        if (id == null) {
            finalid = id + '_' + email;

        }
        if ($("#userli_" + email + "").length > 0) {
            $("#userli_" + email + "").remove();
            code = $('<li class="list" id=userli_' + email + '><img id=userimg_' + email + ' src=' + imgpath + ' alt=' + email + ' class="img-responsive img-circle" style="    width: 10%;    height: 30px;display: inline-block;"/><a id="' + finalid + '" class="user" style="text-decoration:none;">' + subname + '<span id=unreadprivate_' + finalid + ' class=' + badgecss + ' style="position: relative;top: -10px;z-index: 0">&nbsp;&nbsp;&nbsp;' + unreadmessage + '</span><b id=status_' + id + '>&nbsp;&nbsp;&nbsp;' + statusimg + '</b><a></li>');
        }
        else {
            code = $('<li class="list" id=userli_' + email + '><img id=userimg_' + email + ' src=' + imgpath + ' alt=' + email + ' class="img-responsive img-circle" style="    width: 10%;    height: 30px;display: inline-block;"/><a id="' + finalid + '" class="user" style="text-decoration:none;">' + subname + '<span id=unreadprivate_' + finalid + ' class=' + badgecss + ' style="position: relative;top: -10px;z-index: 0">&nbsp;&nbsp;&nbsp;' + unreadmessage + '</span><b id=status_' + id + '>&nbsp;&nbsp;&nbsp;' + statusimg + '</b><a></li>');
        }

        $(code).click(function () {
            debugger
            var id = $(this).find("a").attr('id');
            if (userEmail != email) {

                // $('.draggable').remove();
                OpenPrivateChatWindow(chatHub, id, name, userEmail, email);
            }
        });
    }

    $("#divusers").append(code);
}