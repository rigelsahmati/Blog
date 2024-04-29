document.querySelectorAll('.like-btn').forEach((btn) => {
    $(btn).on('click', function () {
        $.ajax({
            type: 'GET',
            url: "/Home/Like",
            dataType: "json",
            data: { userId: btn.dataset.userid, postId: parseInt(btn.dataset.postid) },
            success: function (result) {
                handleLikeResponse(result, btn.dataset.postid)
            }
        });
    })
});

function handleLikeResponse(data, postId) {
    if (data == null) { return; }
    document.getElementById(`likeCount-${postId}`).innerHTML =data.likes;
    document.getElementById(`likeBtn-${postId}`).classList.toggle("like-btn-active");  
}

