const IMAGE_UPLOAD_API_ENDPOINT = '/api/ImagesApi/Upload';

function initializeUppy() {
    let uppy = new Uppy.Uppy({
        id: 'uppy',
        target: '#uppy',
        inline: true,
        replaceTargetContent: true,
        showProgressDetails: true,
        note: 'Images only',
        restrictions: {
            allowedFileTypes: ['image/*'],
        },
        onBeforeFileAdded: (currentFile, files) => {
            console.log(currentFile)
            console.log(Object.values(files))
            if (Object.values(files).length === 0)
                return true
            if (Object.values(files).filter(f => f.name === currentFile.name)) {
                return true
            } else {
                return false
            }
        },
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
        doneButtonHandler: () => {
            window.location.href = "/Images/Upload";
            // this.reset()
        }

    })
    uppy.use(Uppy.DragDrop, {});
    uppy.use(Uppy.ImageEditor, {target: Uppy.Dashboard})
    uppy.use(Uppy.Form, {target: '#upload-form'})
    // Allow dropping files on any element or the whole document
    uppy.use(Uppy.Compressor)
    uppy.use(Uppy.XHRUpload, {
        endpoint: IMAGE_UPLOAD_API_ENDPOINT,
        formData: true,
    })
    uppy.use(Uppy.StatusBar, {})
    uppy.on('upload-error', (file, error, response) => {
        // show error message
        if (response.body.error) {
            uppy.info(response.body.error, 'error', 10000);
        }
    });

}

function initializeUploadForm() {
    // Form Behavior
    let vehicleInputs = $("#vehicle-inputs");
    let containerInputs = $("#container-inputs");

    // initial behavior
    vehicleInputs.hide();
    containerInputs.hide();

    $("input[name='ImageCategory']").change(function () {
        let category = $(this).val();
        sessionStorage.setItem('type', category)
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

    $('.assetDrop').on('change', function () {
        console.log(this.value)
        const tagIds = []
        $('.tagSelect option').each(function () {
            if ($(this).val() !== '') {
                tagIds.push($(this).val())
            }
           
        })
        const body = {
            tagIds: tagIds.map(id => parseInt(id)),
            assetId: parseInt(this.value)
        }
        axios.post(`/api/TagsApi/GetTagsWithCountForVehicle`, body).then(resp => {
            resp.data.data.forEach(tag => {
                $(`option[value='${tag.id}']`).text(tag.name)
            })
            
        }).catch(err => {
            console.log(err)
        })
    })

    $('#CountryCode').on('change', function () {

        const category = sessionStorage.getItem('type')

        let tags = []
        let id_prefix = 'vehicle'
        if (category === 'Vehicle') {
            tags = vehicleTags
            id_prefix = 'vehicle'
            document.getElementById('VehicleTag').value = '';
        } else {
            tags = containerTags
            id_prefix = 'container'
            document.getElementById('ContainerTag').value = '';
        }

        if (this.value === '') {
            tags.forEach(tag => {
                let option = $('#' + id_prefix + '_option_' + tag.Id)
                option.show()
            })
        } else {
            tags.forEach(tag => {
                if (tag.Country.Code !== this.value) {
                    let option = $('#' + id_prefix + '_option_' + tag.Id)
                    option.hide()
                } else {
                    let option = $('#' + id_prefix + '_option_' + tag.Id)
                    option.show()
                }

            })
        }
    });
}

$(document).ready(function () {
    initializeUppy();
    initializeUploadForm();
});
