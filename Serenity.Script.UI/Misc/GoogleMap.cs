
namespace Serenity
{
    using google.maps;
    using jQueryApi;
    using System;
    using System.Html;
    using System.Runtime.CompilerServices;

    [Imported(ObeysTypeSystem = true), Element("<div/>")]
    public class GoogleMap : Widget<GoogleMapOptions>
    {
        private Map map;

        public GoogleMap(jQueryObject container, GoogleMapOptions opt)
            : base(container, opt)
        {
            var center = new LatLng(options.Latitude ?? 0, options.Longitude ?? 0);

            map = new Map(container[0], new MapOptions          
            {
                Center = center,
                MapTypeId = options.MapTypeId ?? MapTypeId.roadmap,
                Zoom = options.Zoom ?? 15,
                ZoomControl = true
            });

            if (options.MarkerTitle != null)
            {
                new Marker(new MarkerOptions {
                    Position = new LatLng(
                        options.MarkerLatitude ?? options.Latitude ?? 0, 
                        options.MarkerLongitude ?? options.Longitude ?? 0),
                    Map = map,
                    Title = options.MarkerTitle,
                    Animation = Animation.Drop
                });
            }

            LazyLoadHelper.ExecuteOnceWhenShown(container, () =>
            {
                GEvent.Trigger(map, "resize");
                map.SetCenter(center); // in case it wasn't visible (e.g. in dialog)
            });
        }

        public Map Map
        {
            get { return map; }
        }
    }

    [Imported, Serializable]
    public class GoogleMapOptions
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Double? Zoom { get; set; }
        public MapTypeId? MapTypeId { get; set; }
        public string MarkerTitle { get; set; }
        public double? MarkerLatitude { get; set; }
        public double? MarkerLongitude { get; set; }
    }
}