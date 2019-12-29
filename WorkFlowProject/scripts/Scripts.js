$(document).ready(function () {

    $('#MeetingTime').timepicker({
        uiLibrary: 'bootstrap4'
    });

    var readURL = function (input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('.profile-pic').attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }


    $(".file-upload").on('change', function () {
        readURL(this);
    });

    $(".upload-button").on('click', function () {
        $(".file-upload").click();
    });
});


$(function () {
    $("#loaderbody").addClass('hide');


    $(document).bind('ajaxStart', function () {
        $("#loaderbody").removeClass('hide');
    }).bind('ajaxStop', function () {
        $("#loaderbody").addClass('hide');
    });
});


function ShowImagePreview(imageUploader, previewImage) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewImage).attr('src', e.target.result);
        }
        reader.readAsDataURL(imageUploader.files[0]);
    }
}

function jQueryAjaxPost(form) {
    $.validator.unobtrusive.parse(form);
    if ($(form).valid()) {
        var ajaxConfig = {
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            success: function (response) {
                if (response.success) {
                    $("#firstTab").html(response.html);
                    refreshAddNewTab($(form).attr('data-restUrl'), true);
                    $.notify(response.message, "success");
                    if (typeof activatejQueryTable !== 'undefined' && $.isFunction(activatejQueryTable))
                        activatejQueryTable();
                }
                else {
                    $.notify(response.message, "error");
                }
            }
        }
        if ($(form).attr('enctype') == "multipart/form-data") {
            ajaxConfig["contentType"] = false;
            ajaxConfig["processData"] = false;
        }
        $.ajax(ajaxConfig);

    }
    return false;
}

function refreshAddNewTab(resetUrl, showViewTab) {
    $.ajax({
        type: 'GET',
        url: resetUrl,
        success: function (response) {
            $("#secondTab").html(response);
            $('ul.nav.nav-tabs a:eq(1)').html('Add New');
            if (showViewTab)
                $('ul.nav.nav-tabs a:eq(0)').tab('show');
        }

    });
}

function Edit(url) {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (response) {
            $("#secondTab").html(response);
            $('ul.nav.nav-tabs a:eq(1)').html('Edit');
            $('ul.nav.nav-tabs a:eq(1)').tab('show');
        }

    });
}

function Delete(url) {
    if (confirm('Are you sure to delete this record ?') == true)
    {
        $.ajax({
            type: 'POST',
            url: url,
            success: function (response) {
                if (response.success) {
                    $("#firstTab").html(response.html);
                    $.notify(response.message, "warn");
                    if (typeof activatejQueryTable !== 'undefined' && $.isFunction(activatejQueryTable))
                        activatejQueryTable();
                }
                else {
                    $.notify(response.message, "error");
                }
            }

        });

    }
}

   function AddPost(id) {
        var Post = {
            TopicId: $("#TopicId").val(),
            PostContent: $("#" + id).text(),
        }
        if ($("#TopicId").val() != "" && $("#" + id).text() != "")
        {
            $("#ShowErrorTopic").text("");
            $("#ShowErrorTopic").css("color", "red");
            $("#ShowErrorTopic").css("margin-left", "10px");

            $("#ShowErrorContent").text("");
            $("#ShowErrorContent").css("color", "red");
            $("#ShowErrorContent").css("margin-left", "10px");
            $.ajax({

                type: "POST",
                url: "/Forum/AddPost",
                data: JSON.stringify(Post),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    window.location = '/Forum/ViewPost';
                },
                error: function (response) {

                }



            });
        }
        else {
            if ($("#TopicId").val() == "" && $("#" + id).text() == "")
            {
                $("#ShowErrorTopic").text("Topic is required");
                $("#ShowErrorTopic").css("color", "red");
                $("#ShowErrorTopic").css("margin-left", "10px");

                $("#ShowErrorContent").text("Post Content is required");
                $("#ShowErrorContent").css("color", "red");
                $("#ShowErrorContent").css("margin-left", "10px");
            }
            else if($("#TopicId").val() == "" && $("#" + id).text() != "")
            {
                $("#ShowErrorTopic").text("Topic is required");
                $("#ShowErrorTopic").css("color", "red");
                $("#ShowErrorTopic").css("margin-left", "10px");

                $("#ShowErrorContent").text("");
                $("#ShowErrorContent").css("color", "red");
                $("#ShowErrorContent").css("margin-left", "10px");
            }
            else if ($("#TopicId").val() != "" && $("#" + id).text() == "") {
                $("#ShowErrorTopic").text("");
                $("#ShowErrorTopic").css("color", "red");
                $("#ShowErrorTopic").css("margin-left", "10px");

                $("#ShowErrorContent").text("Post Content is required");
                $("#ShowErrorContent").css("color", "red");
                $("#ShowErrorContent").css("margin-left", "10px");
            }

        }
        }
       

   function AddMeeting(id) {
      
       if ($("#" + id).text() != "" && $("#TopicId").val() != "" && $("#UserId").val() != "" && $("#MeetingTime").val() != "" && $("#" + id).text() != "")
       {
           $("#ShowErrorTopic").text("");
           $("#ShowErrorTopic").css("color", "red");
           $("#ShowErrorTopic").css("margin-left", "10px");

           $("#ShowErrorUser").text("");
           $("#ShowErrorUser").css("color", "red");
           $("#ShowErrorUser").css("margin-left", "10px");

           $("#ShowErrorTime").text("");
           $("#ShowErrorTime").css("color", "red");
           $("#ShowErrorTime").css("margin-left", "10px");

           $("#ShowErrorPoints").text("");
           $("#ShowErrorPoints").css("color", "red");
           $("#ShowErrorPoints").css("margin-left", "10px");

           var Meeting = {
               MetingPoints: $("#" + id).text(),
               TopicId: $("#TopicId").val(),
               UserId: $("#UserId").val(),
               MeetingTime: $("#MeetingTime").val(),
               MeetingPoints: $("#" + id).text(),
           }

           $.ajax({

               type: "POST",
               url: "/Admin/CreateMeeting",
               data: JSON.stringify(Meeting),
               contentType: "application/json; charset=utf-8",
               dataType: "json",
               success: function (response) {
                   var SucessMessage = "<div class='alert alert-success alert-dismissible fade show mb-0' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><i class='fa fa-check mx-2'></i> <strong>Success!</strong> You created meeting successfully!</div>";
                   $("#MessageSucess").html(SucessMessage);
                   $('#editor-container').text("");
                   $('#MeetingTime').val("");
                   $('#TopicId').val("");
                   $('#UserId').val("");
                  // window.location = '/Admin/CreateMeeting';
               },
               error: function (response) {

               }



           });
       }
       else
       {
           if ($("#" + id).text() == "" && $("#TopicId").val() == "" && $("#UserId").val() == "" && $("#MeetingTime").val() == "" && $("#" + id).text() == "") {
               $("#ShowErrorTopic").text("Please Select Topic");
               $("#ShowErrorTopic").css("color", "red");
               $("#ShowErrorTopic").css("margin-left", "10px");

               $("#ShowErrorUser").text("Please Select Faculty member");
               $("#ShowErrorUser").css("color", "red");
               $("#ShowErrorUser").css("margin-left", "10px");

               $("#ShowErrorTime").text("Meeting Time is required");
               $("#ShowErrorTime").css("color", "red");
               $("#ShowErrorTime").css("margin-left", "10px");

               $("#ShowErrorPoints").text("Meeting points are required");
               $("#ShowErrorPoints").css("color", "red");
               $("#ShowErrorPoints").css("margin-left", "10px");

           }
           else if ($("#" + id).text() == "" && $("#TopicId").val() != "" && $("#UserId").val() == "" && $("#MeetingTime").val() == "" && $("#" + id).text() == "") {
               $("#ShowErrorTopic").text("");
               $("#ShowErrorTopic").css("color", "red");
               $("#ShowErrorTopic").css("margin-left", "10px");

               $("#ShowErrorUser").text("Please Select Faculty member");
               $("#ShowErrorUser").css("color", "red");
               $("#ShowErrorUser").css("margin-left", "10px");

               $("#ShowErrorTime").text("Meeting Time is required");
               $("#ShowErrorTime").css("color", "red");
               $("#ShowErrorTime").css("margin-left", "10px");

               $("#ShowErrorPoints").text("Meeting points are required");
               $("#ShowErrorPoints").css("color", "red");
               $("#ShowErrorPoints").css("margin-left", "10px");

           }
           else if ($("#" + id).text() == "" && $("#TopicId").val() != "" && $("#UserId").val() != "" && $("#MeetingTime").val() == "" && $("#" + id).text() == "") {
               $("#ShowErrorTopic").text("");
               $("#ShowErrorTopic").css("color", "red");
               $("#ShowErrorTopic").css("margin-left", "10px");

               $("#ShowErrorUser").text("");
               $("#ShowErrorUser").css("color", "red");
               $("#ShowErrorUser").css("margin-left", "10px");

               $("#ShowErrorTime").text("Meeting Time is required");
               $("#ShowErrorTime").css("color", "red");
               $("#ShowErrorTime").css("margin-left", "10px");

               $("#ShowErrorPoints").text("Meeting points are required");
               $("#ShowErrorPoints").css("color", "red");
               $("#ShowErrorPoints").css("margin-left", "10px");

           }
           else if ($("#" + id).text() == "" && $("#TopicId").val() != "" && $("#UserId").val() != "" && $("#MeetingTime").val() != "" && $("#" + id).text() == "") {
               $("#ShowErrorTopic").text("");
               $("#ShowErrorTopic").css("color", "red");
               $("#ShowErrorTopic").css("margin-left", "10px");

               $("#ShowErrorUser").text("");
               $("#ShowErrorUser").css("color", "red");
               $("#ShowErrorUser").css("margin-left", "10px");

               $("#ShowErrorTime").text("");
               $("#ShowErrorTime").css("color", "red");
               $("#ShowErrorTime").css("margin-left", "10px");

               $("#ShowErrorPoints").text("Meeting points are required");
               $("#ShowErrorPoints").css("color", "red");
               $("#ShowErrorPoints").css("margin-left", "10px");

           }
           else if ($("#" + id).text() == "" && $("#TopicId").val() != "" && $("#UserId").val() != "" && $("#MeetingTime").val() != "" && $("#" + id).text() != "") {
               $("#ShowErrorTopic").text("");
               $("#ShowErrorTopic").css("color", "red");
               $("#ShowErrorTopic").css("margin-left", "10px");

               $("#ShowErrorUser").text("");
               $("#ShowErrorUser").css("color", "red");
               $("#ShowErrorUser").css("margin-left", "10px");

               $("#ShowErrorTime").text("");
               $("#ShowErrorTime").css("color", "red");
               $("#ShowErrorTime").css("margin-left", "10px");

               $("#ShowErrorPoints").text("");
               $("#ShowErrorPoints").css("color", "red");
               $("#ShowErrorPoints").css("margin-left", "10px");

           }
               

               
       }
       
   }



    function AddTopic(id) {
        
        var Topic = {
            TopicName: $("#" + id).val(),

        }
        if ($("#" + id).val() != "") {
            $.ajax({

                type: "POST",
                url: "/Forum/AddTopic",
                data: JSON.stringify(Topic),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    window.location = '/Forum/AddPost';
                },
                error: function (response) {

                }



            });
        }
        else {
            $("#ShowError").text("Topic is required");
            $("#ShowError").css("color", "red");
            $("#ShowError").css("margin-left", "10px");
        }

    }
    function HideElements() {
        var x = document.getElementById("HideMe");
        x.style.display = "none";

        $.ajax({

            type: "POST",
            url: "/Account/updateViewdNotification",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
               

            },
            error: function (response) {

            }



        });
       
    }

    function AddComment(id) {
            if ($("#" + id).val() != "") {
                var comment = {
                    PostId: id,
                    Description: $("#" + id).val(),
                }
                $.ajax({

                    type: "POST",
                    url: "/Forum/AddNewComment",
                    data: JSON.stringify(comment),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var commentData = "<small class='comment-item text-muted'><p class='mb-2'><strong><a href=''>" + response.mycomment.name + "</a></strong></p><p id='mycomment'>" + response.mycomment.commentContent + "</p></small><hr>";
                        $("#Comment" + id).append(commentData);
                    
                        $("#" + id).val("");
                        
                    },
                    error: function (response) {

                    }



                });
            }

        }

   



    function BLockComment(id) {

        var ComId = {
            CommentId: id
        }
        $.ajax({

            type: "POST",
            url: "/Home/BlockComments",
            data: JSON.stringify(ComId),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                window.location = '/Home/Index';
            },
            error: function (response) {
             ;
            }



        });
    }

    function PostLike(id) {

        var Like = {
            PostId: id,
        }
        $.ajax({

            type: "POST",
            url: "/Forum/LikePost",
            data: JSON.stringify(Like),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (resposnse) {
                console.log("#like" + id)
                $("#like" + id).text(resposnse.postLikeCount);
                $("#dislike" + id).text(resposnse.dislikecount);

             //   window.location = '/Forum/ViewPost';
            },
            error: function (response) {
               
            }



        });
    }
    function PostDisLike(id) {

        var Like = {
            PostId: id,
        }
        $.ajax({

            type: "POST",
            url: "/Forum/DislikePost",
            data: JSON.stringify(Like),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $("#like" + id).text(response.postLikeCount);
                $("#dislike" + id).text(response.dislikecount);

            },
            error: function (response) {
               
            }



        });
    }

    function DownloadFile(id) {

        var ProjectData = {
            FileID: id,
        }
    
        $.ajax({

            type: "POST",
            url: "/Home/Download",
            data: JSON.stringify(ProjectData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                alert("Success");
            },
            error: function (response) {
                alert("error");
            }



        });
    }


    function ChangePassword(newPass, repPass, oldPass) {
       
        if ($("#" + newPass).val() != "" && $("#" + repPass).val() != "" && $("#" + oldPass).val() != "")
        {
            $("#ShowErrorNewPa").text("");
            $("#ShowErrorNewPa").css("color", "red");
            $("#ShowErrorNewPa").css("margin-left", "10px");

            $("#ShowErrorRepeatPa").text("");
            $("#ShowErrorRepeatPa").css("color", "red");
            $("#ShowErrorRepeatPa").css("margin-left", "10px");


            $("#oldpa").text("");
            $("#oldpa").css("color", "red");
            $("#oldpa").css("margin-left", "10px");
            
            var ResetPassword = {
                NewPassword: $("#" + newPass).val(),
                ConformPassword: $("#" + repPass).val(),
                OldPassword: $("#" + oldPass).val()
            }

            if ($("#" + newPass).val() != $("#" + repPass).val()) {
                $("#ShowErrorRepeatPa").text("Password not matched");
                $("#ShowErrorRepeatPa").css("color", "red");
                $("#ShowErrorRepeatPa").css("margin-left", "10px");
            }
            else
            {
                $.ajax({

                    type: "POST",
                    url: "/Account/ChangePassword",
                    data: JSON.stringify(ResetPassword),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        window.location = "/Account/UserProfile";
                    },
                    error: function (response) {
                        $("#oldpa").text(response.message);
                        $("#oldpa").css("color", "red");
                        $("#oldpa").css("margin-left", "10px");
                    }



                });
            }
            
        }
        else
        {
            if ($("#" + newPass).val() == "" && $("#" + repPass).val() == "" && $("#" + oldPass).val() == "") {
                $("#ShowErrorNewPa").text("New password is required");
                $("#ShowErrorNewPa").css("color", "red");
                $("#ShowErrorNewPa").css("margin-left", "10px");

                $("#ShowErrorRepeatPa").text("Repeat password is required");
                $("#ShowErrorRepeatPa").css("color", "red");
                $("#ShowErrorRepeatPa").css("margin-left", "10px");


                $("#oldpa").text("old password is required");
                $("#oldpa").css("color", "red");
                $("#oldpa").css("margin-left", "10px");
            }
            else if ($("#" + repPass).val() == "") {
                $("#ShowErrorRepeatPa").text("Repeat password is required");
                $("#ShowErrorRepeatPa").css("color", "red");
                $("#ShowErrorRepeatPa").css("margin-left", "10px");
            }
            else if ($("#" + oldPass).val() == "") {
                $("#oldpa").style.display;
                $("#oldpa").text("old password is required");
                $("#oldpa").css("color", "red");
                $("#oldpa").css("margin-left", "10px");
            }
           
        }
        
    }

   

    function bs_input_file() {
        $(".input-file").before(
            function () {
                if (!$(this).prev().hasClass('input-ghost')) {
                    var element = $("<input type='file' class='input-ghost' style='visibility:hidden; height:0'>");
                    element.attr("name", $(this).attr("name"));
                    element.change(function () {
                        element.next(element).find('input').val((element.val()).split('\\').pop());
                    });
                    $(this).find("button.btn-choose").click(function () {
                        element.click();
                    });
                    $(this).find("button.btn-reset").click(function () {
                        element.val(null);
                        $(this).parents(".input-file").find('input').val('');
                    });
                    $(this).find('input').css("cursor", "pointer");
                    $(this).find('input').mousedown(function () {
                        $(this).parents('.input-file').prev().click();
                        return false;
                    });
                    return element;
                }
            }
        );
    }
    $(function () {
        bs_input_file();
    });