(function () {

    var g;
    var hasOwnPropF;
    var isPojo;
    var Object;
    var Object_prototype;
    var Object_prototype_toString;
    var Object_getPrototypeOf;
    var x;

    g = window;
    Object = g.Object;
    Object_prototype = Object.prototype;
    Object_prototype_toString = Object_prototype.toString;
    hasOwnPropF = Object_prototype.hasOwnProperty;

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

    

    setOwnSrcPropsOnDst({
        formatNumberForOldCss: formatNumberForOldCss,
        isArray: isArray,
        isArrayLike: isArrayLike,
        isArrayLike_nonSparse: isArrayLike_nonSparse,
        isObject: isObject,
        isPojo: isPojo,
        setOwnSrcPropsOnDst: setOwnSrcPropsOnDst
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
        svgHostElement = document.createElementNS("http://www.w3.org/2000/svg", type);
        for (i in options) {
            if (!hasOwnPropF.call(options, i)) break;
            switch (i) {
                case "tagName": continue;
            }
            svgHostElement.setAttribute(i, options[i]);
        }
        return svgHostElement;
    }

    setOwnSrcPropsOnDst({
        SvgHostElement_create: SvgHostElement_create
    }, x);

})();






