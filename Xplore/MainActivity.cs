using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Location;
using Xamarin.Forms.Platform.Android;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Util;
using Android.Support.V4.App;
using Android;
using Android.Views.InputMethods;
using Android.Support.V4.Content;
using System.Linq;

namespace Xplore
{
    [Activity(Label = "Xplore", MainLauncher = true, Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity, IOnMapReadyCallback
    {
        FusedLocationProviderClient fused;
        GoogleMap Map;
        EditText searchText;
        Database database;
        ImageView gps;
        ImageView mapType; 
        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            fused = LocationServices.GetFusedLocationProviderClient(this);
            var prems = new string[] { Manifest.Permission.AccessFineLocation};

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            ActivityCompat.RequestPermissions(this, prems, 1);
            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            searchText = FindViewById<EditText>(Resource.Id.input_search);
            mapType = FindViewById<ImageView>(Resource.Id.ic_maptype);
            gps = FindViewById<ImageView>(Resource.Id.ic_gps);
            database = new Database("Markers");
            database.database.CreateTable<MarkerOptionsDBAdapter>();
            if (ContextCompat.CheckSelfPermission(this,Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                mapFragment.GetMapAsync(this);
            }
        }

        private void searchInit()
        { 
            searchText.EditorAction += (r, e) =>
            {
                if(e.ActionId == ImeAction.Search
                ||e.ActionId == ImeAction.Done
                || e.Event.Action == KeyEventActions.Down
                || e.Event.KeyCode == Keycode.Enter)
                {
                    locate();
                }
            };
        }

        private void moveCamera(LatLng latLng, float zoom,string title)
        {
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(latLng);
            builder.Zoom(zoom);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            Map.MoveCamera(cameraUpdate);
            MarkerOptions marker = new MarkerOptions()
                    .SetPosition(latLng)
                    .SetTitle(title)
                    .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_custom_pin));
            Map.AddMarker(marker);
        }

        private void locate()
        {
            string searchString = searchText.Text;
            Geocoder geocoder = new Geocoder(this);
            var list = geocoder.GetFromLocationName(searchString, 1);
            if (list.Count > 0)
            {
                Address address = list[0];
                moveCamera(new LatLng(address.Latitude, address.Longitude), 15f, address.GetAddressLine(0));                    
            }

        }
        public void OnMapReady(GoogleMap map)
        {
            map.MyLocationEnabled = true;
            Map = map;
            Map.Clear();
            databaseInit();
            setMarkers();
            setCamOnMyLoc();

            gps.Click += (r, e) => { setCamOnMyLoc(); };
            mapType.Click += (r, e) =>
            {
                int i = Map.MapType; 
                if (i == 4) i = 0;
                Map.MapType = ++i;
            };
            Map.MapLongClick += (r, e) => {
                MarkerOptions marker = new MarkerOptions()
                .SetPosition(e.Point)
                .Draggable(true)
                .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_custom_pin));
                Map.AddMarker(marker);
                
            };
            Map.MarkerClick += (r, e) => 
            {
               if (e.Marker.Title == null) e.Marker.Remove();
               else e.Marker.ShowInfoWindow();
            };
            Map.InfoWindowLongClick += (r, e) => { if(e.Marker.Draggable == true)e.Marker.Remove(); };
            Map.InfoWindowClose += (r, e) => { if (e.Marker.Title == "Моя позиція") e.Marker.Remove(); };
            Map.MyLocationClick += (r, e) =>
            {
                MarkerOptions marker = new MarkerOptions()
                .SetPosition(new LatLng(e.Location.Latitude,e.Location.Longitude))
                .SetTitle("Моя позиція")
                .SetSnippet(e.Location.Latitude+" "+e.Location.Longitude)
                .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_my_position));
                Map.AddMarker(marker).ShowInfoWindow(); ;
            };

            searchInit();
        }
        private async void setCamOnMyLoc()
        {
            Location location = await fused.GetLastLocationAsync();
            LatLng latlng = new LatLng(location.Latitude, location.Longitude);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(latlng);
            builder.Zoom(15);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            Map.AnimateCamera(cameraUpdate);
            //Map.MoveCamera(cameraUpdate);
        }
        private void setMarkers()
        {
            var table = database.database.Table<MarkerOptionsDBAdapter>();
            var result = from e in table
                         select e;
            foreach(var e in result)
            {
                Map.AddMarker(e.makeMarkerOption());
            }

        }
        private void databaseInit()
        {
            database.database.DeleteAll<MarkerOptionsDBAdapter>();

            MarkerOptions temp = new MarkerOptions()
                .SetPosition(new LatLng(48.530846, 25.044301))
                .SetTitle("Стріт арт")
                .Draggable(false);
            MarkerOptionsDBAdapter t = new MarkerOptionsDBAdapter(temp);
            database.database.Insert(t);
            temp.SetPosition(new LatLng(48.526984, 25.047339)).SetTitle("мистецтво та кава");
            t = new MarkerOptionsDBAdapter(temp);
            database.database.Insert(t);
            temp.SetPosition(new LatLng(48.524757, 25.039614)).SetTitle("Котики").SetSnippet("вниз по сходах");
            t = new MarkerOptionsDBAdapter(temp);
            database.database.Insert(t);
            temp.SetPosition(new LatLng(48.529163, 25.054485)).SetTitle("Стріт арт").SetSnippet("");
            t = new MarkerOptionsDBAdapter(temp);
            database.database.Insert(t);
            temp.SetPosition(new LatLng(48.527280, 25.078487)).SetTitle("Безлюдне місце");
            t = new MarkerOptionsDBAdapter(temp);
            database.database.Insert(t);
            Log.Info("count of database's fields", " "+ database.database.Table<MarkerOptionsDBAdapter>().Count());
        }
    }
}