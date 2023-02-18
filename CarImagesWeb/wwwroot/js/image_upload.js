const IMAGE_UPLOAD_API_ENDPOINT = 'api/ImagesApi/Upload';
function initializeUppy(){
    let uppy = new Uppy.Uppy({
        id: 'uppy',
        target: '#uppy',
        inline: true,
        replaceTargetContent: true,
        showProgressDetails: true,
        note: 'Images and Videos only'
    })

    uppy.use(Uppy.Dashboard, {
        target: '#uppyDashboard',
        inline: true,
        height: 470,
        metaFields: [
            {id: 'name', name: 'Name', placeholder: 'file name'},
            {id: 'caption', name: 'Caption', placeholder: 'add description'},
        ],
        note: 'Images only',
    })
    uppy.use(Uppy.ImageEditor, {target: Uppy.Dashboard})
    uppy.use(Uppy.Form, {target: '#upload-form'})
    // Allow dropping files on any element or the whole document
    uppy.use(Uppy.Compressor)
    uppy.use(Uppy.XHRUpload, {
        endpoint: IMAGE_UPLOAD_API_ENDPOINT,
        formData: true,
        fieldName: 'files[]',
    })
    uppy.on('complete', result => {
        console.log('successful files:', result.successful)
        console.log('failed files:', result.failed)
    })
}
function initializeUploadForm(){
    // Form Behavior
    let vehicleInputs = $("#vehicle-inputs");
    let containerInputs = $("#container-inputs");

    // initial behavior
    vehicleInputs.hide();
    containerInputs.hide();

    $("input[name='ImageCategory']").change(function () {
        let category = $(this).val();
        if (category === "Vehicle") {
            vehicleInputs.show();
            containerInputs.hide();
        } else if (category === "Container") {
            vehicleInputs.hide();
            containerInputs.show();
        } else {
            //default
            vehicleInputs.hide();
            containerInputs.hide();
        }
    });
}

$(document).ready(function () {
    initializeUppy();
    initializeUploadForm();
});
