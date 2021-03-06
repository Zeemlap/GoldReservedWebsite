﻿(function () {

    var g;
    var hasOwnPropF;
    var isFinite;
    var isPojo;
    var Object;
    var Object_prototype;
    var Object_prototype_toString;
    var Object_getPrototypeOf;
    var x;
    var XMLNS_SVG = "http://www.w3.org/2000/svg";
    var PI_OVER_180;
    var doubleFloor;

    g = window;
    isFinite = g.isFinite;
    Object = g.Object;
    Object_prototype = Object.prototype;
    Object_prototype_toString = Object_prototype.toString;
    hasOwnPropF = Object_prototype.hasOwnProperty;
    PI_OVER_180 = Math.PI / 180;
    doubleFloor = g.Math.floor;

    function isObject(value) {
        return (typeof value === "object" && value !== null) || typeof value === "function";
    }
    if (hasOwnPropF.call(Object, "getPrototypeOf")) {
        Object_getPrototypeOf = Object.getPrototypeOf;
        isPojo = function (value) {
            return value != null && Object_getPrototypeOf(value) === Object_prototype;
        };
    } else {
        isPojo = function (value) {
            return value != null && value.constructor == Object;
        };
    }
    if (!hasOwnPropF.call(g, "x")) {
        x = g.x = {};
    } else {
        x = g.x;
        if (!isPojo(x)) throw Error();
    }


    function Object_create_helper() {}
    if (!hasOwnPropF.call(Object, "create")) {
        Object.create = function create(prototype, properties) {
            var object;
            if (1 < arguments.length) throw Error();
            if (!isPojo(prototype)) throw Error();
            Object_create_helper.prototype = prototype;
            object = new Object_create_helper;
            Object_create_helper.prototype = Object_prototype;
            return object;
        };
    }

    function setOwnSrcPropsOnDst(src, dst) {
        var i;
        for (i in src) {
            if (!hasOwnPropF.call(src, i)) break;
            dst[i] = src[i];
        }
        return dst;
    }

    // arrayLike[, arrayOffset, subarrayLength], predicateFunc[, predicateThisp]
    function ArrayLike_findIndex(arrayLike, arrayOffset, subarrayLength, predicateFunc, predicateThisp) {
        var argN, i, n, f;
        argN = arguments.length;
        if (!isArrayLike(arrayLike)) throw Error();
        if (argN < 4) {
            predicateFunc = arrayOffset;
            predicateThisp = subarrayLength;
            arrayOffset = 0;
            subarrayLength = arrayLike.length;
        } else {
            i = arrayLike.length;
            if (typeof arrayOffset !== "number" || arrayOffset < 0 || arrayOffset % 1 !== 0 || !(arrayOffset <= i)) throw Error();
            if (typeof subarrayLength !== "number" || subarrayLength < 0 || !(subarrayLength % 1 === 0) || subarrayLength < i - arrayOffset) throw Error();
        }
        if (!isFunction(predicateFunc)) throw Error();
        for (i = arrayOffset, n = arrayOffset + subarrayLength; i < n; i++) {
            if (hasOwnPropF.call(arrayLike, i)) {
                f = predicateFunc.call(predicateThisp, arrayLike[i]);
                if (typeof f !== "boolean") throw Error();
                if (f) {
                    return i;
                }
            }
        }
        return -1;
    }

    function isFunction(value) {
        return Object_prototype_toString.call(value) === "[object Function]";
    }

    function formatNumberForOldCss(num) {
        if (-1E-6 < num) {
            if (num < 1E-6) {
                return "0";
            }
            if (num < 1E21) {
                return num + "";
            }
            return "999999999999999934463";
        }
        if (-1E21 < num) {
            return num + "";
        }
        return "-999999999999999934463";
    }

    function isArray(value) {
        return Object_prototype_toString.call(value) === "[object Array]";
    }
    function isArrayLike(value) {
        var n;
        if (typeof value !== "object" || value === null) {
            return false;
        }
        n = value.length;
        if (typeof n !== "number"
            || n < 0
            || 9007199254740992 < n
            || n % 1 !== 0
            || !hasOwnPropF.call(value, "length")) {
            return false;
        }
        return true;
    }
    function isArrayLike_nonSparse(value) {
        var n, i;
        if (!isArrayLike(value)) return false;
        n = value.length;
        for (i = 0; i < n; i++) {
            if (!hasOwnPropF.call(value, i)) return false;
        }
        return true;
    }

    function isDouble_finite(value) {
        return typeof value === "number" && isFinite(value);
    }
    function isDouble_integral(value) {
        return typeof value === "number" && value % 1 === 0;
    }
    function isDouble01(value) {
        return typeof value === "number" && !(value < 0) && value <= 1;
    }

    function __areDoublesEqual(x, y) {
        return x === y || (x !== x && y !== y);
    }

    function Vector2(x, y) {
        var argN;
        argN = arguments.length;
        if (argN == 0) {
            x = y = 0;
        } else if (1 < argN) {
            if (typeof x !== "number") throw Error();
            if (typeof y !== "number") throw Error();
        } else {
            if (x == null || x.constructor !== Vector2) throw Error();
            y = x.__y;
            x = x.__x;
        }
        this.__x = x;
        this.__y = y;
    }
    Vector2.prototype = {
        constructor: Vector2,
        assign: function(x, y) {
            var argN;
            argN = arguments.length;
            if (1 < argN) {
                if (typeof x !== "number"
                    || typeof y !== "number") throw Error();
            } else if (1 === argN) {
                if (x == null || x.constructor !== Vector2) throw Error();
                y = x.__y;
                x = x.__x;
            } else {
                throw Error();
            }
            this.__x = x;
            this.__y = y;
            return this;
        },
        equals: function (o) {
            if (o == null || o.constructor !== Vector2) return false;
            return __areDoublesEqual(this.__x, o.__x) && __areDoublesEqual(this.__y, o.__y);
        },
        getX: function () {
            return this.__x;
        },
        getY: function () {
            return this.__y;
        },
        setX: function (value) {
            if (typeof value !== "number") throw Error();
            this.__x = value;
        },
        setY: function (value) {
            if (typeof value !== "number") throw Error();
            this.__y = value;
        }
    };

    function getViewportSize() {
        var w, h;
        w = g.innerWidth;
        h = g.innerHeight;
        if (isDouble_finite(w) && isDouble_finite(h) && 0 <= w && 0 <= h) {
            return new Vector2(w, h);
        }
        throw Error();
    }

    function Rect2D(xmin, xmax, ymin, ymax) {
        var argN;
        argN = arguments.length;
        if (1 < argN) {
            if (typeof xmin !== "number" || xmin !== xmin
                || typeof xmax !== "number" || xmax !== xmax 
                || typeof ymin !== "number" || ymin !== ymin
                || typeof ymax !== "number" || ymax !== ymax
                || xmax < xmin
                || ymax < ymin) {
                throw Error();
            }
        } else if (1 === argN) {
            if (!(xmin instanceof Rect2D)) throw Error();
            xmax = xmin.__xmax;
            ymin = xmin.__ymin;
            ymax = xmin.__ymax;
            xmin = xmin.__xmin;
        } else {
            xmax = 0;
            xmin = 1;
            ymax = 0;
            ymin = 1;
        }
        this.__xmin = xmin;
        this.__xmax = xmax;
        this.__ymin = ymin;
        this.__ymax = ymax;
    }
    Rect2D.prototype = {
        constructor: Rect2D,
        assign: function (xmin, xmax, ymin, ymax) {
            var argN;
            argN = arguments.length;
            if (1 < argN) {
                if (typeof xmin !== "number" || xmin !== xmin
                    || typeof xmax !== "number" || xmax !== xmax 
                    || typeof ymin !== "number" || ymin !== ymin
                    || typeof ymax !== "number" || ymax !== ymax
                    || xmax < xmin
                    || ymax < ymin) {
                    throw Error();
                }
            } else {
                if (!(xmin instanceof Rect2D)) throw Error();
                xmax = xmin.__xmax;
                ymin = xmin.__ymin;
                ymax = xmin.__ymax;
                xmin = xmin.__xmin;
            }
            this.__xmin = xmin;
            this.__ymin = ymin;
            this.__xmax = xmax;
            this.__ymax = ymax;
            return this;
        },
        contains: function (x, y) {
            var argN;
            argN =  arguments.length;
            if (1 < argN) {
                if (typeof x !== "number" || typeof y !== "number") throw Error();
            } else {
                if (!(x instanceof Vector2)) throw Error();
                y = x.__y;
                x = x.__x;
            }
            if (!isFinite(x) || !isFinite(y)) return false;
            return this.__xmin <= x && x <= this.__xmax
                && this.__ymin <= y && y <= this.__ymax;
        },
        equals: function (o) {
            if (o == null || o.constructor !== Rect2D) return false;
            return this.__xmin === o.__xmin
                && this.__xmax === o.__xmax
                && this.__ymin === o.__ymin
                && this.__ymax === o.__ymax;
        },
        getIsEmpty: function () {
            return this.__xmax < this.__xmin;
        },
        getXMin: function () {
            return this.__xmin;
        },
        getYMin: function () {
            return this.__ymin;
        },
        getXMax: function () {
            return this.__xmax;
        },
        getYMax: function () {
            return this.__ymax;
        }
    };


    setOwnSrcPropsOnDst({
        ArrayLike_findIndex: ArrayLike_findIndex,
        formatNumberForOldCss: formatNumberForOldCss,
        getViewportSize: getViewportSize,
        isArray: isArray,
        isArrayLike: isArrayLike,
        isArrayLike_nonSparse: isArrayLike_nonSparse,
        isDouble_finite: isDouble_finite,
        isDouble_integral: isDouble_integral,
        isDouble01: isDouble01,
        isFunction: isFunction, 
        isObject: isObject,
        isPojo: isPojo,
        PI_OVER_180: PI_OVER_180,
        Rect2D: Rect2D, 
        setOwnSrcPropsOnDst: setOwnSrcPropsOnDst,
        Vector2: Vector2
    }, x);

    function HostElement_childNodes_clear(he) {
        var lc;
        while ((lc = he.lastChild) !== null) {
            he.removeChild(lc);
        }
    }

    setOwnSrcPropsOnDst({
        HostElement_childNodes_clear: HostElement_childNodes_clear
    }, x);


    function Color_rgbHtmlComponent(v1) {
        var v2;
        if (v1 < 1) {
            v2 = doubleFloor(v1 * 256);
            if (0xF < v2) return v2.toString(16);
            return "0" + v2.toString(16);
        }
        return "FF";
    }

    function hslToRgb_core(p, q, t) {
        if (t < 1 / 2) {
            if (t < 1 / 6) return p + (q - p) * 6 * t;
            return q;
        }
        if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
        return p;
    }

    function __Color1(r, g, b, a) {
        this.__r = r;
        this.__g = g;
        this.__b = b;

        this.__h = 0;
        this.__s = 0;
        this.__l = -1;

        this.__a = a;
    }
    function __Color2(h, s, l, a) {
        this.__r = 0;
        this.__g = 0;
        this.__b = -1;

        this.__h = h;
        this.__s = s;
        this.__l = l;

        this.__a = a;
    }

    function Color(r, g, b, a) {
        var argN;
        argN = arguments.length;
        if (1 < argN) {
            if (!isDouble01(r)
                || !isDouble01(g)
                || !isDouble01(b)
                || !isDouble01(a)) {
                throw Error();
            }
            this.__r = r;
            this.__g = g;
            this.__b = b;

            this.__h = 0;
            this.__s = 0;
            this.__l = -1;

            this.__a = a;
            return;
        }
        if (!(r instanceof Color)) {
            throw Error();
        }
        this.__r = r.__r;
        this.__g = r.__g;
        this.__b = r.__b;

        this.__h = r.__h;
        this.__s = r.__s;
        this.__l = r.__l;

        this.__a = r.__a;
    }
    Color.prototype = __Color1.prototype = __Color2.prototype = {
        constructor: Color,
        assign: function (r, g, b, a) {
            var argN;
            argN = arguments.length;
            if (1 < argN) {
                if (!isDouble01(r)
                    || !isDouble01(g)
                    || !isDouble01(b)
                    || !isDouble01(a)) {
                    throw Error();
                }
                this.__r = r;
                this.__g = g;
                this.__b = b;

                this.__h = 0;
                this.__s = 0;
                this.__l = -1;

                this.__a = a;
                return this;
            }
            if (!(r instanceof Color)) {
                throw Error();
            }
            this.__r = r.__r;
            this.__g = r.__g;
            this.__b = r.__b;

            this.__h = r.__h;
            this.__s = r.__s;
            this.__l = r.__l;

            this.__a = r.__a;
            return this;
        },
        __computeHsl: function () {
            var max, min;
            var d, i;
            if (this.__r <= this.__g) {
                if (this.__g <= this.__b) {
                    if (this.__r === this.__b) {
                        this.__l = this.__r;
                        return;
                    }
                    min = this.__r;
                    max = this.__b;
                    i = 2;
                } else {
                    min = this.__b < this.__r ? this.__b : this.__r;
                    max = this.__g;
                    i = 1;
                }
            } else {
                if (this.__r <= this.__b) {
                    min = this.__g;
                    max = this.__b;
                    i = 2;
                } else {
                    min = this.__g < this.__b ? this.__g : this.__b;
                    max = this.__r;
                    i = 0;
                }
            }
            this.__l = (max + min) * 0.5;
            d = max - min;
            this.__s = d / (0.5 < this.__l ? (2 - (max + min)) : max + min);
            switch (i) {
                case 0: this.__h = (this.__g - this.__b) / d + (this.__g < this.__b ? 6 : 0); break;
                case 1: this.__h = (this.__b - this.__r) / d + 2; break;
                case 2: this.__h = (this.__r - this.__g) / d + 4; break;
            }
            this.__h /= 6;
        },
        __computeRgb: function() {
            var q, p, t;
            if (this.__s !== 0) {
                q = this.__l < 0.5 ? this.__l * (1 + this.__s) : this.__l + this.__s - this.__l * this.__s;
                p = 2 * this.__l - q;
                t = this.__h + 1 / 3; if (1 < t) t -= 1;
                this.__r = hslToRgb_core(p, q, t);
                this.__g = hslToRgb_core(p, q, this.__h);
                t = this.__h - 1 / 3; if (t < 0) t += 1;
                this.__b = hslToRgb_core(p, q, t);
                return;
            }
            this.__r = this.__l;
            this.__g = this.__l;
            this.__b = this.__l;
        },
        equals: function (o) {
            if (o == null || o.constructor !== Color || this.__a !== o.__a) return false;
            if (this.__b !== -1) {
                if (o.__b === -1) o.__computeRgb();
                return this.__r === o.__r
                    && this.__g === o.__g
                    && this.__b === o.__b;
            }
            if (o.__l === -1) o.__computeHsl();
            return this.__h === o.__h
                && this.__s === o.__s
                && this.__l === o.__l;
        },

        getR: function () {
            if (this.__b === -1) this.__computeRgb();
            return this.__r;
        },
        getG: function () {
            if (this.__b === -1) this.__computeRgb();
            return this.__g;
        },
        getB: function () {
            if (this.__b === -1) this.__computeRgb();
            return this.__b;
        },

        getH: function () {
            if (this.__l === -1) this.__computeHsl();
            return this.__h;
        },
        getS: function () {
            if (this.__l === -1) this.__computeHsl();
            return this.__s;
        },
        getL: function () {
            if (this.__l === -1) this.__computeHsl();
            return this.__l;
        },

        getA: function () {
            return this.__a;
        },
        toString: function (format) {
            var argN;
            argN = arguments.length;
            if (argN === 0) format = "rgb_hexadecimal";
            switch (format) {
                case "rgb_hexadecimal":
                    if (this.__b === -1) this.__computeRgb();
                    return "#" + Color_rgbHtmlComponent(this.__r) + Color_rgbHtmlComponent(this.__g) + Color_rgbHtmlComponent(this.__b);
                case "hsla_css":
                    return "hsla(" + this.__toString_hslCssCore() + "," + this.__a + ")";            
                case "hsl_css":
                    return "hsl(" + this.__toString_hslCssCore() + ")";
                default:
                    throw Error();
            }
        },
        __toString_hslCssCore: function () {
            if (this.__l === -1) this.__computeHsl();
            return this.__h * 360 + "," + this.__s * 100 + "%," + this.__l * 100 + "%";
        }
    };
    Color.fromRgb = function (v) {
        var r, g, b;
        if (!isDouble_integral(v) || v < 0 || 0xFFFFFF < v) throw Error();
        r = (v >> 16) / 256;
        g = ((v >> 8) & 0xFF) / 256;
        b = (v & 0xFF) / 256;
        return new __Color1(r, g, b, 1);
    };
    Color.fromArgb = function (v) {
        var a, r, g, b;
        if (!isDouble_integral(v) || v < 0 || 0xFFFFFFFF < v) throw Error();
        a = (v >>> 24) / 256;
        r = ((v >> 16) & 0xFF) / 256;
        g = ((v >> 8) & 0xFF) / 256;
        b = (v & 0xFF) / 256;
        return new __Color(r, g, b, a);
    };
    Color.fromHsla = function (h, s, l, a) {
        var argN;
        if (!isDouble01(h)
            || !isDouble01(s)
            || !isDouble01(l)) {
            throw Error();
        }
        argN = arguments.length;
        if (argN < 4) a = 1;
        else if (!isDouble01(a)) throw Error();
        return new __Color2(h, s, l, a);
    };

    function __Color_interpolate(c1, c2, v) {
        var vi;
        if (c1.__l === -1) c1.__computeHsl();
        if (c2.__l === -1) c2.__computeHsl();
        vi = 1 - v;
        return new __Color2(
            c1.__h * vi + c2.__h * v,
            c1.__s * vi + c2.__s * v,
            c1.__l * vi + c2.__l * v,
            c1.__a * vi + c2.__a * v);
    }
    Color.interpolate = function (c1, c2, v) {
        if (!(c1 instanceof Color) || !(c2 instanceof Color) || !isDouble01(v)) {
            throw Error();
        }
        return __Color_interpolate(c1, c2, v);
    };

    function ColorMap(colors, isStepped) {
        var i, n, c;
        if (!isArrayLike_nonSparse(colors)
            || colors.length === 0
            || typeof isStepped !== "boolean") {
            throw Error();
        }
        n = colors.length;
        this.__colors = new Array(n);
        for (i = 0; i < n; i++) {
            c = colors[i];
            if (!(c instanceof Color)) throw Error();
            this.__colors[i] = new Color(c);
        }
        this.__isStepped = isStepped;
    }
    ColorMap.prototype = {
        getColor: function (v) {
            var i1, i2, c, cLen;
            if (!isDouble01(v)) throw Error();
            c = this.__colors;
            cLen = c.length;
            if (this.__isStepped) {
                i1 = doubleFloor(v * cLen);
                return i1 < cLen
                    ? c[i1]
                    : c[i1 - 1];
            }
            i1 = v * (cLen - 1);
            if (i1 !== cLen - 1) {
                i2 = doubleFloor(i1);
                return __Color_interpolate(c[i2], c[i2 + 1], i1 - i2);
            }
            return c[i1];
        },
        getIsStepped: function () {
            return this.__isStepped;
        }
    };

    ColorMap.ORANGE_FIVE_CLASS = new ColorMap([
        Color.fromRgb(0xfeedde),
        Color.fromRgb(0xfdbe85),
        Color.fromRgb(0xfd8d3c),
        Color.fromRgb(0xe6550d),
        Color.fromRgb(0xa63603)
    ], true);
    ColorMap.ORANGE = new ColorMap([
        Color.fromRgb(0xffffcc),
        Color.fromRgb(0xffeda0),
        Color.fromRgb(0xfed976),
        Color.fromRgb(0xfeb24c),
        Color.fromRgb(0xfd8d3c),
        Color.fromRgb(0xfc4e2a),
        Color.fromRgb(0xe31a1c),
        Color.fromRgb(0xbd0026),
        Color.fromRgb(0x800026)
    ], false);
    
    function ColorMap_toCssLinearGradientFunction(colorMap, dir) {
        var ca, i, n, str;
        if (!(colorMap instanceof ColorMap)
            || colorMap.getIsStepped()) throw Error();
        ca = colorMap.__colors;
        n = ca.length;
        if (n === 1) throw Error();
        str = "linear-gradient(" + dir;
        for (i = 0; i < n; i++) {
            str += "," + ca[i].toString(ca[i].getA() === 1 ? "hsl_css" : "hsla_css");
        }
        str += ")";
        return str;
    }

    setOwnSrcPropsOnDst({
        Color: Color,
        ColorMap: ColorMap,
        ColorMap_toCssLinearGradientFunction: ColorMap_toCssLinearGradientFunction
    }, x);






    function SvgHostElement_create(options) {
        var i, type, svgHostElement;
        if (!isPojo(options)) {
            throw Error();
        }

        if (!hasOwnPropF.call(options, "type") || typeof (type = options.type) !== "string") {
            throw Error();
        }
        svgHostElement = document.createElementNS(XMLNS_SVG, type);
        for (i in options) {
            if (!hasOwnPropF.call(options, i)) break;
            switch (i) {
                case "type": continue;
            }
            svgHostElement.setAttribute(i, options[i]);
        }
        return svgHostElement;
    }

    function SvgHostElement_setFillColorOnInlineStyle(svgHostElement, v) {
        var s;
        if (v !== null && !(v instanceof Color)) throw Error();
        s = svgHostElement.style;
        if (v !== null) {
            s.fill = v.toString("rgb_hexadecimal");
            s.fillOpacity = v.getA() + "";
        } else {
            s.fill = "";
            s.fillOpacity = "";
        }
    }

    function SvgHostElement_setStrokeColorOnInlineStyle(svgHostElement, v) {
        var s;
        if (v !== null && !(v instanceof Color)) throw Error();
        s = svgHostElement.style;
        if (v !== null) {
            s.stroke = v.toString("rgb_hexadecimal");
            s.strokeOpacity = v.getA() + "";
        } else {
            s.stroke = "";
            s.strokeOpacity = "";
        }
    }

    setOwnSrcPropsOnDst({
        SvgHostElement_create: SvgHostElement_create,
        SvgHostElement_setFillColorOnInlineStyle: SvgHostElement_setFillColorOnInlineStyle,
        SvgHostElement_setStrokeColorOnInlineStyle: SvgHostElement_setStrokeColorOnInlineStyle,
        XMLNS_SVG: XMLNS_SVG
    }, x);

})();






