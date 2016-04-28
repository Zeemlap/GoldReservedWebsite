(function() {

    function TopoJson(data, _2DOnly) {
        var i, n, transform;
        if (!x.isPojo(data)
            || _2DOnly !== true) throw Error();
        if (data.type !== "Topology"
            || !hasOwnPropF.call(data, "type")) throw Error();

        this.__sx = 1;
        this.__sy = 1;
        this.__tx = 0;
        this.__ty = 0;
        if ((transform = data.transform) !== undefined
            && hasOwnPropF.call(data, "transform")) {
            for (i in transform) {
                if (!hasOwnPropF.call(transform, i)) break;
                if (!__TopoJson_isPosValid_2DOnly_partial(transform[i])
                    || transform[i].length !== 2
                    || !isFinite(transform[i][0])
                    || !isFinite(transform[i][1])) throw Error();
                switch (i) {
                    case "scale":
                        this.__sx = transform[i][0];
                        this.__sy = transform[i][1];
                        break;
                    case "transform":
                        this.__sx = transform[i][0];
                        this.__sy = transform[i][1];
                        break;
                    default:
                        throw Error();
                }
            }
        }

        this.__arcs = data.arcs;
        if (!x.isArrayLike_nonSparse(this.__arcs)
            || !hasOwnPropF.call(data, "arcs")) throw Error();
        n = this.__arcs.length;
        for (i = 0; i < n; i++) {
            if (!TopoJson_isArcValid_2DOnly(this.__arcs[i])) throw Error();
        }

        this.__objects = data.objects;
        if (!x.isPojo(this.__objects)) throw Error();
        for (i in this.__objects) {
            if (!hasOwnPropF.call(this.__objects, i)) break;

        }
    }

    function __TopoJson_isPosValid_2DOnly_partial(pos) {
        var n;
        if (typeof pos !== "object" || pos === null) return false;
        if (!hasOwnPropF.call(pos, "length")
            || typeof (n = pos.length) !== "number"
            || n < 2 || !(n % 1 === 0)
            || typeof pos[0] !== "number"
            || typeof pos[1] !== "number") return false;
        return true;
    }
    function TopoJson_isArcValid_2DOnly(arc) {
        var i, n, x, y;
        if (!x.isArrayLike_nonSparse(arc)) return false;
        n = arc.length;
        if (n === 0 || !__TopoJson_isPosValid_2DOnly_partial(arc[0])) return false;
        x = arc[0][0];
        y = arc[0][1];
        for (i = 1; i < n; i++) {
            if (!__TopoJson_isPosValid_partial(arc[i])) return false;
            x += arc[i][0];
            y += arc[i][1];
        }
        if (!isFinite(x)
            || !isFinite(y)) return false;
        return true;
    }

    function AppGeoMap_topoJsonToSvg(topoJson) {
        var objects, object, i;
        if (!x.isPojo(objects = topoJson.objects)) throw Error();

        for (i in objects) {
            if (!hasOwnPropF(objects, i)) break;
            object = objects[i];
            if (!hasOwnPropF.call(object, "type")) throw Error();
            switch (object.type) {
                case "Point":
                    break;
                case "MultiPoint":
                    break;
                case "LineString":
                    break;
                case "MultiLineString":
                    break;
                case "Polygon":

                    break;
                case "MultiPolygon":
                    break;
                case "GeometryCollection":
                    break;
                default:
                    throw Error();
            }
        }
    }


    function MyApp() {
        var i, n;
        this.__countryTopographyTopoJson_xmlHttpRequest = null;
        this.__countryTopographyTopoJson = null;
        this.__countryViewModels = [
        @for (int i = 0, iLast = Model.Countries.Count - 1; i <= iLast; i++)
        {
            var country = Model.Countries[i];
                    @Html.Raw($"new CountryViewModel(\"{Ajax.JavaScriptStringEncode(country.Code)}\"," +
                        $"{country.Latitude.ToString("R", System.Globalization.NumberFormatInfo.InvariantInfo)}," +
                        $"{country.Longitude.ToString("R", System.Globalization.NumberFormatInfo.InvariantInfo)}," +
                        $"\"{Ajax.JavaScriptStringEncode(country.Name)}\")");
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
        __getCountryTopographyTopoJson_onReadyStateChange: function () {
            var req;
            req = this.__countryTopographyTopoJson_xmlHttpRequest;
            if (req.readyState !== 4) return;
            this.__countryTopographyTopoJson_xmlHttpRequest = null;
            if (req.status !== 200) throw Error();
            this.__countryTopographyTopoJson = JSON.parse(req.responseText);
        },
        __initialize: function () {
            var req;
            req = new XMLHttpRequest();
            req.open("GET", "@Ajax.JavaScriptStringEncode(Url.Content("~/Content/countryTopographyTopoJson.json"))");
            req.onreadystatechange = this.__getCountryTopographyTopoJson_onReadyStateChange.bind(this);
            req.send();
            this.__countryTopographyTopoJson_xmlHttpRequest = req;
        }
    };

        
})();