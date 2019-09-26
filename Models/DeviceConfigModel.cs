namespace gPadX.Models {
    public class DeviceConfigModel {
        public string Id { get; set; }
        public string Alias { get; set; }
        public bool MapDPadToLS { get; set; }
        public int LSDeadZone { get; set; }
        public int RSDeadZone { get; set; }
    }
}
