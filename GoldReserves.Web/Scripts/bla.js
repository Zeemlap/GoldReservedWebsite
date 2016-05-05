(function() {

    function MyApp() {
        var i, n;
        this.__geoRegionTopographyTopoJson_xmlHttpRequest = null;
        this.__geoRegionTopographyTopoJson = null;
        this.__geoRegionViewModels = [
        @for (int i = 0, iLast = Model.Countries.Count - 1; i <= iLast; i++)
        {
            var geoRegion = Model.Countries[i];
                    @Html.Raw($"new GeoRegionViewModel(\"{Ajax.JavaScriptStringEncode(geoRegion.Code)}\"," +
                        $"{geoRegion.Latitude.ToString("R", System.Globalization.NumberFormatInfo.InvariantInfo)}," +
                        $"{geoRegion.Longitude.ToString("R", System.Globalization.NumberFormatInfo.InvariantInfo)}," +
                        $"\"{Ajax.JavaScriptStringEncode(geoRegion.Name)}\")");
            if (i < iLast)
            {
                @Html.Raw(",");
            }
        }
    ];

        this.__appGeoMap = new GeoMap();
        this.__initialize();
    }
    function MyApp_d3GeoMap_computeSize(maxWidth, maxHeight) {
        var ar;
        var h;
        var w;
        ar = 1.92;
        // height * 1.92 == width
        h = maxWidth / ar;
        w = maxHeight * ar;
        if (w < maxWidth && h < maxHeight) {
            if (w * maxHeight < h * maxWidth) {
                return new Vector2(maxWidth, h);
            }
            return new Vector2(w, maxHeight);
        }
        if (w < maxWidth) {
            return new Vector2(w, maxHeight);
        }
        if (maxHeight <= h) throw Error("should be unreachable");
        return new Vector2(maxWidth, h);
    }

    MyApp.prototype = {
        __windowOnResize: function () {
            this.__d3GeoMap_updateSize();
        },
        __getGeoRegionTopographyTopoJson_onReadyStateChange: function () {
            var req;
            req = this.__geoRegionTopographyTopoJson_xmlHttpRequest;
            if (req.readyState !== 4) return;
            this.__geoRegionTopographyTopoJson_xmlHttpRequest = null;
            if (req.status !== 200) throw Error();
            this.__geoRegionTopographyTopoJson = JSON.parse(req.responseText);
        },
        __initialize: function () {
            var req;
            req = new XMLHttpRequest();
            req.open("GET", "@Ajax.JavaScriptStringEncode(Url.Content("~/Content/geoRegionTopographyTopoJson.json"))");
            req.onreadystatechange = this.__getGeoRegionTopographyTopoJson_onReadyStateChange.bind(this);
            req.send();
            this.__geoRegionTopographyTopoJson_xmlHttpRequest = req;
        }
    };

        
})();