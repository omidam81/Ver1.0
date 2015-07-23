//118

        function ImageAnalysisModel(image, backgroundColor) {
            this.canvas = null, this.imageData = null, this.backgroundColor = new ColorModel({"value": "rgb(255, 255, 255)"}), this.notUniformPoints = [], this.points = {}, this.checks = 0, this.checks_pass = 0, this.checks_fail = 0, this.checks_skipped = 0, this.enable_gradient_detection = !1;
            var a, n = this;
            n.backgroundColor = backgroundColor || new ColorModel({"value": "rgb(255, 255, 255)"}), n.canvas = document.createElement("canvas"), n.canvas.width = image.width, n.canvas.height = image.height, a = n.canvas.getContext("2d"), a.drawImage(image, 0, 0), n.imageData = a.getImageData(0, 0, n.canvas.width, n.canvas.height), this.colors = n.run()
        }

        ImageAnalysisModel.prototype = {
            "__name": "Image_Analyzer",
            "debug": !1,
            "pointDiameter": 5,
            "imageCoverage": 1,
            "maxDataPoints": 15e3,
            "minDataPoints": 300,
            "maxDistance": 11,
            "maxMedian": 9,
            "posterizeLevels": 10,
            "edgeHunters": .5,
            "run": function () {
                var e, t, a, n, r, i = 0, o = 0, s = [], l = 0, u = this, c = u.canvas.width * u.canvas.height, d = [], h = c / (u.pointDiameter * u.pointDiameter), p = u.imageCoverage * h;
                for (p < u.minDataPoints && (p = h), p > u.maxDataPoints && (p = u.maxDataPoints), l = p, n = Math.floor(Math.sqrt(c / p)) - u.pointDiameter, n += u.pointDiameter, a = [[0 + Math.ceil(u.pointDiameter / 2), 0 + Math.ceil(u.pointDiameter / 2)], [u.canvas.width - Math.ceil(u.pointDiameter / 2), 0 + Math.ceil(u.pointDiameter / 2)], [u.canvas.width - Math.ceil(u.pointDiameter / 2), u.canvas.height - Math.ceil(u.pointDiameter / 2)], [0 + Math.ceil(u.pointDiameter / 2), u.canvas.height - Math.ceil(u.pointDiameter / 2)]]; a.length > 0;)t = this.checkPoint(a.pop()), t && (s.push(t), l--);
                for (s = this.simplifyPalette(s, !0); i * n < this.imageData.width;) {
                    for (o = 0; o * n < this.imageData.height;)d.push([i * n, o * n]), o++;
                    i++
                }
                for (; d.length > 0;) t = this.checkPoint(d.pop(), s), t && (s.push(t), l--);
                s = this.simplifyPalette(s, !0);
                var m = 0, f = 0, g = 0, y = this.notUniformPoints.length, v = Math.min(5, Math.floor(n / 2 / u.pointDiameter)), b = 0;
                if (v > 1)for (e = 0; y > e; e++)r = this.notUniformPoints[e], f = 0, g = 0, u.spiralPoint(function (e, a) {
                    return t = u.checkPoint([r[0] + e * u.pointDiameter, r[1] + a * u.pointDiameter], s), m++, g++, t && (s.push(t), l--, f++), f > 4 ? !1 : (g === v * v && b++, void 0)
                });
                return s = this.simplifyPalette(s, !0), s = this.detectGradients(s)
            },
            "spiralPoint": function (e) {
                var t, a = 0, n = 0, r = 0, i = [0, -1], o = 6, s = 6;
                for (a = Math.pow(Math.max(o, s), 2); a > 0; a--) {
                    if (o / 2 >= (n > -o / 2) && s / 2 >= (r > -s / 2) && (t = e(n, r), t === !1))return;
                    (n === r || 0 > n && n === -r || n > 0 && n === 1 - r) && (i = [-i[1], i[0]]), n += i[0], r += i[1]
                }
            },
            "checkPoint": function (e, t) {
                var a, n, i, s, l, u, c, d = [[0, -2], [-1, -1], [0, -1], [1, -1], [-2, 0], [-1, 0], [0, 0], [1, 0], [2, 0], [-1, 1], [0, 1], [1, 1], [0, 2]], h = [], p = 6, m = t || [], f = [];
                if (this.points[e[0] + "|" + e[1]] === !0)return !1;
                for (this.points[e[0] + "|" + e[1]] = !0, this.checks++, i = 0; i < d.length; i++)if (a = e[0] + d[i][0], n = e[1] + d[i][1], !(0 > a || a >= this.imageData.width || 0 > n || n >= this.imageData.height)) {
                    if (s = this.getColorAt(a, n), null === s)return this.checks_skipped++, !1;
                    for (h.push(s), f.push(s.getRGB()), g = 0; g < m.length; g++)if (this.assertLABColor(s.getLAB(), m[g].getLAB()) === !0)return this.checks_skipped++, !1
                }
                if (0 === h.length)return !1;
                var g, y, v = null, b = h.length;
                for (i = 0; b > i; i++)for (f.push(h[i].getRGB()), g = 0; b > g; g++)g !== i && (y = this.getLABDistance(h[i].getLAB(), h[g].getLAB()), v || (v = y), v = Math.max(v, y));
                if (v > 1.1 * h[0].getTolerance())return this.notUniformPoints.push(e), this.checks_fail++, !1;
                l = quantize.quantize(f, p), u = l.palette(), c = new ColorModel({"value": "rgb(" + u[0][0] + ", " + u[0][1] + ", " + u[0][2] + ")"});
                var N, w = 0, _ = [];
                for (g = 0; g < h.length; g++)y = this.getLABDistance(c.getLAB(), h[g].getLAB()), w += y, _.push(y);
                return N = h.length / 2 % 1 === 0 ? h[h.length / 2] : (_[Math.floor(h.length / 2)] + _[Math.ceil(h.length / 2)]) / 2, N > this.maxMedian ? (this.notUniformPoints.push(e), this.checks_fail++, !1) : (this.checks_pass++, c)
            },
            "simplifyPalette": function (e) {
                var t, a, n, i, o = !0, s = function (e, t) {
                    return e.getHSL()[1] < .05 && t.getHSL()[1] < .05 ? e.getHSL()[2] - t.getHSL()[2] : e.getHSL()[0] - t.getHSL()[0]
                }, l = [], u = [];
                for (n = 0; n < e.length; n++)e[n]instanceof ColorModel || (a = new ColorModel, a.setRGB(e[n]), e[n] = a);
                for (e = e.sort(s); e.length > 0;) {
                    for (a = e.pop(), t = a.getLAB(), n = 0; n < l.length; n++) {
                        for (i = 0; i < l[n].length && this.assertLABColor(t, l[n][i][0]) !== !1; i++);
                        if (i === l[n].length) {
                            l[n].push([t, a]);
                            break
                        }
                    }
                    n === l.length && l.push([[t, a]])
                }
                for (; l.length > 0;) {
                    var c = l.pop();
                    1 === c.length ? u.push(o ? c[0][1] : c[0][1].getRGB()) : u.push(o ? c[Math.round(c.length / 2)][1] : c[Math.round(c.length / 2)][1].getRGB())
                }
                return u
            },
            "detectGradients": function (e) {
                var t, a, n, i, o, s, l, u, c, d = 0, h = 100, p = [], m = null;
                if (e.length <= 2 || this.enable_gradient_detection === !1)return e;
                for (t = 0; t < e.length; t++)for (a = 0; a < e.length; a++)n = e[t].getDistanceTo(e[a]), n > d && (i = e[t], o = e[a], d = n);
                for (l = i.getRGB(), u = o.getRGB(), t = 1; h > t; t++)s = new ColorModel, s.setRGB(l, t / h * 255, u), p.push(s);
                e:for (t = 0; t < e.length; t++)if (e[t].getHex() !== i.getHex() && e[t].getHex() !== o.getHex()) {
                    for (c = 1.5 * e[t].getTolerance(), a = 0; a < p.length; a++)if (n = e[t].getDistanceTo(p[a]), (null === m || n < m.distance) && (m = {
                            "distance": n,
                            "shade": p[a]
                        }), c >= n)continue e;
                    return e
                }
                return [i, o]
            },
            "getLABDistance": function (e, t) {
                var a;
                return e[0] === t[0] && e[1] === t[1] && e[2] === t[2] ? 0 : a = Math.sqrt(Math.pow(e[0] - t[0], 2) + Math.pow(e[1] - t[1], 2) + Math.pow(e[2] - t[2], 2))
            },
            "assertLABColor": function (e, t) {
                var a = t || this.areaBaseColorLAB, n = this.getLABDistance(e, a), i = e[0] < 25 && a[0] < a ? 2 * this.maxDistance : this.maxDistance, o = new ColorModel;
                return o.setLAB(e), n > Math.min(i, o.getTolerance()) ? !1 : !0
            },
            "getColorAt": function (e, t) {
                var a = (this.backgroundColor.getRGB(), 4 * (t * this.imageData.width + e)), n = this.imageData.data, i = [n[a + 0], n[a + 1], n[a + 2], n[a + 3]], o = i[3] / 255;
                return .05 > o ? null : new ColorModel({"value": "rgb(" + i[0] + ", " + i[1] + ", " + i[2] + ")"})
            }
        };
