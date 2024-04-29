window.onload = (event) => {
    searchBtnAction();
}
function searchBtnAction() {
    var buttons = document.querySelectorAll('.searchBtnAction');
    buttons.forEach((button) => {
        button.addEventListener('click', function () {
            if (button.classList.contains('searchBtnAction-active')) {
                return;
            };

            document.querySelector('.searchBtnAction-active').classList.remove('searchBtnAction-active');
            button.classList.add('searchBtnAction-active');

            document.querySelector(`.serachResult-active`).classList.add('slide-out-bottom')
            document.querySelector(`.serachResult-active`).classList.add('serachResult-hidden')
            document.querySelector(`.serachResult-active`).classList.remove('serachResult-active')

            var searchParent = document.getElementById(`searchParent-${button.id}`);
            searchParent.classList.remove('serachResult-hidden');
            searchParent.classList.add('slide-in-bottom');
            searchParent.classList.add('serachResult-active');
            searchParent.classList.remove('slide-out-bottom');

        });
    });
}
