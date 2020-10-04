namespace Serenity {
    export interface GoogleMapOptions {
        latitude?: any;
        longitude?: any;
        zoom?: any;
        mapTypeId?: any;
        markerTitle?: string;
        markerLatitude?: any;
        markerLongitude?: any;
    }

    @Decorators.registerEditor('Serenity.GoogleMap', [])
    @Decorators.element('<div/>')
    export class GoogleMap extends Widget<GoogleMapOptions> {

        private map: any;

        constructor(container: JQuery, opt: GoogleMapOptions) {
            super(container, opt);

            // @ts-ignore
            var center = new google.maps.LatLng(
                Q.coalesce(this.options.latitude, 0),
                Q.coalesce(this.options.longitude, 0));

            var mapOpt: any = new Object();
            mapOpt.center = center;
            mapOpt.mapTypeId = Q.coalesce(this.options.mapTypeId, 'roadmap');
            mapOpt.zoom = Q.coalesce(this.options.zoom, 15);
            mapOpt.zoomControl = true;
            // @ts-ignore
            this.map = new google.maps.Map(container[0], mapOpt);

            if (this.options.markerTitle != null) {
                var markerOpt: any = new Object();

                var lat = this.options.markerLatitude;
                if (lat == null) {
                    lat = Q.coalesce(this.options.latitude, 0);
                }
                var lon = this.options.markerLongitude;
                if (lon == null) {
                    lon = Q.coalesce(this.options.longitude, 0);
                }
                // @ts-ignore
                markerOpt.position = new google.maps.LatLng(lat, lon);
                markerOpt.map = this.map;
                markerOpt.title = this.options.markerTitle;
                markerOpt.animation = 2;
                // @ts-ignore
                new google.maps.Marker(markerOpt);
            }

            Serenity.LazyLoadHelper.executeOnceWhenShown(container, () => {
                // @ts-ignore
                google.maps.event.trigger(this.map, 'resize', []);
                this.map.setCenter(center);
                // in case it wasn't visible (e.g. in dialog)
            });
        }

        get_map(): any {
            return this.map;
        }
}
}