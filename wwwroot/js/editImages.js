function getExtension(filename) {
    var parts = filename.split('.');
    return parts[parts.length - 1];
}
function isImage(filename) {
    var ext = getExtension(filename);
    switch (ext.toLowerCase()) {
        case 'jpg':
        case 'jpeg':
        case 'png':
        case 'web':
            return true;
    }
    return false;
}

const previewImage = (event) => {
    const imageFiles = event.target.files;
    const imageFilesLength = imageFiles.length;
    const imageSrc = URL.createObjectURL(imageFiles[0]);
    const imagePreviewElement = document.querySelector(".imagePreview");
    if (!isImage(imageFiles[0].name)) {
        document.querySelector('.imageName').classList.add('text-danger');
        document.querySelector('.imageName').innerHTML = "Image format not supported!";
        $('.saveImages').attr('disabled', 'disabled');
        return;
    }
    $('.saveImages').removeAttr('disabled');
    imagePreviewElement.src = imageSrc;

    document.querySelector('.imagePreviewParent').classList.add('imagePreviewParent-active')

    imagePreviewElement.style.display = "block";
    imagePreviewElement.classList.toggle('fade-in-top');
    document.querySelector('.imageName').classList.remove('text-danger');
    document.querySelector('.imageName').innerHTML = imageFiles[0].name;
    document.querySelector('.imageSize').classList.remove('text-danger');

    let imgSize = Math.round(imageFiles[0].size);
    if (imgSize > 125000) document.querySelector('.imageSize').innerHTML = (Math.round(imgSize / 1125000)) + "MB";
    if (imgSize <= 125000) document.querySelector('.imageSize').innerHTML = (Math.round(imgSize / 1024)) + "KB";
    if (imageFiles[0].size > 5242880) {
        $('.saveImages').attr('disabled', 'disabled');
        document.querySelector('.imageName').innerHTML = "Image size must be lower then 5MB";
        document.querySelector('.imageSize').classList.add('text-danger');
        document.querySelector('.imageName').classList.add('text-danger');

    }
};
var editBtns = document.querySelectorAll('.editImageType');
editBtns.forEach((btn) => {
    btn.addEventListener('click', function () {
        if (btn.classList.contains('profileEditBtn')) {
            var input = document.querySelector('.profileInput');
            input.addEventListener('input', function () {
                document.querySelector('.imagePreviewParent').classList.add('imagePreviewParent-profile')
                document.querySelector('.imagePreview').classList.add('imagePreview-profile')
                previewImage(event)
            })
            input.click();
        }
        else {
            var input = document.querySelector('.coverInput');
            input.addEventListener('input', function () {
                document.querySelector('.imagePreviewParent').classList.remove('imagePreviewParent-profile')
                document.querySelector('.imagePreview').classList.remove('imagePreview-profile')
                previewImage(event)
            })
            input.click();

        }
    });
});