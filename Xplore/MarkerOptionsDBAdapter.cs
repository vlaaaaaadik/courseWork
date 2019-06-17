using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace Xplore
{
    class MarkerOptionsDBAdapter
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string title { get; set; }
        public string snippet { get; set; }
        public MarkerOptionsDBAdapter() { }
        public MarkerOptionsDBAdapter(MarkerOptions e)
        {
            lat = e.Position.Latitude;
            lng = e.Position.Longitude;
            title = e.Title;
            snippet = e.Snippet;
        }
        public MarkerOptions makeMarkerOption()
        {
            return new MarkerOptions().SetPosition(new LatLng(lat, lng)).SetTitle(title).SetSnippet(snippet).Draggable(false)
                .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_places));
        }
    }
}