//const categorieForm = document.querySelector('.categorie-form').offsetHeight;
var isMobile;



let taskBody = document.querySelector('.menu-iteme-body-task');
taskBody.style.height = taskBody.clientHeight + 'px';

let categorieBody = document.querySelector('.menu-iteme-body-categorie');
categorieBody.style.height = categorieBody.clientHeight + 'px';

let myAccBody = document.querySelector('.menu-iteme-body-myAcc');
myAccBody.style.height = myAccBody.clientHeight + 'px';

handleScreenWidthChange('resize');

window.addEventListener('resize', function () {
    handleScreenWidthChange('menuBtn');
});



//Creates drop down menu
$('.menu-iteme-btn').click(function () {
    if (this.id == 'task') {
        let divBody = document.querySelector('.menu-iteme-body-task');
        divBody.classList.toggle('menu-iteme-body-active');
        document.getElementById('task-icon').classList.toggle('icon-active')
    }
    else if (this.id == "categorie") {
        let divBody = document.querySelector('.menu-iteme-body-categorie');
        divBody.classList.toggle('menu-iteme-body-active');
        document.getElementById('categorie-icon').classList.toggle('icon-active')
    }
    else if (this.id == "myAccount") {
        let divBody = document.querySelector('.menu-iteme-body-myAcc');
        divBody.classList.toggle('menu-iteme-body-active');
        document.getElementById('myAccIcon').classList.toggle('icon-active')
    }
});

//Menu open & close btn
$('.menu-btn').click(function () {
    handleMenuControl('toggle')
})



function handleMenuControl(option) {
    if (option == 'toggle') {
        let menu = document.querySelector('.side-menu');
      //  handleAddMnu('hide')
        menu.classList.toggle('side-menu-hidden');
        document.querySelector('.side-menu-open-btn').classList.toggle('side-menu-open-btn-active')
        document.querySelector('body').classList.toggle('gap-4')
        document.querySelector('.main-body').classList.toggle('px-5')
    }
    else if (option == 'hide') {
        let menu = document.querySelector('.side-menu');
        menu.classList.add('side-menu-hidden');
        document.querySelector('.side-menu-open-btn').classList.add('side-menu-open-btn-active')
        document.querySelector('body').classList.remove('gap-4')
        document.querySelector('.main-body').classList.add('px-5')
    }
    else if (option == 'show') {
        let menu = document.querySelector('.side-menu');


        menu.classList.remove('side-menu-hidden');
        document.querySelector('.side-menu-open-btn').classList.remove('side-menu-open-btn-active')
        document.querySelector('.main-body').classList.remove('px-5')
        document.querySelector('body').classList.add('gap-4')
    }

}


function handleScreenWidthChange(option) {
    var screenWidth = window.innerWidth;
    // let menu = document.querySelector('.side-menu');

    if (option == 'resize') {

        if (screenWidth <= 1100) {
            isMobile = true;
            handleMenuControl('hide');
        }
        else if (screenWidth > 1100) {
            isMobile = false;
            handleMenuControl('show');
        }
    }

    else if (option == 'menuBtn') {
        if (screenWidth <= 1100) {
            isMobile = true;
        }
        else if (screenWidth > 1100) {
            isMobile = false;
        }
    }
}
