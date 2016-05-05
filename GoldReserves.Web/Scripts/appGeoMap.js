(function () {

    var g, x;
    var hasOwnPropF;
    var Vector2;
    var LatLng;
    var GeoRegion;
    var INTERNAL_KEY;
    g = this, x = g.x;
    hasOwnPropF = g.Object.prototype.hasOwnProperty;
    Vector2 = x.Vector2;
    LatLng = x.LatLng
    Color = x.Color;
    GeoRegion = x.GeoRegion;

    INTERNAL_KEY = {};
    function AppGeoMap_GeoRegionView(geoRegion, hePath) {
        this.__geoRegion = geoRegion;
        this.__hePath = hePath;
        this.__key = INTERNAL_KEY;
        this.__fillColor = null;
    }
    AppGeoMap_GeoRegionView.prototype = {
        constructor: AppGeoMap_GeoRegionView,
        getGeoRegion: function () {
            return this.__geoRegion;
        },
        getFillColor: function () {
            return this.__fillColor === null ? null : new Color(this.__fillColor);
        },
        setFillColor: function (value) {
            var valueEffective;
            if (value !== null && !(value instanceof Color)) throw Error();
            if (value === null) {
                this.__fillColor = null;
                valueEffective = DEFAULT_FILL_COLOR;
            } else {
                this.__fillColor = new Color(value);
                valueEffective = value;
            }
            x.SvgHostElement_setFillColorOnInlineStyle(this.__hePath, valueEffective);
        }
    };


    function GeoProjectionContext(geoProj) {
        this.__geoProj = geoProj;
        this.__requiredLngSign = null;
    }
    GeoProjectionContext.prototype = {
        project: function (latLng, v) {
            // requiredLngSign is null, -1 or 1
            var lng;
            lng = latLng.getLng();
            if (this.__requiredLngSign !== null) {
                if (this.__requiredLngSign < 0) {
                    if (0 <= lng) lng -= 360;
                } else {
                    if (lng < 0) lng += 360;
                }
            }
            this.__geoProj.project(latLng.getLat(), lng, v);
        },
        __updateForFeature: function (feature) {
            switch (feature.id) {
                case "FJI":
                    this.__requiredLngSign = 1;
                    break;
                case "RUS":
                    this.__requiredLngSign = 1;
                    break;
                default:
                    this.__requiredLngSign = null;
            }
        }
    };

    var DEFAULT_FILL_COLOR = Color.fromRgb(0xE5E5E5);

    function AppGeoMap_Object() {
        this.__aspectRatio = null;
        this.__minMax2D = null;
        this.__svgHostElem = null;
    }

    function AppGeoMap(heContainer) {
        this.__heContainer = heContainer;
        this.__heSvgRoot = x.SvgHostElement_create({
            type: "svg",
            xmlns: x.XMLNS_SVG,
            version: "1.1",
            "class": "app-geo-map"
        });
        this.__heG = x.SvgHostElement_create({
            type: "g"
        });
        this.__geoRegionViews = [];
        this.__geoRegionViewFromId_a3 = {};
        this.__isGeoRegionTopologyInitialized = false;
        this.__objects = [];
        this.__heSvgRoot.appendChild(this.__heG);
        this.__geoProj = x.NaturalEarthProjection.getInstance();
        heRoot.appendChild(this.__heSvgRoot);
    }
    AppGeoMap.prototype = {
        constructor: AppGeoMap,
        __geoJsonPolygonToPathData: function (geoProjContext, vArrayArray) {
            var i, iLast;
            var vArray;
            var j, n;
            var v, latLng, s;
            n = vArrayArray.length;
            s = "";
            for (j = 0; j < n; j++) {
                vArray = vArrayArray[j];
                iLast = vArray.length - 1;
                if (iLast < 3) throw Error();
                if (vArray[0][0] !== vArray[iLast][0] || vArray[0][1] !== vArray[iLast][1]) throw Error();
                v = new Vector2();
                latLng = new LatLng(vArray[0][1], vArray[0][0]);
                geoProjContext.project(latLng, v);

                s += "M" + v.getX() + "," + v.getY();
                for (i = 1; i < iLast; i++) {
                    latLng.assign(vArray[i][1], vArray[i][0]);
                    geoProjContext.project(latLng, v);
                    s += " " + v.getX() + "," + v.getY();
                }
                s += "Z";
            }
            return s;
        },
        __geoJsonPolygonUpdateMinMax2D: function (geoProjContext, vArrayArray, minMax2D) {
            var vArray;
            var i, r;
            var latLng1, latLng2;
            var v1, v2;
            if (vArrayArray.length === 0) throw Error();
            vArray = vArrayArray[0];
            r = vArray.length;
            i = 0;
            if (1 < r) {
                latLng1 = new LatLng(vArray[0][1], vArray[0][0]);
                latLng2 = new LatLng(vArray[1][1], vArray[1][0]);
                v1 = new Vector2();
                v2 = new Vector2();
                while (true) {
                    geoProjContext.project(latLng1, v1);
                    geoProjContext.project(latLng2, v2);
                    if (v1.getX() < v2.getX()) {
                        if (v1.getX() < minMax2D.__minx) minMax2D.__minx = v1.getX();
                        if (minMax2D.__maxx < v2.getX()) minMax2D.__maxx = v2.getX();
                    } else {
                        if (v2.getX() < minMax2D.__minx) minMax2D.__minx = v2.getX();
                        if (minMax2D.__maxx < v1.getX()) minMax2D.__maxx = v1.getX();
                    }
                    if (v1.getY() < v2.getY()) {
                        if (v1.getY() < minMax2D.__miny) minMax2D.__miny = v1.getY();
                        if (minMax2D.__maxy < v2.getY()) minMax2D.__maxy = v2.getY();
                    } else {
                        if (v2.getY() < minMax2D.__miny) minMax2D.__miny = v2.getY();
                        if (minMax2D.__maxy < v1.getY()) minMax2D.__maxy = v1.getY();
                    }
                    i += 2;
                    r -= 2;
                    if (r < 2) break;
                    latLng1.assign(vArray[i][1], vArray[i][0]);
                    latLng2.assign(vArray[i + 1][1], vArray[i + 1][0]);
                }
            }
            if (r === 0) return;
            if (i === 0) {
                latLng1 = new LatLng(vArray[0][1], vArray[0][0]);
                v1 = new Vector2();
            } else {
                latLng1.assign(vArray[0][1], vArray[0][0]);
            }
            geoProjContext.project(latLng1, v1);
            if (v1.getX() < minMax2D.__minx) minMax2D.__minx = v1.getX();
            else if (minMax2D.__maxx < v1.getX()) minMax2D.__maxx = v1.getX();
            if (v1.getY() < minMax2D.__miny) minMax2D.__miny = v1.getY();
            else if (minMax2D.__maxy < v1.getY()) minMax2D.__maxy = v1.getY();
        },
        getGeoRegionView: function (geoRegionId_alpha3) {
            var cv;
            if (typeof geoRegionId_alpha3 !== "string") throw Error();
            cv = this.__geoRegionViewFromId_a3[geoRegionId_alpha3];
            return cv != null && cv.__key === INTERNAL_KEY
                ? cv
                : null;
        },
        initializeGeoRegionTopography: function (geoJson_features) {
            var feature;
            var i, n;
            var geoRegion;
            var geometry;
            var hePath;
            var pathData, j, o;
            var geoProjContext;
            var obj;
            if (this.__isGeoRegionTopologyInitialized) throw Error(); // not supported multiple maps (aspectRatio needs to be kept per map)
            obj = new AppGeoMap_Object();
            obj.__svgHostElem = x.SvgHostElement_create({
                type: "g"
            });
            x.setOwnSrcPropsOnDst({
                stroke: "#555"/*"rgb(221, 221, 221)"*/,
                "stroke-width": "0.003"
            }, obj.__svgHostElem.style);
            geoProjContext = new GeoProjectionContext(this.__geoProj);
            n = geoJson_features.length;
            for (i = 0; i < n; i++) {
                feature = geoJson_features[i];
                geoProjContext.__updateForFeature(feature);
                geometry = feature.geometry;
                switch (geometry.type) {
                    case "MultiPolygon":
                        o = geometry.coordinates.length;
                        pathData = "";
                        for (j = 0; j < o; j += 1) {
                            pathData += this.__geoJsonPolygonToPathData(geoProjContext, geometry.coordinates[j]);
                        }
                        break;
                    case "Polygon":
                        pathData = this.__geoJsonPolygonToPathData(geoProjContext, geometry.coordinates);
                        break;
                    default:
                        throw Error();
                }
                hePath = x.SvgHostElement_create({
                    type: "path",
                    //id: feature.properties.name + "",
                    d: pathData
                });
                geoRegion = x.AppRepository.getInstance().getGeoRegion(feature.id);
                if (geoRegion !== null) {
                    this.__geoRegionViews.push(new AppGeoMap_GeoRegionView(geoRegion, hePath));
                }
                x.SvgHostElement_setFillColorOnInlineStyle(hePath, DEFAULT_FILL_COLOR);
                heG.appendChild(hePath);
            }
            obj.__minMax2D = new MinMax2D();
            for (i = 0; i < n; i++) {
                feature = geoJson_features[i];
                geoProjContext.__updateForFeature(feature);
                geometry = feature.geometry;
                switch (geometry.type) {
                    case "MultiPolygon":
                        o = geometry.coordinates.length;
                        for (j = 0; j < o; j += 1) {
                            this.__geoJsonPolygonUpdateMinMax2D(geoProjContext, geometry.coordinates[j], obj.__minMax2D);
                        }
                        break;
                    case "Polygon":
                        this.__geoJsonPolygonUpdateMinMax2D(geoProjContext, geometry.coordinates, obj.__minMax2D);
                        break;
                    default:
                        throw Error();
                }
            }
            if (obj.__minMax2D.__minx === 1 / 0
                || obj.__minMax2D.__maxx === -1 / 0
                || obj.__minMax2D.__miny === 1 / 0
                || obj.__minMax2D.__maxy === -1 / 0) {
                throw Error();
            }

            obj.__aspectRatio = (obj.__minMax2D.__maxx - obj.__minMax2D.__minx) / (obj.__minMax2D.__maxy - obj.__minMax2D.__miny);
            this.__objects.push(obj);
            this.__initializeGeoRegionViewsIndexes();
            this.__isGeoRegionTopologyInitialized = true;
            this.__updateLayout();
            this.__heG.appendChild(obj.__svgHostElem);
        },
        __initializeGeoRegionViewsIndexes: function () {
            var a, i, n, t, c;
            a = this.__geoRegionViews;
            t = this.__geoRegionViewFromId_a3;
            for (i = 0, n = a.length; i < n; i++) {
                c = a[i];
                t[c.getGeoRegion().getId_alpha3()] = c;
            }
        },
        notifyOfPotentialSizeChange: function () {
            this.__updateLayout();
        },

        __updateLayout: function () {
            var lsWidth, lsHeight;
            lsWidth = this.__heContainer.clientWidth;
            lsHeight = this.__heContainer.clientHeight;
            this.__heSvgRoot.setAttribute("width", lsWidth + "");
            this.__heSvgRoot.setAttribute("height", lsHeight + "");
            var objects = this.__objects;
            var n = objects.length;
            var i;
            if (n === 0) return;
            var minMax2D, sx1, sy1, s, tx, ty;
            minMax2D = new MinMax2D(this.__objects[0].__minMax2D);
            for (i = 1; i <= n; i++) {

            }
            var xExtent = obj.__minMax2D.__maxx - obj.__minMax2D.__minx;
            var yExtent = obj.__minMax2D.__maxy - obj.__minMax2D.__miny;
            var s = 1 / yExtent;
            var tx = -(xExtent * 0.5 + obj.__minMax2D.__minx);
            var ty = -(yExtent * 0.5 + obj.__minMax2D.__miny);
            obj.__svgHostElem.setAttribute("transform", "scale(" + s + "," + s + ") translate(" + tx + "," + ty + ")");


            sx1 = lsWidth / this.__geoRegionTopographyAspectRatio;
            sy1 = lsHeight;
            s = Math.min(sx1, sy1);
            tx = (lsWidth - s * this.__geoRegionTopographyAspectRatio) * 0.5;
            ty = (lsHeight - s) * 0.5;
            this.__heG.setAttribute("transform", "translate(" + tx + "," + ty + ") scale(" + s + "," + s + ") translate(" + this.__geoRegionTopographyAspectRatio * 0.5 + ", 0.5) scale(1,-1)");
        }
    };


    x.setOwnSrcPropsOnDst({
        AppGeoMap: AppGeoMap
    }, x);

})();