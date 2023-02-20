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
            vehicles:['CID/001','CID/002','CID/003','CID/004','CID/005','CID/006','CID/007','CID/008','CID/009','CID/010'],
            containers:['CoID/001','CoID/002','CoID/003','CoID/004','CoID/005','CoID/006','CoID/007','CoID/008','CoID/009','CoID/010'],
            vehicleTags:['Front','Back','Left','Right','Interior','Engine','Dashboard','Wheels','Other'],
            containerTags:['Front','Back','Left','Right','Interior','Other'],
            imageCategory:'',
        }
    },
    computed:{
        images(){
            return this._images;
        },
        assets(){
            if(this.imageCategory==='Vehicle'){
                return this.vehicles;
            }
            return this.containers;
        },
        assetTags(){
            if(this.imageCategory==='Vehicle'){
                return this.vehicleTags;
            }
            return this.containerTags;
        }
    },
    methods:{
        searchImages() {
            //TODO: send search criteria to the server 
            console.log('searching images');
            console.log(this.imageCategory);
            let assets =[];
            $('#AssetType').select2('data').forEach(asset => {
                assets.push(asset.text);
            });
            console.log(assets);
            
            let tags =[];
            $('#AssetTags').select2('data').forEach(tag => {
                tags.push(tag.text);
            });
            console.log(tags);
        },
        downloadImages(){
            //TODO: send selected images to the server
            
            //print selected images
            this._images.forEach(image => {
                if(image.isSelected){
                    console.log(image.url);
                }
            });
        },
        getImages(){
            //TODO: get image urls from the server
            
            //hardcoded images
            for(let i=0;i<10;i++) {
                this._images.push(new Image('http://localhost:5000/dist/img/photo1.png'));
            }
        },
        selectAll(select=true){
            this._images.forEach(image => {
                image.isSelected=select;
            });
        },
        deselectAll(){
            this.selectAll(false);
        }
    },
    mounted(){
        this.getImages();
    }
})

galleryApp = app.mount('#app');