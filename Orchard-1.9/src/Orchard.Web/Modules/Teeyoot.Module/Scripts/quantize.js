//183
/*!
 * quantize.js Copyright 2008 Nick Rabinowitz.
 * Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
 */
    /*!
     * Block below copied from Protovis: http://mbostock.github.com/protovis/
     * Copyright 2010 Stanford Visualization Group
     * Licensed under the BSD License: http://www.opensource.org/licenses/bsd-license.php
     */
    if (!protoArr)var protoArr = {
        "map": function (e, t) {
            var a = {};
            return t ? e.map(function (e, n) {
                return a.index = n, t.call(a, e)
            }) : e.slice()
        }, "naturalOrder": function (e, t) {
            return t > e ? -1 : e > t ? 1 : 0
        }, "sum": function (e, t) {
            var a = {};
            return e.reduce(t ? function (e, n, r) {
                return a.index = r, e + t.call(a, n)
            } : function (e, t) {
                return e + t
            }, 0)
        }, "max": function (e, t) {
            return Math.max.apply(null, t ? protoArr.map(e, t) : e)
        }
    };
    var quantize = function () {
        function e(e, t, a) {
            return (e << 2 * u) + (t << u) + a
        }

        function t(e) {
            function t() {
                a.sort(e), n = !0
            }

            var a = [], n = !1;
            return {
                "push": function (e) {
                    a.push(e), n = !1
                }, "peek": function (e) {
                    return n || t(), void 0 === e && (e = a.length - 1), a[e]
                }, "pop": function () {
                    return n || t(), a.pop()
                }, "size": function () {
                    return a.length
                }, "map": function (e) {
                    return a.map(e)
                }, "debug": function () {
                    return n || t(), a
                }
            }
        }

        function n(e, t, a, n, r, i, o) {
            var s = this;
            s.r1 = e, s.r2 = t, s.g1 = a, s.g2 = n, s.b1 = r, s.b2 = i, s.histo = o
        }

        function r() {
            this.vboxes = new t(function (e, t) {
                return protoArr.naturalOrder(e.vbox.count() * e.vbox.volume(), t.vbox.count() * t.vbox.volume())
            })
        }

        function i(t) {
            var a, n, r, i, o = 1 << 3 * u, s = new Array(o);
            return t.forEach(function (t) {
                n = t[0] >> c, r = t[1] >> c, i = t[2] >> c, a = e(n, r, i), s[a] = (s[a] || 0) + 1
            }), s
        }

        function o(e, t) {
            var a, r, i, o = 1e6, s = 0, l = 1e6, u = 0, d = 1e6, h = 0;
            return e.forEach(function (e) {
                a = e[0] >> c, r = e[1] >> c, i = e[2] >> c, o > a ? o = a : a > s && (s = a), l > r ? l = r : r > u && (u = r), d > i ? d = i : i > h && (h = i)
            }), new n(o, s, l, u, d, h, t)
        }

        function s(t, n) {
            function r(e) {
                var t, a, r, i, o, s = e + "1", l = e + "2", c = 0;
                for (u = n[s]; u <= n[l]; u++)if (f[u] > m / 2) {
                    for (r = n.copy(), i = n.copy(), t = u - n[s], a = n[l] - u, o = a >= t ? Math.min(n[l] - 1, ~~(u + a / 2)) : Math.max(n[s], ~~(u - 1 - t / 2)); !f[o];)o++;
                    for (c = g[o]; !c && f[o - 1];)c = g[--o];
                    return r[l] = o, i[s] = r[l] + 1, [r, i]
                }
            }

            if (n.count()) {
                var i = n.r2 - n.r1 + 1, o = n.g2 - n.g1 + 1, s = n.b2 - n.b1 + 1, l = protoArr.max([i, o, s]);
                if (1 == n.count())return [n.copy()];
                var u, c, d, h, p, m = 0, f = [], g = [];
                if (l == i)for (u = n.r1; u <= n.r2; u++) {
                    for (h = 0, c = n.g1; c <= n.g2; c++)for (d = n.b1; d <= n.b2; d++)p = e(u, c, d), h += t[p] || 0;
                    m += h, f[u] = m
                } else if (l == o)for (u = n.g1; u <= n.g2; u++) {
                    for (h = 0, c = n.r1; c <= n.r2; c++)for (d = n.b1; d <= n.b2; d++)p = e(c, u, d), h += t[p] || 0;
                    m += h, f[u] = m
                } else for (u = n.b1; u <= n.b2; u++) {
                    for (h = 0, c = n.r1; c <= n.r2; c++)for (d = n.g1; d <= n.g2; d++)p = e(c, d, u), h += t[p] || 0;
                    m += h, f[u] = m
                }
                return f.forEach(function (e, t) {
                    g[t] = m - e
                }), l == i ? r("r") : l == o ? r("g") : r("b")
            }
        }

        function l(e, n) {
            function l(e, t) {
                for (var a, n = 1, r = 0; d > r;)if (a = e.pop(), a.count()) {
                    var i = s(u, a), o = i[0], l = i[1];
                    if (!o)return;
                    if (e.push(o), l && (e.push(l), n++), n >= t)return;
                    if (r++ > d)return
                } else e.push(a), r++
            }

            if (!e.length || 2 > n || n > 256)return !1;
            var u = i(e), c = 0;
            u.forEach(function () {
                c++
            });
            var p = o(e, u), m = new t(function (e, t) {
                return protoArr.naturalOrder(e.count(), t.count())
            });
            m.push(p), l(m, h * n);
            for (var f = new t(function (e, t) {
                return protoArr.naturalOrder(e.count() * e.volume(), t.count() * t.volume())
            }); m.size();)f.push(m.pop());
            l(f, n - f.size());
            for (var g = new r; f.size();)g.push(f.pop());
            return g
        }

        var u = 5, c = 8 - u, d = 1e3, h = .75;
        return n.prototype = {
            "volume": function (e) {
                var t = this;
                return (!t._volume || e) && (t._volume = (t.r2 - t.r1 + 1) * (t.g2 - t.g1 + 1) * (t.b2 - t.b1 + 1)), t._volume
            }, "count": function (t) {
                var a = this, n = a.histo;
                if (!a._count_set || t) {
                    var r, i, o, s = 0;
                    for (r = a.r1; r <= a.r2; r++)for (i = a.g1; i <= a.g2; i++)for (o = a.b1; o <= a.b2; o++) {
                        var l = e(r, i, o);
                        s += n[l] || 0
                    }
                    a._count = s, a._count_set = !0
                }
                return a._count
            }, "copy": function () {
                var e = this;
                return new n(e.r1, e.r2, e.g1, e.g2, e.b1, e.b2, e.histo)
            }, "avg": function (t) {
                var a = this, n = a.histo;
                if (!a._avg || t) {
                    var r, i, o, s, l, c = 0, d = 1 << 8 - u, h = 0, p = 0, m = 0;
                    for (i = a.r1; i <= a.r2; i++)for (o = a.g1; o <= a.g2; o++)for (s = a.b1; s <= a.b2; s++)l = e(i, o, s), r = n[l] || 0, c += r, h += r * (i + .5) * d, p += r * (o + .5) * d, m += r * (s + .5) * d;
                    a._avg = c ? [~~(h / c), ~~(p / c), ~~(m / c)] : [~~(d * (a.r1 + a.r2 + 1) / 2), ~~(d * (a.g1 + a.g2 + 1) / 2), ~~(d * (a.b1 + a.b2 + 1) / 2)]
                }
                return a._avg
            }, "contains": function (e) {
                var t, a, n = this, r = e[0] >> c;
                return t = e[1] >> c, a = e[2] >> c, r >= n.r1 && r <= n.r2 && t >= n.g1 && r <= n.g2 && a >= n.b1 && r <= n.b2
            }
        }, r.prototype = {
            "push": function (e) {
                this.vboxes.push({"vbox": e, "color": e.avg()})
            }, "palette": function () {
                return this.vboxes.map(function (e) {
                    return e.color
                })
            }, "size": function () {
                return this.vboxes.size()
            }, "map": function (e) {
                for (var t = this.vboxes, a = 0; a < t.size(); a++)if (t.peek(a).vbox.contains(e))return t.peek(a).color;
                return this.nearest(e)
            }, "nearest": function (e) {
                for (var t, a, n, r = this.vboxes, i = 0; i < r.size(); i++)a = Math.sqrt(Math.pow(e[0] - r.peek(i).color[0], 2) + Math.pow(e[1] - r.peek(i).color[1], 2) + Math.pow(e[1] - r.peek(i).color[1], 2)), (t > a || void 0 === t) && (t = a, n = r.peek(i).color);
                return n
            }, "forcebw": function () {
                var e = this.vboxes;
                e.sort(function (e, t) {
                    return protoArr.naturalOrder(protoArr.sum(e.color), protoArr.sum(t.color))
                });
                var t = e[0].color;
                t[0] < 5 && t[1] < 5 && t[2] < 5 && (e[0].color = [0, 0, 0]);
                var n = e.length - 1, r = e[n].color;
                r[0] > 251 && r[1] > 251 && r[2] > 251 && (e[n].color = [255, 255, 255])
            }
        }, {"quantize": l}
    }();
