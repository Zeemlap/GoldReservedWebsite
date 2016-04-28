(function () {

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

    g = window;
    isFinite = g.isFinite;
    Object = g.Object;
    Object_prototype = Object.prototype;
    Object_prototype_toString = Object_prototype.toString;
    hasOwnPropF = Object_prototype.hasOwnProperty;
    PI_OVER_180 = Math.PI / 180;

    function isObject(value) {
        return typeof value === "object" && value !== null;
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
            return __areDoublesEqual(v.__x, o.__x) && __areDoublesEqual(v.__y, o.__y);
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
        formatNumberForOldCss: formatNumberForOldCss,
        getViewportSize: getViewportSize,
        isArray: isArray,
        isArrayLike: isArrayLike,
        isArrayLike_nonSparse: isArrayLike_nonSparse,
        isDouble_finite: isDouble_finite,
        isDouble_integral: isDouble_integral,
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

    setOwnSrcPropsOnDst({
        SvgHostElement_create: SvgHostElement_create,
        XMLNS_SVG: XMLNS_SVG
    }, x);

})();






