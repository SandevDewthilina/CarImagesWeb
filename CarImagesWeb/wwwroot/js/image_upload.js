$(document).ready(function () {
    
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
        note: 'Images and video only, 2–3 files, up to 1 MB',
    })
    uppy.use(Uppy.ImageEditor, {target: Uppy.Dashboard})
    uppy.use(Uppy.Form, {target: '#upload-form'})
    // Allow dropping files on any element or the whole document
    uppy.use(Uppy.Compressor)
    uppy.use(Uppy.XHRUpload, {
        endpoint: '/Home/Index',
        formData: true,
        fieldName: 'files[]',
    })
    uppy.on('complete', result => {
        console.log('successful files:', result.successful)
        console.log('failed files:', result.failed)
    })

    let vehicleInputs = $("#vehicle-inputs");
    let containerInputs = $("#container-inputs");
    vehicleInputs.hide();
    containerInputs.hide();
    
    $("input[name='ImageCategory']").change(function () {
        let category = $(this).val();
        if (category === "Vehicles") {
            vehicleInputs.show();
            containerInputs.hide();
        } else if (category === "Containers") {
            vehicleInputs.hide();
            containerInputs.show();
        } else {
            //default
            vehicleInputs.hide();
            containerInputs.hide();
        }
    });
});
