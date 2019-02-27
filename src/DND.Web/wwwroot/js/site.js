$(function () {
    $('#search-form').submit(function () {
        if ($("#s").val().trim())
            return true;
        return false;
    });
});

function stripHashtags(postText)
{
    var regexp = new RegExp('#([^\\s]*)', 'g');
    postText = postText.replace(regexp, '');
    return postText;
}

$('.gallery').each(function () { // the containers for all your galleries
    $(this).magnificPopup({
        delegate: 'a.gallery-image', // the selector for gallery item
        type: 'image',
        gallery: {
            enabled: true
        },
        image: {
            cursor: null,
            titleSrc: 'title'
        }
    });
});

$(window).scroll(function () {
    if ($(".navbar").offset()) {
        if ($(".navbar").offset().top > 50) {
            $(".scrolling-navbar").addClass("top-nav-collapse");
        } else {
            $(".scrolling-navbar").removeClass("top-nav-collapse");
        }
    }
});