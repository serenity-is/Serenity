using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// Possible values for feature types. Specify these values as strings, i.e. 'administrative' or 'poi.park'. Stylers applied to a parent feature type automatically apply to all child feature types. Note however that parent features may include some additional features that are not included in one of their child feature types.
    /// </summary>
    [Imported]
    public enum MapTypeStyleFeatureType
    {
        /// <summary>
        /// Apply the rule to administrative areas.
        /// </summary>
        administrative,
        /// <summary>
        /// Apply the rule to countries.
        /// </summary>
        [ScriptName("administrative.country")]
        administrative_country,
        /// <summary>
        /// Apply the rule to land parcels.
        /// </summary>
        [ScriptName("administrative.land.parcel")]
        administrative_land_parcel,
        /// <summary>
        /// Apply the rule to localities.
        /// </summary>
        [ScriptName("administrative.locality")]
        administrative_locality,
        /// <summary>
        /// Apply the rule to neighborhoods.
        /// </summary>
        [ScriptName("administrative.neighborhood")]
        administrative_neighborhood,
        /// <summary>
        /// Apply the rule to provinces.
        /// </summary>
        [ScriptName("administrative.province")]
        administrative_province,
        /// <summary>
        /// Apply the rule to all selector types.
        /// </summary>
        all,
        /// <summary>
        /// Apply the rule to landscapes.
        /// </summary>
        landscape,
        /// <summary>
        /// Apply the rule to man made structures.
        /// </summary>
        [ScriptName("landscape.man.made")]
        landscape_man_made,
        /// <summary>
        /// Apply the rule to natural features.
        /// </summary>
        [ScriptName("landscape.natural")]
        landscape_natural,
        /// <summary>
        /// Apply the rule to points of interest.
        /// </summary>
        poi,
        /// <summary>
        /// Apply the rule to attractions for tourists.
        /// </summary>
        [ScriptName("poi.attraction")]
        poi_attraction,
        /// <summary>
        /// Apply the rule to businesses.
        /// </summary>
        [ScriptName("poi.business")]
        poi_business,
        /// <summary>
        /// Apply the rule to government buildings.
        /// </summary>
        [ScriptName("poi.government")]
        poi_government,
        /// <summary>
        /// Apply the rule to emergency services (hospitals, pharmacies, police, doctors, etc).
        /// </summary>
        [ScriptName("poi.medical")]
        poi_medical,
        /// <summary>
        /// Apply the rule to parks.
        /// </summary>
        [ScriptName("poi.park")]
        poi_park,
        /// <summary>
        /// Apply the rule to places of worship, such as church, temple, or mosque.
        /// </summary>
        [ScriptName("poi.place.of.worship")]
        poi_place_of_worship,
        /// <summary>
        /// Apply the rule to schools.
        /// </summary>
        [ScriptName("poi.school")]
        poi_school,
        /// <summary>
        /// Apply the rule to sports complexes.
        /// </summary>
        [ScriptName("poi.sports.complex")]
        poi_sports_complex,
        /// <summary>
        /// Apply the rule to all roads.
        /// </summary>
        road,
        /// <summary>
        /// Apply the rule to arterial roads.
        /// </summary>
        [ScriptName("road.arterial")]
        road_arterial,
        /// <summary>
        /// Apply the rule to highways.
        /// </summary>
        [ScriptName("road.highway")]
        road_highway,
        /// <summary>
        /// Apply the rule to local roads.
        /// </summary>
        [ScriptName("road.local")]
        road_local,
        /// <summary>
        /// Apply the rule to all transit stations and lines.
        /// </summary>
        transit,
        /// <summary>
        /// Apply the rule to transit lines.
        /// </summary>
        [ScriptName("transit.line")]
        transit_line,
        /// <summary>
        /// Apply the rule to all transit stations.
        /// </summary>
        [ScriptName("transit.station")]
        transit_station,
        /// <summary>
        /// Apply the rule to airports.
        /// </summary>
        [ScriptName("transit.station.airport")]
        transit_station_airport,
        /// <summary>
        /// Apply the rule to bus stops.
        /// </summary>
        [ScriptName("transit.station.bus")]
        transit_station_bus,
        /// <summary>
        /// Apply the rule to rail stations.
        /// </summary>
        [ScriptName("transit.station.rail")]
        transit_station_rail,
        /// <summary>
        /// Apply the rule to bodies of water.
        /// </summary>
        water,
    }
}