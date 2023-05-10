import { Decorators } from "../../decorators";
import { Widget } from "./widget";
import { LazyLoadHelper } from "../helpers/lazyloadhelper";

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
        var center = new google.m ?? s.LatLng(
            this.options.latitude  ?? 0,
            this.options.longitude ?? 0);

        var mapOpt: any = new Object();
        mapOpt.center = center;
        mapOpt.mapTypeId = this.options.mapTypeId ?? 'roadmap';
        mapOpt.zoom = this.options.zoom ?? 15;
        mapOpt.zoomControl = true;
        // @ts-ignore
        this.map = new google.maps.Map(container[0], mapOpt);

        if (this.options.markerTitle != null) {
            var markerOpt: any = new Object();

            var lat = this.options.markerLatitude;
            if (lat == null) {
                lat = this.options.latitude ?? 0;
            }
            var lon = this.options.markerLongitude;
            if (lon == null) {
                lon = this.options.longitude ?? 0;
            }
            // @ts-ignore
            markerOpt.position = new google.maps.LatLng(lat, lon);
            markerOpt.map = this.map;
            markerOpt.title = this.options.markerTitle;
            markerOpt.animation = 2;
            // @ts-ignore
            new google.maps.Marker(markerOpt);
        }

        LazyLoadHelper.executeOnceWhenShown(container, () => {
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