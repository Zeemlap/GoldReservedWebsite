(function () {

    var g = this,
        x = g.x,
        undefined,
        hasOwnPropF = g.Object.prototype.hasOwnProperty,
        isArrayLike = x.isArrayLike,
        isDouble_finite = x.isDouble_finite,
        
        NE_A0 = 0.8707,
        NE_A1 = -0.131979,
        NE_A2 = -0.013791,
        NE_A3 = 0.003971,
        NE_A4 = -0.001529,
        NE_B0 = 1.007226,
        NE_B1 = 0.015085,
        NE_B2 = -0.044475,
        NE_B3 = 0.028874,
        NE_B4 = -0.005916,
        NE_C0 = NE_B0,
        NE_C1 = 3 * NE_B1,
        NE_C2 = 7 * NE_B2,
        NE_C3 = 9 * NE_B3,
        NE_C4 = 11 * NE_B4,
        NE_EPS = 1e-11,
        NE_instance,
    
        NE_MAX_Y = 0.8707 * 0.52 * Math.PI,

        PI_OVER_180 = x.PI_OVER_180,
        Rect2D = x.Rect2D,
        Vector2 = x.Vector2;
            
    

    function LatLng(lat, lng) {
        var argN;
        argN = arguments.length;
        if (1 < argN) {
            if (typeof lat !== "number" || lat < -90 || 90 < lat || lat !== lat
                || typeof lng !== "number" || lat < -180 || !(lat < 180)) {
                throw Error();
            }
        } else if (argN === 0) {
            lat = lng = 0;
        } else {
            if (lat == null || lat.constructor !== LatLng) throw Error();
            lng = lat.__lng;
            lat = lat.__lat;
        }
        this.__lat = lat;
        this.__lng = lng;
    }
    LatLng.prototype = {
        constructor: LatLng,
        assign: function (lat, lng) {             
            var argN;
            argN = arguments.length;
            if (1 < argN) {
                if (typeof lat !== "number" || lat < -90 || 90 < lat || lat !== lat
                    || typeof lng !== "number" || lat < -180 || !(lat < 180)) {
                    throw Error();
                }
            } else {
                if (lat == null || lat.constructor !== LatLng) throw Error();
                lng = lat.__lng;
                lat = lat.__lat;
            }
            this.__lat = lat;
            this.__lng = lng;
            return this;
        },
        equals: function (o) {
            if (o == null || o.constructor !== LatLng) return false;
            return this.__lat === o.__lat && this.__lng === o.__lng;
        },
        getLat: function () { return this.__lat; },
        getLng: function () { return this.__lng; },
        setLat: function (value) {
            if (typeof value !== "number" || value < -90 || 90 < value || value !== value) throw Error();
            this.__lat = value;
        },
        setLng: function (value) {
            if (typeof value !== "number" || value < -180 || !(value < 180)) throw Error();
            this.__lng = value;
        }
    };



    function GeoProjection() {
        this.__euclideanSpaceBounds = null;
    }
    GeoProjection.prototype = {
        constructor: GeoProjection,
        getEuclideanSpaceBounds: function (r) {
            var latLng, v;
            var xmin, ymin;
            if (!(r instanceof Rect2D)) {
                throw Error();
            }
            if (this.__euclideanSpaceBounds !== null) {
                return r.assign(this.__euclideanSpaceBounds);
            }
            latLng = new LatLng(-90, -180);
            v = new Vector2();
            this.project(latLng, v);
            xmin = v.getX();
            ymin = v.getY();
            latLng.assign(90, x.DoubleUtilities.getInstance().getMaximumValueSmallerThan(180));
            this.project(latLng, v);
            return r.assign(xmin, v.getX(), ymin, v.getY());
        },
        getHasInverse: function () {
            return false;
        },
        project: function (lat, lng, v) {
            if (typeof lat !== "number" || lat < -90 || 90 < lat || lat !== lat) {
                throw Error();
            }
            if (!isDouble_finite(lng)) {
                throw Error();
            }
            return this.__projectCore(
                lat * PI_OVER_180, 
                lng * PI_OVER_180,
                v);
        },
        __projectCore: function(phi, lam) {
            throw Error();
        },
        projectInverse: function (v, latLng) {
            throw Error();
        }
    };

    function __NaturalEarthProjection() {
        GeoProjection.call(this);
    }
    function NaturalEarthProjection() { }
    NaturalEarthProjection.prototype = __NaturalEarthProjection.prototype = x.setOwnSrcPropsOnDst({
        getHasInverse: function () {
            return true;
        },
        __projectCore: function (lpphi, lplam, v) {
            var phi2, phi4;
            phi2 = lpphi * lpphi;
            phi4 = phi2 * phi2;
            v.assign(
                lplam * (NE_A0 + phi2 * (NE_A1 + phi2 * (NE_A2 + phi4 * phi2 * (NE_A3 + phi2 * NE_A4)))),
                lpphi * (NE_B0 + phi2 * (NE_B1 + phi4 * (NE_B2 + NE_B3 * phi2 + NE_B4 * phi4))));
            return v;
        },
        projectInverse: function (v, latLng) {
            var x, y, yc, tol, y2, y4, f, fder, phi;
            if (!(v instanceof Vector2)
                || !(latLng instanceof LatLng)) {
                throw Error();
            }
            y = v.getY();
            x = v.getX();
            // make sure y is inside valid range
            if (y > NE_MAX_Y) {
                y = NE_MAX_Y;
            } else if (y < -NE_MAX_Y) {
                y = -NE_MAX_Y;
            }

            // latitude
            yc = y;
            for (; ;) { // Newton-Raphson
                y2 = yc * yc;
                y4 = y2 * y2;
                f = (yc * (NE_B0 + y2 * (NE_B1 + y4 * (NE_B2 + NE_B3 * y2 + NE_B4 * y4)))) - y;
                fder = NE_C0 + y2 * (NE_C1 + y4 * (NE_C2 + NE_C3 * y2 + NE_C4 * y4));
                tol = f / fder;
                yc -= tol;
                if (abs(tol) < NE_EPS) {
                    break;
                }
            }
            latLng.setLat(yc);

            // longitude
            y2 = yc * yc;
            phi = NE_A0 + y2 * (NE_A1 + y2 * (NE_A2 + y2 * y2 * y2 * (NE_A3 + y2 * NE_A4)));
            latLng.setLng(x / phi);
            return latLng;
        }
    }, Object.create(GeoProjection.prototype));

    NaturalEarthProjection.getInstance = function () {
        if (NE_instance !== undefined) return NE_instance;
        return NE_instance = new __NaturalEarthProjection();
    };

    x.setOwnSrcPropsOnDst({
        GeoProjection: GeoProjection,
        LatLng: LatLng,
        NaturalEarthProjection: NaturalEarthProjection,
        Rect2D: Rect2D
    }, x);



    var Country_fromId_isoTwoLetterCode = {};
    var Country_fromId_isoThreeLetterCode = {};
    var INTERNAL_KEY = Country_fromId_isoTwoLetterCode;
    function Country() {
        this.__name_english = null;
        this.__id_isoTwoLetterCode = null;
        this.__id_isoThreeLetterCode = null;
        this.__key = INTERNAL_KEY;
    }
    Country.prototype = {
        getName_english: function () {
            return this.__name_english;
        },
        __setName_english: function(value) {
            this.__name_english = value;
        },
        getId_isoTwoLetterCode: function () {
            return this.__id_isoTwoLetterCode;
        },
        __setId_isoTwoLetterCode: function(value) {
            this.__id_isoTwoLetterCode = value;
        },
        getId_isoThreeLetterCode: function () {
            return this.__id_isoThreeLetterCode;
        },
        __setId_isoThreeLetterCode: function (value) {
            this.__id_isoThreeLetterCode = value;
        }
    };

    Country.get = function (v) {
        // v is a two letter iso identification code
        // OR v is a three letter iso identification code
        var country, vLen;
        if (typeof v !== "string" || (vLen = v.length) < 2 || 3 < vLen) throw Error();
        country = vLen === 2
            ? Country_fromId_isoTwoLetterCode[v]
            : Country_fromId_isoThreeLetterCode[v];
        return country != null && country.__key === INTERNAL_KEY
            ? country
            : null;
    };
    Country.getAll = function () {
        var i, a, j;
        a = [];
        j = 0;
        for (i in Country_fromId_isoTwoLetterCode) {
            if (!hasOwnPropF.call(Country_fromId_isoTwoLetterCode, i)) break;
            a[j++] = Country_fromId_isoTwoLetterCode[i];
        }
        return a;
    };
    Country.__registerAll = function (countries) {
        var i, n, country;
        if (!isArrayLike(countries)) throw Error();
        for (i = 0, n = countries.length; i < n && hasOwnPropF.call(countries, i); i++) {
            country = countries[i];
            if (!(country instanceof Country)) {
                throw Error();
            }
            if (hasOwnPropF.call(Country_fromId_isoTwoLetterCode, country.getId_isoTwoLetterCode())
                || hasOwnPropF.call(Country_fromId_isoThreeLetterCode, country.getId_isoThreeLetterCode())) {
                throw Error();
            }
            Country_fromId_isoTwoLetterCode[country.getId_isoTwoLetterCode()] = country;
            Country_fromId_isoThreeLetterCode[country.getId_isoThreeLetterCode()] = country;
        }
    };

    x.setOwnSrcPropsOnDst({
        Country: Country
    }, x);

})();
