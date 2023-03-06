namespace CarImagesWeb.Models
{
    public class ContainerVehicleMapping
    {
        public int VehicleAssetId { get; set; }
        public int ContainerAssetId { get; set; }

        public Asset VehicleAsset { get; set; }
        public Asset ContainerAsset { get; set; }
    }
}