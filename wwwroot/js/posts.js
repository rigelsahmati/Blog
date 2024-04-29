
var filterdHtml;
var filterActive = false;
window.onload = (event) => {
    defaultHtml = document.querySelector('.post-parent').innerHTML;
    document.querySelector('#posts').classList.add('menu-iteme-active');
    document.querySelector('#posts').children[0].children[0].style.color = "#f16363";
}

$('.filterBtn').click(function () {
    $('.categorie-parent').toggleClass('post-parent-hidden')
});

$('#searchUser').on('input', function () {
    var inputValue = $(this).val();
    if (inputValue.length < 3) {
        if (inputValue.length == 0) {
            if (filterActive) {
            var categorieId = document.querySelector('.categorie-active').id
                $.ajax({
                    type: 'GET',
                    url: "/Admin/FilterPost",
                    dataType: "json",
                    data: { id: categorieId },
                    success: function (result) {
                        handleResponse(result)
                    }
                });
                AttachEvenListener()
            }
            else {
                $.ajax({
                    type: 'GET',
                    url: "/Admin/GetAllPosts",
                    dataType: "json",
                    data: { userId: $('#userId').val() },
                    success: function (result) {
                        handleResponse(result)
                    }
                });
                AttachEvenListener()
            }
        }

        return;
    };
    if (filterActive) {
        $.ajax({
            type: 'GET',
            url: "/Admin/SearchPost",
            dataType: "json",
            data: { query: $('#searchUser').val(), id: parseInt($('.categorie-active').attr('id')) },
            success: function (result) {
                handleResponse(result)
            }
        });
    }
    else {
        $.ajax({
            type: 'GET',
            url: "/Admin/SearchPost",
            dataType: "json",
            data: { query: $('#searchUser').val(), id: 0 },
            success: function (result) {
                handleResponse(result)
            }
        });
    }



});

document.querySelectorAll('.categorie').forEach((categorie) => {
    $(categorie).on('click', function () {
        if (categorie.classList.contains('categorie-active')) {
            $.ajax({
                type: 'GET',
                url: "/Admin/GetAllPosts",
                dataType: "json",
                data: { userId: $('#userId').val() },
                success: function (result) {
                    handleResponse(result)
                }
            });
            filterActive = false;
            $('.filterBtn').removeClass('filterBtn-active')
            document.querySelector('.categorie-active').classList.remove('categorie-active');
            return;
        }
        else {
        $.ajax({
            type: 'GET',
            url: "/Admin/FilterPost",
            dataType: "json",
            data: { id: categorie.id },
            success: function (result) {
                handleResponse(result)
            }
        });
        filterActive = true;
        $('.filterBtn').addClass('filterBtn-active');
        if (document.querySelector('.categorie-active') != null) document.querySelector('.categorie-active').classList.remove('categorie-active');
        categorie.classList.add('categorie-active');
        }
    })
});

function handleResponse(data) {
    document.querySelector('.post-parent').innerHTML = '';
    if (data == null) {
        document.querySelector('.post-parent').innerHTML = `<h4 class="text-center w-100 mt-5">No posts were found.</h4>`;
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

                            <a asp-action="Profile" asp-route-id="${post.userId}">
                                <img src="/uploads/img/usersImg/${post.profileImage}" width="40px" height="40px" class="postUserImg" />
                            </a>
                            <div class="ms-3 w-100 d-flex align-items-center justify-content-between">
                                <div>
                                    <a asp-action="ViewUser" href="/Admin/ViewUser/${post.userId}">
                                        <p class="m-0 postUsername">${post.fullName}</p>
                                    </a>
                                    <p class="m-0 postDate">${post.createdTime}</p>
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
            `  <button class="col btn">
                                <i class="fa-solid fa-comment"></i>
                                Commet
                            </button>
                            <button data-urlId="${post.postId}" class="copyLinkBtn col btn shareBtn">
                                <i class="fa-solid fa-share"></i>
                                Share
                            </button>
                        </div>
                    </div>`;

        document.querySelector('.post-parent').innerHTML += postHtml;
    })
    AttachEvenListener()
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

}

