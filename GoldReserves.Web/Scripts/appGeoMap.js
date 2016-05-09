(function () {

    var g = this,
        x = g.x,
        hasOwnPropF = g.Object.prototype.hasOwnProperty,
        Vector2 = x.Vector2,
        LatLng = x.LatLng,
        Color = x.Color,
        GeoRegion = x.GeoRegion,
        INTERNAL_KEY = {},
        doubleMin = g.Math.min;
    INTERNAL_KEY = {};



    var DEFAULT_FILL_COLOR = Color.fromRgb((245 << 16) | (245 << 8) | 245);
    var DEFAULT_STROKE_COLOR = Color.fromRgb(0x888888);//Color.fromRgb((221 << 16) | (221 << 8) | 221);
    var DEFAULT_STROKE_WIDTH = 0.003;

    function AppGeoMap_View(svgHostElem) {
        this.__svgHostElem = svgHostElem;
        this.__key = INTERNAL_KEY;
        this.__fillColor = null;
        this.__strokeColor = null;
    }
    AppGeoMap_View.prototype = {
        constructor: AppGeoMap_View,
        getFillColor: function () {
            return this.__fillColor === null ? null : new Color(this.__fillColor);
        },
        getStrokeColor: function () {
            return this.__strokeColor === null ? null : new Color(this.__strokeColor);
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
            x.SvgHostElement_setFillColorOnInlineStyle(this.__svgHostElem, valueEffective);
        },
        setStrokeColor: function (value) {
            var valueEffective;
            if (value !== null && !(value instanceof Color)) throw Error();
            if (value === null) {
                this.__strokeColor = null;
                valueEffective = DEFAULT_STROKE_COLOR;
            } else {
                this.__strokeColor = new Color(value);
                valueEffective = value;
            }
            x.SvgHostElement_setStrokeColorOnInlineStyle(this.__svgHostElem, valueEffective);
        }
    };

    function AppGeoMap_GeoRegionView(geoRegion, hePath) {
        this.__geoRegion = geoRegion;
        AppGeoMap_View.call(this, hePath);
    }
    AppGeoMap_GeoRegionView.prototype = x.setOwnSrcPropsOnDst({
        constructor: AppGeoMap_GeoRegionView,
        getGeoRegion: function () {
            return this.__geoRegion;
        }
    }, Object.create(AppGeoMap_View.prototype));

    function AppGeoMap_CircleView(appGeoMap, radius) {
        AppGeoMap_View.call(this, x.SvgHostElement_create({
            type: "circle",
        }));
        this.__appGeoMap = appGeoMap;
        this.__radius = radius;
        this.__centerLatLng = new LatLng(0, 0);
        this.__obj = new AppGeoMap_Object();
        var c = this.__getCircle();
        this.__obj.__minMax2D = new MinMax2D();
        this.__obj.__svgHostElem = c;
        appGeoMap.__objects.push(this.__obj);
        x.SvgHostElement_setStrokeColorOnInlineStyle(c, DEFAULT_STROKE_COLOR);
        c.style.strokeWidth = DEFAULT_STROKE_WIDTH;
        c.setAttribute("r", this.__radius + "");
        this.__doCenterComputations();
        appGeoMap.__heG.appendChild(c);
    }
    AppGeoMap_CircleView.prototype = x.setOwnSrcPropsOnDst({
        constructor: AppGeoMap_CircleView,
        getCenterLatLng: function () {
            return new LatLng(this.__centerLatLng);
        },
        __getCircle: function() {
            return this.__svgHostElem;
        },
        __doCenterComputations: function () {
            var value = this.__centerLatLng;
            var v = new Vector2();
            this.__appGeoMap.getGeoProjection().project(value.getLat(), value.getLng(), v);
            var c = this.__getCircle();
            c.setAttribute("cx", v.getX() + "");
            c.setAttribute("cy", v.getY() + "");
            this.__obj.__minMax2D.__minX = v.getX() - this.__radius;
            this.__obj.__minMax2D.__maxX = v.getX() + this.__radius;
            this.__obj.__minMax2D.__minY = v.getY() - this.__radius;
            this.__obj.__minMax2D.__maxY = v.getY() + this.__radius;
            this.__appGeoMap.__updateLayout();
        },
        setCenterLatLng: function (value) {
            if (!(value instanceof LatLng)) throw Error();
            this.__centerLatLng = new LatLng(value);
            this.__doCenterComputations();
        }                            
    }, Object.create(AppGeoMap_View.prototype));

    function GeoProjectionContext(geoProjection) {
        this.__geoProjection = geoProjection;
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
            this.__geoProjection.project(latLng.getLat(), lng, v);
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

    function MinMax2D(minMax2D) {
        var argN;
        argN = arguments.length;
        if (argN === 1) {
            if (!(minMax2D instanceof MinMax2D)) throw Error();
            this.__minX = minMax2D.__minX;
            this.__minY = minMax2D.__minY;
            this.__maxX = minMax2D.__maxX;
            this.__maxY = minMax2D.__maxY;
            return;
        }
        this.__minX = 1 / 0;
        this.__minY = 1 / 0;
        this.__maxX = -1 / 0;
        this.__maxY = -1 / 0;
    }
    MinMax2D.prototype = {
        constructor: MinMax2D,
        getHeight: function() {
            return this.__maxY - this.__minY;
        },
        getIsFinite: function () {
            return this.__minX < 1 / 0
                && this.__minY < 1 / 0
                && -1 / 0 < this.__maxX
                && -1 / 0 < this.__maxY;
        },
        getWidth: function() {
            return this.__maxX - this.__minX;
        },
        union: function (o) {
            if (!(o instanceof MinMax2D)) {
                throw Error();
            }
            if (o.__minX < this.__minX) this.__minX = o.__minX;
            if (o.__minY < this.__minY) this.__minY = o.__minY;
            if (this.__maxX < o.__maxX) this.__maxX = o.__maxX;
            if (this.__maxY < o.__maxY) this.__maxY = o.__maxY;
            return this;
        }
    };

    function AppGeoMap_Object() {
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
        this.__geoProjection = x.NaturalEarthProjection.getInstance();
        heContainer.appendChild(this.__heSvgRoot);
    }
    AppGeoMap.prototype = {
        constructor: AppGeoMap,
        createCircle: function (radius) {
            var cv;
            if (arguments.length < 1) radius = 0.08;
            else if (!x.isDouble_finite(radius) || radius < 0) throw Error();
            cv = new AppGeoMap_CircleView(this, radius);
            return cv;
        },
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
                        if (v1.getX() < minMax2D.__minX) minMax2D.__minX = v1.getX();
                        if (minMax2D.__maxX < v2.getX()) minMax2D.__maxX = v2.getX();
                    } else {
                        if (v2.getX() < minMax2D.__minX) minMax2D.__minX = v2.getX();
                        if (minMax2D.__maxX < v1.getX()) minMax2D.__maxX = v1.getX();
                    }
                    if (v1.getY() < v2.getY()) {
                        if (v1.getY() < minMax2D.__minY) minMax2D.__minY = v1.getY();
                        if (minMax2D.__maxY < v2.getY()) minMax2D.__maxY = v2.getY();
                    } else {
                        if (v2.getY() < minMax2D.__minY) minMax2D.__minY = v2.getY();
                        if (minMax2D.__maxY < v1.getY()) minMax2D.__maxY = v1.getY();
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
            if (v1.getX() < minMax2D.__minX) minMax2D.__minX = v1.getX();
            else if (minMax2D.__maxX < v1.getX()) minMax2D.__maxX = v1.getX();
            if (v1.getY() < minMax2D.__minY) minMax2D.__minY = v1.getY();
            else if (minMax2D.__maxY < v1.getY()) minMax2D.__maxY = v1.getY();
        },
        getGeoProjection: function() {
            return this.__geoProjection;
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
            x.SvgHostElement_setStrokeColorOnInlineStyle(obj.__svgHostElem, DEFAULT_STROKE_COLOR);
            x.setOwnSrcPropsOnDst({
                "stroke-width": "0.003"
            }, obj.__svgHostElem.style);
            geoProjContext = new GeoProjectionContext(this.__geoProjection);
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
                obj.__svgHostElem.appendChild(hePath);
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
            if (!obj.__minMax2D.getIsFinite()) {
                throw Error();
            }
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
            var objects = this.__objects, i, n, obj;
            n = objects.length;
            if (n === 0) return;
            var minMax2D;
            minMax2D = new MinMax2D(this.__objects[0].__minMax2D);
            for (i = 1; i < n; i++) {
                obj = objects[i];
                minMax2D.union(obj.__minMax2D);
            }
            var dWidth_contSpace = minMax2D.getWidth();
            var dHeight_contSpace = minMax2D.getHeight();
            var transform = "translate(" + -(dWidth_contSpace * 0.5 + minMax2D.__minX) +
                "," + -(dHeight_contSpace * 0.5 + minMax2D.__minY) + ")";
            var s = 1 / dHeight_contSpace;
            var t = s + "";
            transform = "scale(" + t + "," + t + ") " + transform;
            var ar = minMax2D.getWidth() * s;
            transform = "translate(" + ar * 0.5 + ", 0.5) scale(1,-1) " + transform;
            var sxMax = lsWidth / ar;
            var syMax = lsHeight;
            s = doubleMin(sxMax, syMax);
            t = s + "";
            transform = "scale(" + t + "," + t + ") " + transform;
            transform = "translate(" + (lsWidth - s * ar) * 0.5 + "," + (lsHeight - s) * 0.5 + ") " + transform; 
            this.__heG.setAttribute("transform", transform);
        }
    };


    x.setOwnSrcPropsOnDst({
        AppGeoMap_View,
        AppGeoMap_GeoRegionView,
        AppGeoMap: AppGeoMap
    }, x);

})();