const app = Vue.createApp({
    data() {
        return {
            assets: [],
            datatable: {}
        }
    },
    computed: {
        getAssets() {
            setTimeout(function() {
                // if ( $.fn.dataTable.isDataTable( '#example1' ) ) {
                //     // console.log('destory')
                //     // this.datatable.destroy()
                //     // // this.datatable.destroy()
                //     // this.datatable = $("#example1").DataTable({
                //     //     destroy: true,
                //     //     "responsive": true,
                //     //     "autoWidth": false,
                //     // });
                //     this.datatable.clear().rows.add(response.data.data).draw();
                //
                //     // refresh DataTable
                //     this.datatable.ajax.reload();
                // }
                // else {
                $("#example1").DataTable({
                        "responsive": true,
                        "autoWidth": false,
                    });
                // }
                
            }, 1000)
            return this.assets;
        },
        getAssetCount() {
            return this.assets.length
        }
    },
    methods: {
        search(event) {
            event.preventDefault();
            const startDate = $('#startDateInput').val()
            const endDate = $('#endDateInput').val()
            const code = $('#assetCodeInput').val()

            window.location.href = `/Assets/List?startDate=${startDate}&endDate=${endDate}&code=${code}`;
        }
    },
    mounted() {
    }
})

const galleryApp = app.mount('#app');