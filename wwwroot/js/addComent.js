$('.addComentForm').on('submit', function () {
    var inputValue = $(".addComentInpunt").val()
    if (!inputValue) {
        return;
    }
    $.ajax({
        type: 'GET',
        url: "/Blog/AddComent",
        dataType: "json",
        data: { postId: $('#postId').val(), userId: $('#userId').val(), comentContext: inputValue },
        success: function (result) {
           handleComentResponse(result)
        }
    });
})
function handleComentResponse(data) {
    if (data == null) { return; }
    document.querySelector('.comentsParent').innerHTML = '';
    data.forEach((coment) => {
        var comentHtml = `
         <div class=" my-3 border border-1 rounded-3 p-3">
            <div class="">
                <a href="/Blog/Profile/${coment.userId}" class="d-flex gap-3 align-items-center">
                    <img src="/uploads/img/usersImg/${coment.profileImage}" width="35px" height="35px" style="border-radius: 100px; object-fit:cover;">
                    <p class="m-0">${coment.userName}</p>
                </a>
            </div>
            <p class="m-2 ms-4 comentContext">${coment.comentContext}</p>
            <p class="me-2 mb-0 mt-4 w-100 text-end" style="font-size:.8rem">${coment.createdTime}</p>
         </div>`;
        document.querySelector('.comentsParent').innerHTML += comentHtml;

    })
    document.querySelector('#comentCount').innerHTML = data.length;
    document.querySelector(".addComentInpunt").value = ""

}
