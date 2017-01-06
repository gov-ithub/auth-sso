$(function () {

    //$('#login-form-link').click(function (e) {
    //    $("#login-form").delay(100).fadeIn(100);
    //    $("#register-form").fadeOut(100);
    //    $('#register-form-link').removeClass('active');
    //    $(this).addClass('active');
    //    e.preventDefault();
    //});
    //$('#register-form-link').click(function (e) {
    //    $("#register-form").delay(100).fadeIn(100);
    //    $("#login-form").fadeOut(100);
    //    $('#login-form-link').removeClass('active');
    //    $(this).addClass('active');
    //    e.preventDefault();
    //});

    $("#login-form").submit(function (event) {
        event.preventDefault();
        var form = $(this);
        if (form.valid()) {
            $('.form-group').removeClass('has-error'); // remove the error class
            $('.help-block').remove(); // remove the error text

            $.ajax({
                type: 'POST',
                url: form.attr('action'),
                data: form.serialize(),
                dataType: 'json',
                encode: true
            }).done(function (data) {
                if (data.res) {
                    if (data.returnUrl) {
                        window.location = data.returnUrl;
                    }
                    else {
                        window.location = "/";
                    }
                }
                else {
                    alert(data.msg);
                }
            });
        }
    });

    $("#register-form").submit(function (event) {
        event.preventDefault();
        var form = $(this);

        if (form.valid()) {
            $('.form-group').removeClass('has-error'); // remove the error class
            $('.help-block').remove(); // remove the error text

            $.ajax({
                type: 'POST',
                url: form.attr('action'),
                data: form.serialize(),
                dataType: 'json',
                encode: true
            }).done(function (data) {
                if (data.res) {
                    if (data.returnUrl) {
                        window.location = data.returnUrl;
                    }
                    else {
                        window.location = "/";
                    }
                }
                else {
                    alert(data.msg);
                }
            });
        }
    });
});
