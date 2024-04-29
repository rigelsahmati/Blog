$('#searchUser').on('input', function () {
    var inputValue = $(this).val()
    document.querySelector(".searchFor").innerHTML = $('#searchUser').val();
    if (inputValue.length < 3) {
        document.querySelector('.searchBody').innerHTML = '';
        if (inputValue.length == 0) {
            $('.searchBody').addClass('searchBody-hidden');
            $('.searchBtn').addClass('searchBtn-hidden');
            $('.searchParent').removeClass('p-2');
            $('.searchParent').addClass('p-0');
            return;
        }
        $('.searchBody').removeClass('searchBody-hidden');
        $('.searchBtn').removeClass('searchBtn-hidden');
        $('.searchParent').removeClass('p-0');
        $('.searchParent').addClass('p-2');
        return;
    };
    $.ajax({
        type: 'GET',
        url: "/Blog/SearchByUsername",
        dataType: "json",
        data: { username: $('#searchUser').val() },
        success: function (result) {
            handleSearchResponse(result)
        }
    });
})
function handleSearchResponse(data) {
    if (data == null) { return; }
    document.querySelector('.searchBody').innerHTML = '';
    data.forEach((user) => {
        var userHtml = `
        <a href="/Blog/Profile/${user.id}" class="searchResult d-flex gap-3 p-2 rounded-3 w-100 align-items-center my-2">
              <img src="/uploads/img/usersImg/${user.image}" width="45px" height="45px" style="border-radius:100px;" />
              <p class="m-0">${user.userName}</p>
        </a> `;
        document.querySelector('.searchBody').innerHTML += userHtml;

    })

}
