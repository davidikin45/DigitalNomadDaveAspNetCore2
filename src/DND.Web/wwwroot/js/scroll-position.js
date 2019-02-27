$(document).scroll(function () {
    localStorage['page'] = document.URL;
    localStorage['scrollTop'] = $(document).scrollTop();
});

$(document).ready(function () {
    if (localStorage['page'] == document.URL) {
        $(document).scrollTop(localStorage['scrollTop']);
    }
});