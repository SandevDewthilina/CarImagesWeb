class Image{
    constructor(url){
        this.url=url;
        this.isSelected=false;
    }
}

app = Vue.createApp({
    data(){
        return{
           _images:[],
            imageCategory:'',
            // these hardcoded arrays will be populated from the server on mounted()
            vehicles:['CID/001','CID/002','CID/003','CID/004','CID/005'], 
            containers:['CoID/001','CoID/002','CoID/003','CoID/004'], 
            vehicleTags:['Front','Back','Left','Right','Interior','Engine','Dashboard','Wheels','Other'], 
            containerTags:['Front','Back','Left','Right','Interior','Other'], 
            downloading:false,
            searching:false
        }
    },
    computed:{
        images(){
            return this._images;
        },
        assets(){
            if(this.imageCategory===''){
                return [];
            }
            else if(this.imageCategory==='Vehicle'){
                return this.vehicles;
            }
            return this.containers;
        },
        assetTags(){
            if(this.imageCategory===''){
                return [];
            }
            else if(this.imageCategory==='Vehicle'){
                return this.vehicleTags;
            }
            return this.containerTags;
        },
        isDownloading(){
            return this.downloading;
        },
        isSearching(){
            return this.searching;
        }
    },
    methods:{
        setSearching(isSearching){
            this.searching = isSearching;
        },
        // Search images by category, asset and tags. Calls _getImages() to get the image urls from the server
        searchImages() {
            const SEARCH_URL = '/api/ImagesApi/Search';
            let assetType = this.imageCategory;
            let asset = this._getSelectedAssetType();
            let tags = this._getSelectedAssetTags();
            let searchParams = { assetType, asset, tags };
            this.setSearching(true);
            this._getImages(SEARCH_URL, searchParams).then(data => {
                this._images = [];
                data.forEach(url => {
                    this._images.push(new Image(url));
                });
            }).finally(() => {
                this.setSearching(false)
            });
        },
        setDownloading(isDownloading){
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
        },
        // Get the selected asset type from the select2 dropdown
        _getSelectedAssetType(){
            let assetType = '';
            $('#AssetType').select2('data').forEach(asset => {
                assetType = asset.text;
            });
            return assetType;
        },
        // Get the selected asset tags from the select2 dropdown (multiple select)
        _getSelectedAssetTags(){
            let assetTags = [];
            $('#AssetTags').select2('data').forEach(tag => {
                assetTags.push(tag.text);
            });
            return assetTags;
        },
        //Fetch image urls from the server
        async _getImages(fromUrl, searchParams) {
            //make axios call to get images from the server
            let res = await axios.post(fromUrl, searchParams);
            return res.data.data || [];
        },
        //Select or deselect all images
        selectAll(select=true){
            this._images.forEach(image => {
                image.isSelected=select;
            });
        },
        //Deselect all images
        deselectAll(){
            this.selectAll(false);
        }
    },
    mounted() {
        // Initialize select2 dropdowns:
        //  Get the assets and tags from the server and 
        //  populate the vehicle, container, vehicleTags, containerTag arrays
        const ASSETS_URL = '/api/ImagesApi/GetAssets';
        const TAGS_URL = '/api/ImagesApi/GetTags';
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
        //get assets and tags from the server
        getAssets(ASSETS_URL).then(data => {
            this.vehicles = data.vehicles;
            this.containers = data.containers;
        });
        getTags(TAGS_URL).then(data => {
            this.vehicleTags = data.vehicleTags;
            this.containerTags = data.containerTags;
        });
        
    }
})

galleryApp = app.mount('#app');