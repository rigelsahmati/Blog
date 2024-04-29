

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
                    }
                });
            }
        })
    })
})

function handleDeleteResponse(data) {
    if (data == null) { return; }
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
    btn.addEventListener('click', function(){
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