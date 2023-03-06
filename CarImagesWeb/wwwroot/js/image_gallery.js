class Image {
    constructor(uploadId, url, downloadUrl, imageData) {
        this.uploadId = uploadId
        this.isSelected = false;
        this.url = url;
        this.country = imageData.country;
        this.asset = imageData.asset;
        this.tag = imageData.tag;
        this.downloadUrl = downloadUrl;
        this.assetInfo = imageData.assetInfo
    }
}

app = Vue.createApp({
    data() {
        return {
            _images: [],
            imageCategory: 'Vehicle',
            // these hardcoded arrays will be populated from the server on mounted()
            vehicles: ['CID/001', 'CID/002', 'CID/003', 'CID/004', 'CID/005'],
            containers: ['CoID/001', 'CoID/002', 'CoID/003', 'CoID/004'],
            vehicleTags: ['Front', 'Back', 'Left', 'Right', 'Interior', 'Engine', 'Dashboard', 'Wheels', 'Other'],
            containerTags: ['Front', 'Back', 'Left', 'Right', 'Interior', 'Other'],
            countries: [],
            downloading: false,
            searching: false,
            deletingIndex: null,
            _initialSearch: false,
            selectedCountryCode: ''
        }
    },
    computed: {
        initialSearch() {
            return this._initialSearch;
        },
        images() {
            return this._images;
        },
        assets() {
            if (this.imageCategory === '') {
                return [];
            } else if (this.imageCategory === 'Vehicle') {
                return this.vehicles;
            }
            return this.containers;
        },
        assetTags() {
            const country = this.selectedCountryCode
            if (this.imageCategory === '') {
                return [];
            } else if (this.imageCategory === 'Vehicle') {
                if(country === '')
                    return this.vehicleTags
                else
                    return this.vehicleTags.filter(t => t.country.code === country)
            }
            if (country === '')
                return this.containerTags;
            else
                return this.containerTags.filter(t => t.country.code === country)
        },
        isDownloading() {
            return this.downloading;
        },
        isSearching() {
            return this.searching;
        },
        getDeletingIndex() {
            return this.deletingIndex
        }
    },
    methods: {
        moreInfo(index) {
            const image = this._images[index]
            $(document).Toasts('create', {
                title: 'Toast Title',
                body:
                    `Asset: ${image.asset}<br>
                    Tag: ${image.tag}<br>
                    Country: ${image.country}<br>
                    Market: ${image.assetInfo.market}<br>
                    Sales Segment: ${image.assetInfo.salesSegment}<br>
                    Stock: ${image.assetInfo.stock}<br>
                    Yard In Date: ${image.assetInfo.yardInDate.substring(0, 10)}<br>
                    Purchase Date: ${image.assetInfo.purchaseDate.substring(0, 10)}<br>
                    `
            })
        },
        deleteClick(index) {
            if (this.deletingIndex !== null && this.deletingIndex !== index) {
                return
            }
            const uploadId = this._images[index].uploadId
            this.deletingIndex = index
            axios.get('/api/ImagesApi/DeleteUpload?uploadId=' + uploadId).then(resp => {
                this.deletingIndex = null
                if(resp.data.success) {
                    this.searchImages()
                } 
            }).catch(err => {
                this.deletingIndex = null
                alert(err.message)
            })
        },
        setSearching(isSearching) {
            this.searching = isSearching;
        },
        // Search images by category, asset and tags. Calls _getImages() to get the image urls from the server
        searchImages() {
            const SEARCH_URL = '/api/ImagesApi/Search';

            let assetType = this.imageCategory;
            let asset = this._getSelectedAsset();
            let tags = this._getSelectedAssetTags();
            let country = this._getSelectedCountry();

            let searchParams = {assetType, asset, tags, country};
            //check if the search params are valid
            if (assetType === '' || asset === '' && tags.length === 0) {
                // no search params, do nothing
                return;
            }
            this._initialSearch = true;
            //if not already searching, set searching to true and call _getImages()
            if (this.searching !== true) {
                this.setSearching(true);
                this._getImages(SEARCH_URL, searchParams).then(data => {
                    this._images = [];
                    // TODO: update Image class to include assetType, asset, country , tags and imageUrl as well
                    data.forEach(obj => {
                        this._images.push(new Image(obj.uploadId, obj.url, obj.downloadUrl, obj.imageData));
                    });
                }).finally(() => {
                    this.setSearching(false)
                });
            }
        },
        setDownloading(isDownloading) {
            this.downloading = isDownloading;
        },
        // Download selected images as a zip file to the client
        downloadImages() {
            let imageUrls = [];
            this._images.forEach(image => {
                if (image.isSelected) {
                    imageUrls.push(image.url);
                }
            });
            //check if there are any selected images
            if (imageUrls.length === 0) {
                //no images selected, do nothing
                return;
            }
            //if not already downloading, set downloading to true and call _downloadImages()
            if (this.downloading !== true) {
                this.setDownloading(true);
                axios.post('/api/ImagesApi/Download', imageUrls, {responseType: 'arraybuffer'}).then(response => {
                    if (response.status === 200) {
                        // Create a blob object from the response data
                        return new Blob([response.data], {type: response.headers['content-type']});
                    } else {
                        throw new Error('Failed to download images');
                    }
                }).then(blob => {
                    // Create a data URI for the blob
                    const dataUri = URL.createObjectURL(blob);
                    // open a new window and navigate to the data URI
                    window.open(dataUri, '_blank');
                })
                    .catch(error => {
                        console.error(error);
                    }).finally(() => {
                    this.setDownloading(false)
                });
            }
        },
        // Get the selected asset type from the select2 dropdown
        _getSelectedAsset() {
            let assetType = '';
            $('#Asset').select2('data').forEach(asset => {
                assetType = asset.id;
            });
            return assetType;
        },
        // Get the selected asset tags from the select2 dropdown (multiple select)
        _getSelectedAssetTags() {
            let assetTags = [];
            $('#AssetTags').select2('data').forEach(tag => {
                assetTags.push(tag.id);
            });
            return assetTags;
        },
        // Get the selected country from the select2 dropdown
        _getSelectedCountry() {
            let country = '';
            country = $('#Country').find(":selected").val();
            return country;
        },
        //Fetch image urls from the server
        async _getImages(fromUrl, searchParams) {
            //make axios call to get images from the server
            let res = await axios.post(fromUrl, searchParams);
            return res.data.data || [];
        },
        //Select or deselect all images
        selectAll(select = true) {
            this._images.forEach(image => {
                image.isSelected = select;
            });
        },
        //Deselect all images
        deselectAll() {
            this.selectAll(false);
        }
    },
    mounted() {
        // Initialize select2 dropdowns:
        //  Get the assets and tags from the server and 
        //  populate the vehicle, container, vehicleTags, containerTag arrays
        const ASSETS_URL = '/api/AssetsApi/GetAssets';
        const TAGS_URL = '/api/TagsApi/GetTags';
        const COUNTRIES_URL = '/api/CountryApi/GetCountriesForUser';

        async function getAssets(fromUrl) {
            //make axios call to get assets from the server
            let res = await axios.get(fromUrl);
            return res.data.data || {
                vehicles: [],
                containers: []
            };
        }

        async function getTags(fromUrl) {
            //make axios call to get tags from the server
            let res = await axios.get(fromUrl);
            return res.data.data || {
                vehicleTags: [],
                containerTags: []
            };
        }

        async function getCountries(fromUrl) {
            //make axios call to get countries from the server
            let res = await axios.get(fromUrl);
            return res.data.data || [];
        }


        //get assets and tags from the server
        getAssets(ASSETS_URL).then(data => {
            this.vehicles = data.vehicles;
            this.containers = data.containers;
        });
        getTags(TAGS_URL).then(data => {
            //TODO: instead of having two arrays for vehicleTags and containerTags, 
            // use one array and populate it with the data from the server
            this.vehicleTags = data.vehicleTags;
            // vehicle tags must have container tags also
            if (data.containerTags != null)
                this.vehicleTags = this.vehicleTags.concat(data.containerTags)
            
            this.containerTags = data.containerTags;
        });
        getCountries(COUNTRIES_URL).then(data => {
            this.countries = data;
        });
    }
})

galleryApp = app.mount('#app');