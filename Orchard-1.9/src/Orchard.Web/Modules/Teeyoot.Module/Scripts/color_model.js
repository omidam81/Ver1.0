    function ColorModel(e) {
        this.value = "#000000",
            this.id = "",
            this.name = "",
            this.lab = null,
            this._hex = null,
            this.heather = !1,
            this.origin = null,
            this.texture = "",
            this.rgb = null,
        e && (("string" == typeof e.heather || "number" == typeof e.heather) && (e.heather = 1 === parseInt(e.heather)),
            this.updateWith(e), e.rgb && this.setRGB(e.rgb), !e.rgb && e.lab && this.setLAB(e.lab))
    }

    ColorModel.prototype = {
        "__name": "Color",
        "toJSON": function () {
            return {
                "id": this.id,
                "value": this.value,
                "origin": this.origin,
                "heather": this.heather,
                "texture": this.texture
            }
        },
        "setRGB": function (e, t, a) {
            if (e) {
                var n, r, i = e[0], o = e[1], s = e[2];
                this._reset(), t && (n = t / 255, r = a ? a : [255, 255, 255], i = Math.round((1 - n) * r[0] + n * i), o = Math.round((1 - n) * r[1] + n * o), s = Math.round((1 - n) * r[2] + n * s)), this.value = "rgb(" + i + "," + o + "," + s + ")"
            }
        },
        "getRGB": function () {
            var e, t = this.value;
            if (t)return t && t.constructor == Array && 3 == t.length ? t : (e = /rgb\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*\)/.exec(t)) ? [parseInt(e[1]), parseInt(e[2]), parseInt(e[3])] : (e = /rgb\(\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*\)/.exec(t)) ? [2.55 * parseFloat(e[1]), 2.55 * parseFloat(e[2]), 2.55 * parseFloat(e[3])] : (e = /#([a-fA-F0-9]{2})([a-fA-F0-9]{2})([a-fA-F0-9]{2})/.exec(t)) ? [parseInt(e[1], 16), parseInt(e[2], 16), parseInt(e[3], 16)] : (e = /#([a-fA-F0-9])([a-fA-F0-9])([a-fA-F0-9])/.exec(t), e ? [parseInt(e[1] + e[1], 16), parseInt(e[2] + e[2], 16), parseInt(e[3] + e[3], 16)] : !1)
        },
        "getHex": function () {
            if (this._hex)return this._hex;
            var e, t = this.getRGB();
            for (e = 0; 3 > e; ++e)t[e] = parseInt(t[e]).toString(16), 1 == t[e].length && (t[e] = "0" + t[e]);
            return this._hex = "#" + t.join(""), this._hex
        },
        "getHSL": function () {
            var e, t, a, n, r = this.getRGB(), i = r[0] / 255, o = r[1] / 255, s = r[2] / 255, l = Math.max(i, o, s), u = Math.min(i, o, s), c = 0, d = 0, h = 0;
            return h = (u + l) / 2, 0 >= h ? [c, d, h] : (e = l - u, d = e, d > 0 ? (d /= .5 >= h ? l + u : 2 - l - u, t = (l - i) / e, a = (l - o) / e, n = (l - s) / e, c = i == l ? o == u ? 5 + n : 1 - a : o == l ? s == u ? 1 + t : 3 - n : i == u ? 3 + a : 5 - t, c /= 6, [c, d, h]) : [c, d, h])
        },
        "getXYZ": function () {
            var e = this.getRGB(), t = e[0] / 255, a = e[1] / 255, n = e[2] / 255;
            t = t > .04045 ? Math.pow((t + .055) / 1.055, 2.4) : t / 12.92, a = a > .04045 ? Math.pow((a + .055) / 1.055, 2.4) : a / 12.92, n = n > .04045 ? Math.pow((n + .055) / 1.055, 2.4) : n / 12.92;
            var r = .4124 * t + .3576 * a + .1805 * n, i = .2126 * t + .7152 * a + .0722 * n, o = .0193 * t + .1192 * a + .9505 * n;
            return [100 * r, 100 * i, 100 * o]
        },
        "getLAB": function () {
            if (this.lab)return this.lab;
            var e, t, a, n = this.getXYZ(), r = n[0], i = n[1], o = n[2];
            return r /= 95.047, i /= 100, o /= 108.883, r = r > .008856 ? Math.pow(r, 1 / 3) : 7.787 * r + 16 / 116, i = i > .008856 ? Math.pow(i, 1 / 3) : 7.787 * i + 16 / 116, o = o > .008856 ? Math.pow(o, 1 / 3) : 7.787 * o + 16 / 116, e = 116 * i - 16, t = 500 * (r - i), a = 200 * (i - o), this.lab = [e, t, a], [e, t, a]
        },
        "_reset": function () {
            this.lab = null, this._hex = null
        },
        "setLAB": function (e) {
            var t, a, n, r = e[0], i = e[1], o = e[2];
            return this._reset(), this.lab = e, a = (r + 16) / 116, t = i / 500 + a, n = a - o / 200, a = Math.pow(a, 3) > .008856 ? Math.pow(a, 3) : (a - 16 / 116) / 7.787, t = Math.pow(t, 3) > .008856 ? Math.pow(t, 3) : (t - 16 / 116) / 7.787, n = Math.pow(n, 3) > .008856 ? Math.pow(n, 3) : (n - 16 / 116) / 7.787, t *= 95.047, a *= 100, n *= 108.883, this.setXYZ([t, a, n])
        },
        "setXYZ": function (e) {
            var t, a, n, r = e[0] / 100, i = e[1] / 100, o = e[2] / 100;
            return t = 3.2406 * r + -1.5372 * i + o * -.4986, a = r * -.9689 + 1.8758 * i + .0415 * o, n = .0557 * r + i * -.204 + 1.057 * o, t = t > .0031308 ? 1.055 * Math.pow(t, 1 / 2.4) - .055 : t = 12.92 * t, a = a > .0031308 ? 1.055 * Math.pow(a, 1 / 2.4) - .055 : a = 12.92 * a, n = n > .0031308 ? 1.055 * Math.pow(n, 1 / 2.4) - .055 : n = 12.92 * n, t = 0 > t ? 0 : Math.min(Math.round(255 * t), 255), a = 0 > a ? 0 : Math.min(Math.round(255 * a), 255), n = 0 > n ? 0 : Math.min(Math.round(255 * n), 255), this.value = "rgb(" + t + "," + a + "," + n + ")", [t, a, n]
        },
        "getTolerance": function () {
            var e, t = 1.3, a = 3, n = 9, r = this.getLAB();
            return t *= Math.max(1, 4 * Math.pow(r[2] / 100, 2)), e = (100 - r[0]) / 100, e *= t, e *= n - a, e += a
        },
        "getDistanceTo": function (e) {
            var t = this.getLAB(), n = e instanceof ColorModel ? e.getLAB() : e.lab;
            return ciede.distance(t, n)
        },
        "updateWith": function (t) {
            var a
            for (a in t) t.hasOwnProperty(a) && ("string" != typeof t[a] || null !== this[a] && "number" != typeof this[a] || (t[a] = isNaN(parseFloat(t[a])) ?
                t[a] : parseFloat(t[a])), (typeof t[a] == typeof this[a] || "number" == typeof t[a] && null === this[a]) && (this[a] = "object" == typeof t[a]
            && !(t[a] instanceof Array) && a.toLowerCase().indexOf("color") >= 0 ? new n(t[a]) : t[a]))
        }
    };
