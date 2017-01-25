$(function () {
    var navItems = $('.admin-menu li > a');
    var navListItems = $('.admin-menu li');
    var allWells = $('.admin-content');
    var allWellsExceptFirst = $('.admin-content:not(:first)');

    allWellsExceptFirst.hide();
    navItems.click(function (e) {
        e.preventDefault();
        navListItems.removeClass('active');
        $(this).closest('li').addClass('active');

        allWells.hide();
        var target = $(this).attr('data-target-id');
        $('#' + target).show();
    });

    $('form[action="/Manage/RemoveLogin"]').submit(function (event) {
        var frm = $(this);
        $.ajax({
            type: frm.attr('method'),
            url: frm.attr('action'),
            data: frm.serialize(),
            success: function (data) {
                alert('ok');
            }
        });
        event.preventDefault();
    });
    $('form[action="/Manage/ChangePassword"]').submit(function (event) {
        var frm = $(this);
        if (frm.valid()){
            $.ajax({
                type: frm.attr('method'),
                url: frm.attr('action'),
                data: frm.serialize(),
                success: function (data) {
                    frm.trigger("reset");
                }
            });
        }
        event.preventDefault();
    });
});