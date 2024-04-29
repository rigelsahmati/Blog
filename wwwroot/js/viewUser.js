$('.filterTime').datepicker({
    format: "dd/mm/yyyy",
    weekStart: 1,
    todayBtn: "linked",
    clearBtn: true,
    endDate: "+0d",
});
$("#deleeteBtnConfirm").on('click', function () {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $('#deleteBtn').click();
        }
    })
})
$('#clearFilters').on('click', function () {
    $.ajax({
        type: 'GET',
        url: "/Home/GetUserPosts",
        dataType: "json",
        data: { userId: $('#userId').val() },
        success: function (result) {
            handleResponse(result)
            $('.filterBtn').removeClass('filterBtn-active')
            $('#clearFilters').addClass('clearFilters-hidden')

        }
    });
})
$('#filterSubmit').on('click', function () {
    $.ajax({
        type: 'GET',
        url: "/Home/FilterUserPosts",
        dataType: "json",
        data: { userId: $('#userId').val(), categorieId: $('#filterCategorie').val(), fromTime: $('#filterTimeFrom').val(), toTime: $('#filterTimeTo').val() },
        success: function (result) {
            handleResponse(result)
        }
    });
})
function handleResponse(data) {
    if (data == null) { return; }
    document.querySelector('#userPosts-parent').innerHTML = '';
    if (data == null) {
        document.querySelector('#userPosts-parent').innerHTML = `<h4 class="text-center w-100 mt-5">No posts were found.</h4>`;
        filterBtnView();
        return;
    };
    data.forEach((post) => {
        var likeBtn;
        if (post.liked) {
            likeBtn =
                `<button id="likeBtn-${post.postId}" data-userId="${post.viewerId}" data-postId="${post.postId}" class="like-btn-active like-btn col btn ">
                   <i class="fa-solid fa-thumbs-up"></i>
                   Like
                </button>`
        }
        else {
            likeBtn =
                `<button id="likeBtn-${post.postId}" data-userId="${post.viewerId}" data-postId="${post.postId}" class="like-btn col btn ">
                   <i class="fa-solid fa-thumbs-up"></i>
                   Like
            </button>`
        }

        var postHtml = `<div class="post position-relative p-3 rounded-3 mt-2">
                        <div class="post-header d-flex">

                            <a href="/Blog/Profile/${post.userId}">
                                <img src="/uploads/img/usersImg/${post.profileImage}" width="40px" height="40px" class="postUserImg" />
                            </a>
                            <div class="ms-3 w-100 d-flex align-items-center justify-content-between">
                                <div>
                                    <a href="/Blog/Profile/${post.userId}">
                                        <p class="m-0 postUsername">${post.fullName}</p>
                                    </a>
                                    <p class="m-0 postDate">${post.createdTime}</p>
                                </div>
                                 <button data-actionsId="${post.postId}" class="actionsBtn border border-0 btn">
                                      <i class="fa-solid fa-ellipsis-vertical"></i>
                                 </button>
                                 <div id="actionsParent-${post.postId}" class="actionsParent rounded-3 p-2">
                                     <a href="/Blog/EditPost?postId=${post.postId}" class="btn text-start"><i class="fa-solid fa-pen me-2"></i> Edit Post</a>
                                     <button data-postId="${post.postId}" class="addToDraftsBtn btn text-start"><i class="fa-solid fa-envelope-open-text me-2"></i>Add To Drafts</button>
                                     <button data-postId="${post.postId}" class="deleteBtn btn text-start"><i class="fa-solid fa-trash me-2"></i>Delete Post</button>
                                </div>
                            </div>

                        </div>
                        <div class="postBody w-100 mt-2">
                           <div class="w-100 d-flex justify-content-center rounded-3" style="background:#222222;">
                                 <img src="/uploads/img/postImg/${post.imageUrl}" class="postImg "/>
                             </div>
                             <div class="postTitle mt-2 p-2">
                                <a href="/Blog/Post/${post.postId}" id="postUrlId-${post.postId}" class="w-100 m-0 fs-5">
                                    ${post.title}
                                </a>
                            </div>
                            <div class="likeCount mt-2 p-2 border-top d-flex justify-content-between">
                                <p class="m-0">
                                    <i class="fa-solid fa-thumbs-up"></i>
                                    <b id="likeCount-${post.postId}" class="m-0">${post.likes}</b>
                                </p>
                                <p class="m-0">
                                      <i class="fa-solid fa-comment"></i>
                                      ${post.coments}
                                </p>
                            </div>
                        </div>
                        <div class="postFooter w-100 btn-group mt-2 p-2 border-top rounded-0">`
            + likeBtn +
            `               <a href="/Blog/Post/${post.postId}#coment" class="col btn">
                                <i class="fa-solid fa-comment"></i>
                                Comment
                            </a>
                            <button data-urlId="${post.postId}" class="copyLinkBtn col btn shareBtn">
                                <i class="fa-solid fa-share"></i>
                                Share
                            </button>
                        </div>
                    </div>`;

        document.querySelector('#userPosts-parent').innerHTML += postHtml;
    })
    filterBtnView();
    AttachEvenListener()
}
function filterBtnView() {
    $('.filterBtn').addClass('filterBtn-active')
    $('#clearFilters').removeClass('clearFilters-hidden')
}
function AttachEvenListener() {

    document.querySelectorAll('.like-btn').forEach((btn) => {
        $(btn).on('click', function () {

            $.ajax({
                type: 'GET',
                url: "/Home/Like",
                dataType: "json",
                data: { userId: btn.dataset.userid, postId: parseInt(btn.dataset.postid) },
                success: function (result) {
                    console.log(result);
                    handleLikeResponse(result, btn.dataset.postid)
                }
            });
        })
    });


    const copyButtons = document.querySelectorAll(".copyLinkBtn");
    copyButtons.forEach((copyButton) => {
            copyButton.addEventListener("click", function () {
            const url = document.querySelector(`#postUrlId-${copyButton.dataset.urlid}`).href;
                copyToClipboard(url);

                // You can provide some visual feedback to the user, like a tooltip or a message
                copyButton.innerHTML = "Copied!";
                setTimeout(function () {
                    copyButton.innerHTML = `<i class="fa-solid fa-share"></i> Share`;
                }, 1000); // Reset button text after 2 seconds
            });
        });
    function copyToClipboard(text) {
        const textarea = document.createElement("textarea");
        textarea.value = text;
        document.body.appendChild(textarea);
        textarea.select();
        document.execCommand("copy");
        document.body.removeChild(textarea);
    }



    var buttons = document.querySelectorAll('.actionsBtn');
    buttons.forEach((button) => {
        button.addEventListener('click', function (event) {
            var actionsDiv = document.querySelector(`#actionsParent-${button.dataset.actionsid}`);
            actionsDiv.classList.add("actionsParent-active");

            document.addEventListener("click", function (event) {
                if (!actionsDiv.contains(event.target) && !button.contains(event.target)) {
                    actionsDiv.classList.remove('actionsParent-active')
                };
            });

        })
    })

    var deleteBtn = document.querySelectorAll('.deleteBtn');
    deleteBtn.forEach((btn) => {
        btn.addEventListener('click', function () {
            Swal.fire({
                title: 'Are you sure?',
                text: "You won't be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        type: 'GET',
                        url: "/Blog/DeletePostApi",
                        dataType: "json",
                        data: { ownerId: $('.userId').val(), postId: btn.dataset.postid },
                        success: function (result) {
                            handleDeleteResponse(result)
                            var post = document.querySelector(`.post-${btn.dataset.postid}`)
                            post.style.display = "none";
                            // post.remove();
                        }
                    });
                }
            })
        })
    })

    function handleDeleteResponse(data) {
        if (data == null) {
            return;
        }
        if (data.success) {
            new Notify({
                status: 'success',
                title: 'Sucess',
                text: 'Post deleted successfully.',
                effect: 'fade',
                speed: 300,
                customClass: '',
                customIcon: '',
                showIcon: true,
                showCloseButton: true,
                autoclose: true,
                autotimeout: 3000,
                gap: 20,
                distance: 20,
                type: 1,
                position: 'right top'
            })
            return;
        }
        new Notify({
            status: 'error',
            title: 'Error',
            text: 'Something went wrong,please try again.',
            effect: 'fade',
            speed: 300,
            customClass: '',
            customIcon: '',
            showIcon: true,
            showCloseButton: true,
            autoclose: true,
            autotimeout: 3000,
            gap: 20,
            distance: 20,
            type: 1,
            position: 'right top'
        })



    }


    var draftsBtn = document.querySelectorAll('.addToDraftsBtn');
    draftsBtn.forEach((btn) => {
        btn.addEventListener('click', function () {
            $.ajax({
                type: 'GET',
                url: "/Blog/AddToDrafts",
                dataType: "json",
                data: { ownerId: $('.userId').val(), postId: btn.dataset.postid },
                success: function (result) {
                    document.querySelector(`.post-${btn.dataset.postid}`).remove();
                },
            });
        });
    });

}
